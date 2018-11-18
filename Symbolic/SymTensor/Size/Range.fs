﻿namespace SymTensor

open DeepNet.Utils


/// basic range specification for one dimension
type BaseRangeSpec = SizeSpec * SizeSpec

/// basic range specification for multiple dimensions
type BaseRangesSpec = BaseRangeSpec list

/// Functions for working with BaseRangesSpec.
module BaseRangesSpec =

    /// Try to evalualte a BaseRangesSpecT to a numeric range.
    let tryEval (rng: BaseRangesSpec) =
        let rec doEval rng =
            match rng with
            | (first, last) :: rrng ->
                match SizeSpec.tryEval first, SizeSpec.tryEval last, doEval rrng with
                | Some first, Some last, Some rrng -> Some ((first, last) :: rrng)
                | _ -> None
            | [] -> Some []
        doEval rng

    /// True if a BaseRangesSpecT can be evaluated to a numeric range.
    let canEval (rng: BaseRangesSpec) =
        match tryEval rng with
        | Some _ -> true
        | None -> false

    /// Evaluates a BaseRangesSpecT to a numeric range.
    let eval (rng: BaseRangesSpec) =
        match tryEval rng with
        | Some rng -> rng
        | None -> failwithf "cannot evaluate BaseRangesSpecT %A to numeric range" rng

    /// checks that the BaseRangesSpec is valid
    let check (rng: BaseRangesSpec) =
        match tryEval rng with
        | Some rng ->
            for first, last in rng do
                if last < first then 
                    failwithf "invalid BaseRangesSpec: %A" rng
        | None -> ()

    /// True if two BaseRangesSpec overlap.
    /// Both BaseRangesSpec must be evaluateble to numeric ranges.
    let overlapping (a: BaseRangesSpec) (b: BaseRangesSpec) =
        check a; check b
        (eval a, eval b)
        ||> List.forall2 (fun (aFirst, aLast) (bFirst, bLast) ->
            aFirst <= bFirst && bFirst <= aLast ||
            aFirst <= bLast  && bLast  <= aLast ||
            bFirst <= aFirst && aLast  <= bLast)

    /// True if any two ranges are overlapping.
    /// This has complexity O(N^2) currently.
    /// All BaseRangesSpec must be evaluateble to numeric ranges.
    let areOverlapping (rngs: BaseRangesSpec list) =       
        let rec testOvlp nonOvlp cands =
            match cands with
            | cand::rCands ->
                if nonOvlp |> List.exists (overlapping cand) then true
                else testOvlp (cand::nonOvlp) rCands
            | [] -> false
        testOvlp [] rngs

    /// True if the BaseRangesSpecTs cover a tensor of the specified shape completely without overlap.
    /// All BaseRangesSpecT and the ShapeSpecT must be evaluatable to numeric ranges and a
    /// numeric shape respectively.
    let areCoveringWithoutOverlap (shp: ShapeSpec) (rngs: BaseRangesSpec list) =       
        if areOverlapping rngs then false
        else
            let shpElems = 
                shp 
                |> ShapeSpec.eval 
                |> List.fold (*) 1L
            let rngElems = 
                rngs
                |> List.map eval
                |> List.map (List.fold (fun p (first, last) -> p * (last - first + 1L)) 1L)
                |> List.sum
            shpElems = rngElems


/// symbolic/dynamic range specification for one dimension
[<RequireQualifiedAccess>]
type RangeSpec<'Dyn> = 
    // ranges with symbolic size (length)
    | SymElem            of SizeSpec                           
    | DynElem            of 'Dyn                                
    | SymStartSymEnd     of (SizeSpec option) * (SizeSpec option)
    | DynStartSymSize    of 'Dyn * SizeSpec                    
    | NewAxis                                                   
    | AllFill                                                   
    //| RngSymStartDynEnd     of SizeSpecT * ExprT<int>              // size: dynamic
    //| RngDynStartDynEnd     of ExprT<int> * ExprT<int>             // size: dynamic
    //| RngDynStartSymEnd     of ExprT<int> * SizeSpecT              // size: dynamic
    //| RngDynStartToEnd      of ExprT<int>                          // size: dynamic

    static member All = RangeSpec<'Dyn>.SymStartSymEnd (None, None)

// symbolic/dynamic subtensor specification
type RangesSpec<'Dyn> = RangeSpec<'Dyn> list

/// Simple range specification for one dimension.
[<RequireQualifiedAccess; StructuredFormatDisplay("{Pretty}")>]
type SimpleRangeSpec<'Dyn> =
    | SymStartSymEnd     of SizeSpec * (SizeSpec option)
    | DynStartSymSize    of 'Dyn * SizeSpec                    

    member this.Pretty =
        match this with
        | SimpleRangeSpec.SymStartSymEnd (first, Some last) -> sprintf "%A..%A" first last
        | SimpleRangeSpec.SymStartSymEnd (first, None) -> sprintf "%A.." first
        | SimpleRangeSpec.DynStartSymSize (first, size) -> sprintf "D%A..D%A+%A-1" first first size
    
    static member All = SimpleRangeSpec<'Dyn>.SymStartSymEnd (SizeSpec.zero, None)
     
    /// evaluate a SimpleRangeSpec to a Tensor.Rng
    static member eval dynEvaluator (rs: SimpleRangeSpec<'Dyn>) =
        match rs with
        | SimpleRangeSpec.SymStartSymEnd (s, fo) -> 
            Tensor.Rng.Rng (Some (SizeSpec.eval s), Option.map SizeSpec.eval fo)
        | SimpleRangeSpec.DynStartSymSize (s, elems) -> 
            let sv = dynEvaluator s
            Tensor.Rng.Rng (Some sv, Some (sv + SizeSpec.eval elems))

    static member canEvalSymbols (rs: SimpleRangeSpec<'Dyn>) =
        match rs with
        | SimpleRangeSpec.SymStartSymEnd (s, fo) ->
            SizeSpec.canEval s && Option.forall SizeSpec.canEval fo
        | SimpleRangeSpec.DynStartSymSize (_, elems) ->
            SizeSpec.canEval elems

    static member isDynamic (rs: SimpleRangeSpec<'Dyn>) =
        match rs with
        | SimpleRangeSpec.DynStartSymSize _ -> true
        | _ -> false

    static member toBaseRangeSpec (size: SizeSpec) (rs: SimpleRangeSpec<'Dyn>) =
        match rs with
        | SimpleRangeSpec.SymStartSymEnd (first, Some last) -> first, last
        | SimpleRangeSpec.SymStartSymEnd (first, None) -> first, size - 1L
        | _ -> failwithf "cannot convert %A to BaseRangeSpec" rs

/// Active patterns for SimpleRangeSpec.
module SimpleRangeSpec =

   let (|Dynamic|Static|) rs =
        if SimpleRangeSpec.isDynamic rs then Dynamic else Static


/// Simple range specification for multiple dimensions.
type SimpleRangesSpec<'Dyn> = SimpleRangeSpec<'Dyn> list

/// Functions for working with SimpleRangesSpec.
module SimpleRangesSpec =

    /// evaluate a RangesSpecT to a RangeT list
    let eval dynEvaluator rs =
        rs |> List.map (SimpleRangeSpec.eval dynEvaluator)

    let isDynamic rs =
        rs |> List.exists SimpleRangeSpec.isDynamic

    let canEvalSymbols rs =
        rs |> List.forall SimpleRangeSpec.canEvalSymbols

    let (|Dynamic|Static|) rs =
        if isDynamic rs then Dynamic else Static

    let toBaseRangesSpec (shape: ShapeSpec) rs =
        (shape, rs) ||> List.map2 SimpleRangeSpec.toBaseRangeSpec


﻿namespace Tensor.Expr.Ops

open DeepNet.Utils
open Tensor.Expr
open Tensor.Expr.Base
open Tensor.Expr.Compiler
open Tensor



/// Outputs each argument as a channel.
type Bundle = {ChExprs: Map<Ch, BaseExprCh>} with

    /// Argument corresponding to specifed channel.
    static member chToArg ch =
        match ch with
        | Ch.Default -> Arg.Only
        | Ch.Custom name -> Arg.Custom name
        | Ch.N n -> Arg.N n

    /// Channels corresponding to specifed argument.
    static member argToCh arg =
        match arg with
        | Arg.Only -> Ch.Default
        | Arg.Custom name -> Ch.Custom name
        | Arg.N n -> Ch.N n
        | _ -> failwithf "Argument %A not allowed for Bundle." arg

    interface IOp with
        member this.Check () = ()
        member this.Channels = 
            this.ChExprs |> Map.keys
        member this.TypeNames = 
            this.ChExprs |> Map.map (fun _ expr -> expr.TypeName)
        member this.Devs = 
            this.ChExprs |> Map.map (fun _ expr -> expr.Dev)
        member this.Shapes = 
            this.ChExprs |> Map.map (fun _ expr -> expr.Shape)
        member this.Args = 
            this.ChExprs |> Map.mapKeyValue (fun ch expr -> Bundle.chToArg ch, expr)
        member this.ReplaceArgs args = 
            {this with ChExprs=args |> Map.mapKeyValue (fun arg expr -> Bundle.argToCh arg, expr)} :> _
        member this.SubstSymSizes env = this :> _
        member this.CanEvalAllSymSizes = true
        member this.Eval env argVals = 
            argVals |> Map.mapKeyValue (fun arg value -> Bundle.argToCh arg, value)

    interface IMultiChannelOp

    interface IStubWishingOp with
        member this.WishStubs data = {           
            ChStubs = data.ChStubWishes
            ArgStubWishes =
                data.ChStubWishes
                |> Map.mapKeyValue (fun ch wish -> 
                    Bundle.chToArg ch, wish)
        }
                
    interface ICompilableOp with
        member this.Compile data =
            let chStubs, actions =
                data.ArgStubs
                |> Map.toSeq
                |> Seq.map (fun (arg, argStub) ->
                    let ch = Bundle.argToCh arg
                    match data.ArgStubWishes |> Map.tryFind arg with
                    | Some argStubWish when argStubWish = argStub ->
                        // The stub wish we propagated has been accepted by our argument.
                        (ch, argStub), CompileTools.noAction data
                    | Some argStubWish ->
                        // The stub wish has not been accepeted by our argument.
                        // We need the copy from the argument to the channel stub wish.
                        let copyActions = CompileTools.simpleAction data (fun chVals argVals ->
                            chVals.[ch].CopyFrom argVals.[arg]) 
                        (ch, argStubWish), copyActions
                    | None ->
                        // No wish was made.
                        // We propagate the argument stub.
                        (ch, argStub), CompileTools.noAction data)
                |> List.ofSeq
                |> List.unzip
            {
                ChStubs = Map.ofList chStubs
                Actions = CompileTools.concatActions actions
            }
        

/// Discards the results of all arguments.
type Discard = {Xs: BaseExprCh list} with
    interface IOp with       
        member this.Check () = ()
        member this.Channels = Ch.onlyOne
        member this.TypeNames = TypeName.ofType<int32> |> Ch.only
        member this.Devs = HostTensor.Dev |> Ch.only
        member this.Shapes = Shape.emptyVector |> Ch.only
        member this.Args = Args.nary this.Xs
        member this.ReplaceArgs args = {this with Xs=Args.naryXs args} :> _
        member this.SubstSymSizes env = this :> _
        member this.CanEvalAllSymSizes = true
        member this.Eval env argVals = 
            HostTensor.zeros<int32> [0L] :> ITensor |> Ch.only

    interface ICompilableOp with
        member this.Compile data = {
            ChStubs = CompileTools.chStubs data
            Actions = CompileTools.noAction data
        }


/// Build tensor using numeric ranges.
type BuildTensor = {Shape: Shape; Ranges: BaseRanges list; Xs: BaseExprCh list} with
    interface IOp with       
        member this.Check () = 
            Check.sameType this.Xs
            Check.sameDev this.Xs
            if this.Ranges.Length <> this.Xs.Length then
                failwithf "BuildTensor ranges must match arguments, but got %d ranges and %d arguments."
                            this.Ranges.Length this.Xs.Length
            match Shape.tryEval this.Shape with
            | Some shp ->
                for rng, arg in List.zip this.Ranges this.Xs do
                    if rng.Length <> shp.Length then
                        failwithf "BuildTensor range %A has wrong dimensionality for shape %A." rng shp
                    for (start, stop), size, argSize in List.zip3 rng shp arg.Shape do
                        if argSize <> stop - start + 1L then
                            failwithf "BuildTensor range %A is invalid for argument of shape %A." rng arg.Shape
                        match Size.tryEval start, Size.tryEval stop with
                        | Some start, Some stop when not (0L <= start && start < size && 0L <= stop && 
                                                            stop < size && start <= stop) ->
                            failwithf "BuildTensor range %A is invalid for shape %A." rng shp
                        | _, _ -> ()
            | None -> ()       
        member this.Channels = Ch.onlyOne
        member this.TypeNames = this.Xs.Head.TypeName |> Ch.only
        member this.Devs = this.Xs.Head.Dev |> Ch.only
        member this.Shapes = this.Shape |> Ch.only
        member this.Args = Args.nary this.Xs
        member this.ReplaceArgs args = {this with Xs=Args.naryXs args} :> _
        member this.SubstSymSizes env = 
            let sSize = Size.subst env
            {this with Shape=Shape.subst env this.Shape
                       Ranges=this.Ranges |> List.map (List.map (fun (f,l) -> sSize f, sSize l))} :> _
        member this.CanEvalAllSymSizes = 
            Shape.canEval this.Shape &&
            List.forall BaseRanges.canEval this.Ranges
        member this.Eval env argVals = 
            let vs = ArgValue.naryXs argVals
            let trgt = vs.Head.ZerosOfSameType vs.Head.Dev (Shape.eval this.Shape)
            for rng, e in List.zip this.Ranges vs do                            
                let aryRng = rng |> List.map (fun (first, last) -> 
                    Rng.Rng (Some (Size.eval first), Some (Size.eval last)))
                trgt.[aryRng] <- e 
            trgt |> Ch.only

    interface IStubWishingOp with
        member this.WishStubs data =
            let chStub =
                match data.ChStubWishes |> Map.tryFind Ch.Default with
                | Some chWish -> chWish
                | None -> 
                    TensorStub.alloc (data.Alloc, (this :> IOp).TypeNames.[Ch.Default],
                        Shape.eval (this :> IOp).Shapes.[Ch.Default], (this :> IOp).Devs.[Ch.Default])
            let argWishes = 
                if BaseRanges.areCoveringWithoutOverlap this.Shape this.Ranges then
                    // Ranges are not overlapping, we can evaluate our arguments
                    // directly into the channel tensor.
                    this.Ranges 
                    |> Seq.indexed
                    |> Seq.choose (fun (i, rng) ->
                        let evaledRng = rng |> List.map (fun (first, last) -> 
                            Rng.Rng (Some (Size.eval first), Some (Size.eval last)))
                        chStub 
                        |> TensorStub.tryView evaledRng
                        |> Option.map (fun argWish -> Arg.N i, argWish))
                    |> Map.ofSeq
                else
                    // Ranges are overlapping, we need to copy argument values.
                    Map.empty
            {
                ChStubs = Map [Ch.Default, chStub]
                ArgStubWishes = argWishes
            }

    interface ICompilableOp with
        member this.Compile data =
            let chStub = data.ChStubs.[Ch.Default]
            let actions =
                this.Ranges
                |> List.indexed
                |> List.choose (fun (i, rng) ->
                    let arg = Arg.N i
                    let argStub = data.ArgStubs.[arg]
                    match data.ArgStubWishes |> Map.tryFind arg with
                    | Some argStubWish when argStubWish = argStub ->
                        // Argument was evaluated directly into channel range.
                        None
                    | _ ->
                        // Argument must be copied into channel range.
                        let aryRng = rng |> List.map (fun (first, last) -> 
                            Rng.Rng (Some (Size.eval first), Some (Size.eval last)))
                        match chStub |> TensorStub.tryView aryRng with
                        | Some chRngView ->
                            Some { new IAction with
                                member __.Execute execData =
                                    let rngView = execData.StubValue chRngView
                                    let argVal = execData.StubValue argStub
                                    rngView.CopyFrom argVal
                                    {RuntimeChValues=Map.empty}
                                member __.Dev = chStub.Dev }
                        | None ->
                            Some { new IAction with
                                member __.Execute execData =
                                    let trgt = execData.StubValue chStub
                                    let argVal = execData.StubValue argStub
                                    trgt.[aryRng] <- argVal
                                    {RuntimeChValues=Map.empty} 
                                member __.Dev = chStub.Dev })
            {
                ChStubs = data.ChStubs
                Actions = CompileTools.concatActions actions
            }            
                


/// Elementwise calculated tensor.
type Elements = {Shape: Shape; ElemExpr: Elem.Expr; Xs: BaseExprCh list} with
    // TODO: introduce multi-channel element-wise calculation op.
    interface IOp with       
        member this.Check () = 
            Check.sameDev this.Xs
            let tns = this.Xs |> List.map (fun x -> x.TypeName)
            let ss = this.Xs |> List.map (fun x -> x.Shape)
            Elem.Expr.check this.ElemExpr |> ignore
            Elem.Expr.checkCompatibility this.ElemExpr ss tns this.Shape  
        member this.Channels = Ch.onlyOne            
        member this.TypeNames = Elem.Expr.typeName this.ElemExpr |> Ch.only
        member this.Devs = this.Xs.Head.Dev |> Ch.only
        member this.Shapes = this.Shape |> Ch.only
        member this.Args = Args.nary this.Xs
        member this.ReplaceArgs args = {this with Xs=Args.naryXs args} :> _
        member this.SubstSymSizes env = 
            {this with Shape=Shape.subst env this.Shape
                       ElemExpr=Elem.Expr.substSymSizes env this.ElemExpr} :> _
        member this.CanEvalAllSymSizes = 
            Shape.canEval this.Shape &&
            Elem.Expr.canEvalAllSymSizes this.ElemExpr
        member this.Eval env argVals = 
            let esv = ArgValue.naryXs argVals
            let nResShape = Shape.eval this.Shape
            Elem.Interpreter.evalUntyped this.ElemExpr esv nResShape 
            |> Ch.only

    interface ICompilableOp with
        member this.Compile data = {
            ChStubs = CompileTools.chStubs data
            Actions = failwith "TODO"
        }



/// Elementwise interpolation using a value table.
type Interpolate = {Interpolator: Interpolator; Xs: BaseExprCh list} with
    interface IOp with       
        member this.Check () = 
            Check.sameType this.Xs
            Check.sameDev this.Xs
            let nDims = this.Interpolator.MinArg.Length
            if nDims < 1 then
                failwith "Interpolator must be at least one-dimensional."
            if this.Interpolator.MaxArg.Length <> nDims || this.Interpolator.Outside.Length <> nDims ||
                this.Interpolator.Resolution.Length <> nDims then
                    failwith "MinArg, MaxArg, Resolution and Outside have inconsistent lengths."
            if this.Xs.Length <> nDims then
                failwith "Number of arguments does not match dimensionality of interpolator."
            if not ((this.Interpolator.MinArg, this.Interpolator.MaxArg) 
                ||> List.forall2 (fun mi ma -> conv<float> mi < conv<float> ma)) then
                    failwith "MinArg of interpolator must be smaller than MaxArg."
            if this.Interpolator.Resolution |> List.exists ((>) 0.0) then
                failwith "Resolution of interpolator must be positive."
            for x in this.Xs do 
                if not (Shape.equalIgnoringBc x.Shape this.Xs.Head.Shape) then
                    failwithf "All arguments to interpolator must have equal shape but got shapes %A and %A."
                                this.Xs.Head.Shape x.Shape
        member this.Channels = Ch.onlyOne
        member this.TypeNames = this.Xs.Head.TypeName |> Ch.only
        member this.Devs = this.Xs.Head.Dev |> Ch.only
        member this.Shapes = this.Xs.Head.Shape |> Ch.only
        member this.Args = Args.nary this.Xs
        member this.ReplaceArgs args = {this with Xs=Args.naryXs args} :> _
        member this.SubstSymSizes env = this :> _
        member this.CanEvalAllSymSizes = true
        member this.Eval env argVals = 
            let esv = ArgValue.naryXs argVals
            Interpolator.interpolateUntyped this.Interpolator esv |> Ch.only

    interface ICompilableOp with
        member this.Compile data = {
            ChStubs = CompileTools.chStubs (data, tryInplace=TryInplace.All)
            Actions = failwith "TODO"
        }
        // TODO: handle interpolator
        // Interpolator should be moved into tensor.
        // As such it will be treated as a normal tensor operation.



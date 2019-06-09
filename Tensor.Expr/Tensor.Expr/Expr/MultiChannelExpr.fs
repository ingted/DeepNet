﻿namespace Tensor.Expr

open Tensor.Expr.Ops
open Tensor.Expr.Base
open DeepNet.Utils


/// An tensor-valued expression with multiple output channels.
[<StructuredFormatDisplay("{Pretty}")>]
type MultiChannelExpr (baseExpr: BaseExpr) =    
    do 
        if baseExpr.IsSingleChannel then
            failwithf "MultiChannelExpr is for multi-channel expressions only, but got %A." baseExpr       

    new (op: IOp) =
        MultiChannelExpr (BaseExpr.ofOp op)

    member this.BaseExpr = baseExpr
    static member baseExpr (expr: MultiChannelExpr) = expr.BaseExpr

    member this.Op = baseExpr.Op
    static member op (expr: MultiChannelExpr) = expr.Op

    member this.TypeNames = baseExpr.TypeNames
    static member typeNames (expr: MultiChannelExpr) = expr.TypeNames

    member this.Devs = baseExpr.Devs
    static member devs (expr: MultiChannelExpr) = expr.Devs

    member this.Shapes = baseExpr.Shapes
    static member shapes (expr: MultiChannelExpr) = expr.Shapes

    member this.NDims = baseExpr.NDims
    static member nDims (expr: MultiChannelExpr) = expr.NDims

    member this.NElems = baseExpr.NElems
    static member nElems (expr: MultiChannelExpr) = expr.NElems

    member this.Vars = baseExpr.Vars
    static member vars (expr: MultiChannelExpr) = expr.Vars

    member this.VarMap = baseExpr.VarMap
    static member varMap (expr: MultiChannelExpr) = expr.VarMap

    member this.CanEvalAllSymSizes = baseExpr.CanEvalAllSymSizes
    static member canEvalAllSymSizes (expr: MultiChannelExpr) = expr.CanEvalAllSymSizes
    
    static member substSymSizes (env: SizeEnv) (expr: MultiChannelExpr) =
        expr.BaseExpr |> BaseExpr.substSymSizes env |> MultiChannelExpr

    interface System.IEquatable<MultiChannelExpr> with
        member this.Equals other = this.BaseExpr = other.BaseExpr

    override this.Equals other =
        match other with
        | :? MultiChannelExpr as other -> (this :> System.IEquatable<_>).Equals other
        | _ -> false

    interface System.IComparable<MultiChannelExpr> with
        member this.CompareTo other = compare this.BaseExpr other.BaseExpr

    interface System.IComparable with
        member this.CompareTo other =
            match other with
            | :? MultiChannelExpr as other -> (this :> System.IComparable<_>).CompareTo other
            | _ -> failwithf "Cannot compare MultiChannelExpr to type %A." (other.GetType())

    override this.GetHashCode() = hash this.BaseExpr

    /// Converts expression to string with specified approximate maximum length.
    member this.ToString maxLength =     
        ExprHelpers.exprToString maxLength this.BaseExpr

    /// Converts expression to string with unlimited length.
    override this.ToString () = this.ToString System.Int32.MaxValue

    /// Pretty string.
    member this.Pretty = this.ToString 80

    /// Accesses the specified channel of this multi-channel expression as IExpr.
    member this.Item 
        with get (channel: Ch) : UExpr = 
            UExpr baseExpr.[channel]

    /// Accesses the specified channel of this multi-channel expression as Expr<'T>.
    member this.Ch (channel: Ch) : Expr<'T> = 
        Expr<'T> baseExpr.[channel]

    /// Bundle single-channel expressions into a multi-channel expression. 
    static member bundle (chExprs: Map<Ch, UExpr>) =
        let chExprs = chExprs |> Map.map (fun _ expr -> expr.BaseExprCh)
        MultiChannelExpr {Bundle.ChExprs=chExprs}

    /// A loop provides iterative evaluation of one or multiple expresisons.
    static member loop length (channels: Map<Ch, UExpr * int>) =
        let channels =
            channels
            |> Map.map (fun _ (expr, sliceDim) -> 
                expr.BaseExprCh, sliceDim)
        Ops.Loop.fromLoopArgExpr length channels |> MultiChannelExpr

    /// Evaluates all channels of the expression into numeric values using the specified evaluation envirnoment.
    static member evalWithEnv (evalEnv: EvalEnv) (expr: MultiChannelExpr) = 
        // Infer symbolic sizes from variable environment and substitute them into expression.
        let varValMap = VarValMap.make evalEnv.VarEnv expr.VarMap
        let symSizeEnv = varValMap |> VarValMap.inferSymSizes Map.empty
        let substExpr = expr |> MultiChannelExpr.substSymSizes symSizeEnv

        // Evaluate.
        BaseExprEval.eval evalEnv substExpr.BaseExpr

    /// Evaluates all channels of the expression into numeric values.
    static member eval (varEnv: VarEnv) (expr: MultiChannelExpr) = 
        let evalEnv : EvalEnv = {VarEnv=varEnv; Tracer=NoTracer()}    
        MultiChannelExpr.evalWithEnv evalEnv expr

    /// Substitutes the variables within the expression tree.
    static member substVars (env: Map<VarName, UExpr>) (expr: MultiChannelExpr) =
        let env = env |> Map.map (fun _ sExpr -> sExpr.BaseExpr)
        expr.BaseExpr |> BaseExpr.substVars env |> MultiChannelExpr


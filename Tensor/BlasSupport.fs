﻿namespace Tensor.Backend

open System

open Tensor
open Tensor.Utils


/// backend-neutral BLAS support 
module internal BLAS = 

    /// storage that supports pinning of memory for BLAS
    type IBLASStorage =
        /// pin memory and return handle to unpin and pointer to memory
        abstract Pin: unit -> IDisposable * nativeint

    /// BLAS Transposition
    type Transposition =
        | NoTrans
        | Trans
        | ConjTrans

    /// Information for passing a matrix to BLAS/LAPACK routines.
    type MatrixInfo (storage:   IBLASStorage,
                     offsets:   nativeint[],
                     rows:      int64,
                     cols:      int64,
                     ld:        int64,
                     trans:     Transposition,
                     disposeFn: (unit -> unit)) =

        let memPin, basePtr = storage.Pin()

        member this.Ptr : nativeint = 
            if offsets.Length = 1 then basePtr + offsets.[0]
            else failwith "this Blas.MatrixInfo is representing a batch of matrices"
        member this.Ptrs : nativeint [] =
            offsets |> Array.map (fun o -> basePtr + o)
        member this.Rows    = rows
        member this.Cols    = cols
        member this.Ld      = ld
        member this.Trans   = trans
        member this.OpRows  = 
            match this.Trans with 
            | NoTrans           -> this.Rows
            | Trans | ConjTrans -> this.Cols
        member this.OpCols  = 
            match this.Trans with 
            | NoTrans           -> this.Cols
            | Trans | ConjTrans -> this.Rows
        member this.BatchSize = int64 offsets.Length 

        interface IDisposable with
            member this.Dispose() = 
                disposeFn ()
                memPin.Dispose()


    /// Information for passing a vector to BLAS/LAPACK routines.
    type VectorInfo (storage:   IBLASStorage,
                     offset:    nativeint,
                     size:      int64,
                     inc:       int64,
                     disposeFn: (unit -> unit)) =

        let memPin, basePtr = storage.Pin()

        member this.Ptr       : nativeint            = basePtr + offset
        member this.Size                             = size
        member this.Inc                              = inc

        interface IDisposable with
            member this.Dispose() = 
                disposeFn ()
                memPin.Dispose()


open BLAS

/// backend-neutral BLAS support 
type internal BLAS =

    /// Internal function for GetBlasVector.
    static member private GetVectorInfo (vec: Tensor<'T>, ?disposeFn) =
        let disposeFn = defaultArg disposeFn id
        if vec.NDims <> 1 then 
            failwithf "BLAS operation requires a vector but got tensor of shape %A" vec.Shape
        let storage = vec.Storage :?> IBLASStorage
        match vec.Layout.Stride, vec.Layout.Shape with
        | [m], [ms] when m <> 0L ->   // increment <> 0
            new VectorInfo (storage, nativeint (sizeof64<'T> * vec.Layout.Offset), 
                            ms, m, disposeFn) |> Some
        | _  -> None                  // not acceptable BLAS layout

    /// Returns a BlasVectorInfo that exposes the specfied vector to BLAS
    /// as a source and/or target. 
    /// When allowCopy is true, then:
    /// - the source might be copied into a temporary tensor,
    /// - the result might be copied from a temporary tensor into the target, when
    ///   the returned BlasMatrixInfo is disposed.
    static member GetVector (vec: Tensor<'T>, isSource: bool, isTarget: bool, ?allowCopy: bool) =
        let allowCopy = defaultArg allowCopy true                        
        match BLAS.GetVectorInfo (vec) with
        | Some bi -> bi
        | None when allowCopy ->
            let tmp = Tensor<'T>(vec.Shape, vec.Device, order=ColumnMajor)
            if isSource then tmp.CopyFrom vec
            let disposeFn () = if isTarget then vec.CopyFrom tmp
            BLAS.GetVectorInfo (tmp, disposeFn=disposeFn) 
            |> Option.get
        | None ->
            failwithf "tensor with shape %A and strides %A is not a valid BLAS vector"
                      vec.Shape vec.Layout.Stride

    /// Internal function for GetBlasMatrix.
    static member private GetMatrixInfo (mat: Tensor<'T>, canTranspose, ?disposeFn) =
        let disposeFn = defaultArg disposeFn id
        if mat.NDims < 2 then 
            failwithf "BLAS operation requires a matrix but got tensor of shape %A" mat.Shape
        let storage = mat.Storage :?> IBLASStorage
        let offsets = 
            TensorLayout.allIdxOfShape mat.Shape.[0 .. mat.NDims-3]
            |> Seq.map (fun batchIdx -> 
                let idx = batchIdx @ [0L; 0L]
                sizeof64<'T> * TensorLayout.addr idx mat.Layout |> nativeint)
            |> Array.ofSeq
        match mat.Layout.Stride.[mat.NDims-2 ..], mat.Layout.Shape.[mat.NDims-2 ..] with
        | [m;  1L], [ms; ns] when m >= max 1L ns && canTranspose ->   // row-major
            new MatrixInfo (storage, offsets, ns, ms, m, Trans, disposeFn) |> Some
        | [1L; n],  [ms; ns] when n >= max 1L ms ->                   // column-major
            new MatrixInfo (storage, offsets, ms, ns, n, NoTrans, disposeFn) |> Some            
        | _  -> None                                                  // not acceptable BLAS layout

    /// Returns a BlasMatrixInfo that exposes the specfied matrix to BLAS
    /// as a source and/or target. 
    /// If canTranpose is true, then the BLAS call must accept a tranpose parameter.
    /// When allowCopy is true, then:
    /// - the source might be copied into a temporary tensor,
    /// - the result might be copied from a temporary tensor into the target, when
    ///   the returned BlasMatrixInfo is disposed.
    static member GetMatrix (mat: Tensor<'T>, isSource: bool, isTarget: bool,
                             canTranspose: bool, ?allowCopy: bool) =
        let allowCopy = defaultArg allowCopy true                        
        match BLAS.GetMatrixInfo (mat, canTranspose=canTranspose) with
        | Some bi -> bi
        | None when allowCopy ->
            let order = [mat.NDims-2; mat.NDims-1] @ [0 .. mat.NDims-3]
            let tmp = Tensor<'T> (mat.Shape, mat.Device, order=CustomOrder order)
            if isSource then tmp.CopyFrom mat
            let disposeFn () = if isTarget then mat.CopyFrom tmp
            BLAS.GetMatrixInfo (tmp, canTranspose=canTranspose, disposeFn=disposeFn) 
            |> Option.get
        | None ->
            failwithf "tensor with shape %A and strides %A is not a valid BLAS matrix"
                      mat.Shape mat.Layout.Stride


    /// Call BLAS/LAPACK function depending on data type.
    static member Invoke<'T, 'R> (?singleFn: unit -> 'R, 
                                  ?doubleFn: unit -> 'R,
                                  ?int32Fn: unit -> 'R,
                                  ?int64Fn: unit -> 'R) : 'R =
        match typeof<'T> with
        | t when t=typeof<single> && singleFn.IsSome -> singleFn.Value () 
        | t when t=typeof<double> && doubleFn.IsSome -> doubleFn.Value () 
        | t when t=typeof<int32> && int32Fn.IsSome -> int32Fn.Value () 
        | t when t=typeof<int64> && int64Fn.IsSome -> int64Fn.Value () 
        | t -> failwithf "unsupported data type for BLAS operation: %A" t


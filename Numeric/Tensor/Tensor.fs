namespace rec Tensor

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics

open Tensor.Utils
open Tensor.Backend


/// A singular matrix was encountered during an operation that does not allow singular matrices.
exception SingularMatrixException of msg:string with override __.Message = __.msg


/// Type-neutral interface to Tensor<'T> of any type 'T.
type ITensor =

    /// layout of this tensor (shape, offset and strides)
    abstract Layout:            TensorLayout
    /// storage of this tensor
    abstract Storage:           ITensorStorage
    /// storage factory
    abstract Dev:               ITensorDevice
    /// shape
    abstract Shape:             int64 list
    /// stride
    abstract Stride:            int64 list
    /// stride
    abstract Offset:            int64 
    /// number of dimensions
    abstract NDims:             int
    /// number of elements
    abstract NElems:            int64    
    /// type of data stored in this tensor
    abstract DataType:          System.Type
    /// pretty contents string
    abstract Pretty:            string
    /// full contents string
    abstract Full:              string

    /// a tensor with the same storage but new layout
    abstract Relayout:          TensorLayout -> ITensor
    /// returns a copy of the tensor
    abstract Copy:              ?order:TensorOrder -> ITensor
    /// Transfers this tensor to the specifed device.
    abstract Transfer:          dev:ITensorDevice -> ITensor
    /// fills the tensors with zeros
    abstract FillZero:          unit -> unit

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : m0:Tensor<bool> -> ITensor with get, set
    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : m0:Tensor<bool> * m1:Tensor<bool> -> ITensor with get, set
    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : m0:Tensor<bool> * m1:Tensor<bool> * m2:Tensor<bool> -> ITensor with get, set
    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : m0:Tensor<bool> * m1:Tensor<bool> * m2:Tensor<bool> * m3:Tensor<bool> -> ITensor with get, set
    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : m0:Tensor<bool> * m1:Tensor<bool> * m2:Tensor<bool> * m3:Tensor<bool> * m4:Tensor<bool> -> ITensor with get, set
    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    abstract M : masks:Tensor<bool> list -> ITensor with get, set

    /// n-dimensional slicing using a list of Rngs
    abstract Item : rng:Rng list -> ITensor with get
    /// n-dimensional slicing using a list of Rngs
    abstract Item : rng:Rng list -> ITensor with set

    // type-neutral slicing 
    abstract Item : i0:int64 -> ITensor with get
    abstract Item : i0:int64 * i1:int64 -> ITensor with get
    abstract Item : i0:int64 * i1:int64 * i2:int64 -> ITensor with get
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor with get

    abstract Item : i0:int64 -> ITensor with set
    abstract Item : i0:int64 * i1:int64 -> ITensor with set
    abstract Item : i0:int64 * i1:int64 * i2:int64 -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj * o5:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj * o5:obj * o6:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj * o5:obj * o6:obj * o7:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj * o5:obj * o6:obj * o7:obj * o8:obj -> ITensor with set
    abstract Item : o0:obj * o1:obj * o2:obj * o3:obj * o4:obj * o5:obj * o6:obj * o7:obj * o8:obj * o9:obj -> ITensor with set

    abstract GetSlice : i0s:int64 option * i0f:int64 option -> ITensor
    abstract GetSlice : i0:int64 * i1s:int64 option * i1f:int64 option -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1:int64 -> ITensor
    abstract GetSlice : i0:int64 * i1:int64 * i2:int64 -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option -> ITensor
    abstract GetSlice : i0:int64 * i1:int64 * i2s:int64 option * i2f:int64 option -> ITensor
    abstract GetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2:int64 -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2:int64 -> ITensor
    abstract GetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2s:int64 option * i2f:int64 option -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2:int64 -> ITensor
    abstract GetSlice : i0:int64 * i1:int64 * i2:int64 * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option -> ITensor
    abstract GetSlice : i0:int64 * i1:int64 * i2s:int64 option * i2f:int64 option * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2:int64 * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2:int64 * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2s:int64 option * i2f:int64 option * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2:int64 * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor
    abstract GetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * o3:obj * [<System.ParamArray>] r:obj [] -> ITensor

    abstract SetSlice : i0s:int64 option * i0f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1:int64 * i2:int64 * value:ITensor -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1:int64 * i2s:int64 option * i2f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2:int64 * value:ITensor -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2:int64 * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2s:int64 option * i2f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2:int64 * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1:int64 * i2:int64 * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * value:ITensor -> unit
    abstract SetSlice : i0:int64 * i1:int64 * i2s:int64 option * i2f:int64 option * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2:int64 * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2:int64 * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0:int64 * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1:int64 * i2s:int64 option * i2f:int64 option * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2:int64 * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit
    abstract SetSlice : i0s:int64 option * i0f:int64 option * i1s:int64 option * i1f:int64 option * i2s:int64 option * i2f:int64 option * o3:obj * o4:obj * [<System.ParamArray>] r:obj [] -> unit



/// Block tensor specification.
type BlockTensor<'T> =
    /// A block consisting of multiple sub-blocks.
    | SubBlocks of BlockTensor<'T> list
    /// A block consisting of a single tensor.
    | Block of Tensor<'T>



/// An N-dimensional array with elements of type 'T.
type [<StructuredFormatDisplay("{Pretty}"); DebuggerDisplay("{Shape}-Tensor: {Pretty}")>] 
        Tensor<'T> (layout: TensorLayout, storage: ITensorStorage<'T>) =

    do TensorLayout.check layout
    let backend = storage.Backend layout

    /// value zero of type 'T
    static member Zero : 'T = zero<'T>

    /// value one of type 'T
    static member One = one<'T>

    /// layout of this tensor (shape, offset and strides)
    member val Layout = layout

    /// layout
    static member inline layout (a: #ITensor) = a.Layout

    /// storage of this tensor
    member val Storage = storage

    /// device this tensor is stored on
    member inline this.Dev = this.Storage.Dev

    /// device where the specified tensor is stored
    static member inline dev (a: #ITensor) = a.Dev

    /// backend
    member internal this.Backend = backend

    /// shape
    member inline this.Shape = this.Layout.Shape

    /// shape 
    static member inline shape (a: #ITensor) = a.Shape

    /// number of dimensions
    member inline this.NDims = this.Layout.NDims

    /// number of dimensions
    static member inline nDims (a: #ITensor) = a.NDims

    /// number of elements
    member inline this.NElems = this.Layout.NElems

    /// number of elements 
    static member inline nElems (a: #ITensor) = a.NElems

    /// strides
    member inline this.Stride = this.Layout.Stride

    /// strides
    static member inline stride (a: #ITensor) = a.Stride

    /// offset
    member inline this.Offset = this.Layout.Offset

    /// offset
    static member inline offset (a: #ITensor) = a.Offset

    /// type of data stored in this tensor
    member inline this.DataType = typeof<'T>

    /// type of data stored in the specified tensor
    static member inline dataType (a: #ITensor) = a.DataType

    /// a tensor with the same storage but new layout
    member internal this.Relayout (newLayout: TensorLayout) =
        Tensor<'T> (newLayout, storage)

    /// a tensor with the same storage but new layout
    static member relayout newLayout (a: 'A when 'A :> ITensor) : 'A =
        a.Relayout newLayout :?> 'A

    /// a view of this tensor over the given range 
    member internal this.Range (rng: Rng list) =
        this.Relayout (this.Layout |> TensorLayout.view rng)

    /// a view of the specified tensor over the given range 
    static member range (rng: Rng list) (a: 'A when 'A :> ITensor) : 'A =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.view rng)
   
    /// checks that the given axis is valid
    member inline this.CheckAxis ax = this.Layout |> TensorLayout.checkAxis ax

    /// sequence of all indices 
    static member allIdx (a: #ITensor) = a.Layout |> TensorLayout.allIdx

    /// all indices of the given dimension
    static member allIdxOfDim dim (a: #ITensor) = a.Layout |> TensorLayout.allIdxOfDim dim 
            
    /// sequence of all elements stored in the tensor
    static member allElems (a: Tensor<'T>) = a |> Tensor<_>.allIdx |> Seq.map (fun idx -> a.[idx])
    
    /// inserts a broadcastable dimension of size one as first dimension
    static member padLeft a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.padLeft)

    /// appends a broadcastable dimension of size one as last dimension
    static member padRight a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.padRight)

    /// Inserts an axis of size 1 before the specified position.
    static member insertAxis ax a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.insertAxis ax)

    /// removes the first dimension from the tensor
    static member cutLeft a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.cutLeft)
      
    /// removes the last dimension from the tensor
    static member cutRight a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.cutRight)

    /// broadcast the given dimension to the given size
    static member broadcastDim dim size a =
        a |> Tensor<_>.relayout (a.Layout |> TensorLayout.broadcastDim dim size)       

    /// Creates a new tensor of specifed shape with newly allocated storage using 
    /// the specified storage device.
    new (shape: int64 list, dev: ITensorDevice, ?order: TensorOrder) =
        let order = defaultArg order RowMajor
        let layout = 
            match order with
            | RowMajor -> TensorLayout.newC shape
            | ColumnMajor -> TensorLayout.newF shape
            | CustomOrder perm -> TensorLayout.newOrdered shape perm
        let storage = dev.Create layout.NElems
        Tensor<'T> (layout, storage)

    /// Applies the given function to the tensors' layouts.
    static member inline internal ApplyLayoutFn (fn, a, b) =
        let layouts = [Tensor<_>.layout a; Tensor<_>.layout b]
        let newLayouts = fn layouts
        match newLayouts with
        | [al; bl] -> 
            Tensor<_>.relayout al a, Tensor<_>.relayout bl b
        | _ -> failwith "unexpected layout function result"

    /// Applies the given function to the tensors' layouts.
    static member inline internal ApplyLayoutFn (fn, a, b, c) =
        let layouts = [Tensor<_>.layout a; Tensor<_>.layout b; Tensor<_>.layout c]
        let newLayouts = fn layouts
        match newLayouts with
        | [al; bl; cl] -> 
            Tensor<_>.relayout al a, Tensor<_>.relayout bl b, Tensor<_>.relayout cl c
        | _ -> failwith "unexpected layout function result"

    /// Applies the given function to the tensors' layouts.
    static member inline internal ApplyLayoutFn (fn, xs) =
        let layouts = fn (xs |> List.map Tensor<_>.layout)
        (layouts, xs) ||> List.map2 Tensor<_>.relayout

    /// pads the shapes of all tensors from the left until they have same rank
    static member padToSame (a, b) = 
        Tensor<_>.ApplyLayoutFn (TensorLayout.padToSameMany, a, b)

    /// pads the shapes of all tensors from the left until they have same rank
    static member padToSame (a, b, c) = 
        Tensor<_>.ApplyLayoutFn (TensorLayout.padToSameMany, a, b, c)

    /// pads the shapes of all tensors from the left until they have same rank
    static member padToSame (xs) = 
        Tensor<_>.ApplyLayoutFn (TensorLayout.padToSameMany, xs)

    /// broadcasts all tensors to the same shape 
    static member broadcastToSame (a, b) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameMany, a, b)

    /// broadcasts all tensors to the same shape if possible
    static member broadcastToSame (a, b, c) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameMany, a, b, c)

    /// broadcasts all tensors to the same shape if possible
    static member broadcastToSame (xs) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameMany, xs)

    /// broadcasts all tensors to the same sizes in the given dimensions
    static member broadcastToSameInDims (dims, a, b) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameInDimsMany dims, a, b)

    /// broadcasts all tensors to the same sizes in the given dimensions
    static member broadcastToSameInDims (dims, a, b, c) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameInDimsMany dims, a, b, c)

    /// broadcasts all tensors to the same sizes in the given dimensions
    static member broadcastToSameInDims (dims, xs) =
        Tensor<_>.ApplyLayoutFn (TensorLayout.broadcastToSameInDimsMany dims, xs)

    /// broadcasts the tensor to the given shape
    static member broadcastTo shp a =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.broadcastToShape shp)

    /// returns true if at least one dimension is broadcasted
    static member isBroadcasted a =
        a |> Tensor<_>.layout |> TensorLayout.isBroadcasted 

    /// returns true if tensor is stored in row-major order
    static member isRowMajor a =
        a |> Tensor<_>.layout |> TensorLayout.isC

    /// returns true if tensor is stored in column-major order
    static member isColumnMajor a =
        a |> Tensor<_>.layout |> TensorLayout.isF

    /// Tries to reshape the tensor without copying.
    /// For this to succeed, the tensor must have row-major layout.
    /// If this a reshape without copying is impossible, None is returned.
    static member tryReshapeView shp a =
        match a |> Tensor<_>.layout |> TensorLayout.tryReshape shp with
        | Some newLayout -> a |> Tensor<_>.relayout newLayout |> Some
        | None -> None

    /// Tries to reshape the tensor without copying.
    /// For this to succeed, the tensor must have row-major layout.
    /// If this a reshape without copying is impossible, an error is raised.
    static member reshapeView shp a =
        match Tensor<_>.tryReshapeView shp a with
        | Some res -> res
        | None -> 
            let msg =
                sprintf "cannot reshape tensor of shape %A and strides %A without copying"
                    (Tensor<_>.layout a).Shape (Tensor<_>.layout a).Stride
            raise (InvalidOperationException msg)

    /// Returns true if the tensor can be reshaped without copying.
    static member canReshapeView shp a =
        match Tensor<_>.tryReshapeView shp a with
        | Some _ -> true
        | None -> false

    /// Reshape array assuming a row-major order.
    /// If the array is currently not in row-major order, a reshaped copy is returned.
    /// Otherwise, a reshaped view of the same tensor is returned.
    /// The number of elements must not change.
    /// One element can be 'Remainder', in which case the size of that element is
    /// inferred automatically.
    static member reshape shp a =
        match a |> Tensor<_>.tryReshapeView shp with
        | Some res -> res
        | None ->
            a |> Tensor<_>.copy |> Tensor<_>.reshapeView shp

    /// Flattens the tensor into a (one-dimensional) vector.
    static member flatten a =
        Tensor<_>.reshape [Remainder] a

    /// swaps the given dimensions
    static member swapDim ax1 ax2 a =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.swapDim ax1 ax2)

    /// Transposes the given matrix.
    /// If the given tensor has more then two dimensions, the last two axes are swapped.
    static member transpose a =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.transpose)

    /// Permutes the axes as specified.
    /// Each entry in the specified permutation specifies the new position of 
    /// the corresponding axis, i.e. to which position the axis should move.
    static member permuteAxes (permut: int list) a =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.permuteAxes permut)

    /// Reverses the elements in the specified dimension.
    static member reverseAxis ax a =
        a |> Tensor<_>.relayout (a |> Tensor<_>.layout |> TensorLayout.reverseAxis ax)        

    /// Ensures that the tensor has at least minDims dimensions.
    /// If not, it is padded with size one dimensions from the left.
    static member atLeastND minDims a =
        let nd = Tensor<_>.nDims a
        if nd >= minDims then a
        else
            let newShp = List.init (minDims - nd) (fun _ -> 1L)
            a |> Tensor<_>.reshape newShp

    /// Ensures that the tensor has at least one dimension.
    static member atLeast1D a = a |> Tensor<_>.atLeastND 1

    /// Ensures that the tensor has at least two dimensions.
    /// If not, it is padded with size one dimensions from the left.
    static member atLeast2D a = a |> Tensor<_>.atLeastND 2

    /// Ensures that the tensor has at least three dimensions.
    /// If not, it is padded with size one dimensions from the left.
    static member atLeast3D a = a |> Tensor<_>.atLeastND 3

    /// Transposes the given matrix.
    /// If the given tensor has more then two dimensions, the last two axes are swapped.
    member inline this.T = 
        Tensor<_>.transpose this

    /// returns a copy of the tensor
    member this.Copy (?order) =
        let trgt, src = Tensor.PrepareElemwise (this, ?order=order)
        trgt.Backend.Copy (trgt=trgt, src=src)
        trgt      
        
    /// returns a copy of the tensor
    static member copy (a: 'A when 'A :> ITensor, ?order) =
        a.Copy (?order=order) :?> 'A

    /// Copies the specifed tensor into this tensor.
    /// Both tensors must have same shape and storage.
    member trgt.CopyFrom (src: Tensor<'T>) =
        Tensor.CheckSameShape trgt src
        Tensor.CheckSameStorage [trgt; src]
        trgt.Backend.Copy (trgt=trgt, src=src)

    /// Transfers the specified tensor located on another device into this tensor.
    /// Both tensors must have the same shape.
    member trgt.TransferFrom (src: Tensor<'T>) =
        Tensor.CheckSameShape trgt src
        if trgt.Dev = src.Dev then
            trgt.CopyFrom (src)
        else
            if not (trgt.Backend.Transfer (trgt=trgt, src=src) ||
                    src.Backend.Transfer (trgt=trgt, src=src)) then
                invalidOp "Cannot transfer from storage %s to storage %s." src.Dev.Id trgt.Dev.Id

    /// Transfers this tensor to the specifed device.
    member src.Transfer (dev: ITensorDevice) =
        let trgt = Tensor<'T> (src.Shape, dev)
        trgt.TransferFrom src
        trgt

    /// Transfers the specified tensor to the specifed device.
    static member transfer (dev: ITensorDevice) (src: 'A when 'A :> ITensor) =
        src.Transfer (dev) :?> 'A

    /// this tensor as Tensor<bool>
    member internal this.AsBool : Tensor<bool> =
        if this.DataType = typeof<bool> then
            this |> box :?> Tensor<bool>
        else
            invalidOp "The operation requires a Tensor<bool> but the data type of the specified tensor is %s." 
                      this.DataType.Name

    /// this tensor as Tensor<int64>
    member internal this.AsInt64 : Tensor<int64> =
        if this.DataType = typeof<int64> then
            this |> box :?> Tensor<int64>
        else
            invalidOp "The operation requires a Tensor<int64> but the data type of the specified tensor is %s." 
                      this.DataType.Name

    /// Fills the tensor with the values returned by the function.
    member trgt.Fill (fn: unit -> 'T)  =
        trgt.Backend.Fill (fn=fn, trgt=trgt, useThreads=false)

    /// Fills the tensor with the values returned by the function using multiple threads.
    member trgt.FillParallel (fn: unit -> 'T)  =
        trgt.Backend.Fill (fn=fn, trgt=trgt, useThreads=true)

    /// Fills the tensor with the values returned by the function.
    member trgt.FillIndexed (fn: int64[] -> 'T) =
        trgt.Backend.FillIndexed (fn=fn, trgt=trgt, useThreads=false)

    /// Fills the tensor with the values returned by the function using multiple threads.
    member trgt.FillParallelIndexed (fn: int64[] -> 'T) =
        trgt.Backend.FillIndexed (fn=fn, trgt=trgt, useThreads=true)

    /// Copy source tensor into this tensor.
    /// The source tensor is broadcasted to the size of this tensor, if possible.
    member trgt.FillFrom (src: Tensor<'T>) = 
        let src = Tensor.PrepareElemwiseSources (trgt, src)
        trgt.CopyFrom src

    /// Fills the tensor with the specified constant.
    member trgt.FillConst (value: 'T) =
        trgt.Backend.FillConst (value=value, trgt=trgt)

    /// Fills the one-dimensional tensor with a sequence starting at the specified start value
    /// and using the specified increment.
    member trgt.FillIncrementing (start: 'T, incr: 'T) =
        if trgt.NDims <> 1 then invalidOp "FillIncrementing requires a vector."
        trgt.Backend.FillIncrementing (start=start, incr=incr, trgt=trgt)

    /// Fills the tensor with the values returned by the given sequence.
    member trgt.FillSeq (data: 'T seq) =
        use enumerator = data.GetEnumerator()
        trgt.Fill (fun () -> 
            if enumerator.MoveNext() then enumerator.Current
            else invalidArg "data" "Sequence ended before tensor of shape %A was filled." trgt.Shape)

    /// maps all elements using the specified function into this tensor
    member trgt.FillMap (fn: 'TA -> 'T) (a: Tensor<'TA>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Map (fn=fn, trgt=trgt, src=a, useThreads=false)

    /// maps all elements using the specified function into this tensor using multiple threads
    member trgt.FillParallelMap (fn: 'TA -> 'T) (a: Tensor<'TA>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Map (fn=fn, trgt=trgt, src=a, useThreads=false)

    /// maps all elements using the specified function into a new tensor
    static member map (fn: 'T -> 'R) (a: Tensor<'T>) =
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillMap fn a
        trgt       

    /// maps all elements using the specified indexed function into this tensor
    member trgt.FillMapIndexed (fn: int64[] -> 'TA -> 'T) (a: Tensor<'TA>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.MapIndexed (fn=fn, trgt=trgt, src=a, useThreads=false)

    /// maps all elements using the specified indexed function into this tensor using multiple threads
    member trgt.FillParallelMapIndexed (fn: int64[] -> 'TA -> 'T) (a: Tensor<'TA>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.MapIndexed (fn=fn, trgt=trgt, src=a, useThreads=true)

    /// maps all elements using the specified indexed function into a new tensor
    static member mapi (fn: int64[] -> 'T -> 'R) (a: Tensor<'T>) =
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillMapIndexed fn a
        trgt     

    /// maps all elements using the specified function into this tensor
    member trgt.FillMap2 (fn: 'TA -> 'TB -> 'T) (a: Tensor<'TA>) (b: Tensor<'TB>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Map2 (fn=fn, trgt=trgt, src1=a, src2=b, useThreads=false)

    /// maps all elements using the specified function into this tensor using multiple threads
    member trgt.FillParallelMap2 (fn: 'TA -> 'TB -> 'T) (a: Tensor<'TA>) (b: Tensor<'TB>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Map2 (fn=fn, trgt=trgt, src1=a, src2=b, useThreads=true)

    /// maps all elements using the specified function into a new tensor
    static member map2 (fn: 'TA -> 'TB -> 'R) (a: Tensor<'TA>) (b: Tensor<'TB>) =
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillMap2 fn a b
        trgt       

    /// maps all elements using the specified indexed function into this tensor
    member trgt.FillMapIndexed2 (fn: int64[] -> 'TA -> 'TB -> 'T) (a: Tensor<'TA>) (b: Tensor<'TB>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.MapIndexed2 (fn=fn, trgt=trgt, src1=a, src2=b, useThreads=false)

    /// maps all elements using the specified indexed function into this tensor using multiple threads
    member trgt.FillParallelMapIndexed2 (fn: int64[] -> 'TA -> 'TB -> 'T) (a: Tensor<'TA>) (b: Tensor<'TB>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.MapIndexed2 (fn=fn, trgt=trgt, src1=a, src2=b, useThreads=true)

    /// maps all elements using the specified indexed function into a new tensor
    static member mapi2 (fn: int64[] -> 'TA -> 'TB -> 'R) (a: Tensor<'TA>) (b: Tensor<'TB>) =
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillMapIndexed2 fn a b
        trgt       

    /// copies all elements into this tensor and converts their data type appropriately
    member trgt.FillConvert (a: Tensor<'TA>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Convert (trgt=trgt, src=a)

    /// converts all elements to the specified type
    static member convert<'C> (a: Tensor<'T>) : Tensor<'C> =
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillConvert (a)
        trgt

    /// element-wise unary (prefix) plus using this tensor as target
    member trgt.FillUnaryPlus (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.UnaryPlus (trgt=trgt, src1=a)

    /// element-wise unary (prefix) plus
    static member (~+) (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillUnaryPlus (a)
        trgt

    /// element-wise unary (prefix) minus using this tensor as target
    member trgt.FillUnaryMinus (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.UnaryMinus (trgt=trgt, src1=a)

    /// element-wise unary (prefix) minus
    static member (~-) (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillUnaryMinus (a)
        trgt

    /// <summary>Fills this tensor with the element-wise absolute value of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-2; -1; 1]
    /// a.FillAbs a // a = [2; 1; 1]
    /// </code></example>
    /// <remarks>Computes the absolute value of each element of the specified tensor and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Abs"/>
    member trgt.FillAbs (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Abs (trgt=trgt, src1=a)

    /// <summary>Element-wise absolute value.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-2; -1; 1]
    /// let b = abs a // b = [2; 1; 1]
    /// </code></example>
    /// <remarks>Computes the absolute value of each element of the specified tensor and returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>abs</c> function.</remarks>
    /// <seealso cref="FillAbs"/>
    static member Abs (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillAbs (a)
        trgt

    /// <summary>Fills this tensor with the element-wise sign of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-2; -1; 0; 2]
    /// a.FillSgn a // a = [-1; -1; 0; 1]
    /// </code></example>
    /// <remarks>Computes the sign of each element of the specified tensor and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Sgn"/>
    member trgt.FillSgn (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Sgn (trgt=trgt, src1=a)

    /// <summary>Element-wise sign.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-2; -1; 0; 2]
    /// let b = sgn a // b = [-1; -1; 0; 1]
    /// </code></example>
    /// <remarks>Computes the sign of each element of the specified tensor and returns them as a new tensor.
    /// The type of the returned tensor matches the type of the argument tensor.
    /// Do not call this function directly; instead use the F# <c>sgn</c> function.</remarks>
    /// <seealso cref="FillSgn"/>
    static member Sgn (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillSgn (a)
        trgt

    /// <summary>Fills this tensor with the element-wise natural logarithm of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 2.71828; 4.0]
    /// a.FillLog a // a = [0.0; 1.0; 1.38529]
    /// </code></example>
    /// <remarks>Computes the natural logarithm of each element of the specified tensor and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Log"/>
    member trgt.FillLog (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Log (trgt=trgt, src1=a)

    /// <summary>Element-wise natural logarithm.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 2.71828; 4.0]
    /// let b = log a // b = [0.0; 1.0; 1.38529]
    /// </code></example>
    /// <remarks>Computes the natural logarithm of each element of the specified tensor and returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>log</c> function.</remarks>
    /// <seealso cref="FillLog"/><seealso cref="Log10"/><seealso cref="Exp"/>
    static member Log (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillLog (a)
        trgt

    /// <summary>Fills this tensor with the element-wise common logarithm of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 10.0; 100.0]
    /// a.FillLog a // a = [0.0; 1.0; 2.0]
    /// </code></example>
    /// <remarks>Computes the common logarithm (to base 10) of each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Log10"/>
    member trgt.FillLog10 (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Log10 (trgt=trgt, src1=a)

    /// <summary>Element-wise common logarithm.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 10.0; 100.0]
    /// let b = log10 a // b = [0.0; 1.0; 2.0]
    /// </code></example>
    /// <remarks>Computes the common logarithm (to base 10) of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>log10</c> function.</remarks>
    /// <seealso cref="FillLog10"/><seealso cref="Log"/>
    static member Log10 (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillLog10 (a)
        trgt

    /// <summary>Fills this tensor with the element-wise exponential function of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0; 10.0]
    /// a.FillExp a // a = [0.36787; 1.0; 2.71828; 22026.4657]
    /// </code></example>
    /// <remarks>Computes the exponential function of each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Exp"/>
    member trgt.FillExp (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Exp (trgt=trgt, src1=a)

    /// <summary>Element-wise exponential function.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0; 10.0]
    /// let b = exp a // b = [0.36787; 1.0; 2.71828; 22026.4657]
    /// </code></example>
    /// <remarks>Computes the exponential function of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>exp</c> function.</remarks>
    /// <seealso cref="FillExp"/><seealso cref="Log"/>
    static member Exp (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillExp (a)
        trgt

    /// <summary>Fills this tensor with the element-wise sine of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillSin a // a = [-1.0; 0.0; 1.0]
    /// </code></example>
    /// <remarks>Computes the sine each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Sin"/>
    member trgt.FillSin (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Sin (trgt=trgt, src1=a)

    /// <summary>Element-wise sine.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = sin a // b = [-1.0; 0.0; 1.0]
    /// </code></example>
    /// <remarks>Computes the sine of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>sin</c> function.</remarks>
    /// <seealso cref="FillSin"/><seealso cref="Asin"/>
    static member Sin (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillSin (a)
        trgt

    /// <summary>Fills this tensor with the element-wise cosine of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillCos a // a = [0.0; 1.0; 0.0]
    /// </code></example>
    /// <remarks>Computes the cosine each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Cos"/>
    member trgt.FillCos (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Cos (trgt=trgt, src1=a)

    /// <summary>Element-wise cosine.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = cos a // b = [0.0; 1.0; 0.0]
    /// </code></example>
    /// <remarks>Computes the cosine of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>cos</c> function.</remarks>
    /// <seealso cref="FillCos"/><seealso cref="Acos"/>
    static member Cos (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillCos (a)
        trgt

    /// <summary>Fills this tensor with the element-wise tangent of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillTan a // a = [-158057.9134; 0.0; 158057.9134]
    /// </code></example>
    /// <remarks>Computes the tangent of each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Tan"/>
    member trgt.FillTan (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Tan (trgt=trgt, src1=a)

    /// <summary>Element-wise tangent.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = tan a // b = [-158057.9134; 0.0; 158057.9134]
    /// </code></example>
    /// <remarks>Computes the tangent of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>tan</c> function.</remarks>
    /// <seealso cref="FillTan"/><seealso cref="Atan"/>
    static member Tan (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillTan (a)
        trgt

    /// <summary>Fills this tensor with the element-wise arcsine (inverse sine) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// a.FillAsin a // a = [-1.57079; 0.0; 1.57079]
    /// </code></example>
    /// <remarks>Computes the arcsine (inverse sine) each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Asin"/>
    member trgt.FillAsin (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Asin (trgt=trgt, src1=a)

    /// <summary>Element-wise arcsine (inverse sine).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// let b = asin a // b = [-1.57079; 0.0; 1.57079]
    /// </code></example>
    /// <remarks>Computes the arcsine (inverse sine) of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>asin</c> function.</remarks>
    /// <seealso cref="FillAsin"/><seealso cref="Sin"/>
    static member Asin (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillAsin (a)
        trgt

    /// <summary>Fills this tensor with the element-wise arccosine (inverse cosine) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// a.FillAcos a // a = [3.15159; 1.57079; 0.0]
    /// </code></example>
    /// <remarks>Computes the arccosine (inverse cosine) each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Acos"/>
    member trgt.FillAcos (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Acos (trgt=trgt, src1=a)

    /// <summary>Element-wise arccosine (inverse cosine).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// let b = acos a // b = [3.15159; 1.57079; 0.0]
    /// </code></example>
    /// <remarks>Computes the arccosine (inverse cosine) of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>acos</c> function.</remarks>
    /// <seealso cref="FillAcos"/><seealso cref="Cos"/>
    static member Acos (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillAcos (a)
        trgt

    /// <summary>Fills this tensor with the element-wise arctanget (inverse tangent) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// a.FillAtan a // a = [-0.78539; 0.0; 0.78539]
    /// </code></example>
    /// <remarks>Computes the arctanget (inverse tangent) each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Atan"/>
    member trgt.FillAtan (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Atan (trgt=trgt, src1=a)

    /// <summary>Element-wise arctanget (inverse tangent).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.0; 0.0; 1.0]
    /// let b = atan a // b = [-0.78539; 0.0; 0.78539]
    /// </code></example>
    /// <remarks>Computes the arctanget (inverse tangent) of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>atan</c> function.</remarks>
    /// <seealso cref="FillAcos"/><seealso cref="Tan"/>
    static member Atan (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillAtan (a)
        trgt

    /// <summary>Fills this tensor with the element-wise hyperbolic sine of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillSinh a // a = [-2.30128; 0.0; 2.30128]
    /// </code></example>
    /// <remarks>Computes the hyperbolic sine each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Sinh"/>
    member trgt.FillSinh (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Sinh (trgt=trgt, src1=a)

    /// <summary>Element-wise hyperbolic sine.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = sinh a // b = [-2.30128; 0.0; 2.30128]
    /// </code></example>
    /// <remarks>Computes the hyperbolic sine of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>sinh</c> function.</remarks>
    /// <seealso cref="FillSinh"/>
    static member Sinh (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillSinh (a)
        trgt

    /// <summary>Fills this tensor with the element-wise hyperbolic cosine of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillCosh a // a = [2.50916; 1.0; 2.50916]
    /// </code></example>
    /// <remarks>Computes the hyperbolic cosine each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Cosh"/>
    member trgt.FillCosh (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Cosh (trgt=trgt, src1=a)

    /// <summary>Element-wise hyperbolic cosine.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = cosh a // b = [2.50916; 1.0; 2.50916]
    /// </code></example>
    /// <remarks>Computes the hyperbolic cosine of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>cosh</c> function.</remarks>
    /// <seealso cref="FillCosh"/>
    static member Cosh (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillCosh (a)
        trgt

    /// <summary>Fills this tensor with the element-wise hyperbolic tangent of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// a.FillTanh a // a = [-0.91715; 0.0; 0.91715]
    /// </code></example>
    /// <remarks>Computes the hyperbolic tangent of each element of the specified tensor and writes them 
    /// into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Tanh"/>
    member trgt.FillTanh (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Tanh (trgt=trgt, src1=a)

    /// <summary>Element-wise hyperbolic tangent.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-1.57079; 0.0; 1.57079]
    /// let b = tanh a // b = [-0.91715; 0.0; 0.91715]
    /// </code></example>
    /// <remarks>Computes the hyperbolic tangent of each element of the specified tensor and returns them 
    /// as a new tensor.
    /// Do not call this function directly; instead use the F# <c>tanh</c> function.</remarks>
    /// <seealso cref="FillTanh"/>
    static member Tanh (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillTanh (a)
        trgt

    /// <summary>Fills this tensor with the element-wise square root of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 4.0; 16.0]
    /// a.FillSqrt a // a = [1.0; 2.0; 4.0]
    /// </code></example>
    /// <remarks>Computes the square root of each element of the specified tensor and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Sqrt"/>
    member trgt.FillSqrt (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Sqrt (trgt=trgt, src1=a)

    /// <summary>Element-wise square root.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [1.0; 4.0; 16.0]
    /// let b = sqrt a // b = [1.0; 2.0; 4.0]
    /// </code></example>
    /// <remarks>Computes the square root of each element of the specified tensor and returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>sqrt</c> function.</remarks>
    /// <seealso cref="FillSqrt"/>
    static member Sqrt (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillSqrt (a)
        trgt

    /// <summary>Fills this tensor with the element-wise ceiling (round towards positive infinity) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// a.FillCeiling a // a = [-3.0; -2.0; 3.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the ceiling (round towards positive infinity) of each element of the specified tensor 
    /// and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Ceiling"/>
    member trgt.FillCeiling (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Ceiling (trgt=trgt, src1=a)

    /// <summary>Element-wise ceiling (round towards positive infinity).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// let b = ceil a // b = [-3.0; -2.0; 3.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the ceiling (round towards positive infinity) of each element of the specified tensor and 
    /// returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>ceil</c> function.</remarks>
    /// <seealso cref="FillCeiling"/>
    static member Ceiling (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillCeiling (a)
        trgt

    /// <summary>Fills this tensor with the element-wise floor (round towards negative infinity) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// a.FillCeiling a // a = [-3.0; -3.0; 2.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the ceiling (round towards negative infinity) of each element of the specified tensor 
    /// and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Floor"/>
    member trgt.FillFloor (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Floor (trgt=trgt, src1=a)

    /// <summary>Element-wise floor (round towards negative infinity).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// let b = floor a // b = [-3.0; -3.0; 2.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the floor (round towards negative infinity) of each element of the specified tensor and 
    /// returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>floor</c> function.</remarks>
    /// <seealso cref="FillFloor"/>
    static member Floor (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillFloor (a)
        trgt

    /// <summary>Fills this tensor with the element-wise rounding of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// a.FillRound a // a = [-3.0; -3.0; 3.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the rounding of each element of the specified tensor and writes them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Round"/>
    member trgt.FillRound (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Round (trgt=trgt, src1=a)

    /// <summary>Element-wise rounding.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// let b = round a // b = [-3.0; -3.0; 3.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the rounding of each element of the specified tensor and returns them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>round</c> function.</remarks>
    /// <seealso cref="FillRound"/>
    static member Round (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillRound (a)
        trgt

    /// <summary>Fills this tensor with the element-wise truncation (rounding towards zero) of the argument.</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// a.FillTruncate a // a = [-3.0; -2.0; 2.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the truncation (rounding towards zero) of each element of the specified tensor and writes 
    /// them into this tensor.
    /// This tensor and <paramref name="a"/> must have the same shape, type and storage.</remarks>
    /// <seealso cref="Truncate"/>
    member trgt.FillTruncate (a: Tensor<'T>) = 
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Truncate (trgt=trgt, src1=a)

    /// <summary>Element-wise truncation (rounding towards zero).</summary>
    /// <param name="a">The tensor to apply this operation to.</param>
    /// <returns>A new tensor containing the result of this operation.</returns>
    /// <example><code language="fsharp">
    /// let a = HostTensor.ofList [-3.0; -2.7; 2.7; 3.0]
    /// let b = truncate a // b = [-3.0; -2.0; 2.0; 3.0]
    /// </code></example>
    /// <remarks>Computes the truncation (rounding towards zero) of each element of the specified tensor and returns
    /// them as a new tensor.
    /// Do not call this function directly; instead use the F# <c>truncate</c> function.</remarks>
    /// <seealso cref="FillTruncate"/>
    static member Truncate (a: Tensor<'T>) = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillTruncate (a)
        trgt

    /// element-wise check if elements are finite (not -Inf, Inf or NaN) using this tensor as target
    member trgt.FillIsFinite (a: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        a.Backend.IsFinite (trgt=trgt, src1=a)

    /// element-wise check if elements are finite (not -Inf, Inf or NaN)
    static member isFinite (a: Tensor<'T>) : Tensor<bool> = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillIsFinite (a)
        trgt

    /// element-wise logical negation using this tensor as target
    member trgt.FillNegate (a: Tensor<bool>) = 
        let trgt = trgt.AsBool
        let a = Tensor.PrepareElemwiseSources (trgt, a)
        trgt.Backend.Negate (trgt=trgt, src1=a)

    /// element-wise logical negation
    static member (~~~~) (a: Tensor<bool>) : Tensor<bool> = 
        let trgt, a = Tensor.PrepareElemwise (a)
        trgt.FillNegate (a)
        trgt

    /// element-wise addition of two tensors using this tensor as target
    member trgt.FillAdd (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Add (trgt=trgt, src1=a, src2=b)
   
    /// element-wise addition of two tensors
    static member (+) (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillAdd a b
        trgt
    static member (+) (a: Tensor<'T>, b: 'T) = a + Tensor.scalarLike a b
    static member (+) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a + b

    /// element-wise subtraction of two tensors using this tensor as target
    member trgt.FillSubtract (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Subtract (trgt=trgt, src1=a, src2=b)

    /// element-wise subtraction of two tensors
    static member (-) (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillSubtract a b
        trgt
    static member (-) (a: Tensor<'T>, b: 'T) = a - Tensor.scalarLike a b
    static member (-) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a - b

    /// element-wise multiplication of two tensors using this tensor as target
    member trgt.FillMultiply (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Multiply (trgt=trgt, src1=a, src2=b)

    /// element-wise multiplication of two tensor
    static member (*) (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillMultiply a b
        trgt
    static member (*) (a: Tensor<'T>, b: 'T) = a * Tensor.scalarLike a b
    static member (*) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a * b

    /// element-wise division of two tensors using this tensor as target
    member trgt.FillDivide (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Divide (trgt=trgt, src1=a, src2=b)

    /// element-wise division of two tensors
    static member (/) (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillDivide a b
        trgt
    static member (/) (a: Tensor<'T>, b: 'T) = a / Tensor.scalarLike a b
    static member (/) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a / b

    /// element-wise modulo of two tensors using this tensor as target
    member trgt.FillModulo (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Modulo (trgt=trgt, src1=a, src2=b)

    /// element-wise modulo of two tensors
    static member (%) (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillModulo a b
        trgt
    static member (%) (a: Tensor<'T>, b: 'T) = a % Tensor.scalarLike a b
    static member (%) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a % b

    /// element-wise power of two tensors using this tensor as target
    member trgt.FillPower (a: Tensor<'T>) (b: Tensor<'T>) = 
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.Power (trgt=trgt, src1=a, src2=b)

    /// element-wise power of two tensors
    static member Pow (a: Tensor<'T>, b: Tensor<'T>) = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillPower a b
        trgt
    static member Pow (a: Tensor<'T>, b: 'T) = a ** Tensor.scalarLike a b
    static member Pow (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a ** b

    /// element-wise logical "and" of two tensors using this tensor as target
    member trgt.FillAnd (a: Tensor<bool>) (b: Tensor<bool>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        trgt.Backend.And (trgt=trgt, src1=a, src2=b)

    /// element-wise logical "and"
    static member (&&&&) (a: Tensor<bool>, b: Tensor<bool>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillAnd a b
        trgt
    static member (&&&&) (a: Tensor<bool>, b: bool) = a &&&& Tensor.scalarLike a b
    static member (&&&&) (a: bool, b: Tensor<bool>) = Tensor.scalarLike b a &&&& b
    
    /// element-wise logical "or" of two tensors using this tensor as target
    member trgt.FillOr (a: Tensor<bool>) (b: Tensor<bool>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        trgt.Backend.Or (trgt=trgt, src1=a, src2=b)

    /// element-wise logical "or"
    static member (||||) (a: Tensor<bool>, b: Tensor<bool>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillOr a b
        trgt
    static member (||||) (a: Tensor<bool>, b: bool) = a |||| Tensor.scalarLike a b
    static member (||||) (a: bool, b: Tensor<bool>) = Tensor.scalarLike b a |||| b

    /// element-wise logical "xor" of two tensors using this tensor as target
    member trgt.FillXor (a: Tensor<bool>) (b: Tensor<bool>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        trgt.Backend.Xor (trgt=trgt, src1=a, src2=b)

    /// element-wise logical "xor"
    static member (^^^^) (a: Tensor<bool>, b: Tensor<bool>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillXor a b
        trgt
    static member (^^^^) (a: Tensor<bool>, b: bool) = a ^^^^ Tensor.scalarLike a b
    static member (^^^^) (a: bool, b: Tensor<bool>) = Tensor.scalarLike b a ^^^^ b

    /// element-wise equal of two tensors using this tensor as target
    member trgt.FillEqual (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.Equal (trgt=trgt, src1=a, src2=b)

    /// element-wise equal
    static member (====) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillEqual a b
        trgt
    static member (====) (a: Tensor<'T>, b: 'T) = a ==== Tensor.scalarLike a b
    static member (====) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a ==== b

    /// element-wise not equal of two tensors using this tensor as target
    member trgt.FillNotEqual (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.NotEqual (trgt=trgt, src1=a, src2=b)

    /// element-wise not equal
    static member (<<>>) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillNotEqual a b
        trgt
    static member (<<>>) (a: Tensor<'T>, b: 'T) = a <<>> Tensor.scalarLike a b
    static member (<<>>) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a <<>> b

    /// element-wise less than of two tensors using this tensor as target
    member trgt.FillLess (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.Less (trgt=trgt, src1=a, src2=b)

    /// element-wise less than
    static member (<<<<) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillLess a b
        trgt
    static member (<<<<) (a: Tensor<'T>, b: 'T) = a <<<< Tensor.scalarLike a b
    static member (<<<<) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a <<<< b

    /// element-wise less than of two tensors using this tensor as target
    member trgt.FillLessOrEqual (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.LessOrEqual (trgt=trgt, src1=a, src2=b)

    /// element-wise less than or equal to
    static member (<<==) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillLessOrEqual a b
        trgt
    static member (<<==) (a: Tensor<'T>, b: 'T) = a <<== Tensor.scalarLike a b
    static member (<<==) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a <<== b

    /// element-wise greater than of two tensors using this tensor as target
    member trgt.FillGreater (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.Greater (trgt=trgt, src1=a, src2=b)

    /// element-wise greater than
    static member (>>>>) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillGreater a b
        trgt
    static member (>>>>) (a: Tensor<'T>, b: 'T) = a >>>> Tensor.scalarLike a b
    static member (>>>>) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a >>>> b

    /// element-wise greater than or equal to of two tensors using this tensor as target
    member trgt.FillGreaterOrEqual (a: Tensor<'R>) (b: Tensor<'R>) = 
        let trgt = trgt.AsBool
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)           
        a.Backend.GreaterOrEqual (trgt=trgt, src1=a, src2=b)

    /// element-wise greater than or equal to
    static member (>>==) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<bool> = 
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillGreaterOrEqual a b
        trgt
    static member (>>==) (a: Tensor<'T>, b: 'T) = a >>== Tensor.scalarLike a b
    static member (>>==) (a: 'T, b: Tensor<'T>) = Tensor.scalarLike b a >>== b

    /// element-wise picks the maximum of a or b using this tensor as target
    member trgt.FillMaxElemwise (a: Tensor<'T>) (b: Tensor<'T>) =
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.MaxElemwise (trgt=trgt, src1=a, src2=b)

    /// element-wise picks the maximum of a or b
    static member maxElemwise (a: Tensor<'T>) (b: Tensor<'T>) =
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillMaxElemwise a b
        trgt

    /// element-wise picks the minimum of a or b using this tensor as target
    member trgt.FillMinElemwise (a: Tensor<'T>) (b: Tensor<'T>) =
        let a, b = Tensor.PrepareElemwiseSources (trgt, a, b)
        trgt.Backend.MinElemwise (trgt=trgt, src1=a, src2=b)

    /// element-wise picks the minimum of a or b
    static member minElemwise (a: Tensor<'T>) (b: Tensor<'T>) =
        let trgt, a, b = Tensor.PrepareElemwise (a, b)
        trgt.FillMinElemwise a b
        trgt

    /// Elementwise writes elements from ifTrue if cond is true in this tensor, 
    /// otherwise elements from ifFalse.
    member trgt.FillIfThenElse (cond: Tensor<bool>) (ifTrue: Tensor<'T>) (ifFalse: Tensor<'T>) = 
        let cond, ifTrue, ifFalse = Tensor.PrepareElemwiseSources (trgt, cond, ifTrue, ifFalse)
        trgt.Backend.IfThenElse (trgt=trgt, cond=cond, ifTrue=ifTrue, ifFalse=ifFalse)

    /// Elementwise takes elements from ifTrue if cond is true, 
    /// otherwise elements from ifFalse.
    static member ifThenElse (cond: Tensor<bool>) (ifTrue: Tensor<'T>) (ifFalse: Tensor<'T>) =
        let trgt, cond, ifTrue, ifFalse = Tensor.PrepareElemwise(cond, ifTrue, ifFalse)
        trgt.FillIfThenElse cond ifTrue ifFalse
        trgt

    /// Selects elements from src according to the specified indices.
    /// Indices must be a list of Tensor<int64> options, one per dimension of src. 
    /// If None is specified in an dimension, the source index will match the 
    /// target index in that dimension.
    member trgt.FillGather (indices: Tensor<int64> option list) (src: Tensor<'T>) =
        Tensor.CheckSameStorage ([src :> ITensor] @ 
            List.choose (Option.map (fun t -> t :> ITensor)) indices)
        if src.NDims <> indices.Length then
            invalidArg "indices" "for each dimension of src an index tensor must be specified"        
        if indices |> List.skip trgt.NDims |> List.exists Option.isNone then
            invalidArg "indices" "index dimensions beyond the number of target dimensions must not be None"
        let indices = indices |> List.map (Option.map (fun t -> t |> Tensor<_>.broadcastTo trgt.Shape :> ITensorFrontend<_>))
        trgt.Backend.Gather (trgt=trgt, srcIdxs=indices, src=src)

    /// Creates a new tensor by selecting elements from src according to the specified indices.
    /// Indices must be a list of Tensor<int64> options, one per dimension of src. 
    /// If None is specified in an dimension, the source index will match the 
    /// target index in that dimension.
    /// The result will have the shape of the (broadcasted) index tensors.
    static member gather (indices: Tensor<int64> option list) (src: Tensor<'T>) =
        // broadcast specified indices to same shape
        let specIndices = indices |> List.choose id
        if List.isEmpty specIndices then
            invalidArg "indicies" "at least one index tensor must not be None"
        let bcSpecIndices = Tensor<_>.broadcastToSame specIndices
        let rec rebuild idxs repIdxs =
            match idxs, repIdxs with
            | Some idx :: rIdxs, repIdx :: rRepIdxs -> Some repIdx :: rebuild rIdxs rRepIdxs
            | None :: rIdxs, _ -> None :: rebuild rIdxs repIdxs
            | [], [] -> []
            | _ -> failwith "unbalanced idxs"
        let bcIndices = rebuild indices bcSpecIndices

        // apply gather
        let trgt = Tensor<'T> (bcSpecIndices.Head.Shape, src.Dev)
        trgt.FillGather bcIndices src
        trgt        

    /// Sets the values of this tensor by summing elements from the source tensor into the elements
    /// of this tensor specified by the indices.
    /// If an index tensor is set to None then the target index is used as the source index.
    member trgt.FillScatter (indices: Tensor<int64> option list) (src: Tensor<'T>) =
        Tensor.CheckSameStorage ([src :> ITensor] @ 
            List.choose (Option.map (fun t -> t :> ITensor)) indices)
        if trgt.NDims <> indices.Length then
            invalidArg "indices" "for each dimension of the target an index tensor must be specified"        
        if indices |> List.skip src.NDims |> List.exists Option.isNone then
            invalidArg "indices" "index dimensions beyond the number of source dimensions must not be None"
        let indices = indices |> List.map (Option.map (fun t -> t |> Tensor<_>.broadcastTo src.Shape :> ITensorFrontend<_>))
        trgt.Backend.FillConst (trgt=trgt, value=Tensor<'T>.Zero)
        trgt.Backend.Scatter (trgt=trgt, trgtIdxs=indices, src=src)

    /// Creates a new tensor of shape trgtShp by dispersing elements from src according to 
    /// the specified target indices.
    /// If an index occurs multiple times the corresponding values are summed.
    /// Target elements that do not occur, are set to zero.
    /// Indices must be a list of Tensor<int64> options, one per dimension of trgt and of the same shape
    /// (or broadcastable to) as src.
    /// If None is specified instead of a tensor in an dimension, the source index will match the 
    /// target index in that dimension.
    static member scatter (indices: Tensor<int64> option list) (trgtShp: int64 list) (src: Tensor<'T>) =
        let trgt = Tensor<'T> (trgtShp, src.Dev)
        trgt.FillScatter indices src
        trgt

    /// folds the function over the given axis, using this tensor as target
    member trgt.FillFoldAxis (fn: 'T -> 'TA -> 'T) (initial: Tensor<'T>) (axis: int) (a: Tensor<'TA>) =
        let a, initial = Tensor.PrepareAxisReduceSources (trgt, axis, a, Some initial)
        trgt.Backend.FoldLastAxis (fn=fn, initial=initial.Value, trgt=trgt, src=a, useThreads=false)        

    /// folds the function over the given axis, using this tensor as target and multiple threads
    member trgt.FillParallelFoldAxis (fn: 'T -> 'TA -> 'T) (initial: Tensor<'T>) (axis: int) (a: Tensor<'TA>) =
        let a, initial = Tensor.PrepareAxisReduceSources (trgt, axis, a, Some initial)
        trgt.Backend.FoldLastAxis (fn=fn, initial=initial.Value, trgt=trgt, src=a, useThreads=true) 

    /// folds the function over the given axis
    static member foldAxis (fn: 'T -> 'TA -> 'T) (initial: Tensor<'T>) (axis: int) (a: Tensor<'TA>) =
        let trgt, a = Tensor.PrepareAxisReduceTarget (axis, a)
        trgt.FillFoldAxis fn initial axis a
        trgt
        
    /// count true elements over given axis using this tensor as target
    member trgt.FillCountTrueAxis (ax: int) (src: Tensor<bool>) =
        let trgt = trgt.AsInt64
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.CountTrueLastAxis (trgt=trgt, src1=src)

    /// count true elements over given axis
    static member countTrueAxis (ax: int) (src: Tensor<bool>) : Tensor<int64> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillCountTrueAxis ax src
        trgt

    /// count true elements as tensor
    static member countTrueTensor (src: Tensor<bool>) =
        src |> Tensor<_>.flatten |> Tensor<_>.countTrueAxis 0

    /// count of all true elements
    static member countTrue (src: Tensor<bool>) =
        src |> Tensor.countTrueTensor |> Tensor.value
    
    /// Finds all elements that are true and returns their indices as a matrix.
    /// Each row correspond to a true entry in the original tensor.
    /// Each column corresponds to a dimension of the original tensor. 
    static member trueIdx (src: Tensor<bool>) : Tensor<int64> =
        let nTrue = Tensor<_>.countTrue src
        let trgt = Tensor<int64> ([nTrue; int64 src.NDims], src.Storage.Dev)
        trgt.Backend.TrueIndices (trgt=trgt, src1=src)                
        trgt

    /// sum over given axis using this tensor as target
    member trgt.FillSumAxis (ax: int) (src: Tensor<'T>) =
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.SumLastAxis (trgt=trgt, src1=src)

    /// sum over given axis
    static member sumAxis (ax: int) (src: Tensor<'T>) =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillSumAxis ax src
        trgt

    /// sum of all elements as tensor
    static member sumTensor (src: Tensor<'T>) =
        src |> Tensor<_>.flatten |> Tensor<_>.sumAxis 0

    /// sum of all elements
    static member sum (src: Tensor<'T>) =
        src |> Tensor.sumTensor |> Tensor.value

    /// product over given axis using this tensor as target
    member trgt.FillProductAxis (ax: int) (src: Tensor<'T>) =
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.ProductLastAxis (trgt=trgt, src1=src)

    /// product over given axis
    static member productAxis (ax: int) (src: Tensor<'T>) =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillProductAxis ax src
        trgt

    /// product of all elements as tensor
    static member productTensor (src: Tensor<'T>) =
        src |> Tensor<_>.flatten |> Tensor<_>.productAxis 0

    /// product of all elements
    static member product (src: Tensor<'T>) =
        src |> Tensor.productTensor |> Tensor.value

    /// minimum value over given axis using this tensor as target
    member trgt.FillMinAxis (ax: int) (src: Tensor<'T>) =
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.MinLastAxis (trgt=trgt, src1=src)

    /// minimum value over given axis
    static member minAxis (ax: int) (src: Tensor<'T>) =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillMinAxis ax src
        trgt

    /// minimum of all elements as tensor
    static member minTensor (src: Tensor<'T>) =
        src |> Tensor<_>.flatten |> Tensor<_>.minAxis 0

    /// minimum of all elements
    static member min (src: Tensor<'T>) =
        src |> Tensor.minTensor |> Tensor.value

    /// maximum value over given axis using this tensor as target
    member trgt.FillMaxAxis (ax: int) (src: Tensor<'T>) =
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.MaxLastAxis (trgt=trgt, src1=src)

    /// maximum value over given axis
    static member maxAxis (ax: int) (src: Tensor<'T>) =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillMaxAxis ax src
        trgt

    /// maximum of all elements as tensor
    static member maxTensor (src: Tensor<'T>) =
        src |> Tensor<_>.flatten |> Tensor<_>.maxAxis 0

    /// maximum of all elements
    static member max (src: Tensor<'T>) =
        src |> Tensor.maxTensor |> Tensor.value

    /// positions of minimum values along given axis using this tensor as target
    member trgt.FillArgMinAxis (ax: int) (src: Tensor<'R>) =
        let trgt = trgt.AsInt64
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        src.Backend.ArgMinLastAxis (trgt=trgt, src1=src)

    /// positions of minimum values along given axis 
    static member argMinAxis (ax: int) (src: Tensor<'T>) : Tensor<int64> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillArgMinAxis ax src
        trgt

    /// positions of maximum values along given axis using this tensor as target
    member trgt.FillArgMaxAxis (ax: int) (src: Tensor<'R>) =
        let trgt = trgt.AsInt64
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        src.Backend.ArgMaxLastAxis (trgt=trgt, src1=src)

    /// positions of maximum values along given axis 
    static member argMaxAxis (ax: int) (src: Tensor<'T>) : Tensor<int64> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillArgMaxAxis ax src
        trgt

    /// position of minimum value
    static member argMin (a: Tensor<'T>) =
        a 
        |> Tensor<_>.flatten 
        |> Tensor<_>.argMinAxis 0 
        |> Tensor.value 
        |> TensorLayout.linearToIdx a.Layout

    /// position of maximum value
    static member argMax (a: Tensor<'T>) =
        a 
        |> Tensor<_>.flatten 
        |> Tensor<_>.argMaxAxis 0 
        |> Tensor.value 
        |> TensorLayout.linearToIdx a.Layout
        
    /// Positions of first occurence of value along given axis using this tensor as target.
    /// Sets index to NotFound, if value does not occur.
    member trgt.FillFindAxis (value: 'R) (ax: int) (src: Tensor<'R>) =
        let trgt = trgt.AsInt64
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        src.Backend.FindLastAxis (value=value, trgt=trgt, src1=src)

    /// Positions of first occurence of value along given axis.
    /// Sets index to NotFound, if value does not occur. 
    static member findAxis (value: 'T) (ax: int) (src: Tensor<'T>) : Tensor<int64> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillFindAxis value ax src
        trgt

    /// Position of first occurence of value.
    /// Returns None if value was not found.
    static member tryFind (value: 'T) (a: Tensor<'T>) =
        let pos = 
            a 
            |> Tensor<_>.flatten 
            |> Tensor<_>.findAxis value 0 
            |> Tensor.value
        if pos <> NotFound then
            pos |> TensorLayout.linearToIdx a.Layout |> Some
        else None
        
    /// Position of first occurence of value.
    /// Raises an exception if value cannot be found.
    static member find (value: 'T) (a: Tensor<'T>) =
        match Tensor<_>.tryFind value a with
        | Some pos -> pos
        | None -> invalidOp "Value %A was not found in specifed tensor." value

    /// false if there is at least one false element in given axis, using this tensor as target
    member trgt.FillAllAxis (ax: int) (src: Tensor<bool>) =
        let trgt = trgt.AsBool
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.AllLastAxis (trgt=trgt, src1=src)

    /// false if there is at least one false element in given axis, otherwise true
    static member allAxis (ax: int) (src: Tensor<bool>) : Tensor<bool> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillAllAxis ax src
        trgt

    /// False if there is at least one false element in the tensor, otherwise true.
    /// Returns value as Tensor<bool>.
    static member allTensor (src: Tensor<bool>) =
        src |> Tensor<_>.flatten |> Tensor<_>.allAxis 0 

    /// false if there is at least one false element in the tensor, otherwise true
    static member all (src: Tensor<bool>) =
        src |> Tensor.allTensor |> Tensor.value

    /// true if there is at least one true element in given axis, using this tensor as target
    member trgt.FillAnyAxis (ax: int) (src: Tensor<bool>) =
        let trgt = trgt.AsBool
        let src, _ = Tensor.PrepareAxisReduceSources (trgt, ax, src, None)
        trgt.Backend.AnyLastAxis (trgt=trgt, src1=src)

    /// true if there is at least one true element in given axis, otherwise false
    static member anyAxis (ax: int) (src: Tensor<bool>) : Tensor<bool> =
        let trgt, src = Tensor.PrepareAxisReduceTarget (ax, src)
        trgt.FillAnyAxis ax src
        trgt

    /// true if there is at least one true element in the tensor, otherwise false
    /// Returns value as Tensor<bool>.
    static member anyTensor (src: Tensor<bool>) =
        src |> Tensor<_>.flatten |> Tensor<_>.anyAxis 0 

    /// true if there is at least one true element in the tensor, otherwise false
    static member any (src: Tensor<bool>) =
        src |> Tensor.anyTensor |> Tensor.value

    /// Dot product of two tensors using this tensor as target:
    /// vec*vec=>scalar, mat*vec=>vec, mat*mat=>mat, (batched mat)*(batched mat)=>(batched mat).
    member trgt.FillDot (a: Tensor<'T>) (b: Tensor<'T>) = 
        Tensor.CheckSameStorage [trgt; a; b]
        match trgt.NDims, a.NDims, b.NDims with
        | 0, 1, 1 when a.Shape = b.Shape -> 
            trgt.Backend.VecVecDot (trgt, a, b)
        | 1, 2, 1 when trgt.Shape.[0] = a.Shape.[0] && a.Shape.[1] = b.Shape.[0] -> 
            trgt.Backend.MatVecDot (trgt, a, b)
        | 2, 2, 2 when trgt.Shape.[0] = a.Shape.[0] && trgt.Shape.[1] = b.Shape.[1] &&
                       a.Shape.[1] = b.Shape.[0] ->
            trgt.Backend.MatMatDot (trgt, a, b)
        | nt, na, nb when na > 2 && nt = na && na = nb && a.Shape.[na-1] = b.Shape.[na-2] ->
            let a = a |> Tensor.broadcastTo (trgt.Shape.[0 .. na-3] @ a.Shape.[na-2 ..])
            let b = b |> Tensor.broadcastTo (trgt.Shape.[0 .. na-3] @ b.Shape.[na-2 ..])
            trgt.Backend.BatchedMatMatDot (trgt, a, b)
        | _ -> 
            invalidOp "Cannot compute dot product between tensors of shapes %A and %A 
                       into tensor of shape %A." a.Shape b.Shape trgt.Shape

    /// Dot product of two tensors:
    /// vec*vec=>scalar, mat*vec=>vec, mat*mat=>mat, (batched mat)*(batched mat)=>(batched mat),
    /// (batched mat)*(batched vec)=>(batched vec).
    /// Broadcasting is applied over batch dimensions.
    static member (.*) (a: Tensor<'T>, b: Tensor<'T>) : Tensor<'T> = 
        Tensor.CheckSameStorage [a; b]
        match a.NDims, b.NDims with
        | 1, 1 when a.Shape = b.Shape -> 
            let trgt = Tensor<'T> ([], a.Dev)
            trgt.FillDot a b
            trgt
        | 2, 1 when a.Shape.[1] = b.Shape.[0] -> 
            let trgt = Tensor<'T> ([a.Shape.[0]], a.Dev)
            trgt.FillDot a b
            trgt
        | 2, 2 when a.Shape.[1] = b.Shape.[0] -> 
            let trgt = Tensor<'T> ([a.Shape.[0]; b.Shape.[1]], a.Dev)
            trgt.FillDot a b
            trgt
        | na, nb when na > 2 && na = nb && a.Shape.[na-1] = b.Shape.[na-2] ->
            let a, b = Tensor.broadcastToSameInDims ([0 .. na-3], a, b)
            let trgt = Tensor<'T> (a.Shape.[0 .. na-3] @ [a.Shape.[na-2]; b.Shape.[na-1]], a.Dev)
            trgt.FillDot a b
            trgt
        | na, nb when na > 2 && na = nb+1 && a.Shape.[na-1] = b.Shape.[nb-1] ->
            let bPad = Tensor.padRight b
            let resPad = a .* bPad
            resPad.[Fill, 0L]
        | _ -> 
            invalidOp "Cannot compute dot product between tensors of shapes %A and %A." a.Shape b.Shape 

    /// Dot product of two tensors:
    /// vec*vec=>scalar, mat*vec=>vec, mat*mat=>mat, (batched mat)*(batched mat)=>(batched mat),
    /// (batched mat)*(batched vec)=>(batched vec).
    /// Broadcasting is applied over batch dimensions.
    static member dot (a: Tensor<'T>) (b: Tensor<'T>) =
        a .* b        

    /// Matrix inversion using this tensor as target.
    /// If the specified tensor has more than two dimensions, the matrices
    /// consisting of the last two dimensions are inverted.
    member trgt.FillInvert (a: Tensor<'T>)  = 
        Tensor.CheckSameStorage [trgt; a]
        if a.NDims < 2 then
            invalidArg "a" "Need at least a matrix to invert but got shape %A." a.Shape
        let a = a |> Tensor.broadcastTo trgt.Shape
        trgt.Backend.BatchedInvert (trgt, a)

    /// Matrix inversion.
    /// If the specified tensor has more than two dimensions, the matrices
    /// consisting of the last two dimensions are inverted.
    static member invert (a: Tensor<'T>) = 
        let trgt = Tensor<'T> (a.Shape, a.Dev)
        trgt.FillInvert a
        trgt

    /// Helper function to compute SVD sizes.
    static member internal SVDSizes (a: ITensorFrontend<'T>) =
        if a.NDims < 2 then
            invalidArg "a" "Need at least a matrix to SVD but got shape %A." a.Shape
        let batchShp = a.Shape.[0 .. a.NDims-3]
        let M, N = a.Shape.[a.NDims-2], a.Shape.[a.NDims-1]
        let K = min M N
        batchShp, M, N, K

    /// Singular value decomposition so that a = U .* S .* V.T with trgtUV=(U,V).
    member trgtS.FillSVD (a: Tensor<'T>, ?trgtUV: Tensor<'T> * Tensor<'T>) =
        let batchShp, M, N, K = Tensor.SVDSizes a
        Tensor.CheckSameStorage [trgtS; a]
        if trgtS.Shape <> batchShp @ [K] then
            invalidArg "trgtS" "Need a tensor of shape %A for SVD singular values but got shape %A."
                               (batchShp @ [K]) trgtS.Shape
        match trgtUV with
        | Some (trgtU, trgtV) -> 
            Tensor.CheckSameStorage [trgtS; a; trgtU; trgtV]
            if trgtU.Shape <> batchShp @ [M; M] then
                invalidArg "trgtUV" "Need a tensor of shape %A for SVD left unitary matrices but got shape %A."
                                    (batchShp @ [M; M]) trgtU.Shape
            if trgtV.Shape <> batchShp @ [N; N] then
                invalidArg "trgtUV" "Need a tensor of shape %A for SVD right unitary matrices but got shape %A."
                                    (batchShp @ [N; N]) trgtV.Shape
        | None -> ()
        let trgtUV = trgtUV |> Option.map (fun (trgtU, trgtV) -> trgtU :> ITensorFrontend<_>, 
                                                                 trgtV :> ITensorFrontend<_>)
        trgtS.Backend.BatchedSVD (trgtS, trgtUV, a)                

    /// Singular value decomposition returning (U, S, V) so that a = U .* diagMat(S) .* V.T.    
    static member SVD (a: Tensor<'T>) =
        let batchShp, M, N, K = Tensor.SVDSizes a
        let U = Tensor<'T> (batchShp @ [M;M], a.Dev, order=ColumnMajor)
        let S = Tensor<'T> (batchShp @ [K], a.Dev, order=ColumnMajor)
        let V = Tensor<'T> (batchShp @ [N;N], a.Dev, order=RowMajor)
        S.FillSVD(a, trgtUV=(U, V))
        U, S, V

    /// Singular value decomposition returning S so that a = U .* diagMat(S) .* V.T.
    static member SVDWithoutUV (a: Tensor<'T>) =
        let batchShp, M, N, K = Tensor.SVDSizes a
        let S = Tensor<'T> (batchShp @ [K], a.Dev, order=ColumnMajor)
        S.FillSVD(a)
        S

    /// Matrix pseudo inversion using this tensor as target.
    /// If the specified tensor has more than two dimensions, the matrices
    /// consisting of the last two dimensions are inverted.
    member trgt.FillPseudoInvert (a: Tensor<'T>, ?rCond: 'T)  = 
        let rCond = defaultArg rCond (conv<'T> 1e-15)
        Tensor.CheckSameStorage [trgt; a]
        if a.NDims < 2 then
            invalidArg "a" "Need at least a matrix to pseudo invert but got shape %A." a.Shape
        let a = a |> Tensor.broadcastTo trgt.Shape

        let u, s, v = Tensor.SVD a
        let rCond = Tensor.scalarLike s rCond
        let zero = Tensor.scalarLike s (conv<'T> 0)
        let one = Tensor.scalarLike s (conv<'T> 1)
        s.FillIfThenElse (s >>== rCond) (one / s) (zero)
        trgt.FillDot (v) (Tensor.padRight s * u.T)

    /// Matrix pseudo inversion.
    /// If the specified tensor has more than two dimensions, the matrices
    /// consisting of the last two dimensions are inverted.
    static member pseudoInvert (a: Tensor<'T>, ?rCond: 'T) = 
        let trgt = Tensor<'T> (a.Shape, a.Dev)
        trgt.FillPseudoInvert (a, ?rCond=rCond)
        trgt

    /// Computes the (real) eigenvalues and eigenvectors of the symmetric matrix.
    /// Returns (vals, vecs) where each column of 'vecs' is the eigenvector for the
    /// corresponding eigenvalue in 'vals'.
    static member FillSymmetricEigenDecomposition (part: MatrixPart)
            (trgtEigVals: Tensor<'T>) (trgtEigVecs: Tensor<'T>) (a: Tensor<'T>) =
        Tensor.CheckSameStorage [trgtEigVals; trgtEigVecs; a]
        if a.NDims <> 2 || a.Shape.[0] <> a.Shape.[1] then 
            invalidArg "a" "Require a square matrix for symmetric eigen-decomposition but got %A." a.Shape
        if trgtEigVecs.Shape <> a.Shape then
            invalidArg "trgtEigVecs" "trgtEigVecs and src must have the same shapes but got %A and %A." 
                                     trgtEigVecs.Shape a.Shape
        if trgtEigVals.NDims <> 1 || trgtEigVals.Shape.[0] <> a.Shape.[0] then
            invalidArg "trgtEigVals" "trgtEigVals must be a vector of length %d but it has shape %A."
                                     a.Shape.[0] trgtEigVals.Shape
        trgtEigVals.Backend.SymmetricEigenDecomposition (part, trgtEigVals, trgtEigVecs, a)

    /// Computes the (real) eigenvalues and eigenvectors of the symmetric matrix.
    /// Returns (vals, vecs) where each column of 'vecs' is the eigenvector for the
    /// corresponding eigenvalue in 'vals'.
    static member symmetricEigenDecomposition (part: MatrixPart) (a: Tensor<'T>) =
        if a.NDims <> 2 then
            invalidArg "a" "require a square matrix for symmetric eigen-decomposition"
        let trgtEigVals = Tensor<'T> ([a.Shape.[0]], a.Dev)
        let trgtEigVecs = Tensor<'T> (a.Shape, a.Dev, order=ColumnMajor)
        Tensor.FillSymmetricEigenDecomposition part trgtEigVals trgtEigVecs a
        trgtEigVals, trgtEigVecs
        
    /// a view of this tensor with the given .NET range
    member inline internal this.GetRng (rngArgs: obj[]) =
        this.Range (Rng.ofItemOrSliceArgs rngArgs) 
    member inline internal this.IGetRng (rngArgs: obj[]) =
        this.GetRng rngArgs :> ITensor
    member inline internal this.GetRngWithRest (rngArgs: obj[]) (restArgs: obj[]) =
        Array.concat [rngArgs; restArgs] |> this.GetRng
    member inline internal this.IGetRngWithRest (rngArgs: obj[]) (restArgs: obj[]) =
        Array.concat [rngArgs; restArgs] |> this.IGetRng

    /// write into the view of this tensor with the given .NET range
    member inline internal this.SetRng (rngArgs: obj[]) (value: Tensor<'T>) =
        Tensor.CheckSameStorage [this; value]
        let trgt = this.Range (Rng.ofItemOrSliceArgs rngArgs) 
        value |> Tensor<_>.broadcastTo trgt.Shape |> trgt.CopyFrom
    member inline internal this.ISetRng (rngArgs: obj[]) (value: ITensor) =
        match value with
        | :? Tensor<'T> as value -> this.SetRng rngArgs value
        | _ ->
            invalidOp "Cannot assign data type %s to tensor of data type %s." value.DataType.Name this.DataType.Name
    member inline internal this.SetRngWithRest (rngArgs: obj[]) (restArgs: obj[]) =
        let allArgs = Array.concat [rngArgs; restArgs]
        let value = Array.last allArgs :?> Tensor<'T>
        let args = allArgs.[0 .. allArgs.Length-2]
        this.SetRng args value
    member inline internal this.ISetRngWithRest (rngArgs: obj[]) (restArgs: obj[]) =
        let allArgs = Array.concat [rngArgs; restArgs]
        let value = Array.last allArgs :?> ITensor
        let args = allArgs.[0 .. allArgs.Length-2]
        this.ISetRng args value

    /// Computes the shape of the targets and sources of a mask operation. 
    static member internal MaskShapes (masks: Tensor<bool> option list) (shape: int64 list) =
        match masks with
        | None :: rMasks ->
            // non-specified mask consumes one dimension of tensor
            match List.tryHead shape with
            | Some s -> (s, s) :: Tensor<_>.MaskShapes rMasks (List.tail shape)
            | None -> invalidArg "masks" "Dimension mismatch between masks and tensor shape."
        | Some mask :: rMasks ->
            // specified mask consumes as many dimensions as it has
            if mask.NDims > List.length shape then 
                invalidArg "masks" "Dimension mismatch between masks and tensor shape."
            let s, rShape = shape.[..mask.NDims-1], shape.[mask.NDims..]
            if mask.Shape <> s then
                invalidArg "masks" "Shape of mask %A does not match part %A of tensor shape it applies to." mask.Shape s
            (Tensor<_>.countTrue mask, mask.NElems) :: Tensor<_>.MaskShapes rMasks rShape
        | [] ->
            if not (List.isEmpty shape) then
                invalidArg "masks" "Dimension mismatch between masks and tensor shape."
            []

    /// Converts a list of masks (which may be null) to a list of mask options.
    static member internal MaskOptions (masks: Tensor<bool> list) =
        masks |> List.map (fun m -> match box m with
                                    | null -> None
                                    | _ -> Some m)

    /// Collect all elements of this tensor where mask is true.
    member internal this.MaskedGet (masks: Tensor<bool> list) =
        let masks = Tensor<_>.MaskOptions masks
        masks |> List.iter (Option.iter (fun m -> Tensor.CheckSameStorage [this; m]))
        let trgtShp, srcShp = Tensor<_>.MaskShapes masks this.Shape |> List.unzip
        let trgt = Tensor<'T> (trgtShp, this.Dev)
        let src = this |> Tensor.reshape srcShp
        let masks = masks |> List.map (Option.map (fun t -> t |> Tensor<_>.flatten :> ITensorFrontend<_>)) |> List.toArray
        backend.MaskedGet (trgt=trgt, src=src, masks=masks) 
        trgt    
        
    member inline internal this.IMaskedGet (masks: Tensor<bool> list) = 
        this.MaskedGet masks :> ITensor     
        
    /// Set all elements of this tensor where mask is true to the specfied values.
    member internal this.MaskedSet (masks: Tensor<bool> list) (value: Tensor<'T>) =
        let masks = Tensor<_>.MaskOptions masks
        Tensor.CheckSameStorage [this; value]
        masks |> List.iter (Option.iter (fun m -> Tensor.CheckSameStorage [this; m]))       
        let valueShp, trgtShp = Tensor<_>.MaskShapes masks this.Shape |> List.unzip        
        let masks = masks |> List.map (Option.map (fun t -> t |> Tensor<_>.flatten :> ITensorFrontend<_>)) |> List.toArray
        let value = value |> Tensor<_>.broadcastTo valueShp        
        match this |> Tensor.tryReshapeView trgtShp with
        | Some trgtView -> backend.MaskedSet (trgt=trgtView, masks=masks, src=value)
        | None ->
            let trgt = this |> Tensor.reshape trgtShp
            backend.MaskedSet (trgt=trgt, masks=masks, src=value)
            this.CopyFrom (trgt |> Tensor.reshape this.Shape)
             
    member inline internal this.IMaskedSet (masks: Tensor<bool> list) (value: ITensor) = 
        match value with
        | :? Tensor<'T> as value -> this.MaskedSet masks value
        | _ ->
            invalidOp "Cannot assign data type %s to tensor of data type %s." value.DataType.Name this.DataType.Name

    /// access to a single item using an array of indices
    member this.Item
        with get (idx: int64[]) : 'T = backend.[idx]
        and set (idx: int64[]) (value: 'T) = backend.[idx] <- value
          
    /// access to a single item using a list of indices 
    /// (use array of indices for faster access)
    member this.Item
        with get (idx: int64 list) : 'T = backend.[Array.ofList idx]
        and set (idx: int64 list) (value: 'T) = backend.[Array.ofList idx] <- value

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.   
    member this.M
        with get (m0: Tensor<bool>) = this.MaskedGet [m0]
        and set (m0: Tensor<bool>) (value: Tensor<'T>) = this.MaskedSet [m0] value                          

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.
    member this.M
        with get (m0: Tensor<bool>, m1: Tensor<bool>) = this.MaskedGet [m0; m1]
        and set (m0: Tensor<bool>, m1: Tensor<bool>) (value: Tensor<'T>) = this.MaskedSet [m0; m1] value                          

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.
    member this.M
        with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>) = this.MaskedGet [m0; m1; m2]
        and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>) (value: Tensor<'T>) = this.MaskedSet [m0; m1; m2] value                          

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.
    member this.M
        with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>) = this.MaskedGet [m0; m1; m2; m3]
        and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>) (value: Tensor<'T>) = this.MaskedSet [m0; m1; m2; m3] value                          

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.
    member this.M
        with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>, m4: Tensor<bool>) = this.MaskedGet [m0; m1; m2; m3; m4]
        and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>, m4: Tensor<bool>) (value: Tensor<'T>) = this.MaskedSet [m0; m1; m2; m3; m4] value                          

    /// Element selection using boolean mask. Specify NoMask for a dimension if no masking is desired.
    member this.M
        with get (masks: Tensor<bool> list) = this.MaskedGet masks
        and set (masks: Tensor<bool> list) (value: Tensor<'T>) = this.MaskedSet masks value                          

    /// n-dimensional slicing using a list of TensorRngs
    member this.Item
        with get (rng: Rng list) = this.GetRng [|rng|]
        and set (rng: Rng list) (value: Tensor<'T>) = this.SetRng [|rng|] value

    /// one-dimensional slicing using indices and special axes
    member this.Item
        with get (i0: int64) = this.GetRng [|i0|]
        and set (i0: int64) (value: Tensor<'T>) = this.SetRng [|i0|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option) = this.GetRng [|i0s; i0f|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, value: Tensor<'T>) = this.SetRng [|i0s; i0f|] value

    /// two-dimensional slicing using indices and special axes
    member this.Item
        with get (i0: int64, i1: int64) = this.GetRng [|i0; i1|]
        and set (i0: int64, i1: int64) (value: Tensor<'T>) = this.SetRng [|i0; i1|] value
    member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option) = this.GetRng [|i0; i1s; i1f|]
    member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, value: Tensor<'T>) = this.SetRng [|i0; i1s; i1f|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64) = this.GetRng [|i0s; i0f; i1|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option) = this.GetRng [|i0s; i0f; i1s; i1f|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1s; i1f|] value

    /// three-dimensional slicing using indices and special axes
    member this.Item
        with get (i0: int64, i1: int64, i2: int64) = this.GetRng [|i0; i1; i2|]
        and set (i0: int64, i1: int64, i2: int64) (value: Tensor<'T>) = this.SetRng [|i0; i1; i2|] value
    member this.GetSlice (i0: int64, i1: int64, i2: int64) = this.GetRng [|i0; i1; i2|]
    member this.SetSlice (i0: int64, i1: int64, i2: int64, value: Tensor<'T>) = this.SetRng [|i0; i1; i2|] value
    member this.GetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option) = this.GetRng [|i0; i1; i2s; i2f|]
    member this.SetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, value: Tensor<'T>) = this.SetRng [|i0; i1; i2s; i2f|] value
    member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64) = this.GetRng [|i0; i1s; i1f; i2|]
    member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, value: Tensor<'T>) = this.SetRng [|i0; i1s; i1f; i2|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64) = this.GetRng [|i0s; i0f; i1; i2|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1; i2|] value
    member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option) = this.GetRng [|i0; i1s; i1f; i2s; i2f|]
    member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, value: Tensor<'T>) = this.SetRng [|i0; i1s; i1f; i2s; i2f|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option) = this.GetRng [|i0s; i0f; i1; i2s; i2f|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1; i2s; i2f|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64) = this.GetRng [|i0s; i0f; i1s; i1f; i2|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1s; i1f; i2|] value
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option) = this.GetRng [|i0s; i0f; i1s; i1f; i2s; i2f|]
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, value: Tensor<'T>) = this.SetRng [|i0s; i0f; i1s; i1f; i2s; i2f|] value

    /// four- and more-dimensional slicing using indices and special axes
    member this.Item
        with get (o0: obj, o1: obj, o2: obj, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|o0; o1; o2; o3|] r
        and set (o0: obj, o1: obj, o2: obj, o3: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4; o5|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4; o5; o6|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4; o5; o6; o7|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj, o8: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4; o5; o6; o7; o8|] value
    member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj, o8: obj, o9: obj) (value: Tensor<'T>) = this.SetRng [|o0; o1; o2; o3; o4; o5; o6; o7; o8; o9|] value
    member this.GetSlice (i0: int64, i1: int64, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0; i1; i2; o3|] r
    member this.SetSlice (i0: int64, i1: int64, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0; i1; i2; o3; o4|] r
    member this.GetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0; i1; i2s; i2f; o3|] r
    member this.SetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0; i1; i2s; i2f; o3; o4|] r
    member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0; i1s; i1f; i2; o3|] r
    member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0; i1s; i1f; i2; o3; o4|] r
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0s; i0f; i1; i2; o3|] r
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0s; i0f; i1; i2; o3; o4|] r
    member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0; i1s; i1f; i2s; i2f; o3|] r
    member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0; i1s; i1f; i2s; i2f; o3; o4|] r
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0s; i0f; i1; i2s; i2f; o3|] r
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0s; i0f; i1; i2s; i2f; o3; o4|] r
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0s; i0f; i1s; i1f; i2; o3|] r
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0s; i0f; i1s; i1f; i2; o3; o4|] r
    member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.GetRngWithRest [|i0s; i0f; i1s; i1f; i2s; i2f; o3|] r
    member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.SetRngWithRest [|i0s; i0f; i1s; i1f; i2s; i2f; o3; o4|] r

    /// get element value
    static member inline get (a: Tensor<_>) (pos: int64 list) = 
        a.[pos]
    
    /// set element value
    static member inline set (a: Tensor<_>) (pos: int64 list) value = 
        a.[pos] <- value

    /// checks that this Tensor is a scalar tensor
    member inline internal this.CheckScalar () =
        if this.NDims <> 0 then 
            indexOutOfRange "This operation requires a scalar (0-dimensional) tensor, but its shape is %A." this.Shape

    /// value of scalar (0-dimensional) tensor
    member this.Value 
        with get () = 
            this.CheckScalar()
            this.[[||]]
        and set value = 
            this.CheckScalar()
            this.[[||]] <- value

    /// value of scalar (0-dimensional) tensor
    static member value (a: Tensor<'T>) : 'T =
        a.Value

    /// Pretty string containing maxElems elements per dimension.
    member this.ToString (maxElems) =
        let rec prettyDim lineSpace (a: Tensor<'T>) =
            let ls () = a.Shape.[0]
            let subPrint idxes = 
                idxes
                |> Seq.map (fun i -> 
                    prettyDim (lineSpace + " ") (a.[i, Fill])) 
                |> Seq.toList                   
            let subStrs () = 
                if ls() <= maxElems then
                    subPrint (seq {0L .. ls() - 1L})
                else
                    let leftTo = maxElems / 2L - 1L
                    let remaining = maxElems - 1L - leftTo - 1L
                    let rightFrom = ls() - remaining
                    let leftIdx = seq {0L .. leftTo}
                    let rightIdx = seq {rightFrom .. (ls()-1L)}
                    let elipsis =
                        match typeof<'T> with
                        | t when t=typeof<single> -> "      ..."
                        | t when t=typeof<double> -> "      ..."
                        | t when t=typeof<int>    -> " ..."
                        | t when t=typeof<byte>   -> "..."
                        | t when t=typeof<bool>   -> " ... "
                        | _ -> "..."
                    (subPrint leftIdx) @ [elipsis] @ (subPrint rightIdx)
            match a.NDims with
            | 0 -> 
                let v = box a.Value
                match typeof<'T> with
                | t when t = typeof<single> && unbox v >= 0.0f -> sprintf "%9.4f" (v :?> single)
                | t when t = typeof<single> && unbox v <  0.0f -> sprintf "%9.3f" (v :?> single)
                | t when t = typeof<double> && unbox v >= 0.0  -> sprintf "%9.4f" (v :?> double)
                | t when t = typeof<double> && unbox v <  0.0  -> sprintf "%9.3f" (v :?> double)
                | t when t = typeof<int>                       -> sprintf "%4d"   (v :?> int)
                | t when t = typeof<int64>                     -> sprintf "%4d"   (v :?> int64)
                | t when t = typeof<byte>                      -> sprintf "%3d"   (v :?> byte)
                | t when t = typeof<bool>   && unbox v = true  -> "true "
                | t when t = typeof<bool>   && unbox v = false -> "false"
                | _                                            -> sprintf "%A;" v
            | 1 -> "[" + (String.concat " " (subStrs ())) + "]"
            | _ -> "[" + (String.concat ("\n" + lineSpace) (subStrs ())) + "]"
        prettyDim " " this                       

    /// pretty contents string
    member this.Pretty = this.ToString (maxElems=10L)
    override this.ToString() = this.Pretty

    /// full contents string
    member this.Full = this.ToString (maxElems=Int64.MaxValue)
                               
    // interface for access from backend                             
    interface ITensorFrontend<'T> with
        member this.Storage = this.Storage
        member this.Dev = this.Dev
        member this.Backend = this.Backend
        member this.Layout = this.Layout
        member this.Shape = this.Shape
        member this.Stride = this.Stride
        member this.Offset = this.Offset
        member this.NDims = this.NDims
        member this.NElems = this.NElems
        member this.Relayout layout = this.Relayout layout :> ITensorFrontend<'T>
        member this.Copy (?order) = this.Copy (?order=order) :> ITensorFrontend<'T>
        member this.CopyFrom (src) = this.CopyFrom (src :?> Tensor<'T>)
        member this.Transfer (dev) = this.Transfer (dev) :> ITensorFrontend<'T>
        member this.T = this.T :> ITensorFrontend<'T>

    // type-neural interface
    interface ITensor with
        member this.Layout = this.Layout
        member this.Relayout layout = this.Relayout layout :> ITensor
        member this.Shape = this.Shape
        member this.Stride = this.Stride
        member this.Offset = this.Offset
        member this.NDims = this.NDims
        member this.NElems = this.NElems
        member this.DataType = this.DataType
        member this.Storage = this.Storage :> ITensorStorage
        member this.Dev = this.Dev
        member this.Copy (?order) = this.Copy (?order=order) :> ITensor
        member this.Transfer (dev) = this.Transfer (dev) :> ITensor
        member this.FillZero () = this.FillConst Tensor<'T>.Zero
        member this.Pretty = this.Pretty
        member this.Full = this.Full

        member this.Item
            with get (rng: Rng list) = this.IGetRng [|rng|]
            and set (rng: Rng list) (value: ITensor) = this.ISetRng [|rng|] value

        member this.M
            with get (m0: Tensor<bool>) = this.IMaskedGet [m0]
            and set (m0: Tensor<bool>) (value: ITensor) = this.IMaskedSet [m0] value                          
    
        member this.M
            with get (m0: Tensor<bool>, m1: Tensor<bool>) = this.IMaskedGet [m0; m1]
            and set (m0: Tensor<bool>, m1: Tensor<bool>) (value: ITensor) = this.IMaskedSet [m0; m1] value                          
    
        member this.M
            with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>) = this.IMaskedGet [m0; m1; m2]
            and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>) (value: ITensor) = this.IMaskedSet [m0; m1; m2] value                          
    
        member this.M
            with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>) = this.IMaskedGet [m0; m1; m2; m3]
            and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>) (value: ITensor) = this.IMaskedSet [m0; m1; m2; m3] value                          
    
        member this.M
            with get (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>, m4: Tensor<bool>) = this.IMaskedGet [m0; m1; m2; m3; m4]
            and set (m0: Tensor<bool>, m1: Tensor<bool>, m2: Tensor<bool>, m3: Tensor<bool>, m4: Tensor<bool>) (value: ITensor) = this.IMaskedSet [m0; m1; m2; m3; m4] value                          
    
        member this.M
            with get (masks: Tensor<bool> list) = this.IMaskedGet masks
            and set (masks: Tensor<bool> list) (value: ITensor) = this.IMaskedSet masks value                          

        member this.Item
            with get (i0: int64) = this.IGetRng [|i0|]
            and set (i0: int64) (value: ITensor) = this.ISetRng [|i0|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option) = this.IGetRng [|i0s; i0f|] 
        member this.SetSlice (i0s: int64 option, i0f: int64 option, value: ITensor) = this.ISetRng [|i0s; i0f|] value

        member this.Item
            with get (i0: int64, i1: int64) = this.IGetRng [|i0; i1|]
            and set (i0: int64, i1: int64) (value: ITensor) = this.ISetRng [|i0; i1|] value
        member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option) = this.IGetRng [|i0; i1s; i1f|]
        member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, value: ITensor) = this.ISetRng [|i0; i1s; i1f|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64) = this.IGetRng [|i0s; i0f; i1|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, value: ITensor) = this.ISetRng [|i0s; i0f; i1|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option) = this.IGetRng [|i0s; i0f; i1s; i1f|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, value: ITensor) = this.ISetRng [|i0s; i0f; i1s; i1f|] value

        member this.Item
            with get (i0: int64, i1: int64, i2: int64) = this.IGetRng [|i0; i1; i2|]
            and set (i0: int64, i1: int64, i2: int64) (value: ITensor) = this.ISetRng [|i0; i1; i2|] value
        member this.GetSlice (i0: int64, i1: int64, i2: int64) = this.IGetRng [|i0; i1; i2|]
        member this.SetSlice (i0: int64, i1: int64, i2: int64, value: ITensor) = this.ISetRng [|i0; i1; i2|] value
        member this.GetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option) = this.IGetRng [|i0; i1; i2s; i2f|]
        member this.SetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, value: ITensor) = this.ISetRng [|i0; i1; i2s; i2f|] value
        member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64) = this.IGetRng [|i0; i1s; i1f; i2|]
        member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, value: ITensor) = this.ISetRng [|i0; i1s; i1f; i2|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64) = this.IGetRng [|i0s; i0f; i1; i2|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, value: ITensor) = this.ISetRng [|i0s; i0f; i1; i2|] value
        member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option) = this.IGetRng [|i0; i1s; i1f; i2s; i2f|]
        member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, value: ITensor) = this.ISetRng [|i0; i1s; i1f; i2s; i2f|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option) = this.IGetRng [|i0s; i0f; i1; i2s; i2f|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, value: ITensor) = this.ISetRng [|i0s; i0f; i1; i2s; i2f|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64) = this.IGetRng [|i0s; i0f; i1s; i1f; i2|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, value: ITensor) = this.ISetRng [|i0s; i0f; i1s; i1f; i2|] value
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option) = this.IGetRng [|i0s; i0f; i1s; i1f; i2s; i2f|]
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, value: ITensor) = this.ISetRng [|i0s; i0f; i1s; i1f; i2s; i2f|] value

        member this.Item
            with get (o0: obj, o1: obj, o2: obj, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|o0; o1; o2; o3|] r
            and set (o0: obj, o1: obj, o2: obj, o3: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4; o5|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4; o5; o6|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4; o5; o6; o7|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj, o8: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4; o5; o6; o7; o8|] value
        member this.Item with set (o0: obj, o1: obj, o2: obj, o3: obj, o4: obj, o5: obj, o6: obj, o7: obj, o8: obj, o9: obj) (value: ITensor) = this.ISetRng [|o0; o1; o2; o3; o4; o5; o6; o7; o8; o9|] value
        member this.GetSlice (i0: int64, i1: int64, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0; i1; i2; o3|] r
        member this.SetSlice (i0: int64, i1: int64, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0; i1; i2; o3; o4|] r
        member this.GetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0; i1; i2s; i2f; o3|] r
        member this.SetSlice (i0: int64, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0; i1; i2s; i2f; o3; o4|] r
        member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0; i1s; i1f; i2; o3|] r
        member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0; i1s; i1f; i2; o3; o4|] r
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0s; i0f; i1; i2; o3|] r
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0s; i0f; i1; i2; o3; o4|] r
        member this.GetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0; i1s; i1f; i2s; i2f; o3|] r
        member this.SetSlice (i0: int64, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0; i1s; i1f; i2s; i2f; o3; o4|] r
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0s; i0f; i1; i2s; i2f; o3|] r
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1: int64, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0s; i0f; i1; i2s; i2f; o3; o4|] r
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0s; i0f; i1s; i1f; i2; o3|] r
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2: int64, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0s; i0f; i1s; i1f; i2; o3; o4|] r
        member this.GetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, [<System.ParamArray>] r: obj[]) = this.IGetRngWithRest [|i0s; i0f; i1s; i1f; i2s; i2f; o3|] r
        member this.SetSlice (i0s: int64 option, i0f: int64 option, i1s: int64 option, i1f: int64 option, i2s: int64 option, i2f: int64 option, o3: obj, o4: obj, [<System.ParamArray>] r: obj[]) = this.ISetRngWithRest [|i0s; i0f; i1s; i1f; i2s; i2f; o3; o4|] r

    // enumerator interfaces
    interface IEnumerable<'T> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<'T> = this.Backend.GetEnumerator()
        member this.GetEnumerator() : System.Collections.IEnumerator = (this.Backend :> IEnumerable).GetEnumerator()

    /// two tensors are equal if they have the same underlying memory and layout
    override this.Equals other =
        match other with
        | :? Tensor<'T> as ot ->
            this.Storage = ot.Storage && this.Layout = ot.Layout 
        | _ -> false

    override this.GetHashCode () =
        hash (this.Storage, this.Layout)

    /// Creates a new tensor of the given shape and data type.
    static member NewOfType (shape: int64 list, dataType: Type, dev: ITensorDevice, ?order: TensorOrder) =
        let gt = typedefof<Tensor<_>>.MakeGenericType (dataType)
        Activator.CreateInstance (gt, [|box shape; box dev; box order|]) :?> ITensor

    /// Creates a new empty tensor with the given number of dimensions.
    static member empty (dev: ITensorDevice) (nDims: int) : Tensor<'T> =
        Tensor<'T> (List.init nDims (fun _ -> 0L), dev)

    /// Creates a new tensor of given shape filled with zeros.
    static member zeros (dev: ITensorDevice) (shape: int64 list) : Tensor<'T> =
        let x = Tensor<'T> (shape, dev)
        if not dev.Zeroed then 
            x.FillConst Tensor<'T>.Zero
        x
   
    /// Tensor of same shape as specifed tensor and filled with zeros.
    static member zerosLike (tmpl: Tensor<'T>) : Tensor<'T> =
        Tensor<'T>.zeros tmpl.Storage.Dev tmpl.Shape

    /// Creates a new tensor of given shape filled with ones.
    static member ones (dev: ITensorDevice) (shape: int64 list) : Tensor<'T> =
        let x = Tensor<'T> (shape, dev)
        x.FillConst Tensor<'T>.One
        x
        
    /// Tensor of same shape as specifed tensor and filled with ones.
    static member onesLike (tmpl: Tensor<'T>) : Tensor<'T> =
        Tensor<'T>.ones tmpl.Storage.Dev tmpl.Shape 

    /// Creates a new boolean tensor of given shape filled with false.
    static member falses (dev: ITensorDevice) (shape: int64 list) : Tensor<bool> =
        let x = Tensor<bool> (shape, dev)
        x.FillConst false
        x

    /// Creates a new boolean tensor of given shape filled with true.
    static member trues (dev: ITensorDevice) (shape: int64 list) : Tensor<bool> =
        let x = Tensor<bool> (shape, dev)
        x.FillConst true
        x   

    /// Creates a new tensor of scalar shape with the given value and storage.
    static member scalar (dev: ITensorDevice) (value: 'T) : Tensor<'T> =
        let x = Tensor<'T> ([], dev)
        x.Value <- value
        x

    /// Creates a new tensor of scalar shape with the given value and 
    /// same storage as the specified tensor.
    static member scalarLike (tmpl: ITensor) (value: 'T) : Tensor<'T> =
        Tensor<'T>.scalar tmpl.Storage.Dev value 

    /// Creates a tensor with the values returned by the function.
    static member init (dev: ITensorDevice) (shape: int64 list) (fn: int64[] -> 'T) : Tensor<'T> =
        let x = Tensor<'T> (shape, dev)
        x.FillIndexed fn
        x           

    /// Creates a tensor filled with the specified value.
    static member filled (dev: ITensorDevice) (shape: int64 list) (value: 'T) : Tensor<'T> =
        let x = Tensor<'T> (shape, dev)
        x.FillConst value
        x           

    /// Identity matrix of given size.
    static member identity (dev: ITensorDevice) (size: int64) : Tensor<'T> =
        let x = Tensor<'T>.zeros dev [size; size]
        let d : Tensor<'T> = Tensor.diag x
        d.FillConst Tensor<'T>.One
        x           

    /// Int64 vector containing the numbers [0L; 1L; ...; nElems-1L].
    static member counting (dev: ITensorDevice) (nElems: int64) =
        Tensor.init dev [nElems] (fun idx -> idx.[0])        

    /// Creates a one-dimensiona tensor filled with equaly spaced values from start 
    /// to (excluding) stop using the given increment.
    static member inline arange (dev: ITensorDevice) (start: 'V) (incr: 'V) (stop: 'V) = 
        let nElems = max 0L ((stop - start) / incr |> int64)
        let x = Tensor<'V> ([nElems], dev)
        x.FillIncrementing(start, incr)
        x

    /// Creates a one-dimensional tensor filled with equaly spaced values from start 
    /// to (including) stop.
    static member inline linspace (dev: ITensorDevice) (start: 'V) (stop: 'V) (nElems: int64) =
        if nElems < 2L then invalidArg "nElems" "linspace requires at least two elements."
        let incr = (stop - start) / conv<'V> (nElems - 1L)      
        let x = Tensor<'V> ([nElems], dev)
        x.FillIncrementing(start, incr)
        x

    /// convert tensor data type to bool
    static member bool a : Tensor<bool> = Tensor<_>.convert a

    /// convert tensor data type to byte
    static member byte a : Tensor<byte> = Tensor<_>.convert a

    /// convert tensor data type to sbyte
    static member sbyte a : Tensor<sbyte> = Tensor<_>.convert a

    /// convert tensor data type to int16
    static member int16 a : Tensor<int16> = Tensor<_>.convert a

    /// convert tensor data type to uint16
    static member uint16 a : Tensor<uint16> = Tensor<_>.convert a

    /// convert tensor data type to int32
    static member int32 a : Tensor<int32> = Tensor<_>.convert a

    /// convert tensor data type to uint32
    static member uint32 a : Tensor<uint32> = Tensor<_>.convert a

    /// convert tensor data type to int64
    static member int64 a : Tensor<int64> = Tensor<_>.convert a

    /// convert tensor data type to uint64
    static member uint64 a : Tensor<uint64> = Tensor<_>.convert a

    /// convert tensor data type to int
    static member int a : Tensor<int> = Tensor<_>.convert a

    /// convert tensor data type to nativeint
    static member nativeint a : Tensor<nativeint> = Tensor<_>.convert a

    /// convert tensor data type to single
    static member single a : Tensor<single> = Tensor<_>.convert a

    /// convert tensor data type to double
    static member double a : Tensor<double> = Tensor<_>.convert a

    /// convert tensor data type to float
    static member float a : Tensor<float> = Tensor<_>.convert a

    /// convert tensor data type to float32
    static member float32 a : Tensor<float32> = Tensor<_>.convert a

    /// Element-wise check if two tensors have same (within machine precision) values.
    /// Checks for exact equality for non-floating-point types.
    static member isCloseWithTol (a: Tensor<'T>, b: Tensor<'T>, ?absTol: 'T, ?relTol: 'T) =
        match typeof<'T> with
        | t when t=typeof<single> || t=typeof<double> ->
            let absTol = defaultArg absTol (conv<'T> 1e-8) |> Tensor.scalarLike a
            let relTol = defaultArg relTol (conv<'T> 1e-5) |> Tensor.scalarLike a
            abs (a - b) <<== absTol + relTol * abs b
        | _ -> a ==== b

    /// Element-wise check if two tensors have same (within machine precision) values.
    /// Checks for exact equality for non-floating-point types.
    static member isClose a b = Tensor.isCloseWithTol (a, b)

    /// Returns true if two tensors have same (within specified precision) values in all elements.
    /// If tensors have different shape, then false is returned.
    static member almostEqualWithTol (a: Tensor<'T>, b: Tensor<'T>, ?absTol: 'T, ?relTol: 'T) =
        if a.Shape = b.Shape then
            Tensor.isCloseWithTol (a, b, ?absTol=absTol, ?relTol=relTol) |> Tensor.all
        else false

    /// Returns true if two tensors have same (within machine precision) values in all elements.
    /// If tensors have different shape, then false is returned.
    static member almostEqual (a: Tensor<'T>) (b: Tensor<'T>) =
        Tensor.almostEqualWithTol (a, b)

    /// Returns true if all values in the tensor are finite (not NaN and not infinite).
    static member allFinite (a: Tensor<'T>) =
        a |> Tensor.isFinite |> Tensor.all

    /// mean over given axis
    static member meanAxis axis (a: Tensor<'T>) = 
        Tensor.sumAxis axis a / Tensor.scalarLike a (conv<'T> a.Shape.[axis])

    /// mean 
    static member mean a =
        a |> Tensor.flatten |> Tensor.meanAxis 0 |> Tensor.value

    /// variance over given axis
    static member varAxis (axis, a: Tensor<'T>, ?ddof) =
        let ddof = defaultArg ddof 0L
        let m = Tensor.meanAxis axis a |> Tensor.insertAxis axis
        let v = a - m
        let n = a.Shape.[axis] - ddof
        Tensor.sumAxis axis (v * v) / Tensor.scalarLike a (conv<'T> n)

    /// variances
    static member var (a, ?ddof) =
        Tensor.varAxis (0, Tensor.flatten a, ?ddof=ddof) |> Tensor.value

    /// standard deviation over given axis
    static member stdAxis (ax, a, ?ddof) =
        Tensor.varAxis (ax, a, ?ddof=ddof) |> sqrt

    /// standard deviation 
    static member std (a, ?ddof) =
        Tensor.var (a, ?ddof=ddof) |> sqrt

    /// tensor, matrix or vector norm of given order over given axis
    static member normAxis (axis, a: Tensor<'T>, ?ord: 'T) =
        let ord = defaultArg ord (conv<'T> 2)
        let tOrd = Tensor.scalarLike a ord
        let tOrdRep = Tensor.scalarLike a (conv<'T> 1) / tOrd
        let s = a ** tOrd |> Tensor.sumAxis axis
        s ** tOrdRep 

    /// tensor, matrix or vector norm of given order
    static member norm (a: Tensor<'T>, ?ord: 'T) =
        Tensor.normAxis (0, Tensor.flatten a, ?ord=ord) |> Tensor.value

    /// Returns a view of the diagonal along the given axes.
    /// The diagonal replaces the first axis and the second axis is removed.
    static member diagAxis ax1 ax2 (a: Tensor<'T>) =
        a |> Tensor.relayout (a.Layout |> TensorLayout.diagAxis ax1 ax2)

    /// Returns a view of the diagonal of a matrix as a vector.
    /// If the specified tensor has more than two dimensions, the diagonals
    /// along the last two dimensions are returned.
    static member diag (a: Tensor<'T>) =
        if a.NDims < 2 then
            invalidArg "a" "Need at least a two dimensional array for diagonal but got shape %A." a.Shape
        Tensor.diagAxis (a.NDims-2) (a.NDims-1) a

    /// Creates a new tensor of same shape but with ax2 inserted.
    /// The diagonal over ax1 and ax2 is filled with the elements of the original ax1.
    /// The other elements are set to zero.
    static member diagMatAxis ax1 ax2 (a: Tensor<'T>) =
        if ax1 = ax2 then 
            invalidArg "ax1" "axes to use for diagonal must be different"
        let ax1, ax2 = if ax1 < ax2 then ax1, ax2 else ax2, ax1
        a.CheckAxis ax1
        if not (0 <= ax2 && ax2 <= a.NDims) then
            invalidArg "ax2" "Cannot insert axis at position %d into array of shape %A." ax2 a.Shape
        let dShp = a.Shape |> List.insert ax2 a.Shape.[ax1]
        let d = Tensor.zeros a.Dev dShp
        let dDiag = Tensor.diagAxis ax1 ax2 d
        dDiag.FillFrom a
        d

    /// Creates a new matrix that has the specified diagonal.
    /// All other elements are zero.
    /// If the specified array has more than one dimension, the operation is
    /// performed batch-wise on the last dimension.
    static member diagMat (a: Tensor<'T>) =
        if a.NDims < 1 then
            invalidArg "a" "need at leat a one-dimensional array to create a diagonal matrix"
        Tensor.diagMatAxis (a.NDims-1) a.NDims a

    /// Computes the traces along the given axes.
    static member traceAxis ax1 ax2 (a: Tensor<'T>) =
        let tax = if ax1 < ax2 then ax1 else ax1 - 1
        a |> Tensor.diagAxis ax1 ax2 |> Tensor.sumAxis tax

    /// Computes the trace of a matrix.
    /// If the specified tensor has more than two dimensions, the traces
    /// along the last two dimensions are returned.
    static member trace (a: Tensor<'T>) =
        if a.NDims < 2 then
            invalidArg "a" "Need at least a two dimensional array for trace but got shape %A." a.Shape
        Tensor.traceAxis (a.NDims-2) (a.NDims-1) a

    /// N-dimensional tensor constructed of subtensors using a BlockTensor specification.
    static member ofBlocks (bs: BlockTensor<'T>) =
        let rec commonShape joinDim shps =               
            match shps with
            | [shp] -> List.set joinDim -1L shp
            | shp::rShps ->
                let commonShp = commonShape joinDim [shp]
                if commonShp <> commonShape joinDim rShps then
                    invalidArg "bs" "Block tensor blocks must have same number of dimensions and be 
                                     identical in all but the join dimension."
                commonShp
            | [] -> []

        let joinSize joinDim (shps: int64 list list) =
            shps |> List.map (fun shp -> shp.[joinDim]) |> List.sum

        let joinShape joinDim shps =
            commonShape joinDim shps 
            |> List.set joinDim (joinSize joinDim shps)

        let rec joinedBlocksShape joinDim bs =
            match bs with
            | SubBlocks blcks ->
                blcks |> List.map (joinedBlocksShape (joinDim + 1)) |> joinShape joinDim
            | Block ary -> ary.Shape

        let rec blockPosAndContents (joinDim: int) startPos bs = seq {
            match bs with
            | SubBlocks blcks ->
                let mutable pos = startPos
                for blck in blcks do
                    yield! blockPosAndContents (joinDim + 1) pos blck 
                    let blckShape = joinedBlocksShape (joinDim + 1) blck
                    pos <- List.set joinDim (pos.[joinDim] + blckShape.[joinDim]) pos
            | Block ary -> yield startPos, ary
        }

        let rec anyArray bs =
            match bs with
            | SubBlocks b -> List.tryPick anyArray b
            | Block a -> Some a
                  
        let tmplArray = Option.get (anyArray bs)
        let joinedShape = joinedBlocksShape 0 bs
        let joined = Tensor<_> (joinedShape, tmplArray.Dev)
        let startPos = List.replicate (List.length joinedShape) 0L

        for pos, ary in blockPosAndContents 0 startPos bs do
            let slice = (pos, ary.Shape) ||> List.map2 (fun p s -> Rng.Rng (Some p, Some (p + s - 1L))) 
            joined.[slice] <- ary
        joined

    /// 1d vector constructed of blocks 
    static member ofBlocks (bs: Tensor<'T> list) =
        bs |> List.map Block |> SubBlocks |> Tensor.ofBlocks

    /// 2d matrix constructed of blocks 
    static member ofBlocks (bs: Tensor<'T> list list) =
        bs |> List.map (List.map Block >> SubBlocks) |> SubBlocks |> Tensor.ofBlocks

    /// 3d tensor constructed of blocks
    static member ofBlocks (bs: Tensor<'T> list list list) =
        bs |> List.map (List.map (List.map Block >> SubBlocks) >> SubBlocks) |> SubBlocks |> Tensor.ofBlocks

    /// tensor product
    static member tensorProduct (a: Tensor<'T>) (b: Tensor<'T>) =
        let a, b = Tensor.padToSame (a, b)
        let rec generate (pos: int64 list) = 
            match List.length pos with
            | dim when dim = a.NDims ->
                let slice = pos |> List.map Rng.Elem
                Block (a.[slice] * b)
            | dim ->
                seq {for p in 0L .. a.Shape.[dim] - 1L -> generate (pos @ [p])}
                |> Seq.toList |> SubBlocks
        generate [] |> Tensor.ofBlocks

    /// Concatenates the sequence of tensors along the given axis.
    /// The source tensors are copied.
    static member concat (ax: int) (ts: Tensor<'T> seq) =
        let ts = List.ofSeq ts
        if List.isEmpty ts then
            invalidArg "ts" "Cannot concatenate empty sequence of tensors."

        // check for compatibility
        let shp = ts.Head.Shape
        if not (0 <= ax && ax < shp.Length) then
            invalidArg "ax" "Concatenation axis %d is out of range for shape %A." ax shp
        for aryIdx, ary in List.indexed ts do
            if List.without ax ary.Shape <> List.without ax shp then
                invalidArg "ts" "Concatentation element with index %d with shape %A must 
                                 be equal to shape %A of the first element, except in the concatenation axis %d" 
                                 aryIdx ary.Shape shp ax

        // calculate shape of concatenated tensors
        let totalSize = ts |> List.sumBy (fun ary -> ary.Shape.[ax])
        let concatShape = shp |> List.set ax totalSize

        // copy tensors into concatenated tensor
        let cc = Tensor(concatShape, ts.Head.Dev)
        let mutable pos = 0L
        for ary in ts do
            let aryLen = ary.Shape.[ax]
            if aryLen > 0L then
                let ccRng = 
                    List.init shp.Length (fun idx ->
                        if idx = ax then Rng.Rng (Some pos, Some (pos + aryLen - 1L))
                        else Rng.All)
                cc.[ccRng] <- ary
                pos <- pos + aryLen
        cc

    /// Replicates the tensor the given number of repetitions along the given axis.
    static member replicate (ax: int) (reps: int64) (a: Tensor<'T>) =
        a.CheckAxis ax
        if reps < 0L then invalidArg "reps" "Number of repetitions cannot be negative."

        // 1. insert axis of size one left to repetition axis
        // 2. broadcast along the new axis to number of repetitions
        // 3. reshape to result shape
        a 
        |> Tensor.reshape (a.Shape |> List.insert ax 1L)
        |> Tensor.broadcastDim ax reps
        |> Tensor.reshape (a.Shape |> List.set ax (reps * a.Shape.[ax]))

    /// calculates the pairwise differences along the given axis
    static member diffAxis (ax: int) (a: Tensor<'T>) =
        a.CheckAxis ax 
        let shftRng = 
            [for d=0 to a.NDims-1 do
                if d = ax then yield Rng.Rng (Some 1L, None)
                else yield Rng.All]
        let cutRng = 
            [for d=0 to a.NDims-1 do
                if d = ax then yield Rng.Rng (None, Some (a.Shape.[d] - 2L))
                else yield Rng.All]
        a.[shftRng] - a.[cutRng]

    /// calculates the pairwise differences along the last axis
    static member diff (a: Tensor<'T>) =
        if a.NDims < 1 then invalidArg "a" "Need at least a vector to calculate diff."
        Tensor.diffAxis (a.NDims-1) a
        


/// See Tensor<'T>.
type Tensor = 

    /// checks that all tensors have the same storage
    static member internal CheckSameStorage (xs: ITensor list) =
        match xs with
        | x::rs when rs |> List.exists (fun r -> x.Dev <> r.Dev) ->
            let storages = xs |> List.map (fun x -> x.Dev.Id)
            invalidOp "Storage devices must be equal for this operation, but they are %A." storages
        | _ -> ()            

    /// checks that two tensors have the same shape
    static member internal CheckSameShape (a: ITensor) (b: ITensor) =
        if a.Shape <> b.Shape then
            invalidArg "b" "Tensors of shapes %A and %A were expected to have same shape" a.Shape b.Shape

    /// prepares the sources of an elementwise operation by broadcasting them to the target shape
    static member internal PrepareElemwiseSources<'TR, 'TA> (trgt: Tensor<'TR>, a: Tensor<'TA>) : Tensor<'TA> =
        Tensor.CheckSameStorage [trgt; a]
        let a = a |> Tensor<_>.broadcastTo trgt.Shape
        a

    /// prepares the sources of an elementwise operation by broadcasting them to the target shape
    static member internal PrepareElemwiseSources<'TR, 'TA, 'TB> (trgt: Tensor<'TR>, a: Tensor<'TA>, b: Tensor<'TB>) 
            : (Tensor<'TA> * Tensor<'TB>) =
        Tensor.CheckSameStorage [trgt; a; b]
        let a = a |> Tensor<_>.broadcastTo trgt.Shape
        let b = b |> Tensor<_>.broadcastTo trgt.Shape
        a, b

    /// prepares the sources of an elementwise operation by broadcasting them to the target shape
    static member internal PrepareElemwiseSources<'TR, 'TA, 'TB, 'TC> (trgt: Tensor<'TR>, a: Tensor<'TA>, b: Tensor<'TB>, c: Tensor<'TC>) 
            : (Tensor<'TA> * Tensor<'TB> * Tensor<'TC>) =
        Tensor.CheckSameStorage [trgt; a; b; c]
        let a = a |> Tensor<_>.broadcastTo trgt.Shape
        let b = b |> Tensor<_>.broadcastTo trgt.Shape
        let c = c |> Tensor<_>.broadcastTo trgt.Shape
        a, b, c

    /// Prepares the sources of an axis reduce operation (e.g. sum over axis),
    /// by moving the reduction axis to be the last axis in the source.
    static member internal PrepareAxisReduceSources<'TR, 'TA> 
            (trgt: Tensor<'TR>, axis: int, a: Tensor<'TA>,
             initial: Tensor<'TR> option) : (Tensor<'TA> * Tensor<'TR> option) =
        Tensor.CheckSameStorage [trgt; a]
        a.CheckAxis axis
        let redShp = a.Shape |> List.without axis
        if trgt.Shape <> redShp then
            invalidOp "Reduction of tensor %A along axis %d gives shape %A but target has shape %A." 
                      a.Shape axis redShp trgt.Shape
        let initial =
            match initial with
            | Some initial -> 
                Tensor.CheckSameStorage [trgt; initial]
                initial |> Tensor.broadcastTo redShp |> Some  
            | None -> None                                       
        let axisToLast = [
            for d in 0 .. axis-1 do yield d
            yield a.NDims-1
            for d in axis+1 .. a.NDims-1 do yield d-1
        ]
        let a = a |> Tensor<_>.permuteAxes axisToLast
        if not (trgt.Shape = a.Shape.[0 .. a.NDims-2]) then
            failwith "Internal axis reduce shape computation error."
        a, initial

    /// prepares an axis reduce operation by allocating a target of appropriate size and storage
    static member internal PrepareAxisReduceTarget<'TR, 'TA> (axis: int, a: Tensor<'TA>, ?order: TensorOrder) : (Tensor<'TR> * Tensor<'TA>) =
        a.CheckAxis axis
        let redShp = a.Shape |> List.without axis        
        let trgt = Tensor<'TR> (redShp, a.Storage.Dev, ?order=order)
        trgt, a

    /// prepares an elementwise operation by allocating a target of same size and storage
    static member internal PrepareElemwise<'TR, 'TA> (a: Tensor<'TA>, ?order: TensorOrder) : (Tensor<'TR> * Tensor<'TA>) =
        let trgt = Tensor<'TR> (a.Shape, a.Storage.Dev, ?order=order)
        trgt, a

    /// prepares an elementwise operation by broadcasting both tensors to the same size
    /// and allocating a target of same size and storage
    static member internal PrepareElemwise<'TR, 'TA, 'TB> (a: Tensor<'TA>, b: Tensor<'TB>, ?order: TensorOrder) 
            : (Tensor<'TR> * Tensor<'TA> * Tensor<'TB>) =
        Tensor.CheckSameStorage [a; b]
        let a, b = Tensor<_>.broadcastToSame (a, b)
        let trgt = Tensor<'TR> (a.Shape, a.Storage.Dev, ?order=order)
        trgt, a, b

    /// prepares an elementwise operation by broadcasting all three tensors to the same size
    /// and allocating a target of same size and storage
    static member internal PrepareElemwise<'TR, 'TA, 'TB, 'TC> (a: Tensor<'TA>, b: Tensor<'TB>, c: Tensor<'TC>, ?order: TensorOrder) 
            : (Tensor<'TR> * Tensor<'TA> * Tensor<'TB> * Tensor<'TC>) =
        Tensor.CheckSameStorage [a; b; c]
        let a, b, c = Tensor<_>.broadcastToSame (a, b, c)
        let trgt = Tensor<'TR> (a.Shape, a.Storage.Dev, ?order=order)
        trgt, a, b, c



/// See Tensor.Parallel.
module Tensor =

    /// Multi-threaded operations of Tensor<'T>.
    type Parallel = 

        /// Creates a new tensor with the values returned by the function.
        static member init<'T> (dev: ITensorDevice) (shape: int64 list) (fn: int64[] -> 'T) : Tensor<'T> =
            let x = Tensor<'T> (shape, dev)
            x.FillParallelIndexed fn
            x          

        /// Maps all elements using the specified function into a new tensor.
        static member map (fn: 'T -> 'R) (a: Tensor<'T>) =
            let trgt, a = Tensor.PrepareElemwise (a)
            trgt.FillParallelMap fn a
            trgt       

        /// Maps all elements using the specified indexed function into a new tensor.
        static member mapi (fn: int64[] -> 'T -> 'R) (a: Tensor<'T>) =
            let trgt, a = Tensor.PrepareElemwise (a)
            trgt.FillParallelMapIndexed fn a
            trgt      

        /// Maps all elements using the specified function into a new tensor.
        static member map2 (fn: 'TA -> 'TB -> 'R) (a: Tensor<'TA>) (b: Tensor<'TB>) =
            let trgt, a, b = Tensor.PrepareElemwise (a, b)
            trgt.FillParallelMap2 fn a b
            trgt           

        /// Maps all elements using the specified indexed function into a new tensor.
        static member mapi2 (fn: int64[] -> 'TA -> 'TB -> 'R) (a: Tensor<'TA>) (b: Tensor<'TB>) =
            let trgt, a, b = Tensor.PrepareElemwise (a, b)
            trgt.FillParallelMapIndexed2 fn a b
            trgt            

        /// Folds the function over the given axis.
        static member foldAxis (fn: 'T -> 'TA -> 'T) (initial: 'T) (axis: int) (a: Tensor<'TA>) =
            let trgt, a = Tensor.PrepareAxisReduceTarget (axis, a)
            trgt.FillParallelFoldAxis fn initial axis a
            trgt



/// Special values that can be passed instead of masks.
[<AutoOpen>]
module internal SpecialMask =

    /// Indicates that the dimension is unmasked, i.e. equals specifying a tensor filled with trues. 
    let NoMask : Tensor<bool> = Unchecked.defaultof<_> // = null
        
        
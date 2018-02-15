namespace Tensor

open System
open System.Runtime
open System.Threading
open System.Runtime.InteropServices

open ManagedCuda
open ManagedCuda.BasicTypes

open Tensor
open Tensor.Utils
open Tensor.Cuda
open Tensor.Backend



/// Tensor stored on CUDA device.
module CudaTensor =

    /// Tensor stored on CUDA device.
    let Dev = TensorCudaDevice.Instance :> ITensorDevice

    let transfer x = Tensor.transfer Dev x

    let empty<'T> = Tensor<'T>.empty Dev

    let zeros<'T> = Tensor<'T>.zeros Dev 

    let ones<'T> = Tensor<'T>.ones Dev

    let falses = Tensor.falses Dev

    let trues = Tensor.trues Dev

    let scalar<'T> = Tensor<'T>.scalar Dev

    let init<'T> = Tensor<'T>.init Dev

    let filled<'T> = Tensor<'T>.filled Dev

    let identity<'T> = Tensor<'T>.identity Dev

    let counting = Tensor.counting Dev

    let inline arange start incr stop = 
        Tensor.arange Dev start incr stop

    let inline linspace start stop nElems = 
        Tensor.linspace Dev start stop nElems

    /// Creates a ITensor for the given pointer, allocation size in bytes, type and layout.
    let usingPtrAndType (ptr: CUdeviceptr) (sizeInBytes: SizeT) (typ: Type) (layout: TensorLayout) = 
        let devVarType = typedefof<CudaDeviceVariable<_>>.MakeGenericType [|typ|]
        let devVar = Activator.CreateInstance (devVarType, [|box ptr; box sizeInBytes|])

        let devStorType = typedefof<TensorCudaStorage<_>>.MakeGenericType [|typ|]
        let devStor = Activator.CreateInstance (devStorType, [|devVar|])

        let tensorType = typedefof<Tensor<_>>.MakeGenericType [|typ|]
        Activator.CreateInstance (tensorType, [|box layout; devStor|]) :?> ITensor


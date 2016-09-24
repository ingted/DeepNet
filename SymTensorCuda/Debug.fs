﻿namespace SymTensor.Compiler.Cuda


module Debug =
    /// redirects all stream calls to the null stream
    let mutable DisableStreams = false

    /// disables all events
    let mutable DisableEvents = false

    /// synchronizes the CUDA context after each call to detect errors
    let mutable SyncAfterEachCudaCall = false

    /// outputs messages when a function / kernel is launched
    let mutable TraceCalls = false

    /// compiles kernels with debug information and no optimizations
    let mutable DebugCompile = false

    /// prints timing information during CUDA function compilation
    let mutable Timing = false

    /// prints CUDA memory usage during CUDA function compilation
    let mutable ResourceUsage = false

    /// prints each compile step during compilation
    let mutable TraceCompile = false

    /// prints ptxas verbose information during compilation
    let mutable PtxasInfo = false

    /// dumps kernel code before it is compiled
    let mutable DumpCode = false

    /// terminates the program when a non-finite tensor was found by the CheckFinite op
    let mutable TerminateWhenNonFinite = true

    /// terminates the program after CUDA recipe generation
    let mutable TerminateAfterRecipeGeneration = false

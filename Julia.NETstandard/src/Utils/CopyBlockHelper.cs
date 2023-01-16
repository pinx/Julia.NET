using System;
using System.Reflection.Emit;

namespace JuliaNET.Utils;

internal static class CopyBlockHelper
{
    // https://stackoverflow.com/a/51450057

    internal static unsafe void SmartCopy<T>(T* pointerDataOutCurrent,
                                             T* pointerDataIn,
                                             int length) where T : unmanaged
    {
        var sizeOfType = sizeof(T);

        var numberOfBytesInBlock = Convert.ToUInt32(sizeOfType * length);

        var numOfIterations = numberOfBytesInBlock / BlockSize;
        var overheadOfLastIteration = numberOfBytesInBlock % BlockSize;

        uint offset;
        for (var idx = 0u; idx < numOfIterations; idx++)
        {
            offset = idx * BlockSize;
            CopyBlock(pointerDataOutCurrent + offset / sizeOfType, pointerDataIn + offset / sizeOfType, BlockSize);
        }

        offset = numOfIterations * BlockSize;
        CopyBlock(pointerDataOutCurrent + offset / sizeOfType, pointerDataIn + offset / sizeOfType, overheadOfLastIteration);
    }

    private const int BlockSize = 16384;

    private static readonly CopyBlockDelegate CpBlock = GenerateCopyBlock();

    private unsafe delegate void CopyBlockDelegate(void* des,
                                                   void* src,
                                                   uint bytes);

    private static unsafe void CopyBlock(void* dest,
                                         void* src,
                                         uint count)
    {
        var local = CpBlock;
        local(dest, src, count);
    }

    private static CopyBlockDelegate GenerateCopyBlock()
    {
        // Don't ask...
        var method = new DynamicMethod("CopyBlockIL", typeof(void),
                                       new[] { typeof(void*), typeof(void*), typeof(uint) }, typeof(CopyBlockHelper));
        var emitter = method.GetILGenerator();
        // emit IL
        emitter.Emit(OpCodes.Ldarg_0);
        emitter.Emit(OpCodes.Ldarg_1);
        emitter.Emit(OpCodes.Ldarg_2);
        emitter.Emit(OpCodes.Cpblk);
        emitter.Emit(OpCodes.Ret);

        // compile to delegate
        return (CopyBlockDelegate)method.CreateDelegate(typeof(CopyBlockDelegate));
    }
}

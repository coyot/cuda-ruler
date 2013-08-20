using System;
using System.Runtime.InteropServices;
using GASS.CUDA;
using GASS.CUDA.Types;

namespace aCudaResearch
{
    public class CudaFunctionCall
    {
        private readonly CUDA _cuda;
        private readonly CUfunction _function;
        private int _offset;


        public CudaFunctionCall(CUDA cuda, string functionName)
        {
            _cuda = cuda;
            _function = cuda.GetModuleFunction(functionName);
            _offset = 0;
        }

        public CudaFunctionCall AddParameter(uint value)
        {
            _cuda.SetParameter(_function, _offset, value);
            _offset += Marshal.SizeOf(typeof(uint));

            return this;
        }

        public CudaFunctionCall AddParameter(CUdeviceptr pointer)
        {
            _cuda.SetParameter(_function, _offset, pointer.Pointer);
            _offset += Marshal.SizeOf(typeof(uint));

            return this;
        }

        public void Execute(int blockWidth, int blockHeight, int blockDepth,
            int gridWidth, int gridHeight)
        {
            _cuda.SetParameterSize(_function, (uint)_offset);
            _cuda.SetFunctionBlockShape(_function, blockWidth, blockHeight, blockDepth);
            _cuda.Launch(_function, gridWidth, gridHeight);
            _cuda.SynchronizeContext();
        }

        public CudaFunctionCall AddParameter(CUtexref texture)
        {
            _cuda.SetParameter(_function, texture);
            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CudaRuler
{
    /// <summary>
    /// Defines the interface to create data for the specific algorithm based on
    /// the data source (<see cref="DataProvider"/>).
    /// </summary>
    public interface IDataBuilder<T>
    {
        T BuildInstance(ExecutionSettings executionSettings);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace aCudaResearch.Settings
{
    /// <summary>
    /// Definition of builder interface to prepare <see link="ExecutionSettings"/> ojbect.
    /// </summary>
    [ContractClass(typeof(ISettingsBuilderContract))]
    interface ISettingsBuilder
    {
        /// <summary>
        /// Build settings.
        /// </summary>
        /// <returns><see link="ExecutionSettings"/> object</returns>
        ExecutionSettings Build();
    }

    [ContractClassFor(typeof(ISettingsBuilder))]
    abstract class ISettingsBuilderContract : ISettingsBuilder
    {
        #region ISettingsBuilder Members

        ExecutionSettings ISettingsBuilder.Build()
        {
            Contract.Ensures(Contract.Result<ExecutionSettings>() != null);

            // This is not important because the CodeContract engine will not allowe to execute it
            // but on the other hand there MUST be returning value.
            return default(ExecutionSettings);
        }

        #endregion
    }
}

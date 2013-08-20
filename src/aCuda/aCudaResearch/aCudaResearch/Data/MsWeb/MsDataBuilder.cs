using System;
using System.Collections.Generic;
using System.IO;

namespace aCudaResearch.Data.MsWeb
{
    public class MsDataBuilder : IDataBuilder<MsInstance<int>>
    {
        public MsInstance<int> BuildInstance(ExecutionSettings executionSettings)
        {
            var instance = new MsInstance<int>(executionSettings.TransactionsNumber);
            if (!File.Exists(executionSettings.DataSourcePath))
            {
                //! here an exception should be thrown!
                return null;
            }

            var r = new StreamReader(executionSettings.DataSourcePath);
            string line, transactionId = null;
            var votes = new List<int>();

            while (!r.EndOfStream)
            {
                line = r.ReadLine();
                if (line != null)
                    switch (line[0])
                    {
                        case 'A':
                            instance.AddElement(line);
                            break;
                        case 'C':
                            if (transactionId != null)
                            {
                                instance.AddEntry(Convert.ToInt32(transactionId), votes.ToArray());
                                votes.Clear();
                            }
                            transactionId = line.Split(',')[2];
                            break;
                        case 'V':
                            votes.Add(Convert.ToInt32(line.Split(',')[1]));
                            break;
                    }
            }
            if (transactionId != null)
            {
                instance.AddEntry(Convert.ToInt32(transactionId), votes.ToArray());
            }
            return instance;
        }
    }
}

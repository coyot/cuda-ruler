using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using aCudaResearch.Helpers;
using aCudaResearch.Settings;

namespace aCudaResearch
{
    /// <summary>
    /// Main class of the aCuda project.
    /// 
    /// EXECUTION: [TODO]
    /// </summary>
    public class ACudaMain
    {
        public static void Main(string[] args)
        {
            var settingsFilePath = string.Empty;
            var printResults = true;
            //var settingOverridedTransactionNumber = new List<int>()
            //                                            {
            //                                                1000,
            //                                                2000,
            //                                                5000,
            //                                                10000
            //                                            };

            //var settingOverridedTransactionNumber = new List<int>()
            //                                            {
            //                                                15000,
            //                                                20000,
            //                                                25000,
            //                                            }; 
            var settingOverridedTransactionNumber = new List<int>()
                                                        {
                                                            30000,
                                                            32711
                                                        };

            var p = new OptionSet()
                .Add("s=", v => settingsFilePath = v)
                .Add("print=", v => printResults = bool.Parse(v));

            p.Parse(args);

            if (String.IsNullOrEmpty(settingsFilePath))
            {
                Console.WriteLine("You should specify the input file with program settings!");
                EndMessage();
                return;
            }

            if(!File.Exists(settingsFilePath))
            {
                Console.WriteLine("File {0} does not exists", settingsFilePath);
                EndMessage();
                return;
            }

            ISettingsBuilder builder = new XmlSettingsBuilder(settingsFilePath);

            try
            {
                var settings = builder.Build();
                if (settings != null)
                {
                    foreach (var overridedTransactionNumber in settingOverridedTransactionNumber)
                    {
                        settings.TransactionsNumber = overridedTransactionNumber;
                        var engine = new ExecutionEngine(settings);
                        var result = engine.ExecuteComputation(printResults);

                        if (String.IsNullOrEmpty(settings.OutputFile))
                        {
                            Console.WriteLine(result.Print());
                        }
                        else
                        {
                            var s = new FileStream(settings.OutputFile, FileMode.Append);
                            var writer = new StreamWriter(s);

                            writer.Write(settings.TransactionsNumber + " " + result.Print());
                            writer.Close();
                            s.Close();
                        }
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("There was an error during XML settings parsing.");
                Console.WriteLine(e.ToString());
            }
            EndMessage();
        }

        private static void EndMessage()
        {
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey();
        }
    }
}

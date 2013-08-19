using System;
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
            Console.WriteLine("This is my master thesis research app.");
            ISettingsBuilder builder = new XmlSettingsBuilder(@"D:\MGR\aCuda\src\aCuda\aCudaResearch\aCudaResearch\Data\Settings.xml");

            try
            {
                var settings = builder.Build();
                if (settings != null)
                {
                    var engine = new ExecutionEngine(settings);

                    var result = engine.ExecuteComputation();
                    result.Print();
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("There was an error during XML settings parsing.");
                Console.WriteLine(e.ToString());
            }

            Console.ReadKey();
        }
    }
}

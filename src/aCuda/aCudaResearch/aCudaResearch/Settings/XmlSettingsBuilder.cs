using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace aCudaResearch.Settings
{
    public class XmlSettingsBuilder : ISettingsBuilder
    {
        private string Path { get; set; }
        public XmlSettingsBuilder(string path)
        {
            Path = path;
        }

        #region ISettingsBuilder Members

        public ExecutionSettings Build()
        {
            var serializer = new XmlSerializer(typeof(ExecutionSettings));
            var stream = new FileStream(Path, FileMode.Open);
            var set = (ExecutionSettings)serializer.Deserialize(stream);

            stream.Close();

            return set;
        }

        #endregion
    }
}

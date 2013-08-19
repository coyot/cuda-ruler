namespace aCudaResearch.Data.MsWeb
{
    /// <summary>
    /// Describes entry from the MsWeb data.
    /// </summary>
    public class MsElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="aCudaResearch.FpGrowth.Data.MsWeb.MsElement"/> class.
        /// </summary>
        public MsElement(string title, string url)
        {
            Title = title;
            Url = url;
        }

        /// <summary>
        /// Page title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Page URL address.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents 
        /// the current <see cref="aCudaResearch.FpGrowth.Data.MsWeb.MsElement"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents 
        /// the current <see cref="aCudaResearch.FpGrowth.Data.MsWeb.MsElement"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Title: {0}, Url: {1}", Title, Url);
        }
    }
}

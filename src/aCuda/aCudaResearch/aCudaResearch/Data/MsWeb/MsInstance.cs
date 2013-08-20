using System;
using System.Collections.Generic;

namespace CudaRuler.Data.MsWeb
{
    public class MsInstance<T>
    {
        private int _maxTransactions;
        private int _numberOfTransactions;
        #region Data information

        public const int IdPlace = 1;
        public const int TitlePlace = 3;
        public const int UrlPlace = 4;
        #endregion

        /// <summary>
        /// Information about individual elements from the website.
        /// 
        /// e_id -> info
        /// </summary>
        public Dictionary<T, MsElement> Elements { get; set; }

        /// <summary>
        /// Read data from the archive file.
        /// 
        /// t_id -> table od e_ids
        /// </summary>
        public Dictionary<T, T[]> Transactions { get; set; }

        public MsInstance(int maxTransactions)
        {
            Elements = new Dictionary<T, MsElement>();
            Transactions = new Dictionary<T, T[]>();
            _maxTransactions = maxTransactions;
            _numberOfTransactions = 0;
        }

        /// <summary>
        /// Adding new Element to the definition dictionary.
        /// </summary>
        /// <param name="textLine">One line from the file with data needed to create an element.</param>
        public void AddElement(string textLine)
        {
            if (textLine == null) throw new ArgumentNullException("textLine");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            var parts = textLine.Split(',');
            var id = (T)Convert.ChangeType(Convert.ToInt32(parts[IdPlace]), typeof(T));
            var attr = new MsElement(parts[TitlePlace], parts[UrlPlace]);

            Elements.Add(id, attr);
        }

        /// <summary>
        /// Adding information about the path.
        /// </summary>
        /// <param name="id">Element id number.</param>
        /// <param name="values">Data associated with this element.</param>
        public void AddEntry(T id, T[] values)
        {
            if (_numberOfTransactions < _maxTransactions)
            {
                Transactions.Add(id, values);
                _numberOfTransactions++;
            }
        }
    }
}

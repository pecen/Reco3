using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace ExcelReader
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// Modified by Thomas Petersson @ Scania AB, 2017
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel cell
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Used for converting from Excel column/row to column index starting at 0
        /// </summary>
        [XmlAttribute("r")]
        public string CellReference
        {
            get
            {
                return _cellReference;
            }
            set
            {
                _cellReference = value;
                ColumnIndex = GetColumnIndex(value);
            }
        }
        private string _cellReference;

        [XmlAttribute("t")]
        public string tType = "";
        /// <summary>
        /// Original value of the Excel cell
        /// </summary>
        [XmlElement("v")]
        public string Value { get; set; }

        /// <summary>
        /// Index of the orignal Excel cell column starting at 0
        /// </summary>
        [XmlIgnore] public int ColumnIndex;

        /// <summary>
        /// Text of the Excel cell (if it was a string)
        /// </summary>
        [XmlIgnore] public string Text;

        /// <summary>
        /// Amount of the Excel cell (if it was a number)
        /// </summary>
        [XmlIgnore]
        public double? Amount;

        public void ExpandCell(SharedString[] si)
        {
            if (Value == null)
                return;
            if (tType.Equals("s"))
            {
                Text = si[Int32.Parse(Value)].t;
                return;
            }
            if (tType.Equals("str"))
            {
                Text = Value;
                return;
            }
            try
            {
                var re = new Regex("\\d+");
                var m = re.Match(Value);
                if (m.Success)
                {
                    Amount = double.Parse(Value, CultureInfo.InvariantCulture);
                    Text = Amount.Value.ToString("G");
                }
            }
            catch (Exception ex)
            {
                Amount = 0;
                Text = $"Cell Value '{Value}': {ex.Message}";
            }

        }

        public static int GetColumnIndex(string cellReference)
        {
            string colLetter = new Regex("[A-Za-z]+").Match(cellReference).Value.ToUpper();
            return colLetter.Select(c => c - 'A' + 1).Aggregate((sum, x) => sum * 26 + x) - 1;
        }
    }
}

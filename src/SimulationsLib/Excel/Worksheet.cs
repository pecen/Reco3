using System;
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
    /// Deals with an Excel worksheet in an xlsx-file
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    [XmlRoot("worksheet", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    public class Worksheet
    {
        [XmlArray("sheetData")] [XmlArrayItem("row")] public Row[] FilledRows;

        [XmlIgnore] public Row[] Rows;

        /// <summary>
        /// Note that rows are numbered from 1.. while columns are numbered from 0.., so upper left cell is (1,0).
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public Cell GetCell(int r, int c) => r >= Rows.Length ? null : Rows[r]?.GetCell(c);

        public Cell GetCell(string a1ref)
        {
            var c = Cell.GetColumnIndex(a1ref);
            var r = int.Parse(new Regex(@"\d+").Match(a1ref).Value);
            return GetCell(r, c);
        }

        public void ExpandRows(SharedString[] si)
        {
            var maxRow = FilledRows.Max(r => r.RowNo);
            Rows = new Row[maxRow + 1];
            foreach (var row in FilledRows)
            {
                Rows[row.RowNo] = row;
                row.ExpandCells(si);
            }
        }
    }
}

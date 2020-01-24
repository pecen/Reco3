using System.Linq;
using System.Xml.Serialization;

namespace ExcelReader
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// Modified by Thomas Petersson @ Scania AB, 2017
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel row
    /// </summary>
    public class Row
    {
        [XmlAttribute("r")]
        public int RowNo { get; set; }

        [XmlElement("c")] public Cell[] FilledCells;

        [XmlIgnore] public Cell[] Cells;

        private int _maxColumn;

        public Cell GetCell(int col) => col > _maxColumn ? null : Cells[col];

        public void ExpandCells(SharedString[] si)
        {
            _maxColumn = FilledCells?.Max(c => c.ColumnIndex) ?? -1;
            if (FilledCells == null) return;
            Cells = new Cell[_maxColumn + 1];
            foreach (var cell in FilledCells)
            {
                cell.ExpandCell(si);
                Cells[cell.ColumnIndex] = cell;
            }
        }
    }
}

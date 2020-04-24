using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ExcelReader
{
    /// <summary>
    /// (c) 2014 Vienna, Dietmar Schoder
    /// Modified by Thomas Petersson @ Scania AB, 2017
    /// 
    /// Code Project Open License (CPOL) 1.02
    /// 
    /// Deals with an Excel workbook in an xlsx-file and provides all worksheets in it
    /// </summary>
    [XmlRoot("workbook", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
    public class Workbook : IDisposable
    {
        [XmlIgnore] public sst SharedStrings;

        [XmlArray("sheets")]
        public Sheet[] Sheets { get; set; }


        public static Workbook OpenWorkbook(string excelFileName)
        {
            var za = ZipFile.Open(excelFileName, ZipArchiveMode.Read);
            var wb = DeserializedZipEntry<Workbook>(za.GetEntry("xl/workbook.xml"));
            wb.SharedStrings = DeserializedZipEntry<sst>(za.GetEntry(@"xl/sharedStrings.xml"));
            wb.zipArchive = za;
            return wb;
        }

        public Worksheet GetWorksheet(string sheetName)
        {
            int ndx = 1;
            foreach (Sheet ws in Sheets)
            {
                if (ws.Name==sheetName)
                {
                    var sheet = DeserializedZipEntry<Worksheet>(zipArchive.GetEntry($"xl/worksheets/sheet{ndx}.xml"));
                    sheet.ExpandRows(SharedStrings.si);
                    return sheet;
                }
                ndx++;
            }

            var sheetId = Sheets.FirstOrDefault(s => s.Name == sheetName)?.SheetId ?? -1;
            if (sheetId < 0) return null;
            else
            {
                var sheet = DeserializedZipEntry<Worksheet>(zipArchive.GetEntry($"xl/worksheets/sheet{sheetId}.xml"));
                sheet.ExpandRows(SharedStrings.si);
                return sheet;
            }
            return null;
        }

        public Worksheet GetWorksheetByIndex(int nSheetIndex)
        {
            int ndx = 0;
            foreach (Sheet ws in Sheets)
            {
                if (ndx == nSheetIndex)
                {
                    var sheet = DeserializedZipEntry<Worksheet>(zipArchive.GetEntry($"xl/worksheets/sheet{ndx}.xml"));
                    sheet.ExpandRows(SharedStrings.si);
                    return sheet;
                }
                ndx++;
            }

            return null;
        }


        private ZipArchive zipArchive;
        

        /// <summary>
        /// Method converting an Excel cell value to a date
        /// </summary>
        /// <param name="ExcelCellValue"></param>
        /// <returns></returns>
        public static DateTime DateFromExcelFormat(string ExcelCellValue)
        {
            return DateTime.FromOADate(Convert.ToDouble(ExcelCellValue));
        }
        
        private static T DeserializedZipEntry<T>(ZipArchiveEntry ZipArchiveEntry)
        {
            using (Stream stream = ZipArchiveEntry.Open())
                return (T)new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(stream));
        }

        public void Dispose()
        {
            ((IDisposable)zipArchive).Dispose();
        }
    }
}

using System.Xml.Serialization;

namespace ExcelReader
{
    [XmlType("sheet")]
    public class Sheet
    {
        [XmlAttribute("sheetId")] public int SheetId { get; set; }
        [XmlAttribute("name")] public string Name { get; set; }
    }
}

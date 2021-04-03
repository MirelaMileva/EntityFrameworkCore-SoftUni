using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Export.Dto
{
    [XmlType("User")]
    public class UserXmlExportModel
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public PurchaseXmlExportModel[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal? TotalSpent { get; set; }

    }

    [XmlType("Purchase")]
    public class PurchaseXmlExportModel
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string Cvc { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }

        [XmlElement("Game")]
        public GameXmlExportModel Game { get; set; }
    }

    [XmlType("Game")]
    public class GameXmlExportModel
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal? Price { get; set; }
    }
}

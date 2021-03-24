using System.Xml.Serialization;

namespace CarDealer.DTOModels.Output
{
    [XmlType("suplier")]
    public class SuppliersOutputModel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
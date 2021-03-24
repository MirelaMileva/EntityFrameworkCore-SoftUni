using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    public class UserCountOutputModel
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UsersOutputModel[] Users { get; set; }
    }

    [XmlType("User")]
    public class UsersOutputModel
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ProductCountOutputModel SoldProduct { get; set; }
    }

    public class ProductCountOutputModel
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ExportProductOutputModel[] Products { get; set; }
    }

    [XmlType("Product")]
    public class ExportProductOutputModel
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
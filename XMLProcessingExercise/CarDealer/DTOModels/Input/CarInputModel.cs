using CarDealer.Models;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CarDealer.DTOModels.Input
{
    [XmlType("Car")]
    public class CarInputModel
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public CarPartsInputModel[] CarPartsInputModel { get; set; }
    }
}
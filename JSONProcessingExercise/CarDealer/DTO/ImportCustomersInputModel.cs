using System;

namespace CarDealer.DTO
{
    class ImportCustomersInputModel
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsYoungDriver { get; set; }
    }
}
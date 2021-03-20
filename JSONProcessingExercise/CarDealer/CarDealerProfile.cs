using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportPartsInputModel, Part>();

            this.CreateMap<ImportCarsInputModel, Car>();

            this.CreateMap<ImportCustomersInputModel, Customer>();

            this.CreateMap<ImportSalesInputModel, Sale>();
        }
    }
}

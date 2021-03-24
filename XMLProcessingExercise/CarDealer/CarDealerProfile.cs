using AutoMapper;
using CarDealer.DTOModels.Output;
using CarDealer.Models;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<Part, CarPartInfoOutputModel>();

            this.CreateMap<Car, CarPartOutputModel>()
                .ForMember(x => x.Parts, y=> y.MapFrom(x => x.PartCars.Select(pc => pc.Part)));

            this.CreateMap<Supplier, SuppliersOutputModel>()
                .ForMember(x => x.PartsCount, y => y.MapFrom(x => x.Parts.Count));

        }
    }
}

using System;
using System.IO;
using System.Linq;
using CarDealer.Data;
using System.Collections.Generic;
using System.Xml.Serialization;

using CarDealer.Models;
using CarDealer.DTOModels.Input;
using CarDealer.DTOModels.Output;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Text;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            ////context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var supplierXml = File.ReadAllText("./Datasets/suppliers.xml");
            //var partXml = File.ReadAllText("./Datasets/parts.xml");
            //var carXml = File.ReadAllText("./Datasets/cars.xml");
            //var customerXml = File.ReadAllText("./Datasets/customers.xml");
            //var saleXml = File.ReadAllText("./Datasets/sales.xml");

            //var suppliers = ImportSuppliers(context, supplierXml);
            //var parts = ImportParts(context, partXml);
            //var cars = ImportCars(context, carXml);
            //var customers = ImportCustomers(context, customerXml);
            //var sales = ImportSales(context, saleXml);

            var result = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(result);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new SaleOutputModel
                {
                    Car = new CarSaleOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWithDiscount = (x.Car.PartCars.Sum(p => p.Part.Price) * (1 - x.Discount / 100m)),
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SaleOutputModel[]), new XmlRootAttribute("sales"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, sales, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new CustomersOutputModel
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales
                    .Select(x => x.Car)
                    .SelectMany(x => x.PartCars)
                    .Sum(x => x.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CustomersOutputModel[]), new XmlRootAttribute("customers"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, customers, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            //InitializeAutoMapper();

            var cars = context
                .Cars
                .Select(x => new CarPartOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(x => new CarPartInfoOutputModel
                    {
                        Name = x.Part.Name,
                        Price = x.Part.Price,
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(p => p.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarPartOutputModel[]), new XmlRootAttribute("cars"));

            using var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, ns);

            var result = textWriter.ToString();

            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new SuppliersOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(SuppliersOutputModel[]), new XmlRootAttribute("suppliers"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, suppliers, ns);
            var result = textWriter.ToString();

            return result;
        }
           

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new BmwOutputModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(BmwOutputModel[]), new XmlRootAttribute("cars"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarOutputModel[]), new XmlRootAttribute("cars"));

            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, cars, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string root = "Sales";

            var xmlSerializer = new XmlSerializer(typeof(SaleInputModel[]), new XmlRootAttribute(root));

            var textReader = new StringReader(inputXml);

            var saleInputModels = xmlSerializer.Deserialize(textReader) as SaleInputModel[];

            var carIds = context.Cars.Select(x => x.Id).ToList();

            var sales = saleInputModels
                .Where(c => carIds.Contains(c.CarId))
                .Select(x => new Sale
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string root = "Customers";

            var xmlSerializer = new XmlSerializer(typeof(CustomerInputModel[]), new XmlRootAttribute(root));

            var textReader = new StringReader(inputXml);

            var customerInputModel = xmlSerializer.Deserialize(textReader) as CustomerInputModel[];

            var customers = customerInputModel.
                Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string root = "Cars";

            var xmlSerializer = new XmlSerializer(typeof(CarInputModel[]), new XmlRootAttribute(root));

            var textReader = new StringReader(inputXml);

            var carInputModels = xmlSerializer.Deserialize(textReader) as CarInputModel[];

            var allParts = context.Parts.Select(x => x.Id).ToList();

            var cars = carInputModels
                .Select(x => new Car
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    PartCars = x.CarPartsInputModel.Select(x => x.Id)
                    .Distinct()
                    .Intersect(allParts)
                    .Select(pc => new PartCar
                    { 
                        PartId = pc
                    })
                    .ToList()
                })
                .ToList();

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string root = "Parts";

            var xmlSerializer = new XmlSerializer(typeof(PartInputModel[]), new XmlRootAttribute(root));

            var textReader = new StringReader(inputXml);

            var partInputModels = xmlSerializer.Deserialize(textReader) as PartInputModel[];

            var supplierIds = context.Suppliers.Select(x => x.Id).ToList();

            var parts = partInputModels
                .Where(s => supplierIds.Contains(s.SupplierId))
                .Select(x => new Part
            {
                Name = x.Name,
                Price = x.Price,
                Quantity = x.Quantity,
                SupplierId = x.SupplierId,
            })
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(SupplierInputModel[]), new XmlRootAttribute("Suppliers"));
            var textRead = new StringReader(inputXml);

            var suppliersDto = xmlSerializer.Deserialize(textRead) as SupplierInputModel[];

            var suppliers = suppliersDto.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter,
            })
                .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
                
            });

            mapper = config.CreateMapper();
        }
    }
}
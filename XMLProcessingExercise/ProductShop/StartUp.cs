using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            //var productsXml = File.ReadAllText("../../../Datasets/products.xml");
            //var categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var categoriesProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //ImportUsers(context, usersXml);
            //ImportProducts(context, productsXml);
            //ImportCategories(context, categoriesXml);
            //ImportCategoryProducts(context, categoriesProductsXml);

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersAndProducts = context
                .Users
                .ToArray()
                .Where(x => x.ProductsSold.Any())
                .Select(u => new UsersOutputModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProduct = new ProductCountOutputModel
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new ExportProductOutputModel
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProduct.Count)
                .Take(10)
                .ToArray();

            var resultDto = new UserCountOutputModel
            {
                Count = usersAndProducts.Length,
                Users = usersAndProducts
            };

            var xmlSerializer = new XmlSerializer(typeof(UserCountOutputModel), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, resultDto, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new CategoryOutputModel
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Select(cp => cp.Product).Average(p => p.Price),
                    TotalRevenue = x.CategoryProducts.Select(t => t.Product).Sum(t => t.Price),
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(t => t.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CategoryOutputModel[]), new XmlRootAttribute("Categories"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, categories, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersProducts = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .Select(x => new UserSoldProductOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold.Select(p => new UserProductOutputModel
                    {
                        Name = p.Name,
                        Price = p.Price
                    })              
                    .ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(UserSoldProductOutputModel[]), new XmlRootAttribute("Users"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, usersProducts, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new ProductsInRangeOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName,
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ProductsInRangeOutputModel[]), new XmlRootAttribute("Products"));

            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(textWriter, products, ns);
            var result = textWriter.ToString();

            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerialixer = new XmlSerializer(typeof(CategoryProductsInputModel[]), new XmlRootAttribute("CategoryProducts"));
            var textReader = new StringReader(inputXml);

            var categories = context.Categories.Select(x => x.Id);
            var products = context.Products.Select(x => x.Id);

            var categoryProductsDto = xmlSerialixer.Deserialize(textReader) as CategoryProductsInputModel[];

            var categoriesProducts = categoryProductsDto
                .Where(x => categories.Contains(x.CategoryId) && products.Contains(x.ProductId))
                .Select(x => new CategoryProduct
                {
                    CategoryId = x.CategoryId,
                    ProductId = x.ProductId,
                })
                .ToArray();

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerialixer = new XmlSerializer(typeof(CategoryInputModel[]), new XmlRootAttribute("Categories"));
            var textReader = new StringReader(inputXml);

            var categoriesDto = xmlSerialixer.Deserialize(textReader) as CategoryInputModel[];

            var categories = categoriesDto
                .Where(x => x.Name != null)
                .Select(x => new Category
                {
                    Name = x.Name
                })
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerialixer = new XmlSerializer(typeof(ProductInputModel[]), new XmlRootAttribute("Products"));
            var textReader = new StringReader(inputXml);

            var productsDto = xmlSerialixer.Deserialize(textReader) as ProductInputModel[];

            var products = productsDto.Select
                (x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerId = x.BuyerId,
                    SellerId = x.SellerId,
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerialixer = new XmlSerializer(typeof(UserInputModel[]), new XmlRootAttribute("Users"));
            var textReader = new StringReader(inputXml);

            var usersDto = xmlSerialixer.Deserialize(textReader) as UserInputModel[];

            var users = usersDto
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age
                })
                .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }
    }
}
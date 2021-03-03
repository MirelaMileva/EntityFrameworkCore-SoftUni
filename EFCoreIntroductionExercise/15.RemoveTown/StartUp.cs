namespace _15.RemoveTown
{
    using System;
    using SoftUni.Data;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var softUniContext = new SoftUniContext();
            var result = RemoveTown(softUniContext);
            Console.WriteLine(result);
        }
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(x => x.Addresses)
                .FirstOrDefault(x => x.Name == "Seattle");
            var allAddressIds = town.Addresses.Select(x => x.AddressId).ToList();

            var employeesIds = context.Employees
                .Where(x => allAddressIds.Contains(x.AddressId.Value))
                .ToList();

            foreach (var employee in employeesIds)
            {
                employee.AddressId = null;
            }

            foreach (var addressId in allAddressIds)
            {
                var address = context.Addresses.FirstOrDefault(x => x.AddressId == addressId);
                context.Addresses.Remove(address);
            }

            context.Towns.Remove(town);

            context.SaveChanges();

            var result = $"{allAddressIds.Count} addresses in Seattle were deleted";

            return result;
        }

    }
}

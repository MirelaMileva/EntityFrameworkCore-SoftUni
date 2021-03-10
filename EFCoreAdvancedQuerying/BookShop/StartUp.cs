namespace BookShop
{
    using Data;
    using System;
    using System.Linq;
    using Initializer;
    using BookShop.Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            var command = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, command));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}

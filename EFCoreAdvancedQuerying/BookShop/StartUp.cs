namespace BookShop
{
    using Data;
    using System;
    using System.Linq;
    using Initializer;
    using BookShop.Models.Enums;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            var input = Console.ReadLine();
            Console.WriteLine(GetBookTitlesContaining(db, input));
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

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .Select(x => new 
                {
                    x.BookId,
                    x.Title,
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));

            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));

            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToArray();

            var books = context.Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
                .ToList()
                .Where(x => x.BookCategories.Any(category => categories.Contains(category.Category.Name.ToLower())))
                .Select(books => books.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateGiven = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(book => book.ReleaseDate.Value < dateGiven)
                .Select(book => new
                {
                    book.Title,
                    book.EditionType,
                    book.Price,
                    book.ReleaseDate.Value
                })
                .OrderByDescending(x => x.Value)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}"));

            return result;
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(x => EF.Functions.Like(x.FirstName, $"%{input}"))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var result = string.Join(Environment.NewLine, authors.Select(a => $"{a.FirstName} {a.LastName}"));

            return result;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            //var titles = context.Books.Select(x => x.Title.ToLower());

            var books = context.Books
                .Where(titles => EF.Functions.Like(titles.Title, $"%{input}%"))
                .Select(x => new
                {
                    x.Title
                })
                .OrderBy(x => x.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(b => $"{b.Title}"));

            return result;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Include(x => x.Author)
                .Where(x => EF.Functions.Like(x.Author.LastName, $"{input}%"))
                .Select(x => new
                {
                    x.BookId,
                    x.Title,
                    x.Author.FirstName,
                    x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(b => $"{b.Title} ({b.FirstName + " " + b.LastName})"));

            return result;
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .ToList();

            var result = books.Count();

            return result;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var autors = context.Authors
                .Include(x => x.Books)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    TotalCopies = x.Books.Sum(x => x.Copies),
                })
                .OrderByDescending(x => x.TotalCopies)
                .ToList();

            var result = string.Join(Environment.NewLine, autors.Select(x => $"{x.FirstName} {x.LastName} - {x.TotalCopies}"));

            return result;
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Profit = x.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies)
                })
                .OrderByDescending(p => p.Profit)
                .ThenBy(x => x.Name)
                .ToList();

            var result = string.Join(Environment.NewLine, categories.Select(c => $"{c.Name} ${c.Profit:F2}"));

            return result;
        }

        //public static string GetMostRecentBooks(BookShopContext context)
        //{

        //}
    }
}

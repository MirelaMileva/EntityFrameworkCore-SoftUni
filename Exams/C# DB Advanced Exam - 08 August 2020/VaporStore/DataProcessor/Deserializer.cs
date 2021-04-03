namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;
    using VaporStore.DataProcessor.Import.Dto;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var gamesDto = JsonConvert.DeserializeObject<IEnumerable<GamesImportModel>>(jsonString);

            foreach (var currGame in gamesDto)
            {
                if (!IsValid(currGame) || currGame.Tags.Count() == 0)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var genre = context.Genres.FirstOrDefault(x => x.Name == currGame.Genre);
                if (genre == null)
                {
					genre = new Genre { Name = currGame.Genre };
                }

				var developer = context.Developers.FirstOrDefault(x => x.Name == currGame.Developer);
				if (developer == null)
                {
					developer = new Developer { Name = currGame.Developer };
                }

				var game = new Game
				{
					Name = currGame.Name,
					Price = currGame.Price,
					ReleaseDate = currGame.ReleaseDate.Value,
					Developer = developer,
					Genre = genre,
				};

                foreach (var currTag in currGame.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == currTag);
                    if (tag == null)
                    {
						tag = new Tag { Name = currTag };
                    }

					game.GameTags.Add(new GameTag { Tag = tag });
                }

				context.Games.Add(game);
				context.SaveChanges();
				sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count()} tags");
            }

			return sb.ToString();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();

			var usersDto = JsonConvert.DeserializeObject<IEnumerable<UsersJsonImportModel>>(jsonString);

            foreach (var currUser in usersDto)
            {
                if (!IsValid(currUser) || !currUser.Cards.All(IsValid))
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var user = new User
				{
					FullName = currUser.FullName,
					Username = currUser.Username,
					Email = currUser.Email,
					Age = currUser.Age,
					Cards = currUser.Cards.Select(c => new Card
					{
						Number = c.Number,
						Cvc = c.CVC,
						Type = c.Type.Value
					})
					.ToList(),
				};

				context.Users.Add(user);
				context.SaveChanges();

				sb.AppendLine($"Imported {currUser.Username} with {currUser.Cards.Count()} cards");
            }

			return sb.ToString();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var sb = new StringBuilder();

			var xmlSerializer = new XmlSerializer(typeof(PurchaseXmlImportModel[]),
				new XmlRootAttribute("Purchases"));

			var textRead = new StringReader(xmlString);

			var purchasesDto = xmlSerializer.Deserialize(textRead) as PurchaseXmlImportModel[];

            foreach (var currPurchase in purchasesDto)
            {
                if (!IsValid(currPurchase))
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var parsedDate = DateTime.TryParseExact(currPurchase.Date, "dd/MM/yyyy HH:mm", 
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var date);

                if (!parsedDate)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }

				var purchase = new Purchase
				{
					Date = date,
					Type = currPurchase.Type.Value,
					ProductKey = currPurchase.Key,
				};

				purchase.Card = context.Cards.FirstOrDefault(x => x.Number == currPurchase.Card);

				purchase.Game = context.Games.FirstOrDefault(x => x.Name == currPurchase.Title);

				context.Purchases.Add(purchase);
				context.SaveChanges();

				var username = context.Users
					.Where(x => x.Id == purchase.Card.UserId)
					.Select(x => x.Username)
					.FirstOrDefault();

				sb.AppendLine($"Imported {currPurchase.Title} for {username}");
            }

			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}
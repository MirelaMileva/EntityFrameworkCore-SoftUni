namespace _07.PrintAllMinionNames
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = .; Integrated Security = true; Database = MinionsDB";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var minionsQuery = "SELECT Name FROM Minions";

                using var minionsCommand = new SqlCommand(minionsQuery, connection);
                using var reader = minionsCommand.ExecuteReader();

                var minions = new List<string>();

                while (reader.Read())
                {
                    minions.Add((string)reader[0]);
                }

                int counter = 0;

                for (int i = 0; i < minions.Count / 2; i++)
                {
                    Console.WriteLine(minions[0 + counter]);
                    Console.WriteLine(minions[minions.Count - 1 - counter]);
                    counter++;
                }

                if (minions.Count % 2 != 0)
                {
                    Console.WriteLine(minions[minions.Count / 2]);
                }
                
            }
        }
    }
}
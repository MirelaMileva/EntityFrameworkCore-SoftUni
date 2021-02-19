namespace _08.IncreaseMinionAge
{
    using System;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = .; Integrated Security = true; Database = MinionsDB";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int[] minionsIds = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray();

                string updateMinionsQuery = @" UPDATE Minions
                                               SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), 
                                               Age += 1
                                               WHERE Id = @Id";

                foreach (var id in minionsIds)
                {
                    using var sqlCommand = new SqlCommand(updateMinionsQuery, connection);
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    sqlCommand.ExecuteNonQuery();
                }

                var selectMinions = @"SELECT Name, Age FROM Minions";

                using var selectCommand = new SqlCommand(selectMinions, connection);
                using var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} {reader[1]}");
                }
            }
        }
     }
}
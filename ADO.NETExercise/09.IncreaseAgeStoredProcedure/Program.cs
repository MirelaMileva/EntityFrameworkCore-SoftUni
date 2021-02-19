namespace _09.IncreaseAgeStoredProcedure
{
    using System;
    using Microsoft.Data.SqlClient;
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = .; Integrated Security = true; Database = MinionsDB";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int id = int.Parse(Console.ReadLine());

                string query = "EXEC usp_GetOlder @id";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();

                string selectQuery = "SELECT Name, Age FROM Minions WHERE Id = @Id";
                using var selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@id", id);
                using var reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} - {reader[1]} years old");
                }

            }
        }
    }
}
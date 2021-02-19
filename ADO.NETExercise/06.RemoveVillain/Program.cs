namespace _06.RemoveVillain
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

                int value = int.Parse(Console.ReadLine());

                string evilNameQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
                using var sqlCommand = new SqlCommand(evilNameQuery, connection);
                sqlCommand.Parameters.AddWithValue("@villainId", value);
                var name = (string)sqlCommand.ExecuteScalar();

                if (name == null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }

                var deleteMinionsVillainsQuery = @"DELETE FROM MinionsVillains 
                                           WHERE VillainId = @villainId";

                using var sqlDeleteMinionsVillainsCommand = new SqlCommand(deleteMinionsVillainsQuery, connection);
                sqlDeleteMinionsVillainsCommand.Parameters.AddWithValue("@villainId", value);
                var affectedRows = sqlDeleteMinionsVillainsCommand.ExecuteNonQuery();

                var deleteVillainQuery = @"DELETE FROM Villains
                                           WHERE Id = @villainId";
                using var sqlDeleteVillainsCommand = new SqlCommand(deleteVillainQuery, connection);
                sqlDeleteVillainsCommand.Parameters.AddWithValue("@villainId", value);
                sqlDeleteVillainsCommand.ExecuteNonQuery();

                Console.WriteLine($"{name} was deleted.");
                Console.WriteLine($"{affectedRows} minions were released.");
            }
        }
    }
}
namespace _02.VillainNames
{
    using System;
    using System.Data.SqlClient;
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = .; Integrated Security = true; Database = MinionsDB";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT [Name], COUNT(mv.MinionId) 
                                FROM Villains AS v
                                JOIN MinionsVillains AS mv ON mv.VillainId = v.Id
                                GROUP BY v.Id, v.[Name]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader[0];
                            var count = reader[1];

                            Console.WriteLine($"{name} - {count}");
                        }
                        
                    }
                        
                }
            }
        }
    }
}

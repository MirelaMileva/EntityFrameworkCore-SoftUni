namespace _01.InitialSetup
{
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = .; Integrated Security = true; Database = MinionsDB";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string createDatabase = "CREATE DATABASE MinionsDB";
                var createTableStatements = GetCreateTableStatements();

                foreach (var query in createTableStatements)
                {
                    ExecuteNonQuery(connection, query);
                }

                var insertStatemensts = GetInsertDataStatements();

                foreach (var query in insertStatemensts)
                {
                    ExecuteNonQuery(connection, query);
                }
                
            }

        }

        private static void ExecuteNonQuery(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                var result = command.ExecuteNonQuery();
                ;
            }
        }

        private static string[] GetInsertDataStatements()
        {
            var result = new string[]
                {  "INSERT INTO Countries (Id, [Name]) VALUES (1, 'Bulgaria'), (2, 'Norway'), (3, 'Cyprus'), (4, 'Greece'), (5, 'UK')",
                "INSERT INTO Towns (Id, [Name], CountryCode) VALUES (1, 'Plovdiv', 1), (2, 'Oslo', 2), (3, 'Larnaca', 3), (4, 'Athens', 4), (5, 'London', 5)",
                "INSERT INTO Minions (Id, [Name], Age, TownId) VALUES (1, 'Mirela', 12, 1), (2, 'George', 22, 2), (3, 'Ivan', 25, 3), (4, 'Pesho', 33, 4), (5, 'Bobi', 36, 5)",
                "INSERT INTO EvilnessFactors VALUES (1, 'super good'), (2, 'good'), (3, 'bad'), (4,'evil'), (5, 'super evil')",
                "INSERT INTO Villains VALUES (1, 'Gru', 1), (2, 'Bob', 2), (3, 'Ivo', 3), (4, 'Gosho', 4), (5, 'Pesho', 5)",
                "INSERT INTO MinionsVillains VALUES (1,1), (2,2), (3,3), (4,4), (5,5)"
                };

            return result;
        }
        private static string[] GetCreateTableStatements()
        {
            var result = new string[]
                {
                    "CREATE TABLE Countries(Id INT PRIMARY KEY,[Name] VARCHAR(50))",
                    "CREATE TABLE Towns (Id INT PRIMARY KEY,[Name] VARCHAR(50),CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                    "CREATE TABLE Minions(Id INT PRIMARY KEY,[Name] VARCHAR(50),Age INT,TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                    "CREATE TABLE EvilnessFactors(Id INT PRIMARY KEY,[Name] VARCHAR(50))",
                    "CREATE TABLE Villains(Id INT PRIMARY KEY,[Name] VARCHAR(50),EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                    "CREATE TABLE MinionsVillains(MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id) CONSTRAINT PK_MinionsVillains PRIMARY KEY(MinionId,VillainId))"
                };

            return result;
        }
    }
}

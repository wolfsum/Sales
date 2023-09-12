using System.Diagnostics;
using System.Security;
using System.Data.SqlClient;
using System.IO;
using System;
using System.Globalization;
using System.Collections.Generic;

public class Saless
{

    public void Run()
    {
        Sales.DatabaseManager dbmanager = new Sales.DatabaseManager("Data Source=localhost; Application Name=Sales; Initial Catalog=Sales; User ID=sa;Password=sa;");
        dbmanager.CreateTables();
        dbmanager.PopulateUsers();
        dbmanager.PopulateProducts();
        dbmanager.PopulateOrders();
    }
    public class Sales
    {
        public class DatabaseManager
        {
            private string connectionString;

            public DatabaseManager(string connectionString)
            {
                this.connectionString = connectionString;
            }
            private bool IsTableExists(string tableName, SqlConnection connection)
            {
                string checkTableQuery = $"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}')) SELECT 1 ELSE SELECT 0";

                using (SqlCommand command = new SqlCommand(checkTableQuery, connection))
                {
                    return (int)command.ExecuteScalar() == 1;
                }
            }
            private bool IsTablePopulated(string tableName, SqlConnection connection)
            {
                string checkDataQuery = $"SELECT COUNT(*) FROM {tableName}";

                using (SqlCommand command = new SqlCommand(checkDataQuery, connection))
                {
                    return (int)command.ExecuteScalar() > 0;
                }
            }
            public void CreateTables()
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (!IsTableExists("Users", connection))
                    {
                        string createUserTable = @"CREATE TABLE Users (
                                        UserId INT PRIMARY KEY IDENTITY(1,1),
                                        Name NVARCHAR(255),
                                        DateOfBirth DATE
                                    )";

                        using (SqlCommand command = new SqlCommand(createUserTable, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }


                    if (!IsTableExists("Products", connection))
                    {
                        string createProductTable = @"CREATE TABLE Products (
                                            ProductId INT PRIMARY KEY IDENTITY(1,1),
                                            ProductName NVARCHAR(255),
                                            Price DECIMAL(18,2)
                                        )";

                        using (SqlCommand command = new SqlCommand(createProductTable, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    if (!IsTableExists("Orders", connection))
                    {
                        string createOrderTable = @"CREATE TABLE Orders (
                                        OrderId INT PRIMARY KEY IDENTITY(1,1),
                                        UserId BIGINT,
                                        ProductId INT,
                                        TransactionID BIGINT,
                                        OrderDate DATE
                                    )";

                        using (SqlCommand command = new SqlCommand(createOrderTable, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }


            public void PopulateProducts()
            {
                string[] productNames = new string[]
                {
            "Кортошко",
            "Морковко",
            "Огурецко",
            "Помидорко",
            "Капустко"
                };

                Random random = new Random();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (!IsTablePopulated("Products", connection))
                    {
                        foreach (string productName in productNames)
                        {
                            decimal price = random.Next(1000, 5001) + (decimal)(random.NextDouble()); 

                            string query = $"INSERT INTO Products (ProductName, Price) VALUES ('{productName}', {price.ToString("0.00", CultureInfo.InvariantCulture)})"; 

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            public void PopulateUsers()
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (!IsTablePopulated("Users", connection))
                    {
                        for (int i = 1; i <= 1000; i++)
                        {
                            string query = $@"
                                            INSERT INTO Users (Name, DateOfBirth) 
                                            VALUES ('Customer_{i.ToString("000")}', 
                                            DATEADD(DAY, 
                                            (DATEDIFF(DAY, '1950-01-01', '2010-01-01') * RAND()), 
                                            '1950-01-01'))";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            public void PopulateOrders()
            {
                
                Random random = new Random();

                List<int> userIds = new List<int>();
                List<int> productIds = new List<int>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (!IsTablePopulated("Orders", connection))
                    {
                        using (SqlCommand getUsers = new SqlCommand("SELECT UserId FROM Users", connection))
                        {
                            using (SqlDataReader reader = getUsers.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    userIds.Add(reader.GetInt32(0));
                                }
                            }
                        }
                        using (SqlCommand getProducts = new SqlCommand("SELECT ProductId FROM Products", connection))
                        {
                            using (SqlDataReader reader = getProducts.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    productIds.Add(reader.GetInt32(0));
                                }
                            }
                        }

                        for (int i = 0; i < 10000; i++)
                        {
                            int randomUserId = userIds[random.Next(userIds.Count)];
                            int randomProductId = productIds[random.Next(productIds.Count)];



                            int MAX_VALUE = 1000000000;
                            int Rand = 0;
                            long transactionCode = 0;

                            for (int j = 0; j < 20; j++)
                            {
                                if (i != 19)
                                {
                                    Rand = MAX_VALUE - random.Next(1, MAX_VALUE);
                                }
                                else
                                {
                                    Rand = MAX_VALUE;
                                }
                                MAX_VALUE = MAX_VALUE - Rand;
                                long randomValue = Rand * (j + 1);
                                transactionCode = transactionCode + randomValue;

                            }
                            //Небольшое пояснение, учитывая долгое время выполнения для цикла, считающего сумму чисел от 1 до 20 1 миллиард раз, немного сделал иначе для более быстрого выполнения

                            string query = $"INSERT INTO Orders (UserId, ProductId, TransactionID, OrderDate) VALUES ({randomUserId}, {randomProductId},{transactionCode}, GETDATE())"; // Здесь я предполагаю, что у вас есть поле OrderDate в таблице Orders.

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                    }
                }
            }


        }

    }
}


using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using WebApplication1.Models;
using MySqlConnector;
using Dapper;
using System.Data;

namespace WebApplication1.Data
{
    public class DataMigration
    {
        private readonly string _mysqlConnectionString;
        private readonly IMongoDatabase _mongoDatabase;

        public DataMigration(string mysqlConnectionString, IMongoDatabase mongoDatabase)
        {
            _mysqlConnectionString = mysqlConnectionString;
            _mongoDatabase = mongoDatabase;
        }

        public async Task MigrateAllData()
        {
            Console.WriteLine("Starting data migration...");
            try
            {
                Console.WriteLine("Migrating suppliers...");
                await MigrateSuppliers();
                
                Console.WriteLine("Migrating products...");
                await MigrateProducts();
                
                Console.WriteLine("Migrating orders...");
                await MigrateOrders();
                
                Console.WriteLine("Migration completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task MigrateSuppliers()
        {
            var collection = _mongoDatabase.GetCollection<Supplier>("suppliers");
            using (IDbConnection connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();
                Console.WriteLine("Connected to MySQL database");
                var items = await connection.QueryAsync<Supplier>("SELECT * FROM Suppliers");
                Console.WriteLine($"Found {items.Count()} suppliers in MySQL");
                
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        item.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    }
                    await collection.InsertManyAsync(items);
                }
            }
        }

        private async Task MigrateProducts()
        {
            var collection = _mongoDatabase.GetCollection<Product>("products");
            using (IDbConnection connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();
                var items = await connection.QueryAsync<Product>("SELECT * FROM Products");
                
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        item.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                        // Chuyển đổi SupplierId sang ObjectId
                        if (item.SupplierId != null)
                        {
                            var supplier = await _mongoDatabase.GetCollection<Supplier>("suppliers")
                                .Find(s => s.Name == item.SupplierName).FirstOrDefaultAsync();
                            if (supplier != null)
                            {
                                item.SupplierId = supplier.Id;
                            }
                        }
                    }
                    await collection.InsertManyAsync(items);
                }
            }
        }

        private async Task MigrateOrders()
        {
            var collection = _mongoDatabase.GetCollection<Order>("orders");
            using (IDbConnection connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();
                var items = await connection.QueryAsync<Order>("SELECT * FROM Orders");
                
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        item.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                        
                        // Lấy chi tiết đơn hàng
                        var details = await connection.QueryAsync<OrderDetail>(
                            "SELECT * FROM OrderDetails WHERE OrderId = @OrderId",
                            new { OrderId = item.Id }
                        );

                        if (details.Any())
                        {
                            foreach (var detail in details)
                            {
                                // Chuyển đổi ProductId sang ObjectId
                                var product = await _mongoDatabase.GetCollection<Product>("products")
                                    .Find(p => p.Name == detail.ProductName).FirstOrDefaultAsync();
                                if (product != null)
                                {
                                    detail.ProductId = product.Id;
                                }
                            }
                            item.OrderDetails = details.ToList();
                        }
                    }
                    await collection.InsertManyAsync(items);
                }
            }
        }
    }
}

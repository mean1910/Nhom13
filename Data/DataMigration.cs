using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Nhom13.Models;
using MySqlConnector;
using Dapper;
using System.Data;

namespace Nhom13.Data
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
                        item.CreatedAt = DateTime.Now.AddDays(-new Random().Next(1, 60)); // Random date in last 60 days
                        item.IsActive = true;
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

        public async Task SeedSampleData()
        {
            Console.WriteLine("Seeding sample data...");
            try
            {
                var supplierCollection = _mongoDatabase.GetCollection<Supplier>("suppliers");
                var count = await supplierCollection.CountDocumentsAsync(FilterDefinition<Supplier>.Empty);
                
                if (count == 0)
                {
                    var suppliers = new List<Supplier>
                    {
                        new Supplier
                        {
                            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                            Name = "Công ty TNHH Thực phẩm ABC",
                            Address = "123 Nguyễn Văn A, Quận 1, TP.HCM",
                            Phone = "0123456789",
                            Email = "abc@food.com",
                            Note = "Nhà cung cấp thực phẩm chính",
                            CreatedAt = DateTime.Now.AddDays(-30),
                            IsActive = true
                        },
                        new Supplier
                        {
                            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                            Name = "Công ty CP Thực phẩm XYZ",
                            Address = "456 Lê Văn B, Quận 2, TP.HCM",
                            Phone = "0987654321",
                            Email = "xyz@food.com",
                            Note = "Nhà cung cấp gia vị",
                            CreatedAt = DateTime.Now.AddDays(-15),
                            IsActive = true
                        },
                        new Supplier
                        {
                            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                            Name = "Công ty TNHH Rau củ quả Sạch",
                            Address = "789 Trần Văn C, Quận 3, TP.HCM",
                            Phone = "0369852147",
                            Email = "fresh@veggie.com",
                            Note = "Nhà cung cấp rau củ",
                            CreatedAt = DateTime.Now.AddDays(-7),
                            IsActive = true
                        }
                    };

                    await supplierCollection.InsertManyAsync(suppliers);
                    Console.WriteLine($"Added {suppliers.Count} sample suppliers");

                    // Thêm sản phẩm mẫu
                    var productCollection = _mongoDatabase.GetCollection<Product>("products");
                    var productCount = await productCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty);

                    if (productCount == 0)
                    {
                        var products = new List<Product>
                        {
                            new Product
                            {
                                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                                Name = "Thịt bò Úc",
                                Description = "Thịt bò nhập khẩu từ Úc, đảm bảo chất lượng",
                                Price = 350000,
                                Quantity = 100,
                                SupplierId = suppliers[0].Id,
                                SupplierName = suppliers[0].Name,
                                CreatedDate = DateTime.Now.AddDays(-20),
                                CreatedBy = "Admin"
                            },
                            new Product
                            {
                                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                                Name = "Gia vị lẩu Thái",
                                Description = "Gia vị ướp lẩu Thái chuẩn vị",
                                Price = 45000,
                                Quantity = 200,
                                SupplierId = suppliers[1].Id,
                                SupplierName = suppliers[1].Name,
                                CreatedDate = DateTime.Now.AddDays(-15),
                                CreatedBy = "Admin"
                            },
                            new Product
                            {
                                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                                Name = "Rau cải thìa",
                                Description = "Rau cải thìa tươi sạch",
                                Price = 15000,
                                Quantity = 300,
                                SupplierId = suppliers[2].Id,
                                SupplierName = suppliers[2].Name,
                                CreatedDate = DateTime.Now.AddDays(-10),
                                CreatedBy = "Admin"
                            }
                        };

                        await productCollection.InsertManyAsync(products);
                        Console.WriteLine($"Added {products.Count} sample products");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}

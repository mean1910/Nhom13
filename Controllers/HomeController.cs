using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using MongoDB.Driver;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Supplier> _suppliers;

        public HomeController(ILogger<HomeController> logger, IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
            _orders = database.GetCollection<Order>("orders");
            _products = database.GetCollection<Product>("products");
            _suppliers = database.GetCollection<Supplier>("suppliers");
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tổng số nhà cung cấp
            var totalSuppliers = await _suppliers.CountDocumentsAsync(FilterDefinition<Supplier>.Empty);
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.SupplierGrowth = 15.5;
            ViewBag.NewSuppliers = await _suppliers.CountDocumentsAsync(s => s.CreatedAt >= DateTime.Now.AddDays(-30));

            // Lấy tổng số đơn hàng
            var totalOrders = await _orders.CountDocumentsAsync(FilterDefinition<Order>.Empty);
            ViewBag.TotalOrders = totalOrders;
            ViewBag.OrderGrowth = 25.3;
            ViewBag.NewOrders = await _orders.CountDocumentsAsync(o => o.OrderDate >= DateTime.Now.AddDays(-30));

            // Lấy tổng số tiền
            var allOrders = await _orders.Find(_ => true).ToListAsync();
            var totalAmount = allOrders.Sum(o => o.TotalAmount);
            ViewBag.TotalAmount = totalAmount;
            ViewBag.AmountGrowth = 30.2;
            ViewBag.NewAmount = allOrders.Where(o => o.OrderDate >= DateTime.Now.AddDays(-30)).Sum(o => o.TotalAmount);

            // Lấy tổng số sản phẩm
            var totalProducts = await _products.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            ViewBag.TotalProducts = totalProducts;
            ViewBag.ProductGrowth = 18.7;
            ViewBag.NewProducts = await _products.CountDocumentsAsync(p => p.CreatedAt >= DateTime.Now.AddDays(-30));

            // Thống kê khác
            ViewBag.WeeklyRevenue = allOrders.Where(o => o.OrderDate >= DateTime.Now.AddDays(-7)).Sum(o => o.TotalAmount);
            ViewBag.RevenueGrowth = 45.14;
            ViewBag.ExpenseRatio = 0.58;
            ViewBag.BusinessRisk = "Thấp";

            // Lấy đơn hàng gần đây
            var recentOrders = await _orders
                .Find(_ => true)
                .SortByDescending(o => o.OrderDate)
                .Limit(5)
                .ToListAsync();

            // Lấy thông tin nhà cung cấp cho mỗi đơn hàng
            foreach (var order in recentOrders)
            {
                var supplier = await _suppliers.Find(s => s.Id == order.SupplierId).FirstOrDefaultAsync();
                order.SupplierName = supplier?.Name;
            }

            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

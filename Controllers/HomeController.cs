using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Nhom13.Models;

namespace Nhom13.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Order> _orderCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<Supplier> _supplierCollection;

        public HomeController(ILogger<HomeController> logger, IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
            _orderCollection = database.GetCollection<Order>("Orders");
            _productCollection = database.GetCollection<Product>("Products");
            _supplierCollection = database.GetCollection<Supplier>("Suppliers");
        }

        public async Task<IActionResult> Index()
        {
            // Lấy tổng số nhà cung cấp
            var totalSuppliers = await _supplierCollection.CountDocumentsAsync(FilterDefinition<Supplier>.Empty);
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.SupplierGrowth = 15.5;
            ViewBag.NewSuppliers = await _supplierCollection.CountDocumentsAsync(s => s.CreatedAt >= DateTime.Now.AddDays(-30));

            // Lấy tổng số đơn hàng
            var totalOrders = await _orderCollection.CountDocumentsAsync(FilterDefinition<Order>.Empty);
            ViewBag.TotalOrders = totalOrders;
            ViewBag.OrderGrowth = 25.3;
            ViewBag.NewOrders = await _orderCollection.CountDocumentsAsync(o => o.CreatedDate >= DateTime.Now.AddDays(-30));

            // Lấy tổng số tiền
            var allOrders = await _orderCollection.Find(_ => true).ToListAsync();
            var totalAmount = allOrders.Sum(o => o.TotalAmount);
            ViewBag.TotalAmount = totalAmount;
            ViewBag.AmountGrowth = 30.2;
            ViewBag.NewAmount = allOrders.Where(o => o.CreatedDate >= DateTime.Now.AddDays(-30)).Sum(o => o.TotalAmount);

            // Lấy tổng số sản phẩm
            var totalProducts = await _productCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            ViewBag.TotalProducts = totalProducts;
            ViewBag.ProductGrowth = 18.7;
            ViewBag.NewProducts = await _productCollection.CountDocumentsAsync(p => p.CreatedDate >= DateTime.Now.AddDays(-30));

            // Thống kê khác
            ViewBag.WeeklyRevenue = allOrders.Where(o => o.CreatedDate >= DateTime.Now.AddDays(-7)).Sum(o => o.TotalAmount);
            ViewBag.RevenueGrowth = 45.14;
            ViewBag.ExpenseRatio = 0.58;
            ViewBag.BusinessRisk = "Thấp";

            // Lấy đơn hàng gần đây
            var recentOrders = await _orderCollection
                .Find(_ => true)
                .SortByDescending(o => o.CreatedDate)
                .Limit(5)
                .ToListAsync();

            // Lấy thông tin sản phẩm cho mỗi đơn hàng
            foreach (var order in recentOrders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productCollection.Find(p => p.Id == detail.ProductId).FirstOrDefaultAsync();
                    detail.ProductName = product?.Name;
                }
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

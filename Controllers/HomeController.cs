using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu mẫu cho thống kê
            ViewBag.TotalSuppliers = 150;
            ViewBag.SupplierGrowth = 15.5;
            ViewBag.NewSuppliers = 20;
            ViewBag.TotalOrders = 450;
            ViewBag.OrderGrowth = 25.3;
            ViewBag.NewOrders = 90;
            ViewBag.TotalAmount = 150000000;
            ViewBag.AmountGrowth = 30.2;
            ViewBag.NewAmount = 35000000;
            ViewBag.TotalProducts = 1200;
            ViewBag.ProductGrowth = 18.7;
            ViewBag.NewProducts = 200;
            ViewBag.WeeklyRevenue = 25000000;
            ViewBag.RevenueGrowth = 45.14;
            ViewBag.ExpenseRatio = 0.58;
            ViewBag.BusinessRisk = "Thấp";

            // Dữ liệu mẫu cho đơn hàng gần đây
            ViewBag.RecentOrders = new[]
            {
                new { OrderCode = "DH001", SupplierName = "Công ty A", Quantity = 100, Status = "Đã duyệt", TotalAmount = 15000000 },
                new { OrderCode = "DH002", SupplierName = "Công ty B", Quantity = 50, Status = "Chờ duyệt", TotalAmount = 7500000 },
                new { OrderCode = "DH003", SupplierName = "Công ty C", Quantity = 200, Status = "Từ chối", TotalAmount = 30000000 },
                new { OrderCode = "DH004", SupplierName = "Công ty D", Quantity = 75, Status = "Đã duyệt", TotalAmount = 11250000 },
                new { OrderCode = "DH005", SupplierName = "Công ty E", Quantity = 150, Status = "Chờ duyệt", TotalAmount = 22500000 }
            };

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

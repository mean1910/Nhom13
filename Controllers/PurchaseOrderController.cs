using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrder
        public async Task<IActionResult> Index()
        {
            var orders = await _context.PurchaseOrders
                .Include(p => p.OrderDetails)
                .ThenInclude(d => d.Ingredient)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // GET: PurchaseOrder/Create
        public IActionResult Create()
        {
            ViewBag.Ingredients = _context.Ingredients.ToList();
            return View();
        }

        // POST: PurchaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] ingredientIds, int[] quantities, string notes)
        {
            if (ingredientIds.Length != quantities.Length || ingredientIds.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng nhập ít nhất một nguyên liệu");
                ViewBag.Ingredients = _context.Ingredients.ToList();
                return View();
            }

            var purchaseOrder = new PurchaseOrder
            {
                Notes = notes
            };

            for (int i = 0; i < ingredientIds.Length; i++)
            {
                if (int.TryParse(ingredientIds[i], out int ingredientId) && quantities[i] > 0)
                {
                    purchaseOrder.OrderDetails.Add(new PurchaseOrderDetail
                    {
                        IngredientId = ingredientId,
                        Quantity = quantities[i]
                    });
                }
            }

            if (purchaseOrder.OrderDetails.Count == 0)
            {
                ModelState.AddModelError("", "Vui lòng nhập ít nhất một nguyên liệu hợp lệ");
                ViewBag.Ingredients = _context.Ingredients.ToList();
                return View();
            }

            _context.Add(purchaseOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: PurchaseOrder/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var order = await _context.PurchaseOrders
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            
            // Nếu đơn hàng hoàn thành, cập nhật số lượng trong kho
            if (newStatus == OrderStatus.Completed)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var warehouseIngredient = await _context.WarehouseIngredients
                        .FirstOrDefaultAsync(w => w.IngredientId == detail.IngredientId);

                    if (warehouseIngredient != null)
                    {
                        warehouseIngredient.Quantity += detail.Quantity;
                    }
                    else
                    {
                        _context.WarehouseIngredients.Add(new WarehouseIngredient
                        {
                            IngredientId = detail.IngredientId,
                            Quantity = detail.Quantity
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 
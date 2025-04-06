using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Nhom13.Models;

namespace Nhom13.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IMongoCollection<Expense> _expenseCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public ExpensesController(IMongoDatabase database)
        {
            _expenseCollection = database.GetCollection<Expense>("Expenses");
            _productCollection = database.GetCollection<Product>("Products");
        }

        // GET: Expenses
        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseCollection.Find(_ => true).ToListAsync();
            return View(expenses);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseCollection
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(expense.ProductId))
            {
                var product = await _productCollection
                    .Find(p => p.Id == expense.ProductId)
                    .FirstOrDefaultAsync();
                ViewBag.ProductName = product?.Name;
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public async Task<IActionResult> Create()
        {
            // Get list of products for dropdown
            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View();
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,Description,Amount,Date,Note,ProductId")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                expense.CreatedDate = DateTime.Now;
                expense.CreatedBy = User.Identity?.Name;
                await _expenseCollection.InsertOneAsync(expense);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseCollection
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Type,Description,Amount,Date,Note,ProductId")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingExpense = await _expenseCollection
                    .Find(e => e.Id == id)
                    .FirstOrDefaultAsync();

                if (existingExpense == null)
                {
                    return NotFound();
                }

                expense.CreatedBy = existingExpense.CreatedBy;
                expense.CreatedDate = existingExpense.CreatedDate;

                await _expenseCollection.ReplaceOneAsync(e => e.Id == id, expense);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = await _productCollection.Find(_ => true).ToListAsync();
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseCollection
                .Find(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(expense.ProductId))
            {
                var product = await _productCollection
                    .Find(p => p.Id == expense.ProductId)
                    .FirstOrDefaultAsync();
                ViewBag.ProductName = product?.Name;
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _expenseCollection.DeleteOneAsync(e => e.Id == id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Expenses/Report
        public async Task<IActionResult> Report()
        {
            var startDate = DateTime.Today.AddDays(-30); // Last 30 days by default
            var endDate = DateTime.Today;

            var expenses = await _expenseCollection
                .Find(e => e.Date >= startDate && e.Date <= endDate)
                .ToListAsync();

            // Group expenses by type
            var expensesByType = expenses
                .GroupBy(e => e.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .ToList();

            ViewBag.ExpensesByType = expensesByType;
            ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(expenses);
        }
    }
} 
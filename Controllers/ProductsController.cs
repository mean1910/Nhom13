using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Nhom13.Models;

namespace Nhom13.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductsController(IMongoDatabase database)
        {
            _productCollection = database.GetCollection<Product>("Products");
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productCollection.Find(_ => true).ToListAsync();
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productCollection
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Quantity,SupplierId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                product.CreatedBy = User.Identity?.Name;
                await _productCollection.InsertOneAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productCollection
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Price,Quantity,SupplierId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productCollection
                    .Find(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    return NotFound();
                }

                product.CreatedBy = existingProduct.CreatedBy;
                product.CreatedDate = existingProduct.CreatedDate;

                await _productCollection.ReplaceOneAsync(p => p.Id == id, product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productCollection
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _productCollection.DeleteOneAsync(p => p.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
} 
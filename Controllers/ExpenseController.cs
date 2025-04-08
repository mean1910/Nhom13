using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Data;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;

namespace WebApplication1.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense - Hiển thị danh sách chi tiêu
        public async Task<IActionResult> Index()
        {
            return View(await _context.Expenses.ToListAsync());
        }

        // GET: Expense/Create - Form thêm chi tiêu mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Expense/Create - Xử lý thêm chi tiêu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,Description,Amount,Date,Note")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expense/Report - Báo cáo chi tiêu
        public async Task<IActionResult> Report()
        {
            var expenses = await _context.Expenses.ToListAsync();
            var report = new Dictionary<string, decimal>();
            
            foreach (var expense in expenses)
            {
                if (report.ContainsKey(expense.Type))
                    report[expense.Type] += expense.Amount;
                else
                    report[expense.Type] = expense.Amount;
            }
            
            return View(report);
        }

        // GET: Expense/ExportExcel - Xuất báo cáo Excel
        public async Task<IActionResult> ExportExcel()
        {
            var expenses = await _context.Expenses.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Chi tiêu");
                
                // Tạo header
                worksheet.Cell(1, 1).Value = "Loại chi tiêu";
                worksheet.Cell(1, 2).Value = "Mô tả";
                worksheet.Cell(1, 3).Value = "Số tiền";
                worksheet.Cell(1, 4).Value = "Ngày chi tiêu";
                worksheet.Cell(1, 5).Value = "Ghi chú";

                // Điền dữ liệu
                int row = 2;
                foreach (var expense in expenses)
                {
                    worksheet.Cell(row, 1).Value = expense.Type;
                    worksheet.Cell(row, 2).Value = expense.Description;
                    worksheet.Cell(row, 3).Value = expense.Amount;
                    worksheet.Cell(row, 4).Value = expense.Date.ToShortDateString();
                    worksheet.Cell(row, 5).Value = expense.Note;
                    row++;
                }

                // Format bảng
                var range = worksheet.Range(1, 1, row - 1, 5);
                range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoChiTieu.xlsx");
                }
            }
        }
    }
} 
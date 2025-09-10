using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvenientStore.Data;
using ConvenientStore.Models;

namespace ConvenientStore.Controllers;

public class SalesController : Controller
{
    private readonly ConvenientStoreContext _context;

    public SalesController(ConvenientStoreContext context)
    {
        _context = context;
    }

    // GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
        return View(sales);
    }

    // GET: Sales/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (sale == null)
        {
            return NotFound();
        }

        return View(sale);
    }

    // GET: Sales/Create
    public IActionResult Create()
    {
        ViewBag.Products = _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToList();
        return View();
    }

    // POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CustomerName,PaymentMethod")] Sale sale, List<int> ProductIds, List<int> Quantities)
    {
        if (ModelState.IsValid && ProductIds.Count > 0)
        {
            sale.SaleDate = DateTime.Now;
            sale.TotalAmount = 0;

            for (int i = 0; i < ProductIds.Count; i++)
            {
                if (Quantities[i] > 0)
                {
                    var product = await _context.Products.FindAsync(ProductIds[i]);
                    if (product != null && product.StockQuantity >= Quantities[i])
                    {
                        var saleItem = new SaleItem
                        {
                            ProductId = ProductIds[i],
                            Quantity = Quantities[i],
                            UnitPrice = product.Price
                        };
                        sale.SaleItems.Add(saleItem);
                        sale.TotalAmount += saleItem.TotalPrice;

                        // Update stock quantity
                        product.StockQuantity -= Quantities[i];
                        _context.Update(product);
                    }
                }
            }

            if (sale.SaleItems.Count > 0)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        ViewBag.Products = _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToList();
        return View(sale);
    }

    // GET: Sales/Receipt/5
    public async Task<IActionResult> Receipt(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (sale == null)
        {
            return NotFound();
        }

        return View(sale);
    }

    // GET: Sales/DailySummary
    public async Task<IActionResult> DailySummary(DateTime? date)
    {
        var selectedDate = date ?? DateTime.Today;
        var sales = await _context.Sales
            .Where(s => s.SaleDate.Date == selectedDate.Date)
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .ToListAsync();

        ViewBag.SelectedDate = selectedDate;
        ViewBag.TotalSales = sales.Sum(s => s.TotalAmount);
        ViewBag.TotalTransactions = sales.Count;

        return View(sales);
    }
}
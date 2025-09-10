using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvenientStore.Models;
using ConvenientStore.Data;

namespace ConvenientStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ConvenientStoreContext _context;

    public HomeController(ILogger<HomeController> logger, ConvenientStoreContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Dashboard data
        var totalProducts = await _context.Products.Where(p => p.IsActive).CountAsync();
        var lowStockProducts = await _context.Products.Where(p => p.IsActive && p.StockQuantity <= 10).CountAsync();
        var todaySales = await _context.Sales.Where(s => s.SaleDate.Date == DateTime.Today).SumAsync(s => s.TotalAmount);
        var totalCategories = await _context.Categories.Where(c => c.IsActive).CountAsync();

        ViewBag.TotalProducts = totalProducts;
        ViewBag.LowStockProducts = lowStockProducts;
        ViewBag.TodaySales = todaySales;
        ViewBag.TotalCategories = totalCategories;

        // Recent sales
        var recentSales = await _context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .Take(5)
            .ToListAsync();

        ViewBag.RecentSales = recentSales;

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

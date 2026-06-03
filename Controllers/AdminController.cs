using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyWise.Data;
using PennyWise.Models;

namespace PennyWise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext db,
                               UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // Admin Dashboard: tüm kullanıcıların özeti
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var allTransactions = await _db.Transactions
                .Include(t => t.Category)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            ViewBag.TotalUsers       = users.Count;
            ViewBag.TotalTransactions = allTransactions.Count;
            ViewBag.TotalIncome       = allTransactions.Where(t => t.Type == TransactionType.Gelir).Sum(t => t.Amount);
            ViewBag.TotalExpense      = allTransactions.Where(t => t.Type == TransactionType.Gider).Sum(t => t.Amount);
            ViewBag.RecentTransactions = allTransactions.Take(10).ToList();
            ViewBag.Users            = users;

            return View();
        }

        // Tüm işlemleri listele (kullanıcı bazında)
        public async Task<IActionResult> AllTransactions(string? userId)
        {
            var query = _db.Transactions
                .Include(t => t.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(t => t.UserId == userId);

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            ViewBag.Users  = await _userManager.Users.ToListAsync();
            ViewBag.UserId = userId;
            return View(transactions);
        }
    }
}

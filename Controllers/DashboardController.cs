using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyWise.Data;
using PennyWise.Models;
using PennyWise.ViewModels;

namespace PennyWise.Controllers
{
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext db,
                                   UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var now    = DateTime.Today;

            // --- LINQ: Bu aya ait gelir/gider ---
            var monthlyTransactions = await _db.Transactions
                .Where(t => t.UserId == userId
                         && t.TransactionDate.Month == now.Month
                         && t.TransactionDate.Year  == now.Year)
                .Include(t => t.Category)
                .ToListAsync();

            var allTransactions = await _db.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            decimal totalIncome  = allTransactions.Where(t => t.Type == TransactionType.Gelir).Sum(t => t.Amount);
            decimal totalExpense = allTransactions.Where(t => t.Type == TransactionType.Gider).Sum(t => t.Amount);
            decimal balance      = totalIncome - totalExpense;

            // Header'da her sayfada göstermek için ViewBag
            ViewBag.CurrentBalance = balance;

            // --- LINQ: Kategoriye göre gruplu gider (Chart.js için) ---
            var categoryExpenses = monthlyTransactions
                .Where(t => t.Type == TransactionType.Gider)
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Name   = g.Key?.Name ?? "Diğer",
                    Color  = g.Key?.Color ?? "#64748b",
                    Total  = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // --- Bütçe durumları ---
            var budgets = await _db.Budgets
                .Where(b => b.UserId == userId && b.Month == now.Month && b.Year == now.Year)
                .Include(b => b.Category)
                .ToListAsync();

            var budgetStatuses = budgets.Select(b => new BudgetStatusViewModel
            {
                Budget     = b,
                TotalSpent = monthlyTransactions
                    .Where(t => t.CategoryId == b.CategoryId && t.Type == TransactionType.Gider)
                    .Sum(t => t.Amount)
            }).ToList();

            var vm = new DashboardViewModel
            {
                TotalBalance       = balance,
                MonthlyIncome      = monthlyTransactions.Where(t => t.Type == TransactionType.Gelir).Sum(t => t.Amount),
                MonthlyExpense     = monthlyTransactions.Where(t => t.Type == TransactionType.Gider).Sum(t => t.Amount),
                RecentTransactions = allTransactions.Take(5).ToList(),
                ActiveGoals        = await _db.SavingsGoals.Where(g => g.UserId == userId).Take(3).ToListAsync(),
                BudgetStatuses     = budgetStatuses,
                ChartLabels        = categoryExpenses.Select(x => x.Name).ToList(),
                ChartData          = categoryExpenses.Select(x => x.Total).ToList(),
                ChartColors        = categoryExpenses.Select(x => x.Color).ToList()
            };

            return View(vm);
        }
    }
}

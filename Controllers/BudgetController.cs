using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PennyWise.Data;
using PennyWise.Models;
using PennyWise.ViewModels;

namespace PennyWise.Controllers
{
    [Authorize(Roles = "User")]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public BudgetController(ApplicationDbContext db,
                                UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Budget
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var now    = DateTime.Today;

            var budgets = await _db.Budgets
                .Where(b => b.UserId == userId && b.Month == now.Month && b.Year == now.Year)
                .Include(b => b.Category)
                .ToListAsync();

            var monthlyTransactions = await _db.Transactions
                .Where(t => t.UserId == userId
                         && t.Type  == TransactionType.Gider
                         && t.TransactionDate.Month == now.Month
                         && t.TransactionDate.Year  == now.Year)
                .ToListAsync();

            var statuses = budgets.Select(b => new BudgetStatusViewModel
            {
                Budget     = b,
                TotalSpent = monthlyTransactions
                    .Where(t => t.CategoryId == b.CategoryId)
                    .Sum(t => t.Amount)
            }).ToList();

            var goals = await _db.SavingsGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var all = await _db.Transactions.Where(t => t.UserId == userId).ToListAsync();
            ViewBag.CurrentBalance = all.Where(t => t.Type == TransactionType.Gelir).Sum(t => t.Amount)
                                   - all.Where(t => t.Type == TransactionType.Gider).Sum(t => t.Amount);

            ViewBag.BudgetStatuses = statuses;
            ViewBag.Goals          = goals;
            ViewBag.Categories     = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");

            return View();
        }

        // POST: /Budget/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget model)
        {
            var userId = _userManager.GetUserId(User)!;
            model.UserId = userId;
            model.Month  = DateTime.Today.Month;
            model.Year   = DateTime.Today.Year;

            // UserId, Month, Year form'dan gelmediği için ModelState'i temizleyip sadece CategoryId ve MonthlyLimit'i kontrol et
            ModelState.ClearValidationState(nameof(model.UserId));
            ModelState.ClearValidationState(nameof(model.Month));
            ModelState.ClearValidationState(nameof(model.Year));
            ModelState.MarkFieldValid(nameof(model.UserId));
            ModelState.MarkFieldValid(nameof(model.Month));
            ModelState.MarkFieldValid(nameof(model.Year));

            if (model.CategoryId == 0 || model.MonthlyLimit <= 0)
            {
                TempData["Error"] = "Lütfen kategori ve geçerli bir limit giriniz.";
                return RedirectToAction(nameof(Index));
            }

            // Aynı kategori/ay/yıl varsa güncelle
            var existing = await _db.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId && b.CategoryId == model.CategoryId &&
                b.Month  == model.Month && b.Year  == model.Year);

            if (existing != null)
                existing.MonthlyLimit = model.MonthlyLimit;
            else
                _db.Budgets.Add(model);

            await _db.SaveChangesAsync();
            TempData["Success"] = "Limit kaydedildi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Budget/DeleteBudget/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var b = await _db.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (b != null) { _db.Budgets.Remove(b); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Budget/CreateGoal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGoal(SavingsGoal model)
        {
            var userId = _userManager.GetUserId(User)!;
            model.UserId = userId;

            ModelState.ClearValidationState(nameof(model.UserId));
            ModelState.MarkFieldValid(nameof(model.UserId));

            if (string.IsNullOrWhiteSpace(model.Name) || model.TargetAmount < 10)
            {
                TempData["Error"] = "Lütfen hedef adı ve geçerli bir tutar giriniz.";
                return RedirectToAction(nameof(Index));
            }

            model.CreatedAt = DateTime.Today;
            _db.SavingsGoals.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Hedef oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Budget/AddFund
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFund(int id, decimal amount)
        {
            var userId = _userManager.GetUserId(User)!;
            var goal   = await _db.SavingsGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

            if (goal != null && amount > 0)
            {
                goal.CurrentAmount += amount;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"{amount:N2} ₺ hedefe eklendi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Budget/DeleteGoal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var g = await _db.SavingsGoals.FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
            if (g != null) { _db.SavingsGoals.Remove(g); await _db.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}

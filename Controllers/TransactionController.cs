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
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionController(ApplicationDbContext db,
                                     UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Transaction
        public async Task<IActionResult> Index(string? search, TransactionType? type, int? catId)
        {
            var userId = _userManager.GetUserId(User)!;

            // LINQ: Filtreleme
            var query = _db.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Description.Contains(search));

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (catId.HasValue)
                query = query.Where(t => t.CategoryId == catId.Value);

            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // Kalan bakiyeyi header için hesapla
            var all = await _db.Transactions.Where(t => t.UserId == userId).ToListAsync();
            ViewBag.CurrentBalance = all.Where(t => t.Type == TransactionType.Gelir).Sum(t => t.Amount)
                                   - all.Where(t => t.Type == TransactionType.Gider).Sum(t => t.Amount);

            var vm = new TransactionListViewModel
            {
                Transactions = transactions,
                SearchTerm   = search,
                FilterType   = type,
                FilterCatId  = catId,
                Categories   = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name")
            };

            return View(vm);
        }

        // GET: /Transaction/Create
        public async Task<IActionResult> Create()
        {
            var vm = new TransactionCreateViewModel
            {
                Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name")
            };
            return View(vm);
        }

        // POST: /Transaction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = new SelectList(await _db.Categories.ToListAsync(), "Id", "Name");
                return View(vm);
            }

            var userId = _userManager.GetUserId(User)!;

            var transaction = new Transaction
            {
                UserId          = userId,
                CategoryId      = vm.CategoryId,
                Description     = vm.Description,
                Amount          = vm.Amount,
                Type            = vm.Type,
                TransactionDate = vm.TransactionDate
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            // Limit aşımı kontrolü
            await CheckBudgetAlert(userId, vm.CategoryId, vm.Type);

            TempData["Success"] = "İşlem başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Transaction/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var t = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (t != null)
            {
                _db.Transactions.Remove(t);
                await _db.SaveChangesAsync();
                TempData["Success"] = "İşlem silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Limit aşımı kontrolü (uyarı mesajı)
        private async Task CheckBudgetAlert(string userId, int categoryId, TransactionType type)
        {
            if (type != TransactionType.Gider) return;

            var now    = DateTime.Today;
            var budget = await _db.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId && b.CategoryId == categoryId &&
                b.Month  == now.Month && b.Year == now.Year);

            if (budget == null) return;

            var spent = await _db.Transactions
                .Where(t => t.UserId == userId && t.CategoryId == categoryId
                         && t.Type  == TransactionType.Gider
                         && t.TransactionDate.Month == now.Month
                         && t.TransactionDate.Year  == now.Year)
                .SumAsync(t => t.Amount);

            if (spent > budget.MonthlyLimit)
            {
                var cat = await _db.Categories.FindAsync(categoryId);
                TempData["Warning"] = $"⚠️ '{cat?.Name}' kategorisinde aylık limitinizi ({budget.MonthlyLimit:N2} ₺) aştınız! Toplam: {spent:N2} ₺";
            }
        }
    }
}

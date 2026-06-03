using PennyWise.Models;

namespace PennyWise.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalBalance    { get; set; }
        public decimal MonthlyIncome   { get; set; }
        public decimal MonthlyExpense  { get; set; }

        public List<Transaction>   RecentTransactions { get; set; } = new();
        public List<SavingsGoal>   ActiveGoals        { get; set; } = new();
        public List<BudgetStatusViewModel> BudgetStatuses { get; set; } = new();

        // Chart.js için kategori bazlı gider verisi
        public List<string>  ChartLabels { get; set; } = new();
        public List<decimal> ChartData   { get; set; } = new();
        public List<string>  ChartColors { get; set; } = new();
    }

    public class BudgetStatusViewModel
    {
        public Budget  Budget       { get; set; } = null!;
        public decimal TotalSpent   { get; set; }
        public int     Percent      => Budget.MonthlyLimit > 0
                                       ? (int)Math.Min(100, Math.Round(TotalSpent / Budget.MonthlyLimit * 100))
                                       : 0;
        public bool    IsOverLimit  => TotalSpent > Budget.MonthlyLimit;
    }
}

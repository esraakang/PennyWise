# PennyWise – Kurulum Adımları

## 1. Proje Klasörüne Gir
```
cd PennyWise
```

## 2. NuGet Paketlerini Yükle
```
dotnet restore
```

## 3. Migration Oluştur
```
dotnet ef migrations add InitialCreate
```

## 4. Veritabanını Oluştur
```
dotnet ef database update
```
> Bu adım otomatik olarak Program.cs içindeki `db.Database.Migrate()` ile de yapılır.

## 5. Uygulamayı Çalıştır
```
dotnet run
```
Tarayıcıda çıkan adresi kullanın (genelde `http://localhost:5xxx`).

> Proje **.NET 10** hedefler. Makinede yalnızca .NET 8 varsa `dotnet ef` için .NET 10 SDK/runtime gerekir veya `TargetFramework`'ü `net8.0` yapıp paketleri 8.x sürümüne indirin.

---

## Varsayılan Admin Hesabı
- **E-posta:** admin@pennywise.com
- **Şifre:** Admin123!

---

## Proje Yapısı
```
PennyWise/
├── Controllers/
│   ├── AccountController.cs    ← Login / Register / Logout
│   ├── DashboardController.cs  ← Ana sayfa (User rolü)
│   ├── TransactionController.cs← CRUD işlemleri
│   ├── BudgetController.cs     ← Limitler & Hedefler
│   └── AdminController.cs      ← Yönetici paneli (Admin rolü)
├── Models/
│   ├── Category.cs
│   ├── Transaction.cs          ← TransactionType enum dahil
│   ├── Budget.cs
│   └── SavingsGoal.cs
├── ViewModels/
│   ├── AccountViewModels.cs    ← Login / Register VM
│   ├── DashboardViewModel.cs   ← Dashboard + BudgetStatus VM
│   └── TransactionViewModels.cs
├── Data/
│   └── ApplicationDbContext.cs ← EF Core + Identity + Data Seeding
├── Views/
│   ├── Shared/_Layout.cshtml   ← Ortak layout + dinamik bakiye header
│   ├── Account/Login.cshtml
│   ├── Account/Register.cshtml
│   ├── Dashboard/Index.cshtml  ← Chart.js grafikleri
│   ├── Transaction/Index.cshtml
│   ├── Transaction/Create.cshtml
│   ├── Budget/Index.cshtml
│   └── Admin/Index.cshtml
└── Program.cs                  ← Identity + EF Core + Migrate
```

## Hocanın Şartlarının Karşılanması

| Şart | Durum |
|------|-------|
| EF Core Code-First + Migrations | ✅ ApplicationDbContext + Migration |
| CRUD İşlemleri | ✅ Transaction Create/Delete, Budget Create/Delete |
| Authentication | ✅ ASP.NET Core Identity |
| Role-based Authorization | ✅ Admin / User rolleri |
| _Layout.cshtml | ✅ Ortak header/footer |
| ViewModel kullanımı | ✅ DashboardVM, TransactionVM, BudgetStatusVM |
| Data Annotations + Validation | ✅ Required, Range, StringLength, Compare |
| LINQ GroupBy + Sum | ✅ DashboardController'da kategori bazlı gruplama |
| Client-side Validation | ✅ min="0.01" + `[Range]` attribute |
| Header'da dinamik bakiye | ✅ ViewBag.CurrentBalance → _Layout.cshtml |
| Bütçe limit aşımı uyarısı | ✅ TempData["Warning"] ile toast mesajı |
| Data Seeding | ✅ 10 kategori + Admin hesabı + 2 rol |

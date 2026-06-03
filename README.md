# 💰 PennyWise — Kişisel Finans ve Bütçe Takip Sistemi

> Gelirlerinizi, giderlerinizi ve tasarruf hedeflerinizi tek bir yerden yönetin.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF%20Core-Code--First-blue?style=flat-square)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite)
![Identity](https://img.shields.io/badge/Auth-ASP.NET%20Identity-green?style=flat-square)

---

## 📸 Özellikler

- 📊 **Dashboard** — Aylık gelir/gider grafikleri, bakiye özeti, son işlemler
- 💳 **İşlem Yönetimi** — Kategori bazlı gelir ve gider kaydı, arama ve filtreleme
- 🎯 **Bütçe Limitleri** — Kategorilere aylık harcama limiti koy, aşımında otomatik uyarı al
- 🏆 **Tasarruf Hedefleri** — Hedef oluştur, biriktirdikçe ilerleme çubuğunu izle
- 👤 **Profil & Güvenlik** — Kullanıcı profili ve şifre değiştirme
- 🛡️ **Admin Paneli** — Tüm kullanıcıların istatistiklerini görüntüle

---

## 🏗️ Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core 8.0 MVC |
| ORM | Entity Framework Core (Code-First) |
| Veritabanı | SQLite |
| Auth | ASP.NET Core Identity |
| Frontend | Tailwind CSS, FontAwesome |
| Grafikler | Chart.js |

---

## 🚀 Kurulum

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### Adımlar

```bash
# 1. Repoyu klonla
git clone https://github.com/esraakang/PennyWise.git
cd PennyWise

# 2. Bağımlılıkları yükle
dotnet restore

# 3. Veritabanını oluştur
dotnet ef database update

# 4. Uygulamayı çalıştır
dotnet run
```

Tarayıcıda `http://localhost:5000` adresini aç.

---

## 🔑 Varsayılan Hesaplar

| Rol | E-posta | Şifre |
|-----|---------|-------|
| Admin | admin@pennywise.com | Admin123! |
| User | Kayıt ol sayfasından oluştur | — |

---

## 📁 Proje Yapısı

```
PennyWise/
├── Controllers/
│   ├── AccountController.cs    ← Login / Register / Profil / Şifre
│   ├── DashboardController.cs  ← Ana sayfa (User)
│   ├── TransactionController.cs← Gelir/Gider CRUD
│   ├── BudgetController.cs     ← Limitler & Hedefler
│   └── AdminController.cs      ← Admin paneli
├── Models/
│   ├── Transaction.cs          ← TransactionType enum dahil
│   ├── Budget.cs
│   ├── Category.cs
│   └── SavingsGoal.cs
├── ViewModels/
│   ├── DashboardViewModel.cs
│   ├── TransactionViewModels.cs
│   └── AccountViewModels.cs
├── Data/
│   └── ApplicationDbContext.cs ← DbContext + Identity + Data Seeding
├── Views/
│   ├── Shared/_Layout.cshtml   ← Ortak layout + dinamik bakiye
│   ├── Account/                ← Login, Register, Profile, ChangePassword
│   ├── Dashboard/
│   ├── Transaction/
│   ├── Budget/
│   └── Admin/
└── Program.cs
```

---

## ✅ Teknik Özellikler

- **EF Core Code-First** — Migration ve Data Seeding
- **Role-based Authorization** — `[Authorize(Roles = "Admin/User")]`
- **LINQ** — `GroupBy`, `Sum`, `Where`, `OrderByDescending`
- **Data Annotations** — `[Required]`, `[Range]`, `[StringLength]`, `[Compare]`
- **ViewBag & ViewModel** — Veri taşıma ve dinamik header bakiyesi
- **Limit Aşımı Uyarısı** — Gider eklenince anlık kontrol, `TempData` ile bildirim
- **Chart.js** — Kategori pasta grafiği + Gelir/Gider bar grafiği

---

## 👨‍💻 Geliştirici

**Esra Kang** — Bilgisayar Mühendisliği, 2025-2026 Bahar Dönemi
**Emre Turan** — Bilgisayar Mühendisliği, 2025-2026 Bahar Dönemi
**Ferza Er** — Bilgisayar Mühendisliği, 2025-2026 Bahar Dönemi



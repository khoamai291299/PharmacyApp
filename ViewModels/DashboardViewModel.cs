using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PharmacyApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DashboardRepository _repo = new();
        private readonly StockWarningRepository _stockRepo = new();


        public ObservableCollection<TopMedicineModel> TopMedicines { get; } = new();
        public ObservableCollection<CustomerModel> LoyalCustomers { get; } = new();
        private ObservableCollection<StockWarningModel> _stockWarnings = new();
        public ObservableCollection<StockWarningModel> StockWarnings
        {
            get => _stockWarnings;
            set
            {
                _stockWarnings = value;
                OnPropertyChanged();
            }
        }



        public int TodayRevenue { get; set; }
        public int TotalImport { get; set; }

        public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime ToDate { get; set; } = DateTime.Today;

        private UserModel _currentUser;
        public UserModel CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        private bool _canExport;
        public bool CanExport
        {
            get => _canExport;
            set
            {
                _canExport = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ExportPdfCommand { get; }

        public List<string> PeriodOptions { get; } = new()
        {
            "10 ngày",
            "10 tuần",
            "10 tháng"
        };

        private string _selectedPeriod = "10 ngày";
        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                _selectedPeriod = value;
                OnPropertyChanged();
                LoadRevenuePlot();
            }
        }

        public double[] RevenueValues { get; set; }
        public string[] RevenueLabels { get; set; }

        public RelayCommand ApplyDateFilterCommand { get; }

        public DashboardViewModel()
        {
            ExportPdfCommand = new RelayCommand(_ => ExportPdf());
            ApplyDateFilterCommand = new RelayCommand(_ => ApplyFilter());
            ApplyFilter();
            LoadRevenuePlot();
        }

        // ===== FILTER =====
        private void ApplyFilter()
        {
            var summary = _repo.GetSummaryByDateRange(FromDate, ToDate);
            TodayRevenue = summary.revenue;
            TotalImport = summary.import;

            LoadTopMedicines();
            LoadLoyalCustomers();
            LoadStockWarnings();
            CanExport = true;

            OnPropertyChanged(nameof(TodayRevenue));
            OnPropertyChanged(nameof(TotalImport));
        }
        private void ExportPdf()
        {
            var (revenue, import) =
                _repo.GetDashboardSummary(FromDate, ToDate);

            var dailyRevenue =
                _repo.GetDailyRevenue(FromDate, ToDate);

            var dailyImport =
                _repo.GetDailyImport(FromDate, ToDate);

            var path = DashboardReportPdfService.Create(
                FromDate,
                ToDate,
                revenue,
                import,
                dailyRevenue,
                dailyImport);

            DashboardReportPdfService.Open(path);
        }




        private void LoadTopMedicines()
        {
            TopMedicines.Clear();
            var dt = _repo.GetTopMedicine(FromDate, ToDate);
            foreach (DataRow r in dt.Rows)
            {
                TopMedicines.Add(new TopMedicineModel
                {
                    MedicineId = r["id"].ToString(),
                    Name = r["name"].ToString(),
                    TotalSold = Convert.ToInt32(r["TotalSold"])
                });
            }
        }

        private void LoadLoyalCustomers()
        {
            LoyalCustomers.Clear();
            var dt = _repo.GetTopCustomers(FromDate, ToDate);
            foreach (DataRow r in dt.Rows)
            {
                LoyalCustomers.Add(new CustomerModel
                {
                    Id = r["id"].ToString(),
                    Name = r["name"].ToString(),
                    Phone = r["phone"].ToString(),
                    TotalExpenditure = Convert.ToInt32(r["totalExpenditure"]),
                    CumulativePoints = Convert.ToInt32(r["cumulativePoints"])
                });
            }
        }
        private void LoadStockWarnings()
        {
            StockWarnings.Clear();

            var list = _stockRepo.GetStockWarnings();
            foreach (var item in list)
            {
                StockWarnings.Add(item);
            }
        }


        // ===== BIỂU ĐỒ =====
        private void LoadRevenuePlot()
        {
            if (SelectedPeriod == "10 ngày")
                LoadDayChart();
            else if (SelectedPeriod == "10 tuần")
                LoadWeekChart();
            else
                LoadMonthChart();
        }

        // --- 10 NGÀY (8 + 2 ĐỆM) ---
        private void LoadDayChart()
        {
            var dt = _repo.GetRevenueLastNDays(10);
            var dict = dt.AsEnumerable()
                .ToDictionary(
                    r => Convert.ToDateTime(r["Date"]),
                    r => Convert.ToDouble(r["Revenue"])
                );

            var values = new List<double>();
            var labels = new List<string>();

            for (int i = 9; i >= 0; i--)
            {
                var d = DateTime.Today.AddDays(-i);
                values.Add(dict.ContainsKey(d) ? dict[d] : 0);
                labels.Add(d.ToString("dd/MM"));
            }

            RevenueValues = values.ToArray();
            RevenueLabels = labels.ToArray();
            NotifyChart();
        }

        // --- 10 TUẦN ---
        private void LoadWeekChart()
        {
            var dt = _repo.GetRevenueLastNWeeks(10);
            var dict = dt.AsEnumerable()
                .ToDictionary(
                    r => $"{r["Year"]}-{r["Week"]}",
                    r => Convert.ToDouble(r["Revenue"])
                );

            var values = new List<double>();
            var labels = new List<string>();
            var cal = CultureInfo.CurrentCulture.Calendar;

            var start = DateTime.Today.AddDays(-7 * 9);

            for (int i = 0; i < 10; i++)
            {
                var d = start.AddDays(7 * i);
                int week = cal.GetWeekOfYear(d,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);

                var key = $"{d.Year}-{week}";
                values.Add(dict.ContainsKey(key) ? dict[key] : 0);
                labels.Add($"Tuần {week}");
            }

            RevenueValues = values.ToArray();
            RevenueLabels = labels.ToArray();
            NotifyChart();
        }

        // --- 10 THÁNG ---
        private void LoadMonthChart()
        {
            var dt = _repo.GetRevenueLastNMonths(10);
            var dict = dt.AsEnumerable()
                .ToDictionary(
                    r => r["Month"].ToString(),
                    r => Convert.ToDouble(r["Revenue"])
                );

            var values = new List<double>();
            var labels = new List<string>();

            for (int i = 9; i >= 0; i--)
            {
                var d = DateTime.Today.AddMonths(-i);
                var key = d.ToString("yyyy-MM");

                values.Add(dict.ContainsKey(key) ? dict[key] : 0);
                labels.Add(d.ToString("MM/yyyy"));
            }

            RevenueValues = values.ToArray();
            RevenueLabels = labels.ToArray();
            NotifyChart();
        }

        private void NotifyChart()
        {
            OnPropertyChanged(nameof(RevenueValues));
            OnPropertyChanged(nameof(RevenueLabels));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}

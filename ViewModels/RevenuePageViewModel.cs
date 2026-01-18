using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class RevenuePageViewModel : BaseViewModel
    {
        private readonly RevenueRepository _repo = new();

        public ObservableCollection<EmployeeRevenueModel> Items { get; }
            = new ObservableCollection<EmployeeRevenueModel>();

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                SetProperty(ref _fromDate, value);
                Load();
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                SetProperty(ref _toDate, value);
                Load();
            }
        }

        public RevenuePageViewModel()
        {
            Load();
        }

        private void Load()
        {
            Items.Clear();
            foreach (var item in _repo.GetRevenueByEmployee(FromDate, ToDate))
                Items.Add(item);
        }
    }

}

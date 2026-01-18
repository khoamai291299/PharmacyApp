using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.IO;

namespace PharmacyApp.ViewModels
{
    public class WarehouseReceiptManagementViewModel : BaseViewModel
    {
        private readonly WarehouseReceiptRepository _repo = new();

        public ObservableCollection<WarehouseReceiptModel> Items { get; }
            = new ObservableCollection<WarehouseReceiptModel>();

        private ObservableCollection<WarehouseReceiptModel> _allItems
            = new ObservableCollection<WarehouseReceiptModel>();

        private string _searchId;
        public string SearchId
        {
            get => _searchId;
            set
            {
                SetProperty(ref _searchId, value);
                ApplyFilter();
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                SetProperty(ref _fromDate, value);
                ApplyFilter();
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                SetProperty(ref _toDate, value);
                ApplyFilter();
            }
        }

        public RelayCommand ViewDetailCommand { get; }
        public RelayCommand ViewPdfCommand { get; }

        public WarehouseReceiptManagementViewModel()
        {
            Load();
            ViewDetailCommand = new RelayCommand(
                x => ViewDetail(x as WarehouseReceiptModel),
                x => x is WarehouseReceiptModel
            );
            ViewPdfCommand = new RelayCommand(obj =>
            {
                if (obj is not WarehouseReceiptModel r) return;

                string path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "WarehouseReceipts",
                    $"Warehouse_{r.Id}.pdf");

                WarehouseReceiptPdfService.Open(path);
            });
        }

        private void Load()
        {
            _allItems.Clear();
            Items.Clear();

            foreach (var r in _repo.GetAll())
                _allItems.Add(r);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Items.Clear();
            var query = _allItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchId))
            {
                query = query.Where(x =>
                    x.Id != null &&
                    x.Id.Contains(SearchId, StringComparison.OrdinalIgnoreCase));
            }

            if (FromDate.HasValue)
            {
                query = query.Where(x =>
                    x.InputDay.HasValue &&
                    x.InputDay.Value.Date >= FromDate.Value.Date);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(x =>
                    x.InputDay.HasValue &&
                    x.InputDay.Value.Date <= ToDate.Value.Date);
            }

            foreach (var item in query)
                Items.Add(item);
        }

        //private void ViewDetail(WarehouseReceiptModel receipt)
        //{
        //    if (receipt == null) return;
        //    MessageBox.Show($"Xem chi tiết phiếu nhập: {receipt.Id}", "Thông báo");
        //}

        private void ViewDetail(WarehouseReceiptModel receipt)
        {
            if (receipt == null) return;

            string folder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "WarehouseReceipts"
            );

            string path = System.IO.Path.Combine(folder, $"Warehouse_{receipt.Id}.pdf");

            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("Không tìm thấy file hóa đơn PDF.", "Thông báo");
                return;
            }

            InvoicePdfService.OpenPdf(path);
        }
    }
}

using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class BillPageViewModel : BaseViewModel
    {
        private readonly BillRepository _repo = new BillRepository();

        public ObservableCollection<BillModel> Items { get; set; }
            = new ObservableCollection<BillModel>();

        private ObservableCollection<BillModel> _allItems
            = new ObservableCollection<BillModel>();

        private string _searchBillId;
        public string SearchBillId
        {
            get => _searchBillId;
            set
            {
                SetProperty(ref _searchBillId, value);
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



        private BillModel _selectedItem;
        public BillModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    EditModel = new BillModel
                    {
                        Id = value.Id,
                        DateOfcreate = value.DateOfcreate,
                        Cid = value.Cid,
                        Eid = value.Eid,
                        TotalAmount = value.TotalAmount
                    };
                }
            }
        }

        private BillModel _editModel = new BillModel();
        public BillModel EditModel
        {
            get => _editModel;
            set => SetProperty(ref _editModel, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand ViewInvoiceCommand { get; }
        public BillPageViewModel()
        {
            Load();

            AddCommand = new RelayCommand(_ => Add());
            UpdateCommand = new RelayCommand(_ => Update(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedItem != null);
            ClearCommand = new RelayCommand(_ => Clear());
            ViewInvoiceCommand = new RelayCommand(
                bill => ViewInvoice(bill as BillModel),
                bill => bill is BillModel
            );
        }
        private void ViewInvoice(BillModel bill)
        {
            if (bill == null) return;

            string folder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Invoices"
            );

            string path = System.IO.Path.Combine(folder, $"Invoice_{bill.Id}.pdf");

            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("Không tìm thấy file hóa đơn PDF.", "Thông báo");
                return;
            }

            InvoicePdfService.OpenPdf(path);
        }

        private void Load()
        {
            _allItems.Clear();
            Items.Clear();

            foreach (var item in _repo.GetAll())
                _allItems.Add(item);

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Items.Clear();

            var query = _allItems.AsEnumerable();

            // Lọc theo mã hóa đơn
            if (!string.IsNullOrWhiteSpace(SearchBillId))
            {
                query = query.Where(b =>
                    b.Id != null &&
                    b.Id.Contains(SearchBillId, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo khoảng ngày
            if (FromDate.HasValue)
            {
                query = query.Where(b =>
                    b.DateOfcreate.HasValue &&
                    b.DateOfcreate.Value.Date >= FromDate.Value.Date);
            }

            if (ToDate.HasValue)
            {
                query = query.Where(b =>
                    b.DateOfcreate.HasValue &&
                    b.DateOfcreate.Value.Date <= ToDate.Value.Date);
            }

            foreach (var item in query)
                Items.Add(item);
        }



        private void Add()
        {
            try
            {
                _repo.Add(EditModel);
                Load();
                MessageBox.Show("Thêm thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Update()
        {
            try
            {
                _repo.Update(EditModel);
                Load();
                MessageBox.Show("Sửa thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Delete()
        {
            try
            {
                _repo.Delete(SelectedItem.Id);
                Load();
                MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Clear()
        {
            EditModel = new BillModel();
            SelectedItem = null;
            SearchBillId = null;
            FromDate = null;
            ToDate = null;
        }
    }
}

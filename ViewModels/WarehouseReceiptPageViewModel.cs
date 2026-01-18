using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class WarehouseReceiptPageViewModel : BaseViewModel
    {
        private readonly WarehouseReceiptRepository _repo = new();
        private readonly MedicineRepository _medicineRepo = new();
        private readonly TypeMedicineRepository _typeRepo = new();

        private ObservableCollection<MedicineModel> _allMedicines
            = new ObservableCollection<MedicineModel>();

        public ObservableCollection<MedicineModel> Medicines { get; }
            = new ObservableCollection<MedicineModel>();

        public ObservableCollection<TypeMedicineModel> Types { get; }
            = new ObservableCollection<TypeMedicineModel>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); ApplyFilter(); }
        }

        private TypeMedicineModel _selectedType;
        public TypeMedicineModel SelectedType
        {
            get => _selectedType;
            set { SetProperty(ref _selectedType, value); ApplyFilter(); }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set { SetProperty(ref _fromDate, value); ApplyFilter(); }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set { SetProperty(ref _toDate, value); ApplyFilter(); }
        }

        public ObservableCollection<WarehouseDetailItemViewModel> Cart { get; }

        public WarehouseReceiptModel NewReceipt { get; set; }

        public int TotalImport => Cart.Sum(x => x.TotalAmount);

        // Các lệnh
        public RelayCommand AddToCartCommand { get; }
        public RelayCommand RemoveFromCartCommand { get; }
        public RelayCommand SaveReceiptCommand { get; }
        public RelayCommand AddNewMedicineCommand { get; }

        private readonly EmployeeModel _employee;

        public WarehouseReceiptPageViewModel(EmployeeModel emp)
        {
            _employee = emp;

            Cart = new ObservableCollection<WarehouseDetailItemViewModel>();
            Cart.CollectionChanged += Cart_CollectionChanged;

            LoadMedicines();
            LoadTypes();

            NewReceipt = NewReceipt = new WarehouseReceiptModel
            {
                Id = _repo.GenerateNewId(),
                InputDay = DateTime.Now,
                Eid = emp.Id
            };

            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            SaveReceiptCommand = new RelayCommand(_ => Save());
            AddNewMedicineCommand = new RelayCommand(_ => AddNewMedicine());
        }

        // Load dữ liệu thuốc
        private void LoadMedicines()
        {
            _allMedicines.Clear();
            foreach (var m in _medicineRepo.GetAll())
                _allMedicines.Add(m);

            ApplyFilter();
        }

        private void LoadTypes()
        {
            Types.Clear();
            foreach (var t in _typeRepo.GetAll())
                Types.Add(t);
        }

        // Tìm kiếm
        private void ApplyFilter()
        {
            Medicines.Clear();

            var query = _allMedicines.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(m =>
                    m.Name != null &&
                    m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (SelectedType != null)
                query = query.Where(m => m.Tid == SelectedType.Id);

            if (FromDate.HasValue)
                query = query.Where(m => m.ProductionDate >= FromDate.Value);

            if (ToDate.HasValue)
                query = query.Where(m => m.ExpirationDate <= ToDate.Value);

            foreach (var m in query)
                Medicines.Add(m);
        }

        // Giỏ nhập kho
        private void Cart_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (WarehouseDetailItemViewModel item in e.NewItems)
                    item.PropertyChanged += CartItem_PropertyChanged;

            if (e.OldItems != null)
                foreach (WarehouseDetailItemViewModel item in e.OldItems)
                    item.PropertyChanged -= CartItem_PropertyChanged;

            OnPropertyChanged(nameof(TotalImport));
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WarehouseDetailItemViewModel.TotalAmount))
            {
                OnPropertyChanged(nameof(TotalImport));
            }
        }

        // Thêm sản phẩm vào kho
        private void AddToCart(object obj)
        {
            if (obj is not MedicineModel med) return;

            var item = Cart.FirstOrDefault(x => x.MedicineId == med.Id);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                Cart.Add(new WarehouseDetailItemViewModel
                {
                    MedicineId = med.Id,
                    MedicineName = med.Name,
                    Quantity = 1,
                    UnitPrice = med.ImportPrice
                });
            }
        }

        private void RemoveFromCart(object obj)
        {
            if (obj is WarehouseDetailItemViewModel item)
                Cart.Remove(item);
        }

        // Lưu lại định dạng PDF và thông tin nhập kho
        private void Save()
        {
            if (!Cart.Any())
            {
                MessageBox.Show("Chưa có thuốc nhập.");
                return;
            }

            NewReceipt.TotalImport = TotalImport;
            _repo.Insert(NewReceipt);

            using var conn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["PharmacyDb"].ConnectionString);
            conn.Open();

            foreach (var item in Cart)
            {
                var cmd = new SqlCommand(
                    @"INSERT INTO WarehouseDetails(wid, mid, quantity, unitPrice)
                      VALUES (@wid, @mid, @qty, @price)", conn);

                cmd.Parameters.AddWithValue("@wid", NewReceipt.Id);
                cmd.Parameters.AddWithValue("@mid", item.MedicineId);
                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                cmd.Parameters.AddWithValue("@price", item.UnitPrice);
                cmd.ExecuteNonQuery();
            }

            var path = WarehouseReceiptPdfService.CreateReceipt(
                NewReceipt,
                _employee,
                Cart.ToList());

            WarehouseReceiptPdfService.Open(path);

            MessageBox.Show("Nhập kho thành công");

            Cart.Clear();
            NewReceipt = new WarehouseReceiptModel
            {
                Id = _repo.GenerateNewId(),
                InputDay = DateTime.Now,
                Eid = _employee.Id
            };

            OnPropertyChanged(nameof(NewReceipt));
            OnPropertyChanged(nameof(TotalImport));
        }

        // Thêm thuốc mới khi cần
        private void AddNewMedicine()
        {
            var vm = new MedicineFormViewModel(null, 0);
            vm.OnSaved += m =>
            {
                _allMedicines.Add(m);
                ApplyFilter();
            };

            new MedicineFormWindow { DataContext = vm }.ShowDialog();
        }
    }
}

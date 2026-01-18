using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class EmployeeSaleViewModel : BaseViewModel
    {
        private readonly MedicineRepository _medicineRepo;
        private readonly TypeMedicineRepository _typeRepo = new();


        public EmployeeModel CurrentEmployee { get; }

        public ObservableCollection<MedicineModel> Medicines { get; }
        public ObservableCollection<SaleItemViewModel> Cart { get; }
        private ObservableCollection<MedicineModel> _allMedicines
            = new ObservableCollection<MedicineModel>();

        public ObservableCollection<TypeMedicineModel> Types { get; }
            = new ObservableCollection<TypeMedicineModel>();


        public int TotalAmount => Cart.Sum(x => x.TotalAmount);

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand CheckoutCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilter();
            }
        }

        private TypeMedicineModel _selectedType;
        public TypeMedicineModel SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                ApplyFilter();
            }
        }


        public EmployeeSaleViewModel(EmployeeModel employee)
        {
            CurrentEmployee = employee;

            _medicineRepo = new MedicineRepository();
            Medicines = new ObservableCollection<MedicineModel>();
            Cart = new ObservableCollection<SaleItemViewModel>();

            Cart.CollectionChanged += Cart_CollectionChanged;

            LoadMedicines();
            LoadTypes();


            AddToCartCommand = new RelayCommand(AddToCart);
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            CheckoutCommand = new RelayCommand(Checkout);
            ClearCartCommand = new RelayCommand(_ => ClearCart());
        }
        private void Cart_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SaleItemViewModel item in e.NewItems)
                {
                    item.PropertyChanged += CartItem_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SaleItemViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= CartItem_PropertyChanged;
                }
            }

            OnPropertyChanged(nameof(TotalAmount));
        }

        private void CartItem_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItemViewModel.Quantity) ||
                e.PropertyName == nameof(SaleItemViewModel.TotalAmount))
            {
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void ClearCart()
        {
            Cart.Clear();
            LoadMedicines();
            OnPropertyChanged(nameof(TotalAmount));
        }
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


        private void AddToCart(object obj)
        {
            if (obj is not MedicineModel med) return;

            var item = Cart.FirstOrDefault(x => x.MedicineId == med.Id);

            if (item != null)
            {
                if (item.Quantity + 1 > med.Quantity)
                {
                    MessageBox.Show("Không đủ tồn kho.");
                    return;
                }
                item.Quantity++;
            }
            else
            {
                if (med.Quantity <= 0)
                {
                    MessageBox.Show("Thuốc đã hết hàng.");
                    return;
                }

                Cart.Add(new SaleItemViewModel
                {
                    MedicineId = med.Id,
                    MedicineName = med.Name,
                    Quantity = 1,
                    UnitPrice = med.SellingPrice
                });
            }

            OnPropertyChanged(nameof(TotalAmount));
        }

        private void RemoveFromCart(object obj)
        {
            if (obj is SaleItemViewModel item)
            {
                Cart.Remove(item);
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void Checkout(object obj)
        {
            if (!Cart.Any())
            {
                MessageBox.Show("Giỏ hàng trống.");
                return;
            }

            var vm = new PaymentViewModel(
                CurrentEmployee,
                Cart.ToList(),
                TotalAmount
            );

            var win = new PaymentWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            if (win.ShowDialog() == true)
            {
                Cart.Clear();
                LoadMedicines();
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void ApplyFilter()
        {
            Medicines.Clear();

            var query = _allMedicines.AsEnumerable();

            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(m =>
                    m.Name != null &&
                    m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo loại
            if (SelectedType != null)
            {
                query = query.Where(m => m.Tid == SelectedType.Id);
            }

            foreach (var m in query)
                Medicines.Add(m);
        }

    }
}

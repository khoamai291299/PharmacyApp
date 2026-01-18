using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private CustomerModel _customer;
        private BillModel _bill;
        private bool _hasSearched;
        private string _customerName;
        private string _customerPhone;
        private string _customerAddress;
        private PromotionModel _selectedPromotion;
        private int _totalAmount;
        private bool _hasPaid;
        public event Action<bool?> RequestClose;

        public bool HasPaid
        {
            get => _hasPaid;
            set { _hasPaid = value; OnPropertyChanged(); NotifyCommands(); }
        }

        public EmployeeModel CurrentEmployee { get; }
        public ObservableCollection<SaleItemViewModel> Cart { get; }
        public ObservableCollection<PromotionModel> AvailablePromotions { get; set; } = new();

        public string CustomerPhone
        {
            get => _customerPhone;
            set
            {
                _customerPhone = value;
                _hasSearched = false;
                _customer = null;
                IsCustomerFound = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCustomerNotFound));
                OnPropertyChanged(nameof(CanAddCustomer));
                OnPropertyChanged(nameof(CanConfirmPayment));
                NotifyCommands();
                AvailablePromotions.Clear();
                SelectedPromotion = null;
                TotalAmount = Cart.Sum(x => x.Quantity * x.UnitPrice);
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); NotifyCommands(); }
        }

        public string CustomerAddress
        {
            get => _customerAddress;
            set { _customerAddress = value; OnPropertyChanged(); }
        }

        public bool IsCustomerFound { get; private set; }
        public bool IsCustomerNotFound => _hasSearched && !IsCustomerFound && !string.IsNullOrWhiteSpace(CustomerPhone);
        public bool CanAddCustomer => IsCustomerNotFound && !string.IsNullOrWhiteSpace(CustomerName) && !string.IsNullOrWhiteSpace(CustomerPhone) && CustomerPhone.Length == 10;
        public bool CanConfirmPayment => _customer != null && !string.IsNullOrWhiteSpace(CustomerPhone) && CustomerPhone.Length == 10 && !HasPaid;
        public bool CanPrintInvoice => _bill != null;

        public PromotionModel SelectedPromotion
        {
            get => _selectedPromotion;
            set
            {
                _selectedPromotion = value;
                OnPropertyChanged();
                ApplyPromotion();
            }
        }

        public int TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; OnPropertyChanged(); }
        }

        public ICommand FindCustomerCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand PrintInvoiceCommand { get; }

        public PaymentViewModel(EmployeeModel employee, List<SaleItemViewModel> cart, int totalAmount)
        {
            CurrentEmployee = employee;
            Cart = new ObservableCollection<SaleItemViewModel>(cart);
            TotalAmount = totalAmount;

            FindCustomerCommand = new RelayCommand(_ => FindCustomer());
            AddCustomerCommand = new RelayCommand(_ => AddCustomer(), _ => CanAddCustomer);
            ConfirmCommand = new RelayCommand(_ => ConfirmPayment(), _ => CanConfirmPayment);
            PrintInvoiceCommand = new RelayCommand(_ => PrintInvoice(), _ => CanPrintInvoice);
        }

        private void FindCustomer()
        {
            if (string.IsNullOrWhiteSpace(CustomerPhone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CustomerPhone.Length != 10)
            {
                MessageBox.Show("Số điện thoại phải đủ 10 chữ số.", "Sai định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var repo = new CustomerRepository();
            _customer = repo.GetByPhone(CustomerPhone);
            _hasSearched = true;

            if (_customer != null)
            {
                CustomerName = _customer.Name;
                CustomerAddress = _customer.Address;
                IsCustomerFound = true;
                LoadAvailablePromotions();
            }
            else
            {
                CustomerName = string.Empty;
                CustomerAddress = string.Empty;
                IsCustomerFound = false;
                _customer = null;
                AvailablePromotions.Clear();
                SelectedPromotion = null;
            }

            NotifyAll();
        }

        private void AddCustomer()
        {
            var repo = new CustomerRepository();
            var customer = new CustomerModel
            {
                Id = repo.GenerateNewId(),
                Name = CustomerName,
                Phone = CustomerPhone,
                Address = CustomerAddress,
                Tid = repo.GetDefaultTypeId(),
                TotalExpenditure = 0,
                CumulativePoints = 0
            };
            repo.Add(customer);
            _customer = customer;
            IsCustomerFound = true;
            _hasSearched = false;

            MessageBox.Show("Đã thêm khách hàng mới.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            NotifyAll();
        }

        private void LoadAvailablePromotions()
        {
            var promoRepo = new PromotionRepository();
            AvailablePromotions = new ObservableCollection<PromotionModel>(
                promoRepo.GetAll()
                         .Where(p => p.Quantity > 0 && p.RequiredPoints <= (_customer?.CumulativePoints ?? 0))
                         .OrderByDescending(p => p.DiscountPercent)
            );
            OnPropertyChanged(nameof(AvailablePromotions));
        }

        private void ApplyPromotion()
        {
            int originalTotal = Cart.Sum(x => x.Quantity * x.UnitPrice);
            TotalAmount = _selectedPromotion != null ? originalTotal - (originalTotal * _selectedPromotion.DiscountPercent / 100) : originalTotal;
        }

        private void ConfirmPayment()
        {
            if (_customer == null)
            {
                MessageBox.Show("Khách hàng chưa tồn tại.", "Chưa có khách hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo hóa đơn
            _bill = new BillModel
            {
                Id = Guid.NewGuid().ToString("N")[..10],
                DateOfcreate = DateTime.Now,
                Cid = _customer.Id,
                Eid = CurrentEmployee.Id,
                PromotionId = SelectedPromotion?.Id
                // TotalAmount không cần set, trigger sẽ tính
            };
            new BillRepository().Add(_bill);

            // Thêm chi tiết hóa đơn
            foreach (var item in Cart)
            {
                new BillDetailsRepository().Add(new BillDetailsModel
                {
                    Bid = _bill.Id,
                    Mid = item.MedicineId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            HasPaid = true;
            OnPropertyChanged(nameof(CanPrintInvoice));
            NotifyCommands();
            MessageBox.Show("Thanh toán thành công.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void PrintInvoice()
        {
            if (_bill == null)
            {
                MessageBox.Show("Chưa có hóa đơn để in.");
                return;
            }

            var path = InvoicePdfService.CreateInvoice(_bill, _customer, CurrentEmployee, Cart.ToList(), SelectedPromotion);
            InvoicePdfService.OpenPdf(path);
        }

        private void NotifyAll()
        {
            OnPropertyChanged(nameof(IsCustomerFound));
            OnPropertyChanged(nameof(IsCustomerNotFound));
            OnPropertyChanged(nameof(CanAddCustomer));
            OnPropertyChanged(nameof(CanConfirmPayment));
            OnPropertyChanged(nameof(CanPrintInvoice));
            NotifyCommands();
        }

        private void NotifyCommands()
        {
            (FindCustomerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AddCustomerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ConfirmCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PrintInvoiceCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }
}

using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using PharmacyApp.Views.Pages;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;

        public UserModel CurrentUser { get; }
        public EmployeeModel CurrentEmployee { get; }

        public bool IsAdmin => CurrentUser.IsAdmin;
        public bool IsSeller => CurrentUser.IsSeller;
        public bool IsWarehouse => CurrentUser.IsWarehouse;
        public bool CanManageWarehouse => IsAdmin;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand ShowDashboardCommand { get; }
        public ICommand ShowSaleCommand { get; }
        public ICommand ShowWarehouseReceiptCommand { get; }

        public ICommand ShowBillCommand { get; }
        public ICommand ShowWarehouseReceiptManagementCommand { get; }
        public ICommand ShowPromotionCommand { get; }

        public ICommand ShowMedicineCommand { get; }
        public ICommand ShowEmployeeCommand { get; }
        public ICommand ShowCustomerCommand { get; }
        public ICommand ShowDepartmentCommand { get; }
        public ICommand ShowPositionCommand { get; }
        public ICommand ShowManufacturerCommand { get; }
        public ICommand ShowRevenueManagementCommand { get; }
        public ICommand ShowAccountCommand { get; }

        public ICommand LogoutCommand { get; }

        private readonly DashboardViewModel _dashboardVm;

        public MainViewModel(UserModel user, EmployeeModel employee)
        {
            CurrentUser = user;
            CurrentEmployee = employee;

            _dashboardVm = new DashboardViewModel
            {
                CurrentUser = CurrentUser
            };
            CurrentViewModel = _dashboardVm;

            // Dashboard – ai cũng vào được
            ShowDashboardCommand = new RelayCommand(_ =>
            {
                CurrentViewModel = _dashboardVm;
            });

            // Seller ONLY
            ShowSaleCommand = new RelayCommand(_ =>
            {
                if (!IsSeller)
                {
                    Deny();
                    return;
                }
                CurrentViewModel = new EmployeeSaleViewModel(CurrentEmployee);
            });

            // Warehouse ONLY
            ShowWarehouseReceiptCommand = new RelayCommand(_ =>
            {
                if (!IsWarehouse)
                {
                    Deny();
                    return;
                }
                CurrentViewModel = new WarehouseReceiptPageViewModel(CurrentEmployee);
            });

            // Admin ONLY
            ShowBillCommand = AdminOnly(() => new BillPageViewModel());
            ShowWarehouseReceiptManagementCommand = AdminOnly(() => new WarehouseReceiptManagementViewModel());
            ShowMedicineCommand = AdminOnly(() => new MedicinePageViewModel());
            ShowEmployeeCommand = AdminOnly(() => new EmployeePageViewModel());
            ShowCustomerCommand = AdminOnly(() => new CustomerPageViewModel());
            ShowDepartmentCommand = AdminOnly(() => new DepartmentPageViewModel());
            ShowPositionCommand = AdminOnly(() => new PositionPageViewModel());
            ShowManufacturerCommand = AdminOnly(() => new ManufacturerPageViewModel());
            ShowRevenueManagementCommand = AdminOnly(() => new RevenuePageViewModel());
            ShowAccountCommand = AdminOnly(() => new UserPageViewModel());
            ShowPromotionCommand = AdminOnly(() => new PromotionPageViewModel());
            // Logout
            LogoutCommand = new RelayCommand(_ =>
            {
                if (MessageBox.Show(
                    "Bạn có chắc chắn muốn đăng xuất?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                new LoginWindow().Show();

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is MainWindow)
                    ?.Close();
            });
        }

        private ICommand AdminOnly(Func<object> createVm)
        {
            return new RelayCommand(_ =>
            {
                if (!IsAdmin)
                {
                    Deny();
                    return;
                }
                CurrentViewModel = createVm();
            });
        }

        private void Deny()
        {
            MessageBox.Show(
                "Bạn không có quyền truy cập chức năng này.",
                "Từ chối truy cập",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

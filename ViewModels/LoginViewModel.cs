using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepo = new();
        private readonly EmployeeRepository _employeeRepo = new();

        // CALLBACK: trả User + Employee (có thể null)
        private readonly Action<UserModel, EmployeeModel> _onLoginSuccess;

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel(Action<UserModel, EmployeeModel> onLoginSuccess)
        {
            _onLoginSuccess = onLoginSuccess;
            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            var user = _userRepo.Login(Username, Password);

            if (user == null || user.Status != "Active")
            {
                MessageBox.Show(
                    "Sai tài khoản, mật khẩu hoặc tài khoản đã bị khóa!",
                    "Đăng nhập thất bại",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            EmployeeModel employee = null;

            // Seller & Warehouse mới cần Employee
            if (user.Role == "Seller" || user.Role == "Warehouse")
            {
                employee = _employeeRepo.GetById(user.Eid);

                if (employee == null)
                {
                    MessageBox.Show(
                        "Không tìm thấy thông tin nhân viên!",
                        "Lỗi dữ liệu",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            // TRẢ ĐÚNG THỨ CẦN
            _onLoginSuccess?.Invoke(user, employee);
        }
    }
}

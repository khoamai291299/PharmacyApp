using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class UserFormViewModel : BaseViewModel
    {
        private readonly UserRepository _repo = new();
        private readonly EmployeeRepository _empRepo = new();

        // ================= DATA =================
        public UserModel User { get; }
        public bool IsEdit { get; }

        public ObservableCollection<EmployeeModel> Employees { get; }
        public ObservableCollection<string> Roles { get; }
        public ObservableCollection<string> Statuses { get; }

        // ================= COMMANDS =================
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        // ================= SELECTED EMPLOYEE =================
        private EmployeeModel _selectedEmployee;
        public EmployeeModel SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();

                if (value != null)
                {
                    User.Eid = value.Id;
                    User.Phone = value.Phone;

                    // ❗ chỉ chặn khi ADD
                    EmployeeHasAccount =
                        !IsEdit && _repo.ExistsByEmployeeId(value.Id);
                }
                else
                {
                    User.Eid = null;
                    User.Phone = null;
                    EmployeeHasAccount = false;
                }

                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _employeeHasAccount;
        public bool EmployeeHasAccount
        {
            get => _employeeHasAccount;
            private set
            {
                _employeeHasAccount = value;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        // ================= CONSTRUCTOR =================
        public UserFormViewModel(UserModel user, bool isEdit)
        {
            User = user ?? new UserModel();
            IsEdit = isEdit;

            SaveCommand = new RelayCommand(w => Save(w), _ => CanSave());
            CancelCommand = new RelayCommand(w => Close(w));

            var allEmployees = _empRepo.GetAll();

            // 👉 chỉ lấy NV chưa có tài khoản
            var availableEmployees = allEmployees
                .Where(e => !_repo.ExistsByEmployeeId(e.Id))
                .ToList();

            // 👉 nếu Edit → cho phép NV hiện tại
            if (IsEdit && !string.IsNullOrEmpty(User.Eid))
            {
                var currentEmp = allEmployees
                    .FirstOrDefault(e => e.Id == User.Eid);

                if (currentEmp != null &&
                    !availableEmployees.Any(e => e.Id == currentEmp.Id))
                {
                    availableEmployees.Add(currentEmp);
                }
            }

            Employees = new ObservableCollection<EmployeeModel>(availableEmployees);

            Roles = new ObservableCollection<string>
            {
                "Admin",
                "Seller",
                "Warehouse"
            };

            Statuses = new ObservableCollection<string>
            {
                "Active",
                "Locked"
            };

            SelectedEmployee = Employees
                .FirstOrDefault(e => e.Id == User.Eid);
        }

        // ================= VALIDATION =================
        private bool CanSave()
        {
            if (EmployeeHasAccount)
                return false;

            if (string.IsNullOrWhiteSpace(User.Username))
                return false;

            if (string.IsNullOrWhiteSpace(User.Email))
                return false;

            if (SelectedEmployee == null)
                return false;

            // Add → bắt buộc password
            if (!IsEdit && string.IsNullOrWhiteSpace(User.Password))
                return false;

            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            return password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        private bool IsValidGmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email)
                && email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
        }

        // ================= SAVE =================
        private void Save(object w)
        {
            // ❗ chặn nghiệp vụ rõ ràng
            if (EmployeeHasAccount)
            {
                MessageBox.Show(
                    "Nhân viên này đã có tài khoản.",
                    "Không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validate password
            if (!IsEdit || !string.IsNullOrWhiteSpace(User.Password))
            {
                if (!IsValidPassword(User.Password))
                {
                    MessageBox.Show(
                        "Mật khẩu phải ≥ 6 ký tự và có ký tự đặc biệt.",
                        "Lỗi mật khẩu",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            if (!IsValidGmail(User.Email))
            {
                MessageBox.Show(
                    "Email phải có định dạng @gmail.com",
                    "Lỗi email",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsEdit)
                    _repo.Update(User);
                else
                    _repo.Add(User);

                MessageBox.Show(
                    "Lưu tài khoản thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close(w);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ================= CLOSE =================
        private void Close(object w)
        {
            if (w is Window win)
                win.Close();
        }
    }
}

using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class EmployeeFormViewModel : BaseViewModel
    {
        private readonly EmployeeRepository _repo = new();
        private readonly DepartmentRepository _deptRepo = new();
        private readonly PositionRepository _posRepo = new();

        public EmployeeModel Employee { get; }

        public ObservableCollection<DepartmentModel> Departments { get; }
        public ObservableCollection<PositionModel> Positions { get; }

        // ===================== VALIDATION =====================
        private string _phoneError;
        public string PhoneError
        {
            get => _phoneError;
            set
            {
                SetProperty(ref _phoneError, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        // Property trung gian cho Phone (View bind vào đây)
        public string Phone
        {
            get => Employee.Phone;
            set
            {
                Employee.Phone = value;
                OnPropertyChanged();
                ValidatePhone();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        private bool IsPhoneValid()
        {
            if (string.IsNullOrWhiteSpace(Employee.Phone))
                return false;

            if (Employee.Phone.Length != 10)
                return false;

            return Employee.Phone.All(char.IsDigit);
        }

        private void ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(Phone))
            {
                PhoneError = "Số điện thoại không được để trống";
                return;
            }

            if (!Phone.All(char.IsDigit))
            {
                PhoneError = "Số điện thoại chỉ được chứa chữ số";
                return;
            }

            if (Phone.Length != 10)
            {
                PhoneError = "Số điện thoại phải đúng 10 chữ số";
                return;
            }

            PhoneError = null;
        }

        // ===================== COMBOBOX =====================
        public DepartmentModel SelectedDepartment
        {
            get => Departments.FirstOrDefault(d => d.Id == Employee.Did);
            set
            {
                Employee.Did = value?.Id;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public PositionModel SelectedPosition
        {
            get => Positions.FirstOrDefault(p => p.Id == Employee.Pid);
            set
            {
                Employee.Pid = value?.Id;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEdit { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        // ===================== CONSTRUCTOR =====================
        public EmployeeFormViewModel(EmployeeModel employee, bool isEdit)
        {
            IsEdit = isEdit;
            Employee = employee ?? new EmployeeModel();

            SaveCommand = new RelayCommand(w => Save(w), _ => CanSave());
            CancelCommand = new RelayCommand(w => Close(w));

            Employee.PropertyChanged += Employee_PropertyChanged;

            Departments = new ObservableCollection<DepartmentModel>(_deptRepo.GetAll());
            Positions = new ObservableCollection<PositionModel>(_posRepo.GetAll());

            if (!IsEdit)
                Employee.Id = _repo.GenerateNewEmployeeId();

            ValidatePhone();
        }

        private void Employee_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Employee.Name)
                || e.PropertyName == nameof(Employee.Phone)
                || e.PropertyName == nameof(Employee.Did)
                || e.PropertyName == nameof(Employee.Pid))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Employee.Name)
                && IsPhoneValid()
                && Employee.Did != null
                && Employee.Pid != null;
        }


        private void Save(object w)
        {
            if (!IsPhoneValid())
            {
                MessageBox.Show(
                    "Số điện thoại phải gồm đúng 10 chữ số.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (IsEdit)
                _repo.Update(Employee);
            else
                _repo.Add(Employee);

            MessageBox.Show(
                "Lưu nhân viên thành công!",
                "Thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close(w);
        }


        private void Close(object w)
        {
            if (w is Window win)
                win.Close();
        }
    }
}

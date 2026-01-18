using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class DepartmentFormViewModel : BaseViewModel
    {
        private readonly DepartmentRepository _repo;
        private readonly ObservableCollection<DepartmentModel> _items;
        private readonly DepartmentModel _original;

        public DepartmentModel Department { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public DepartmentFormViewModel(
            DepartmentModel department,
            ObservableCollection<DepartmentModel> items,
            DepartmentRepository repo,
            DepartmentModel original = null)
        {
            Department = department;
            _items = items;
            _repo = repo;
            _original = original;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(w =>
            {
                if (w is Window win) win.Close();
            });
        }

        private void Save(object obj)
        {
            if (string.IsNullOrWhiteSpace(Department.Id) ||
                string.IsNullOrWhiteSpace(Department.Name))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (_original == null && _repo.Exists(Department.Id))
            {
                MessageBox.Show("Mã bộ phận đã tồn tại!");
                return;
            }

            try
            {
                if (_original == null)
                {
                    _repo.Add(Department);
                    _items.Add(Department);
                }
                else
                {
                    _repo.Update(Department);
                    _original.Name = Department.Name;
                }

                if (obj is Window win) win.Close();
            }
            catch
            {
                MessageBox.Show("Lưu thất bại!");
            }
        }
    }
}

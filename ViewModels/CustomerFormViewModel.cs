using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class CustomerFormViewModel : BaseViewModel
    {
        private readonly CustomerRepository _repo;
        private readonly ObservableCollection<CustomerModel> _items;
        private readonly CustomerModel _original;

        public CustomerModel Customer { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CustomerFormViewModel(CustomerModel customer, ObservableCollection<CustomerModel> items, CustomerRepository repo, CustomerModel original = null)
        {
            Customer = customer;
            _items = items;
            _repo = repo;
            _original = original;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(param =>
            {
                if (param is Window w) w.Close();
            });
        }

        private void Save(object obj)
        {
            if (string.IsNullOrWhiteSpace(Customer.Name) ||
                string.IsNullOrWhiteSpace(Customer.Phone) ||
                string.IsNullOrWhiteSpace(Customer.Address))
            {
                MessageBox.Show("Hãy bổ sung tất cả thông tin!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var exist = _repo.GetByPhone(Customer.Phone);
            if (exist != null && (_original == null || exist.Id != _original.Id))
            {
                MessageBox.Show("Số điện thoại đã tồn tại!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_original == null)
                {
                    _repo.Add(Customer);
                    _items.Add(Customer);
                    MessageBox.Show("Thêm khách hàng thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Cập nhật toàn bộ, vì Payment cũng dùng Update()
                    _repo.Update(Customer);

                    // Cập nhật UI
                    _original.Name = Customer.Name;
                    _original.Phone = Customer.Phone;
                    _original.Address = Customer.Address;
                    _original.Tid = Customer.Tid;
                    _original.TotalExpenditure = Customer.TotalExpenditure;
                    _original.CumulativePoints = Customer.CumulativePoints;

                    MessageBox.Show("Chỉnh sửa khách hàng thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (obj is Window w) w.Close();
            }
            catch
            {
                MessageBox.Show("Lưu thất bại!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

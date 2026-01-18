using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class ManufacturerFormViewModel : BaseViewModel
    {
        private readonly ManufacturerRepository _repo;
        private readonly ObservableCollection<ManufacturerModel> _items;
        private readonly ManufacturerModel _original;

        public ManufacturerModel Manufacturer { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public ManufacturerFormViewModel(
            ManufacturerModel manufacturer,
            ManufacturerModel original,
            ManufacturerRepository repo,
            ObservableCollection<ManufacturerModel> items)
        {
            Manufacturer = manufacturer;
            _original = original;
            _repo = repo;
            _items = items;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(w =>
            {
                if (w is Window win) win.Close();
            });
        }

        private void Save(object obj)
        {
            if (string.IsNullOrWhiteSpace(Manufacturer.Id)
                || string.IsNullOrWhiteSpace(Manufacturer.Name))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (_original == null && _repo.Exists(Manufacturer.Id))
            {
                MessageBox.Show("Mã đã tồn tại!");
                return;
            }

            if (_original == null)
            {
                _repo.Add(Manufacturer);
                _items.Add(Manufacturer);
            }
            else
            {
                _repo.Update(Manufacturer);
                _original.Name = Manufacturer.Name;
                _original.Country = Manufacturer.Country;
            }

            if (obj is Window win) win.Close();
        }
    }

}

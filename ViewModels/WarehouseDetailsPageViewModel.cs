using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class WarehouseDetailsPageViewModel : BaseViewModel
    {
        private readonly WarehouseDetailsRepository _repo = new WarehouseDetailsRepository();
        public ObservableCollection<WarehouseDetailsModel> Items { get; set; } = new ObservableCollection<WarehouseDetailsModel>();

        private WarehouseDetailsModel _selectedItem;
        public WarehouseDetailsModel SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); } }

        public RelayCommand LoadCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public WarehouseDetailsPageViewModel()
        {
            LoadCommand = new RelayCommand(_ => Load());
            RefreshCommand = new RelayCommand(_ => Load());
            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedItem != null);
            Load();
        }

        private void Load()
        {
            Items = new ObservableCollection<WarehouseDetailsModel>(_repo.GetAll());
            RaisePropertyChanged(nameof(Items));
            SelectedItem = Items.FirstOrDefault();
        }

        private void Add()
        {
            var model = new WarehouseDetailsModel { Wid = "W001", Mid = "MED001", Quantity = 0, UnitPrice = 0, TotalAmount = 0 };
            try { _repo.Insert(model); Items.Add(model); SelectedItem = model; }
            catch (Exception ex) { MessageBox.Show("Add failed: " + ex.Message); }
        }

        private void Edit()
        {
            try { _repo.Update(SelectedItem); }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void Delete()
        {
            if (MessageBox.Show($"Delete {SelectedItem?.Wid}/{SelectedItem?.Mid} ?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try { _repo.Delete(SelectedItem.Wid, SelectedItem.Mid); Items.Remove(SelectedItem); SelectedItem = Items.FirstOrDefault(); }
            catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
        }
    }
}

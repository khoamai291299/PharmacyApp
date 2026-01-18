using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class RollCallPageViewModel : BaseViewModel
    {
        private readonly RollCallRepository _repo = new RollCallRepository();
        public ObservableCollection<RollCallModel> Items { get; set; } = new ObservableCollection<RollCallModel>();

        private RollCallModel _selectedItem;
        public RollCallModel SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); } }

        public RelayCommand LoadCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public RollCallPageViewModel()
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
            Items = new ObservableCollection<RollCallModel>(_repo.GetAll());
            RaisePropertyChanged(nameof(Items));
            SelectedItem = Items.FirstOrDefault();
        }

        private void Add()
        {
            var model = new RollCallModel { Months = DateTime.Today.Month, Years = DateTime.Today.Year };
            try { _repo.Insert(model); Load(); }
            catch (Exception ex) { MessageBox.Show("Add failed: " + ex.Message); }
        }

        private void Edit()
        {
            try { _repo.Update(SelectedItem); Load(); }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void Delete()
        {
            if (MessageBox.Show($"Delete {SelectedItem?.Id} ?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try { _repo.Delete(SelectedItem.Id); Load(); }
            catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
        }
    }
}

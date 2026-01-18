using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class DetailedAttendancePageViewModel : BaseViewModel
    {
        private readonly DetailedAttendanceRepository _repo = new DetailedAttendanceRepository();
        public ObservableCollection<DetailedAttendanceModel> Items { get; set; } = new ObservableCollection<DetailedAttendanceModel>();

        private DetailedAttendanceModel _selectedItem;
        public DetailedAttendanceModel SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); } }

        public RelayCommand LoadCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public DetailedAttendancePageViewModel()
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
            Items = new ObservableCollection<DetailedAttendanceModel>(_repo.GetAll());
            RaisePropertyChanged(nameof(Items));
            SelectedItem = Items.FirstOrDefault();
        }

        private void Add()
        {
            var model = new DetailedAttendanceModel { Eid = "EMP001", Days = DateTime.Today, Status = "Present" };
            try { _repo.Add(model); Items.Add(model); SelectedItem = model; }
            catch (Exception ex) { MessageBox.Show("Add failed: " + ex.Message); }
        }

        private void Edit()
        {
            try { _repo.Update(SelectedItem); }
            catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void Delete()
        {
            if (MessageBox.Show($"Delete {SelectedItem?.Id} ?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            try { _repo.Delete(SelectedItem.Id); Items.Remove(SelectedItem); SelectedItem = Items.FirstOrDefault(); }
            catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
        }
    }
}

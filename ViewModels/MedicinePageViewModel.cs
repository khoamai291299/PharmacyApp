using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class MedicinePageViewModel : BaseViewModel
    {
        private readonly MedicineRepository _repo = new MedicineRepository();

        // ===== DATA GỐC =====
        private List<MedicineModel> _allItems = new List<MedicineModel>();

        // ===== DATA HIỂN THỊ =====
        public ObservableCollection<MedicineModel> Items { get; } = new ObservableCollection<MedicineModel>();

        // ===== SELECTED ITEM =====
        private MedicineModel _selectedItem;
        public MedicineModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        // ===== SEARCH TEXT =====
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplySearch();
            }
        }

        // ===== COMMANDS =====
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public MedicinePageViewModel()
        {
            AddCommand = new RelayCommand(_ => OpenForm());
            EditCommand = new RelayCommand(param =>
            {
                var item = param as MedicineModel;
                if (item != null) OpenForm(item);
            }, param => param != null);

            DeleteCommand = new RelayCommand(param =>
            {
                var item = param as MedicineModel;
                if (item != null) Delete(item);
            }, param => param != null);

            RefreshCommand = new RelayCommand(_ => Load());

            Load();
        }


        // ================= METHODS =================
        private void Load()
        {
            _allItems = _repo.GetAll();
            ApplySearch();
            SelectedItem = Items.FirstOrDefault();
        }

        private void ApplySearch()
        {
            Items.Clear();

            IEnumerable<MedicineModel> source = _allItems;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string keyword = SearchText.Trim().ToLower();
                source = source.Where(m =>
                    m.Id.ToLower().Contains(keyword) ||
                    m.Name.ToLower().Contains(keyword)
                );
            }

            foreach (var item in source)
                Items.Add(item);
        }

        private void OpenForm(MedicineModel model = null)
        {
            var form = new MedicineFormWindow();
            var vm = new MedicineFormViewModel(model);

            vm.OnSaved += savedItem =>
            {
                var existing = _allItems.FirstOrDefault(x => x.Id == savedItem.Id);

                if (existing == null)
                    _allItems.Add(savedItem);
                else
                {
                    var index = _allItems.IndexOf(existing);
                    _allItems[index] = savedItem;
                }

                ApplySearch();
                SelectedItem = savedItem;
            };

            form.DataContext = vm;
            form.ShowDialog();
        }

        private void Delete(MedicineModel item)
        {
            if (item == null) return;

            if (MessageBox.Show($"Xóa thuốc {item.Name} ?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                _repo.Delete(item.Id);
                _allItems.Remove(item);
                ApplySearch();
                SelectedItem = Items.FirstOrDefault();
                MessageBox.Show("Xóa thành công.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Xóa thất bại: " + ex.Message);
            }
        }
    }
}

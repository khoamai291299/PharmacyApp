using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PharmacyApp.ViewModels
{
    public class ManufacturerPageViewModel : BaseViewModel
    {
        private readonly ManufacturerRepository _repo = new();

        public ObservableCollection<ManufacturerModel> Items { get; } = new();

        public ICollectionView ItemsView { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ItemsView.Refresh();
            }
        }

        private ManufacturerModel _selectedItem;
        public ManufacturerModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public ManufacturerPageViewModel()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = Filter;

            AddCommand = new RelayCommand(_ => OpenForm());
            EditCommand = new RelayCommand(_ => OpenForm(SelectedItem));
            DeleteCommand = new RelayCommand(_ => Delete());
            RefreshCommand = new RelayCommand(_ => Load());

            Load();
        }

        private void Load()
        {
            Items.Clear();
            foreach (var m in _repo.GetAll())
                Items.Add(m);

            SelectedItem = null;
        }

        private bool Filter(object obj)
        {
            if (obj is not ManufacturerModel m) return false;
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            return m.Id.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || m.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || m.Country.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        private void Delete()
        {
            if (MessageBox.Show(
                $"Xóa nhà sản xuất {SelectedItem.Name}?",
                "Confirm",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _repo.Delete(SelectedItem.Id);
            Items.Remove(SelectedItem);
        }

        private void OpenForm(ManufacturerModel model = null)
        {
            ManufacturerModel working;

            if (model == null)
            {
                working = new ManufacturerModel
                {
                    Id = _repo.GenerateNewId()
                };
            }
            else
            {
                working = new ManufacturerModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Country = model.Country
                };
            }

            var win = new ManufacturerFormWindow
            {
                DataContext = new ManufacturerFormViewModel(
                    working,
                    model,
                    _repo,
                    Items)
            };

            win.ShowDialog();
        }

    }
}

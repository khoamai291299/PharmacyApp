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
    public class PositionPageViewModel : BaseViewModel
    {
        private readonly PositionRepository _repo = new PositionRepository();

        public ObservableCollection<PositionModel> Items { get; }
            = new ObservableCollection<PositionModel>();

        // ===== VIEW / SEARCH =====
        private ICollectionView _itemsView;
        public ICollectionView ItemsView
        {
            get => _itemsView;
            set => SetProperty(ref _itemsView, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ItemsView?.Refresh();
            }
        }

        // ===== SELECTED =====
        private PositionModel _selectedItem;
        public PositionModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                UpdateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        // ===== COMMANDS =====
        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public PositionPageViewModel()
        {
            AddCommand = new RelayCommand(_ => OpenForm());
            UpdateCommand = new RelayCommand(_ => OpenForm(SelectedItem));
            DeleteCommand = new RelayCommand(_ => Delete());

            Load();
        }

        private void Load()
        {
            Items.Clear();
            foreach (var p in _repo.GetAll())
                Items.Add(p);

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterPosition;
            SelectedItem = null;
        }

        private bool FilterPosition(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is not PositionModel p) return false;

            return p.Id.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)
                || p.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase);
        }

        private void Delete()
        {
            if (MessageBox.Show("Xóa chức vụ này?", "Confirm",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _repo.Delete(SelectedItem.Id);
            Load();
        }

        private void OpenForm(PositionModel model = null)
        {
            var window = new PositionFormWindow();

            PositionModel working;
            PositionModel original = null;

            if (model == null)
            {
                working = new PositionModel
                {
                    Id = _repo.GenerateNewId()
                };
            }
            else
            {
                original = model;
                working = new PositionModel
                {
                    Id = model.Id,
                    Name = model.Name
                };
            }

            window.DataContext = new PositionFormViewModel(
                working,
                Items,
                _repo,
                original
            );

            window.ShowDialog();
            Load();
        }
    }
}

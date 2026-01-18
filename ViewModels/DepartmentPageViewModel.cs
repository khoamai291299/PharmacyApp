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
    public class DepartmentPageViewModel : BaseViewModel
    {
        private readonly DepartmentRepository _repo = new DepartmentRepository();

        public ObservableCollection<DepartmentModel> Items { get; set; }
            = new ObservableCollection<DepartmentModel>();

        // ===== SEARCH =====
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
        private DepartmentModel _selectedItem;
        public DepartmentModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // ===== COMMANDS =====
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public DepartmentPageViewModel()
        {
            AddCommand = new RelayCommand(_ => OpenForm());
            EditCommand = new RelayCommand(
                p => OpenForm(p as DepartmentModel)
            );
            DeleteCommand = new RelayCommand(_ => Delete());
            RefreshCommand = new RelayCommand(_ => Load());

            Load();
        }

        private void Load()
        {
            Items = new ObservableCollection<DepartmentModel>(_repo.GetAll());
            OnPropertyChanged(nameof(Items));

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterDepartment;

            SelectedItem = null;
        }

        private bool FilterDepartment(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            if (obj is not DepartmentModel d) return false;

            return
                (d.Id?.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) == true) ||
                (d.Name?.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) == true);
        }

        private void Delete()
        {
            if (SelectedItem == null) return;

            if (MessageBox.Show($"Xóa bộ phận {SelectedItem.Name}?",
                "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                _repo.Delete(SelectedItem.Id);
                Items.Remove(SelectedItem);
            }
            catch
            {
                MessageBox.Show("Xóa thất bại!");
            }
        }

        private void OpenForm(DepartmentModel model = null)
        {
            DepartmentModel working;
            DepartmentModel original = null;

            if (model == null)
            {
                working = new DepartmentModel
                {
                    Id = _repo.GenerateNewId()
                };
            }
            else
            {
                original = model;
                working = new DepartmentModel
                {
                    Id = model.Id,
                    Name = model.Name
                };
            }

            var window = new DepartmentFormWindow
            {
                DataContext = new DepartmentFormViewModel(
                    working,
                    Items,
                    _repo,
                    original)
            };

            window.ShowDialog();
        }
    }
}

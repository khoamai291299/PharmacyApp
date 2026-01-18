using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PharmacyApp.ViewModels
{
    public class CustomerPageViewModel : BaseViewModel
    {
        private readonly CustomerRepository _repo = new CustomerRepository();

        public ObservableCollection<CustomerModel> Items { get; set; } = new ObservableCollection<CustomerModel>();

        private CustomerModel _selectedItem;
        public CustomerModel SelectedItem
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

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand RefreshCommand { get; }

        public CustomerPageViewModel()
        {
            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit());
            DeleteCommand = new RelayCommand(_ => Delete());
            RefreshCommand = new RelayCommand(_ => Load());
            Load();
        }

        private void Load()
        {
            Items = new ObservableCollection<CustomerModel>(_repo.GetAll());
            OnPropertyChanged(nameof(Items));

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterCustomer;

            SelectedItem = null;
        }


        private void Add()
        {
            var model = new CustomerModel
            {
                Id = _repo.GenerateNewId(),
                Tid = _repo.GetDefaultTypeId() ?? "TC01",
                TotalExpenditure = 0,
                CumulativePoints = 0
            };

            var form = new Views.CustomerFormWindow
            {
                DataContext = new CustomerFormViewModel(model, Items, _repo)
            };
            form.ShowDialog();
        }

        private void Edit()
        {
            if (SelectedItem == null) return;

            var modelCopy = new CustomerModel
            {
                Id = SelectedItem.Id,
                Name = SelectedItem.Name,
                Phone = SelectedItem.Phone,
                Address = SelectedItem.Address,
                Tid = SelectedItem.Tid,
                TotalExpenditure = SelectedItem.TotalExpenditure,
                CumulativePoints = SelectedItem.CumulativePoints
            };

            var form = new Views.CustomerFormWindow
            {
                DataContext = new CustomerFormViewModel(modelCopy, Items, _repo, SelectedItem)
            };
            form.ShowDialog();
        }

        private void Delete()
        {
            if (SelectedItem == null) return;
            if (MessageBox.Show($"Xóa {SelectedItem.Name}?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

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
        private bool FilterCustomer(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var c = obj as CustomerModel;
            if (c == null) return false;

            return
                (c.Id?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                (c.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) ||
                (c.Phone?.Contains(SearchText) == true);
        }

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
    }
}

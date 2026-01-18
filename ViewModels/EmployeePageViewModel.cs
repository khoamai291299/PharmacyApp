using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
namespace PharmacyApp.ViewModels
{
    public class EmployeePageViewModel : BaseViewModel
    {
        private readonly EmployeeRepository _repo = new EmployeeRepository();

        // ====== DATA ======
        private ObservableCollection<EmployeeModel> _items;
        public ObservableCollection<EmployeeModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private EmployeeModel _selectedItem;

        public EmployeeModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        // ====== FORM STATE ======
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }

        public bool IsEditMode { get; set; }

        private EmployeeModel _editingEmployee;
        public EmployeeModel EditingEmployee
        {
            get => _editingEmployee;
            set
            {
                SetProperty(ref _editingEmployee, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        // ====== COMMANDS ======
        public RelayCommand LoadCommand { get; }
        public RelayCommand RefreshCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        // ====== CONSTRUCTOR ======
        public EmployeePageViewModel()
        {
            LoadCommand = new RelayCommand(_ => Load());
            RefreshCommand = new RelayCommand(_ => Load());

            AddCommand = new RelayCommand(_ => OpenAddForm());
            EditCommand = new RelayCommand(_ => OpenEditForm());
            DeleteCommand = new RelayCommand(_ => Delete());

            SaveCommand = new RelayCommand(
                _ => Save(),
                _ => EditingEmployee != null && !string.IsNullOrWhiteSpace(EditingEmployee.Name)
            );
            CancelCommand = new RelayCommand(_ => Cancel());

            Load();
        }

        private bool FilterEmployee(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            var e = obj as EmployeeModel;
            return e != null &&
                   (e.Id?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    e.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    e.Phone?.Contains(SearchText) == true);
        }

        // ====== METHODS ======
        private void Load()
        {
            Items = new ObservableCollection<EmployeeModel>(_repo.GetAll());

            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterEmployee;

            SelectedItem = null;
        }

        private void OpenAddForm()
        {
            var emp = new EmployeeModel
            {
                Id = GenerateId()
            };

            var vm = new EmployeeFormViewModel(emp, false);
            var win = new EmployeeFormWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            win.ShowDialog();
            Load();
        }

        private void OpenEditForm()
        {
            if (SelectedItem == null) return;

            var emp = new EmployeeModel
            {
                Id = SelectedItem.Id,
                Name = SelectedItem.Name,
                Phone = SelectedItem.Phone,
                Address = SelectedItem.Address,
                startday = SelectedItem.startday,
                birthday = SelectedItem.birthday,
                Salary = SelectedItem.Salary,
                Did = SelectedItem.Did,
                Pid = SelectedItem.Pid
            };

            var vm = new EmployeeFormViewModel(emp, true);
            var win = new EmployeeFormWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            win.ShowDialog();
            Load();
        }

        private void Save()
        {
            try
            {
                if (IsEditMode)
                {
                    _repo.Update(EditingEmployee);
                }
                else
                {
                    _repo.Add(EditingEmployee);
                }

                Load();
                IsFormVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message);
            }
        }

        private void Cancel()
        {
            IsFormVisible = false;
        }

        private void Delete()
        {
            if (SelectedItem == null) return;

            if (MessageBox.Show(
                $"Delete employee {SelectedItem.Id} ?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                _repo.Delete(SelectedItem.Id);
                Items.Remove(SelectedItem);
                SelectedItem = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message);
            }
        }

        private string GenerateId()
        {
            const string prefix = "EMP";
            int max = 0;

            foreach (var i in Items)
            {
                if (i?.Id != null &&
                    i.Id.StartsWith(prefix) &&
                    int.TryParse(i.Id.Substring(prefix.Length), out int n))
                {
                    if (n > max) max = n;
                }
            }

            return $"{prefix}{(max + 1).ToString("D3")}";
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

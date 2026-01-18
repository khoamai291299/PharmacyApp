using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using PharmacyApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PharmacyApp.ViewModels
{
    public class UserPageViewModel : BaseViewModel
    {
        private readonly UserRepository _repo = new();

        // ===== DATA =====
        public ObservableCollection<UserModel> Items { get; } = new();

        private ICollectionView _itemsView;
        public ICollectionView ItemsView
        {
            get => _itemsView;
            set => SetProperty(ref _itemsView, value);
        }

        private UserModel _selected;
        public UserModel Selected
        {
            get => _selected;
            set
            {
                SetProperty(ref _selected, value);
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        // ===== SEARCH =====
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

        // ===== COMMANDS =====
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        // ===== CONSTRUCTOR =====
        public UserPageViewModel()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = FilterUser;

            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit());
            DeleteCommand = new RelayCommand(_ => Delete());

            Load();
        }

        // ===== LOAD =====
        private void Load()
        {
            Items.Clear();
            foreach (var u in _repo.GetAll())
                Items.Add(u);

            Selected = null;
        }

        // ===== FILTER =====
        private bool FilterUser(object obj)
        {
            if (obj is not UserModel u)
                return false;

            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return
                u.Username?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                u.Phone?.Contains(SearchText) == true;
        }

        // ===== ACTIONS =====
        private void Add()
        {
            var vm = new UserFormViewModel(new UserModel(), false);
            var win = new UserFormWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            win.ShowDialog();
            Load();
        }

        private void Edit()
        {
            if (Selected == null) return;

            var clone = new UserModel
            {
                Id = Selected.Id,
                Username = Selected.Username,
                Password = Selected.Password,
                Email = Selected.Email,
                Phone = Selected.Phone,
                Eid = Selected.Eid,
                Role = Selected.Role,
                Status = Selected.Status
            };

            var vm = new UserFormViewModel(clone, true);
            var win = new UserFormWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            win.ShowDialog();
            Load();
        }

        private void Delete()
        {
            if (Selected == null) return;

            if (MessageBox.Show(
                $"Delete account '{Selected.Username}' ?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _repo.Delete(Selected.Username);
            Items.Remove(Selected);
        }
    }
}

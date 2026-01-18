using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class TypeCustomerPageViewModel : BaseViewModel
    {
        private readonly TypeCustomerRepository _repo = new TypeCustomerRepository();

        public ObservableCollection<TypeCustomerModel> Items { get; set; }
            = new ObservableCollection<TypeCustomerModel>();

        private TypeCustomerModel _selectedItem;
        public TypeCustomerModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                    EditModel = new TypeCustomerModel
                    {
                        Id = value.Id,
                        Name = value.Name,
                        MinimumLevel = value.MinimumLevel,
                        MaximumLevel = value.MaximumLevel
                    };
            }
        }

        private TypeCustomerModel _editModel = new TypeCustomerModel();
        public TypeCustomerModel EditModel
        {
            get => _editModel;
            set => SetProperty(ref _editModel, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public TypeCustomerPageViewModel()
        {
            Load();

            AddCommand = new RelayCommand(_ => Add());
            UpdateCommand = new RelayCommand(_ => Update(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedItem != null);
            ClearCommand = new RelayCommand(_ => Clear());
        }

        private void Load()
        {
            Items.Clear();
            foreach (var item in _repo.GetAll())
                Items.Add(item);
        }

        private void Add()
        {
            try
            {
                _repo.Insert(EditModel);
                Load();
                MessageBox.Show("Thêm thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Update()
        {
            try
            {
                _repo.Update(EditModel);
                Load();
                MessageBox.Show("Sửa thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Delete()
        {
            try
            {
                _repo.Delete(SelectedItem.Id);
                Load();
                MessageBox.Show("Xóa thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Clear()
        {
            EditModel = new TypeCustomerModel();
            SelectedItem = null;
        }
    }
}

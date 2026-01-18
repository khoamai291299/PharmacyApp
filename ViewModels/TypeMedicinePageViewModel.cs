using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class TypeMedicinePageViewModel : BaseViewModel
    {
        private readonly TypeMedicineRepository _repo = new TypeMedicineRepository();

        public ObservableCollection<TypeMedicineModel> Items { get; set; }
            = new ObservableCollection<TypeMedicineModel>();

        private TypeMedicineModel _selectedItem;
        public TypeMedicineModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    EditModel = new TypeMedicineModel
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Description = value.Description
                    };
                }
            }
        }

        private TypeMedicineModel _editModel = new TypeMedicineModel();
        public TypeMedicineModel EditModel
        {
            get => _editModel;
            set => SetProperty(ref _editModel, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public TypeMedicinePageViewModel()
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
                MessageBox.Show("Cập nhật thành công!");
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
            EditModel = new TypeMedicineModel();
            SelectedItem = null;
        }
    }
}

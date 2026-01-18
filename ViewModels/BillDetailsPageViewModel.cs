using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace PharmacyApp.ViewModels
{
    public class BillDetailsPageViewModel : BaseViewModel
    {
        private readonly BillDetailsRepository _repo = new BillDetailsRepository();

        public ObservableCollection<BillDetailsModel> Items { get; set; }
            = new ObservableCollection<BillDetailsModel>();

        private BillDetailsModel _selectedItem;

        public BillDetailsModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);

                if (value != null)
                {
                    EditModel = new BillDetailsModel
                    {
                        Bid = value.Bid,
                        Mid = value.Mid,
                        Quantity = value.Quantity,
                        UnitPrice = value.UnitPrice,
                        TotalAmount = value.TotalAmount
                    };
                }
            }
        }

        private BillDetailsModel _editModel = new BillDetailsModel();
        public BillDetailsModel EditModel
        {
            get => _editModel;
            set => SetProperty(ref _editModel, value);
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand UpdateCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearCommand { get; }

        public BillDetailsPageViewModel()
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
                _repo.Add(EditModel);
                Load();
                MessageBox.Show("Thêm thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Update()
        {
            try
            {
                _repo.Update(EditModel);
                Load();
                MessageBox.Show("Sửa thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Delete()
        {
            try
            {
                _repo.Delete(SelectedItem.Bid, SelectedItem.Mid);
                Load();
                MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void Clear()
        {
            EditModel = new BillDetailsModel();
            SelectedItem = null;
        }
    }
}

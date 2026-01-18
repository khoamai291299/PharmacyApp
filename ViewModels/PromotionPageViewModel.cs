    using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;

namespace PharmacyApp.ViewModels
{
    public class PromotionPageViewModel : BaseViewModel
    {
        private readonly PromotionRepository _repo = new PromotionRepository();

        public ObservableCollection<PromotionModel> Promotions { get; set; }
        public ICollectionView PromotionsView { get; set; }

        private string _searchId;
        public string SearchId
        {
            get => _searchId;
            set
            {
                SetProperty(ref _searchId, value);
                PromotionsView?.Refresh();
            }
        }

        private DateTime? _searchDate;
        public DateTime? SearchDate
        {
            get => _searchDate;
            set
            {
                SetProperty(ref _searchDate, value);
                PromotionsView?.Refresh();
            }
        }

        public PromotionPageViewModel()
        {
            Promotions = new ObservableCollection<PromotionModel>(_repo.GetAll());
            PromotionsView = CollectionViewSource.GetDefaultView(Promotions);
            PromotionsView.Filter = FilterPromotion;
        }

        private ICommand _addPromotionCommand;
        public ICommand AddPromotionCommand => _addPromotionCommand ??= new RelayCommand(o =>
        {
            OpenPromotionForm();
        });

        private ICommand _editPromotionCommand;
        public ICommand EditPromotionCommand => _editPromotionCommand ??= new RelayCommand(o =>
        {
            if (o is PromotionModel promo)
                OpenPromotionForm(promo);
        });

        private ICommand _deletePromotionCommand;
        public ICommand DeletePromotionCommand => _deletePromotionCommand ??= new RelayCommand(o =>
        {
            if (o is PromotionModel promo)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa khuyến mãi {promo.Id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _repo.Delete(promo.Id);
                        Promotions.Remove(promo);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        });



        // Mở form thêm hoặc sửa
        private void OpenPromotionForm(PromotionModel promo = null)
        {
            var formVm = promo == null
                ? new PromotionFormViewModel()
                : new PromotionFormViewModel(promo);

            var formWindow = new PharmacyApp.Views.PromotionFormWindow
            {
                DataContext = formVm
            };

            formWindow.ShowDialog();

            // Sau khi đóng form, refresh danh sách
            RefreshPromotions();
        }

        private void RefreshPromotions()
        {
            Promotions.Clear();
            foreach (var p in _repo.GetAll())
                Promotions.Add(p);

            PromotionsView?.Refresh();
        }


        private bool FilterPromotion(object obj)
        {
            if (obj is not PromotionModel p)
                return false;

            // Lọc theo mã
            if (!string.IsNullOrWhiteSpace(SearchId))
            {
                if (p.Id == null || !p.Id.Contains(SearchId, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // Lọc theo ngày áp dụng
            if (SearchDate.HasValue)
            {
                var d = SearchDate.Value.Date;
                if (!(p.StartDate.Date <= d && d <= p.EndDate.Date))
                    return false;
            }

            return true;
        }

    }
}

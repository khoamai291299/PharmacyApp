using PharmacyApp.DataAccess;
using PharmacyApp.Models;
using PharmacyApp.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PharmacyApp.ViewModels
{
    public class PromotionFormViewModel : INotifyPropertyChanged
    {
        private readonly PromotionRepository _repo = new PromotionRepository();
        public PromotionModel Promotion { get; set; }
        public bool IsEdit { get; set; }

        public PromotionFormViewModel()
        {
            Promotion = new PromotionModel
            {
                Id = _repo.GenerateNewId(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DiscountPercent = 5 // mặc định 5%
            };
        }

        public PromotionFormViewModel(PromotionModel existingPromo)
        {
            Promotion = existingPromo;
            IsEdit = true;
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(o =>
        {
            try
            {
                // Validate input
                if (Promotion.RequiredPoints < 0 || Promotion.Quantity < 1 || Promotion.DiscountPercent < 1)
                {
                    MessageBox.Show("Vui lòng nhập thông tin hợp lệ!");
                    return;
                }

                if (IsEdit)
                    _repo.Update(Promotion);
                else
                    _repo.Add(Promotion);

                MessageBox.Show("Lưu thành công!");
                ((Window)o).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        });

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(o =>
        {
            ((Window)o).Close();
        });

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

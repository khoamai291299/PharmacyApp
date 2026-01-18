using System.ComponentModel;

namespace PharmacyApp.ViewModels
{
    public class WarehouseDetailItemViewModel : BaseViewModel
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 1) value = 1;
                if (SetProperty(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        private int _unitPrice;
        public int UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (value < 0) value = 0;
                if (SetProperty(ref _unitPrice, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        public int TotalAmount => Quantity * UnitPrice;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyApp.ViewModels
{
    public class SaleItemViewModel : BaseViewModel
    {
        public string MedicineId { get; set; }
        public string MedicineName { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 1) return;
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public int UnitPrice { get; set; }

        public int TotalAmount => Quantity * UnitPrice;
    }
}

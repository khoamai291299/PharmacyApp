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
    public class BillItemViewModel : BaseViewModel
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

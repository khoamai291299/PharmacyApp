using PharmacyApp.ViewModels;
using System.Windows;

namespace PharmacyApp.Views
{
    public partial class PaymentWindow : Window
    {
        public PaymentWindow()
        {
            InitializeComponent();
            DataContextChanged += PaymentWindow_DataContextChanged;
        }

        private void PaymentWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PaymentViewModel oldVm)
            {
                oldVm.RequestClose -= OnRequestClose;
            }

            if (e.NewValue is PaymentViewModel newVm)
            {
                newVm.RequestClose += OnRequestClose;
            }
        }

        private void OnRequestClose(bool? dialogResult)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}

using PharmacyApp.ViewModels;
using System.Windows;

namespace PharmacyApp.Views
{
    /// <summary>
    /// Interaction logic for MedicineFormWindow.xaml
    /// </summary>
    public partial class MedicineFormWindow : Window
    {
        public MedicineFormWindow()
        {
            InitializeComponent();
            DataContext = new MedicineFormViewModel(); // gán ViewModel
        }

        // nếu muốn constructor truyền sẵn ViewModel từ bên ngoài
        public MedicineFormWindow(MedicineFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

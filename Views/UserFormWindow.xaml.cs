using PharmacyApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PharmacyApp.Views
{
    /// <summary>
    /// Interaction logic for UserFormWindow.xaml
    /// </summary>
    public partial class UserFormWindow : Window
    {
        public UserFormWindow()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserFormViewModel vm &&
                sender is PasswordBox pb)
            {
                vm.User.Password = pb.Password;
                vm.SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}

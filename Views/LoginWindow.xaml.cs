using PharmacyApp.Models;
using PharmacyApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PharmacyApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                DataContext ??= new LoginViewModel(OnLoginSuccess);
            };
        }

        // ==============================
        //  BẮT PASSWORDBOX → VIEWMODEL
        // ==============================
        private void PasswordBox_OnChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        // ==============================
        //  CALLBACK LOGIN THÀNH CÔNG
        // ==============================
        private void OnLoginSuccess(UserModel user, EmployeeModel employee)
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(user, employee)
            };

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            this.Close();
        }
    }
}

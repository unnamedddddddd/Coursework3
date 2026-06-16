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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Курсовая_3_курс
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Page
    {
        public AdminPanel()
        {
            InitializeComponent();
        }

        private void CreateUsersBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new CreateUserAdminPanel();
        }

        private void UpdateRoleUserBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new UpdateRole();
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ShowUsers();
        }

        private void RecordsBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ShowAttendanceRecords();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
               "Вы уверены, что хотите выйти?",
               "Подтверждение",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question
           );

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}

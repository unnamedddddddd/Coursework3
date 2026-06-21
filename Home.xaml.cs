using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True";

        public Home()
        {
            InitializeComponent();
            getUserFullName();
        }

        public void getUserFullName()
        {
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(
                        $@"SELECT full_name FROM Users WHERE user_id = @id",
                        conn
                    );

                    DA.SelectCommand.Parameters.AddWithValue("@id", App.CurrentUserId);

                    DataSet dataSet = new DataSet();
                    DA.Fill(dataSet);

                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        userFullName.Text = dataSet.Tables[0].Rows[0]["full_name"].ToString();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);

            }
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

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            HelpStudent helpWindow = new HelpStudent();
            helpWindow.Owner = Window.GetWindow(this);
            helpWindow.ShowDialog();
        }
    }
}

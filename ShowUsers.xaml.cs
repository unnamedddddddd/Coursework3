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
    /// Логика взаимодействия для ShowUsers.xaml
    /// </summary>
    public partial class ShowUsers : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True";

        public ShowUsers()
        {
            InitializeComponent();
            ShowAllUsers();
        }

        public void ShowAllUsers()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT * FROM Users",
                    conn             
                );

                DataSet DS = new DataSet();
                DA.Fill(DS);

                if (DS.Tables.Count > 0)
                {
                    UsersTable.ItemsSource = DS.Tables[0].DefaultView;
                }
                else
                {
                    MessageBox.Show("Данные не загружены");
                }
            }
        }
    }
}

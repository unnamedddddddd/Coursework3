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
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True"; //public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Курсовая 3 курс\\Course DB.mdf\";Integrated Security=True";


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
    }
}

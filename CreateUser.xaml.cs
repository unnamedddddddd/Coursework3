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
using BCrypt.Net;
using Microsoft.Data.SqlClient;

namespace Курсовая_3_курс
{
    /// <summary>
    /// Логика взаимодействия для CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Page
    {

        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Курсовая 3 курс\\Course DB.mdf\";Integrated Security=True";

        public CreateUser()
        {
            InitializeComponent();
        }

        private void Create_user_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userLogin = login.Text.Trim();
                string userPassword = password.Text.Trim();
                string repeatPassword = repeat_password.Text.Trim();

                if (userPassword != repeatPassword)
                {
                    throw new Exception("Пароли не совпадают");
                }

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(
                        $@"SELECT user_login FROM Users WHERE user_login = @login",
                        conn
                    );

                    DA.SelectCommand.Parameters.AddWithValue("@login", userLogin);
                    DA.SelectCommand.Parameters.AddWithValue("@password", userPassword);

                    DataSet dataSet = new DataSet();
                    DA.Fill(dataSet);

                    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        throw new Exception("Пользователь с таким именем уже есть");
                    }
                    conn.Close();
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userPassword);

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Users (user_login, user_password) VALUES(@login, @password)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", userLogin);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                MessageBox.Show("Пользователь создан");
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new Login());
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        //public void LoadComboBox(string sql, ComboBox data, string text, string id)
        //{
        //    try
        //    {
        //        using (var conn = new SqlConnection(ConnString))
        //        {
        //            conn.Open();
        //            SqlDataAdapter DA = new SqlDataAdapter(sql, conn);
        //            DataSet dataSet = new DataSet();
        //            DA.Fill(dataSet);

        //            data.ItemsSource = dataSet.Tables[0].DefaultView; 
        //            data.DisplayMemberPath = text;  
        //            data.SelectedValuePath = id;

        //        }
        //    } catch(Exception ex) {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void return_login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}

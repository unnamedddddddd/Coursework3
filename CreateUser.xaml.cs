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

        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True";

        public CreateUser()
        {
            InitializeComponent();
            LoadGroups();
        }

        private void Create_user_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userLogin = login.Text.Trim();
                string userFullName = fullName.Text.Trim();
                string userPassword = password.Password.Trim();
                string repeatPassword = repeat_password.Password.Trim();
                int groupId = Convert.ToInt32(GroupsComboBox.SelectedValue);

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
                    string sql = "INSERT INTO Users (user_login, user_password, full_name, group_id) VALUES(@login, @password, @fullname, @groupId)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", userLogin);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@fullName", userFullName);
                        cmd.Parameters.AddWithValue("@groupId", groupId);

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
        private void LoadGroups()
        {
            string query = "SELECT group_id, group_name FROM Student_groups ORDER BY group_name";

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                SqlDataAdapter DA = new SqlDataAdapter(query, conn);
                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    GroupsComboBox.ItemsSource = DT.DefaultView;
                    GroupsComboBox.DisplayMemberPath = "group_name";
                    GroupsComboBox.SelectedValuePath = "group_id";
                    GroupsComboBox.SelectedIndex = 0;
                }
            }
        }

        private void return_login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            HelpCreateMain helpWindow = new HelpCreateMain();
            helpWindow.Owner = Window.GetWindow(this);
            helpWindow.ShowDialog();
        }
    }
}

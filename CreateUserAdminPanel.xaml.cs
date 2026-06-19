using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
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
    /// Логика взаимодействия для CreateUserAdminPanel.xaml
    /// </summary>
    public partial class CreateUserAdminPanel : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Курсовая 3 курс\\Course DB.mdf\";Integrated Security=True";

        public CreateUserAdminPanel()
        {
            InitializeComponent();
            LoadCheckBoxRoles();
        }

        public void LoadCheckBoxRoles()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                try
                {
                    conn.Open();

                    SqlDataAdapter DA = new SqlDataAdapter(
                        "SELECT * FROM Roles ORDER BY role_name",
                        conn
                    );

                    DataTable DT = new DataTable();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        RolesComboBox.ItemsSource = DT.DefaultView;
                        RolesComboBox.DisplayMemberPath = "role_name";
                        RolesComboBox.SelectedValuePath = "role_id";
                        RolesComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("Данные не загружены");
                    }

                }
                catch (Exception ex) { 
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RolesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RolesComboBox.SelectedValue == null) return;

            int selectedRoleId = Convert.ToInt32(RolesComboBox.SelectedValue);

            if (selectedRoleId == 5)
            {
                GroupPanel.Visibility = Visibility.Visible;
                LoadGroups();
            }
            else
            {
                GroupPanel.Visibility = Visibility.Collapsed;
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

        private void Create_user_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userLogin = login.Text.Trim();
                string userFullName = fullName.Text.Trim();

                if (RolesComboBox.SelectedValue == null)
                {
                    throw new Exception("Выберите роль пользователя");
                }

                if (Convert.ToInt32(RolesComboBox.SelectedValue) == 5 && GroupsComboBox.SelectedValue == null)
                {
                    throw new Exception("Выберите группу студента");
                }

                int userRoleId = Convert.ToInt32(RolesComboBox.SelectedValue);
                int userGroupId = Convert.ToInt32(GroupsComboBox.SelectedValue);
                string userPassword = password.Password.Trim();
                string repeatPassword = repeat_password.Password.Trim();

                if (userPassword != repeatPassword)
                {
                    throw new Exception("Пароли не совпадают");
                }

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(
                        "SELECT user_login FROM Users WHERE user_login = @login",
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

                using (SqlConnection conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    if (Convert.ToInt32(RolesComboBox.SelectedValue) == 5) 
                    {
                        string sql = "INSERT INTO Users (user_login, user_password, full_name, role_id, group_id) VALUES(@login, @password, @fullname, @role_id, @group_id)";

                        using (var cmd = new SqlCommand(sql, conn)) 
                        {
                            cmd.Parameters.AddWithValue("@login", userLogin);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                            cmd.Parameters.AddWithValue("@fullname", userFullName);
                            cmd.Parameters.AddWithValue("@role_id", userRoleId);
                            cmd.Parameters.AddWithValue("@group_id", userGroupId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else 
                    {
                        string sql = "INSERT INTO Users (user_login, user_password, full_name, role_id) VALUES(@login, @password, @fullname, @role_id)";

                        using (var cmd = new SqlCommand(sql, conn)) 
                        {
                            cmd.Parameters.AddWithValue("@login", userLogin);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                            cmd.Parameters.AddWithValue("@fullname", userFullName);
                            cmd.Parameters.AddWithValue("@role_id", userRoleId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    conn.Close();
                }

                MessageBox.Show("Пользователь создан");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void return_login_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

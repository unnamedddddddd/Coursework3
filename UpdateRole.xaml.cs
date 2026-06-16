using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
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
    /// Логика взаимодействия для UpdateRole.xaml
    /// </summary>
    public partial class UpdateRole : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True";

        public UpdateRole()
        {
            InitializeComponent();
            LoadUsersComboBox();
            LoadRolesComboBox();
        }

        public void LoadUsersComboBox() 
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT user_id, user_login FROM Users ORDER BY user_login",
                    conn
                );

                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    UsersComboBox.ItemsSource = DT.DefaultView;
                }
            }
        }

        public void LoadRolesComboBox()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT role_id, role_name FROM Roles ORDER BY role_name",
                    conn
                );

                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    RolesComboBox.ItemsSource = DT.DefaultView;
                }
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsersComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите пользователя");
                    return;
                }

                if (RolesComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите роль");
                    return;
                }

                string selectedUserId = UsersComboBox.SelectedValue.ToString();
                string selectedRoleId = RolesComboBox.SelectedValue.ToString();

                MessageBoxResult resultMessage = MessageBox.Show(
                   "Внимание! Этот пользователь может быть участником записей в системе.\n\n" +
                   "Изменение роли может привести к потере доступа к этим записям или нарушению работы системы.\n\n" +
                   "Вы действительно хотите изменить роль?",
                   "Предупреждение",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Warning
                );

                if (resultMessage != MessageBoxResult.Yes)
                {
                    return;
                }

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    string sql = "UPDATE Users SET role_id = @selectedRoleId WHERE user_id = @selectedUserId";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@selectedRoleId", selectedRoleId);
                    cmd.Parameters.AddWithValue("@selectedUserId", selectedUserId);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Роль обновлена");
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден");
                    }
                    conn.Close();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void UsersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int userId = Convert.ToInt32(UsersComboBox.SelectedValue);

            string sql = @"SELECT 
                            U.role_id, 
                            R.role_name
                        FROM Users U
                        INNER JOIN Roles R ON U.role_id = R.role_id
                        WHERE U.user_id = @userId";

            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                SqlDataReader DR = cmd.ExecuteReader();
                if (DR.Read())
                {
                    CurrentRoleText.Text = DR.GetString(1);
                }
                conn.Close();
            }
        }
    }
}

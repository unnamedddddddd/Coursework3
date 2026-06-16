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
            LoadGroups();
            LoadCheckBoxRoles();
        }

        public void ShowAllUsers()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                string sql = @"SELECT 
                                u.user_id,
                                u.user_login,
                                u.full_name,
                                r.role_name,
                                g.group_name,
                                u.phone_number
                              FROM Users u
                              LEFT JOIN Roles r ON u.role_id = r.role_id
                              LEFT JOIN Student_groups g ON u.group_id = g.group_id";

                SqlDataAdapter DA = new SqlDataAdapter(sql, conn);

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
                    DataRow newRow = DT.NewRow();
                    newRow["group_id"] = -1;
                    newRow["group_name"] = "Все";
                    DT.Rows.InsertAt(newRow, 0);

                    GroupsComboBox.ItemsSource = DT.DefaultView;
                    GroupsComboBox.DisplayMemberPath = "group_name";
                    GroupsComboBox.SelectedValuePath = "group_id";
                    GroupsComboBox.SelectedIndex = 0;
                }
            }
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
                        DataRow newRow = DT.NewRow();
                        newRow["role_id"] = -1;
                        newRow["role_name"] = "Все";
                        DT.Rows.InsertAt(newRow, 0);

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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RolesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();
        }

        private void GroupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();
        }
        
        private void ApplyAllFilters()
        {
            if (UsersTable == null || UsersTable.ItemsSource == null) return;

            DataView dv = UsersTable.ItemsSource as DataView;
            if (dv == null) return;

            List<string> filters = new List<string>();

            if (RolesComboBox.SelectedItem is DataRowView roleRow && RolesComboBox.SelectedIndex > 0)
            {
                string roleName = roleRow["role_name"].ToString();

                if (roleName != "Все")
                {
                    filters.Add($"role_name = '{roleName}'");
                }

                if (roleName == "Студент")
                {
                    GroupsStackPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    GroupsStackPanel.Visibility = Visibility.Collapsed;
                    GroupsComboBox.SelectedIndex = -1; 
                }
            }
            else
            {
                GroupsStackPanel.Visibility = Visibility.Collapsed;
                GroupsComboBox.SelectedIndex = -1;
            }

            if (GroupsComboBox.SelectedItem is DataRowView groupRow && GroupsComboBox.SelectedIndex > 0)
            {
                string groupName = groupRow["group_name"].ToString();
                if (groupName != "Все")
                {
                    string currentRole = RolesComboBox.Text;
                    if (currentRole == "Студент")
                    {
                        filters.Add($"group_name = '{groupName}'");
                    }
                }
            }

            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            DataView dv = UsersTable.ItemsSource as DataView;
            if (dv != null)
            {
                dv.RowFilter = "";
            }

            GroupsComboBox.SelectedIndex = -1;
            RolesComboBox.SelectedIndex = -1;
        }
    }
}

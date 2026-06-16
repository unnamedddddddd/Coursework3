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
    /// Логика взаимодействия для AttendanceRecords.xaml
    /// </summary>
    public partial class ShowAttendanceRecords : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True";

        public ShowAttendanceRecords()
        {
            InitializeComponent();

            LoadTeacherComboBox();
            LoadStudentComboBox();
            LoadSubjectsComboBox();

            ShowAllAttendanceRecords();
        }

        public void ShowAllAttendanceRecords()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        ar.record_id,
                        ar.student_id,
                        u.full_name AS student_name,
                        s.subject_name,
                        ar.status,
                        ar.created_at,
                        ar.updated_at,
                        t.full_name AS teacher_name
                    FROM Attendance_records ar
                    INNER JOIN Users u ON ar.student_id = u.user_id
                    INNER JOIN Teacher_assignments ta ON ar.assignment_id = ta.assignment_id
                    INNER JOIN Subjects s ON ta.subject_id = s.subject_id
                    INNER JOIN Users t ON ta.teacher_id = t.user_id";

                string userRole = CheckUserRole();

                if (userRole == "Студент")
                {
                    sql += " WHERE ar.student_id = @studentId";
                }
                else
                {
                    StackPanelStudentComboBox.Visibility = Visibility.Visible;
                }

                sql += " ORDER BY ar.created_at DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (CheckUserRole() == "Студент")
                    {
                        cmd.Parameters.AddWithValue("@studentId", App.CurrentUserId);
                    }

                    SqlDataAdapter DA = new SqlDataAdapter(cmd);
                    DataTable DT = new DataTable();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        RecordsTable.ItemsSource = DT.DefaultView;
                    }
                    else
                    {
                        MessageBox.Show("Данные не загружены");
                    }
                }

                conn.Close();
            }
        }

        private string CheckUserRole()
        {
            string roleName = "";

            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT 
                        R.role_name 
                    FROM Users U
                    INNER JOIN Roles R ON R.role_id = U.role_id
                    WHERE U.user_id = @userId",
                    conn
                );

                cmd.Parameters.AddWithValue("@userId", App.CurrentUserId);

                object result = cmd.ExecuteScalar(); 

                if (result != null)
                {
                    roleName = result.ToString();
                }

                conn.Close();
            }

            return roleName;
        }

        private void RecordsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void LoadStudentComboBox()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlDataAdapter DA = new SqlDataAdapter(
                    "SELECT * FROM Users WHERE role_id = 5",
                    conn
                );

                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    DataRow newRow = DT.NewRow();
                    newRow["user_id"] = -1;
                    newRow["full_name"] = "Все";
                    DT.Rows.InsertAt(newRow, 0);

                    StudentComboBox.ItemsSource = DT.DefaultView;
                    StudentComboBox.DisplayMemberPath = "full_name";
                    StudentComboBox.SelectedValuePath = "user_id";
                    StudentComboBox.SelectedIndex = 0;
                }
            }
        }

        public void LoadTeacherComboBox()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlDataAdapter DA = new SqlDataAdapter(
                    @"SELECT DISTINCT 
                        U.user_id as teacher_id,
                        U.full_name as teacher_name
                    FROM Users U
                    INNER JOIN Teacher_assignments TA ON TA.teacher_id = U.user_id
                    ORDER BY U.full_name",
                    conn
                );

                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    DataRow newRow = DT.NewRow();
                    newRow["teacher_id"] = -1;
                    newRow["teacher_name"] = "Все";
                    DT.Rows.InsertAt(newRow, 0);

                    TeacherComboBox.ItemsSource = DT.DefaultView;
                    TeacherComboBox.SelectedValuePath = "teacher_id";
                    TeacherComboBox.DisplayMemberPath = "teacher_name";
                    TeacherComboBox.SelectedIndex = 0;
                }
            }
        }

        public void LoadSubjectsComboBox()
        {
            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                string query = @"
                SELECT
                    ta.assignment_id,
                    s.subject_name
                FROM Teacher_assignments ta
                INNER JOIN Subjects s ON ta.subject_id = s.subject_id
                ORDER BY s.subject_name";

                SqlDataAdapter DA = new SqlDataAdapter(query, conn);
                DataTable DT = new DataTable();
                DA.Fill(DT);

                if (DT.Rows.Count > 0)
                {
                    DataRow newRow = DT.NewRow();
                    newRow["assignment_id"] = -1; 
                    newRow["subject_name"] = "Все";
                    DT.Rows.InsertAt(newRow, 0);

                    SubjectComboBox.ItemsSource = DT.DefaultView;
                    SubjectComboBox.DisplayMemberPath = "subject_name";
                    SubjectComboBox.SelectedValuePath = "assignment_id";
                    SubjectComboBox.SelectedIndex = 0;
                }

                conn.Close();
            }
        }

        private void StudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();
        }

        private void SubjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();

        }

        private void TeacherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            if (RecordsTable == null || RecordsTable.ItemsSource == null) return;

            DataView dv = RecordsTable.ItemsSource as DataView;
            if (dv == null) return;

            List<string> filters = new List<string>();

            if (StudentComboBox.SelectedItem is DataRowView studentRow && StudentComboBox.SelectedIndex > 0)
            {
                int studentId = Convert.ToInt32(studentRow["user_id"]);
                if (studentId != -1)
                {
                    filters.Add($"student_id = {studentId}");
                }
            }

            if (SubjectComboBox.SelectedItem is DataRowView subjectRow && SubjectComboBox.SelectedIndex > 0)
            {
                string subjectName = subjectRow["subject_name"].ToString();
                if (subjectName != "Все")
                {
                    filters.Add($"subject_name = '{subjectName}'");
                }
            }

            if (StatusFilterComboBox.SelectedItem is ComboBoxItem statusItem && StatusFilterComboBox.SelectedIndex > 0)
            {
                string status = statusItem.Content.ToString();
                if (status != "Все")
                {
                    filters.Add($"status = '{status}'");
                }
            }

            if (TeacherComboBox.SelectedItem is DataRowView teacherRow && TeacherComboBox.SelectedIndex > 0)
            {
                string teacherName = teacherRow["teacher_name"].ToString();
                if (teacherName != "Все")
                {
                    filters.Add($"teacher_name = '{teacherName}'");
                }
            }

            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            DataView dv = RecordsTable.ItemsSource as DataView;
            if (dv != null)
            {
                dv.RowFilter = "";
            }

            StudentComboBox.SelectedIndex = -1;
            SubjectComboBox.SelectedIndex = -1;
            StatusFilterComboBox.SelectedIndex = 0;
            TeacherComboBox.SelectedIndex = -1;
        }
    }
}

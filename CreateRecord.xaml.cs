using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
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
    /// Логика взаимодействия для CreateRecord.xaml
    /// </summary>
    public partial class CreateRecord : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True"; // public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Курсовая 3 курс\\Course DB.mdf\";Integrated Security=True"; 

        public CreateRecord()
        {
            InitializeComponent();

            LoadStudentComboBox();
            LoadSubjectsComboBox();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int studentId = Convert.ToInt32(StudentComboBox.SelectedValue);
            int assignmentId = Convert.ToInt32(SubjectComboBox.SelectedValue);
            string status = ((ComboBoxItem)StatusComboBox.SelectedItem).Tag.ToString();
            var createdAt = DateTimePicker.Value;

            if (StudentComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите студента");
                return;
            }

            if (SubjectComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите предмет");
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус посещения");
                return;
            }

            if (DateTimePicker.Value == null)
            {
                MessageBox.Show("Выберите дату и время");
                return;
            }

            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Статус не определён");
                return;
            }

            using (var conn = new SqlConnection(ConnString))
            {
                conn.Open();

                string sql = "INSERT INTO Attendance_records (student_id, assignment_id, status, created_at) VALUES(@studentId, @assignmentId, @status, @createdAt)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@createdAt", createdAt);

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }

            MessageBox.Show("Запись создана");

            StudentComboBox.SelectedIndex = 0;
            SubjectComboBox.SelectedIndex = 0;
            StatusComboBox.SelectedIndex = 0;
            DateTimePicker.Value = DateTime.Now;
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
                    StudentComboBox.ItemsSource = DT.DefaultView;
                    StudentComboBox.DisplayMemberPath = "full_name";
                    StudentComboBox.SelectedValuePath = "user_id";
                    StudentComboBox.SelectedIndex = 0;
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
                    SubjectComboBox.ItemsSource = DT.DefaultView;
                    SubjectComboBox.DisplayMemberPath = "subject_name"; 
                    SubjectComboBox.SelectedValuePath = "assignment_id"; 
                    SubjectComboBox.SelectedIndex = 0;
                }

                conn.Close();
            }
        }
    }
}

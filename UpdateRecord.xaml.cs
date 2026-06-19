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
using Xceed.Wpf.Toolkit;
using static System.Net.Mime.MediaTypeNames;
using MsgBox = System.Windows.MessageBox;

namespace Курсовая_3_курс
{
    /// <summary>
    /// Логика взаимодействия для UpdateRecord.xaml
    /// </summary>
    public partial class UpdateRecord : UserControl
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Курсовая 3 курс\\Course DB.mdf\";Integrated Security=True";

        public UpdateRecord()
        {
            InitializeComponent();
            LoadAttedanceRecordDataGrid();
            LoadStudentComboBox();
            LoadSubjectsComboBox();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordIdText.Text == "")
            {
                MsgBox.Show("Выберите запись");
                return;
            }

            try
            {
                int recordId = Convert.ToInt32(RecordIdText.Text);
                int studentId = Convert.ToInt32(StudentComboBox.SelectedValue);
                int assignmentId = Convert.ToInt32(SubjectComboBox.SelectedValue);
                string status = StatusComboBox.Text;

                DateTime? newDate = DateTimePicker.Value;

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    string sql = @"UPDATE Attendance_records
                                    SET student_id = @studentId, 
                                        assignment_id = @assignmentId, 
                                        status = @status, 
                                        created_at = @newDate,
                                        updated_at = GETDATE()
                                    WHERE record_id = @recordId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                        cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@newDate", newDate);
                        cmd.Parameters.AddWithValue("@recordId", recordId);

                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
                MsgBox.Show("Запись обновлена");
            }
            catch (Exception ex) {
                MsgBox.Show(ex.Message);
            }
        }

        private void StudentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RecordsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecordsGrid.SelectedItem == null) return;

            DataRowView selectedRow = (DataRowView)RecordsGrid.SelectedItem;

            RecordIdText.Text = Convert.ToInt32(selectedRow["record_id"]).ToString(); 
            CurrentStudentText.Text = selectedRow["student_name"].ToString();
            CurrentSubjectText.Text = selectedRow["subject_name"].ToString();
            CurrentStatusText.Text = selectedRow["status"].ToString();
            CurrentDateTime.Text = selectedRow["created_at"].ToString();

            StudentComboBox.SelectedValue = Convert.ToInt32(selectedRow["student_id"]);
            SubjectComboBox.SelectedValue = Convert.ToInt32(selectedRow["assignment_id"]);

            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Tag.ToString() == CurrentStatusText.Text) 
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }

            DateTime oldCreatedDate = Convert.ToDateTime(selectedRow["created_at"]);

            DateTimePicker.Value = oldCreatedDate;
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
    

        private void LoadAttedanceRecordDataGrid()
        {
            try
            {
                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    string sql = @"
                    SELECT 
                        ar.record_id,
                        ar.student_id,       
                        ar.assignment_id,
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
                    INNER JOIN Users t ON ta.teacher_id = t.user_id
                    ORDER BY ar.created_at DESC";

                    SqlDataAdapter DA = new SqlDataAdapter(sql, conn);

                    DataTable DT = new DataTable();
                    DA.Fill(DT);

                    if (DT.Rows.Count > 0)
                    {
                        RecordsGrid.ItemsSource = DT.DefaultView;
                    }
                    else
                    {
                        throw new Exception("Данные не загружены");
                    }

                    conn.Close();
                }
            }
            catch (Exception ex) {
                MsgBox.Show(ex.Message);
            }
        }
    }
}

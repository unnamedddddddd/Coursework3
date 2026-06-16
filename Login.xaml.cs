using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Курсовая_3_курс
{
    public partial class Login : Page
    {
        public string ConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\denis\\source\\repos\\Coursework3\\Course DB.mdf\";Integrated Security=True;Encrypt=True";

        public Login()
        {
            InitializeComponent();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new CreateUser());
        }

        private void Check_Auth_Click(object sender, RoutedEventArgs e)
        {
            string userLogin = login_textBox.Text.Trim();
            string userPassword = password_textBox.Text.Trim();

            using (var connect = new SqlConnection(ConnString))
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT user_id, user_login, user_password, role_id FROM Users WHERE user_login = @login";

                    using (SqlCommand cmd = new SqlCommand(sql, connect))
                    {
                        cmd.Parameters.AddWithValue("@login", userLogin);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["user_password"].ToString();

                                if (BCrypt.Net.BCrypt.Verify(userPassword, storedHash))
                                {
                                    int userId = Convert.ToInt32(reader["user_id"]);
                                    string userRole = reader["role_id"].ToString();

                                    App.CurrentUserId = userId;

                                    if (userRole == "7")  
                                    {
                                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                                        mainWindow.MainFrame.Navigate(new AdminPanel());
                                    }
                                    else if (userRole == "6")
                                    {
                                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                                        mainWindow.MainFrame.Navigate(new HomeTeacher());
                                    }
                                    else
                                    {
                                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                                        mainWindow.MainFrame.Navigate(new Home());
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Неверный пароль");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Пользователь не найден");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
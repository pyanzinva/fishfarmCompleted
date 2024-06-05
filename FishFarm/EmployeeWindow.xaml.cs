using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace FishFarm
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow()
        {
            InitializeComponent();
        }

        private string connectionString = @"Data Source=DESKTOP-VINGOMG;Initial Catalog=рыбы;Integrated Security=True;TrustServerCertificate=True";
        private void btnДобавить_Click(object sender, RoutedEventArgs e)
        {
            string имя = txtИмя.Text;
            string фамилия = txtФамилия.Text;
            string должность = txtДолжность.Text;
            string query = $"INSERT INTO Сотрудники (имя, фамилия, должность) VALUES ('{имя}', '{фамилия}', '{должность}')";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Сотрудник успешно добавлен!"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}"); 
            }
        }


    }

}

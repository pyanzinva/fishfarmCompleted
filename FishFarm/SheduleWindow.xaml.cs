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
using FishFarm;

namespace FishFarm
{
    /// <summary>
    /// Логика взаимодействия для SheduleWindow.xaml
    /// </summary>
    public partial class SheduleWindow
    {

        private string connectionString = @"Data Source=DESKTOP-VINGOMG;Initial Catalog=рыбы;Integrated Security=True;TrustServerCertificate=True";
        public SheduleWindow()
        {
            InitializeComponent();
        }

        private void btnДобавить_Click(object sender, RoutedEventArgs e)
        {
            string idМероприятия = txtIDМероприятия.Text;
            string idСотрудника = txtIDСотрудника.Text;

            string query = $"INSERT INTO Расписание (ID_мероприятие, ID_сотрудник) VALUES ('{idМероприятия}', '{idСотрудника}')";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Запись успешно добавлена!"); // Вывод сообщения об успешном добавлении
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}"); // Вывод сообщения об ошибке
            }
        }

    }
}

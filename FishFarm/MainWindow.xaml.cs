using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FishFarm
{
    public partial class MainWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-VINGOMG;Initial Catalog=рыбы;Integrated Security=True;TrustServerCertificate=True";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void comboTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
            UpdateAddPanel(selectedTable);
            LoadData();
        }

        private void UpdateAddPanel(string selectedTable)
        {
            addPanel.Children.Clear();

            if (string.IsNullOrEmpty(selectedTable))
                return;

            switch (selectedTable)
            {
                case "Сотрудники":
                    addPanel.Children.Add(CreateTextBox("txtИмя", "Имя"));
                    addPanel.Children.Add(CreateTextBox("txtФамилия", "Фамилия"));
                    addPanel.Children.Add(CreateTextBox("txtДолжность", "Должность"));
                    break;
                case "Расписание":
                    addPanel.Children.Add(CreateTextBox("txtID_мероприятие", "ID Мероприятия"));
                    addPanel.Children.Add(CreateTextBox("txtID_сотрудник", "ID Сотрудника"));
                    break;
                default:
                    break;
            }

            // Отладочный вывод значения txtID.Text
            foreach (var child in addPanel.Children)
            {
                if (child is TextBox textBox)
                {
                    textBox.KeyDown += TextBox_KeyDown;
                    if (textBox.Name == "txtID")
                    {
                        MessageBox.Show($"Инициализация txtID: {textBox.Text}");
                    }
                }
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedTable))
                {
                    MessageBox.Show("Выберите таблицу.");
                    return;
                }

                Dictionary<string, string> fieldValues = new Dictionary<string, string>();

                foreach (var child in addPanel.Children)
                {
                    if (child is TextBox textBox)
                    {
                        if (textBox.Foreground != Brushes.Gray)
                        {
                            string fieldName = textBox.Name.Replace("txt", string.Empty);
                            string fieldValue = textBox.Text;
                            fieldValues.Add(fieldName, fieldValue);
                        }
                    }
                }

                InsertRecord(selectedTable, fieldValues);
                LoadData();
            }
        }

        private TextBox CreateTextBox(string name, string placeholder)
        {
            return new TextBox
            {
                Name = name,
                Width = 200,
                Margin = new Thickness(0, 0, 0, 10),
                Text = placeholder,
                Foreground = Brushes.Gray
            };
        }

        private void LoadData()
        {
            var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedTable))
                return;

            string query;

            if (selectedTable == "Расписание")
            {
                query = @"SELECT Расписание.ID_расписание, 
                                 Сотрудники.фамилия + ' ' + Сотрудники.имя + ' '  AS Сотрудник,
                                 Мероприятия.тип_мероприятия AS Мероприятие
                          FROM Расписание
                          JOIN Сотрудники ON Расписание.ID_сотрудник = Сотрудники.ID_сотрудник
                          JOIN Мероприятия ON Расписание.ID_мероприятие = Мероприятия.ID_мероприятие";
            }
            else
            {
                query = $"SELECT * FROM {selectedTable}";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void InsertRecord(string tableName, Dictionary<string, string> fieldValues)
        {
            try
            {
                List<string> columns = new List<string>();
                List<string> values = new List<string>();
                List<SqlParameter> parameters = new List<SqlParameter>();

                foreach (var field in fieldValues)
                {
                    columns.Add(field.Key);
                    values.Add($"@{field.Key}");
                    parameters.Add(new SqlParameter($"@{field.Key}", field.Value));
                }

                string query = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        private void btnУдалить_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedTable))
                {
                    MessageBox.Show("Пожалуйста, выберите таблицу.");
                    return;
                }

                // Отладочный вывод значения txtID.Text
                MessageBox.Show($"Введенный ID: {txtID.Text}");

                if (!int.TryParse(txtID.Text, out int id))
                {
                    MessageBox.Show("Пожалуйста, введите корректный ID (числовое значение).");
                    return;
                }

                string query = $"DELETE FROM {selectedTable} WHERE ID_расписание = @ID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@ID", id));

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    else
                    {
                        MessageBox.Show("Запись с указанным ID не найдена.");
                    }
                }

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
            }
        }


        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedTable))
                    return;

                DataRowView rowView = e.Row.Item as DataRowView;
                if (rowView == null)
                    return;

                DataRow row = rowView.Row;
                string updateQuery = $"UPDATE {selectedTable} SET ";

                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.ColumnName != "ID_расписание")
                    {
                        updateQuery += $"{column.ColumnName} = @{column.ColumnName}, ";
                        parameters.Add(new SqlParameter($"@{column.ColumnName}", row[column.ColumnName]));
                    }
                }

                updateQuery = updateQuery.TrimEnd(',', ' ');
                updateQuery += " WHERE ID_расписание = @ID";
                parameters.Add(new SqlParameter("@ID", row["ID_расписание"]));

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
            }
        }

        private void btnОбновить_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }

        private void btnДобавить_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTable = (comboTables.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (string.IsNullOrEmpty(selectedTable))
                    return;

                Dictionary<string, string> fieldValues = new Dictionary<string, string>();

                foreach (var child in addPanel.Children)
                {
                    if (child is TextBox textBox)
                    {
                        if (textBox.Foreground != System.Windows.Media.Brushes.Gray)
                        {
                            string fieldName = textBox.Name.Replace("txt", string.Empty);
                            string fieldValue = textBox.Text;
                            fieldValues.Add(fieldName, fieldValue);
                        }
                    }
                }

                if (selectedTable == "Расписание")
                {
                    // Вставляем запись в таблицу "Расписание" без ID_расписание
                    InsertRecord(selectedTable, fieldValues);
                }
                else
                {
                    // Вставляем запись в другие таблицы
                    InsertRecord(selectedTable, fieldValues);
                }

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
            }
        }


    }
}

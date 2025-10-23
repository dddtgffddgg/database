using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace fond
{
    public partial class Donations : Form
    {
        public Donations()
        {
            InitializeComponent();

            DGV1();
        }

        static string DBconnection = "server = 127.0.0.1; user = root; password = diana@Bakieva_1304; database = fond";
        static public MySqlDataAdapter msDataAdapter;
        static MySqlConnection myconnect;
        static public MySqlCommand msCommand;

        public static bool ConnectionDB()
        {
            try
            {
                myconnect = new MySqlConnection(DBconnection);
                myconnect.Open();
                msCommand = new MySqlCommand();
                msCommand.Connection = myconnect;
                msDataAdapter = new MySqlDataAdapter(msCommand);
                return true;
            }
            catch
            {
                MessageBox.Show("Ошибка соединения с базой данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void CloseDB()
        {
            myconnect.Close();
        }

        public MySqlConnection getConnection()
        {
            return myconnect;
        }

        private void DGV1()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"SELECT 
                    fond.donations.idDonations,
                    fond.donations.Sum,
                    fond.needy.idNeedy,
                    fond.needy.surname,
                    fond.needy.name,
                    fond.needy.patronymic,
                    fond.sacrificing.idSacrificing,
                    fond.sacrificing.surname,
                    fond.sacrificing.name,
                    fond.sacrificing.patronymic,
                    fond.donations.donations_date
                FROM 
                    fond.donations
                JOIN 
                    fond.needy ON fond.donations.idNeedy_fk = fond.needy.idNeedy
                JOIN 
                    fond.sacrificing ON fond.donations.idSacrificing_fk = fond.sacrificing.idSacrificing;";
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, myconnect);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    CloseDB();
                }

                ColumnName1();
                ComboboxChoosing1();
                ComboboxChoosing2();
            }
        }

        private void InsertDonation(string needyId, string sacrificingId, int sum, DateTime donationDate)
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "INSERT INTO fond.donations (idNeedy_fk, idSacrificing_fk, Sum, donations_date) VALUES (@idNeedy, @idSacrificing, @Sum, @donations_date)";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@idNeedy", needyId);
                    cmd.Parameters.AddWithValue("@idSacrificing", sacrificingId);
                    cmd.Parameters.AddWithValue("@Sum", sum);
                    cmd.Parameters.AddWithValue("@donations_date", donationDate);
                    
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить запись.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
        }

        private string GetNeedyId(string fullName)
        {
            try
            {
                string[] nameParts = fullName.Split(' ');
                string surname = nameParts[0];
                string name = nameParts[1];
                string patronymic = nameParts[2];

                if (ConnectionDB())
                {
                    string query = "SELECT idNeedy FROM fond.needy WHERE surname = @surname AND name = @name AND patronymic = @patronymic";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@surname", surname);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@patronymic", patronymic);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении ID: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }

            return null;
        }

        private string GetSacrificingId(string fullName)
        {
            try
            {
                string[] nameParts = fullName.Split(' ');
                string surname = nameParts[0];
                string name = nameParts[1];
                string patronymic = nameParts[2];

                if (ConnectionDB())
                {
                    string query = "SELECT idSacrificing FROM fond.sacrificing WHERE surname = @surname AND name = @name AND patronymic = @patronymic";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@surname", surname);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@patronymic", patronymic);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении ID: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }

            return null;
        }
        public void ColumnName1()
        {
            // Затем устанавливаем названия и порядок столбцов
            dataGridView1.Columns[0].HeaderText = "ID";
            dataGridView1.Columns[1].HeaderText = "Сумма";
            dataGridView1.Columns[3].HeaderText = "Фамилия Получателя";
            dataGridView1.Columns[4].HeaderText = "Имя Получателя";
            dataGridView1.Columns[5].HeaderText = "Отчество Получателя";
            dataGridView1.Columns[7].HeaderText = "Фамилия Благотворителя";
            dataGridView1.Columns[8].HeaderText = "Имя Благотворителя";
            dataGridView1.Columns[9].HeaderText = "Отчество Благотворителя";
            dataGridView1.Columns[10].HeaderText = "Дата платежа";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[6].Visible = false;

            // Устанавливаем окончательный порядок
            dataGridView1.Columns[1].DisplayIndex = 9;
            dataGridView1.Columns[2].DisplayIndex = 1;
            dataGridView1.Columns[3].DisplayIndex = 2;
            dataGridView1.Columns[4].DisplayIndex = 3;
            dataGridView1.Columns[5].DisplayIndex = 4;
            dataGridView1.Columns[6].DisplayIndex = 5;
            dataGridView1.Columns[7].DisplayIndex = 6;
            dataGridView1.Columns[8].DisplayIndex = 7;
            dataGridView1.Columns[9].DisplayIndex = 8;
        }

        private void ComboboxChoosing1()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.needy";
                myconnect.Open();
                MySqlCommand msCommnd = new MySqlCommand(selectQuery, myconnect);
                MySqlDataReader msDataReader = msCommnd.ExecuteReader();

                comboBox1.Items.Clear();
                
                while (msDataReader.Read())
                {
                    string fullName = $"{msDataReader.GetString("surname")} {msDataReader.GetString("name")} {msDataReader.GetString("patronymic")}";
                    comboBox1.Items.Add(fullName);
                }

                msDataReader.Close();
                myconnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных в ComboBox: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }
        private void ComboboxChoosing2()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.sacrificing";
                myconnect.Open();
                MySqlCommand msCommnd = new MySqlCommand(selectQuery, myconnect);
                MySqlDataReader msDataReader = msCommnd.ExecuteReader();

                comboBox2.Items.Clear();

                while (msDataReader.Read())
                {
                    string fullName = $"{msDataReader.GetString("surname")} {msDataReader.GetString("name")} {msDataReader.GetString("patronymic")}";
                    comboBox2.Items.Add(fullName);
                }

                msDataReader.Close();
                myconnect.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных в ComboBox: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Предполагается, что ComboBox содержит полное имя, но вам нужно получить соответствующий ID из базы данных
                string selectedNeedy = comboBox1.SelectedItem.ToString();
                string selectedSacrificing = comboBox2.SelectedItem.ToString();
                int sum = int.Parse(textBox1.Text); // Предполагается, что у вас есть TextBox для ввода суммы
                DateTime donationDate = dateTimePicker1.Value; // Получение выбранной даты


                // Получить ID для selectedNeedy и selectedSacrificing
                string needyId = GetNeedyId(selectedNeedy);
                string sacrificingId = GetSacrificingId(selectedSacrificing);

                // Вставить данные в базу данных
                if (!string.IsNullOrEmpty(needyId) && !string.IsNullOrEmpty(sacrificingId))
                {
                    InsertDonation(needyId, sacrificingId, sum, donationDate);
                }
                else
                {
                    MessageBox.Show("Не удалось найти соответствующие записи для нуждающегося или жертвователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Обновить DataGridView
                DGV1();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Отчет отчет = new Отчет();
            отчет.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) &&
            !string.IsNullOrEmpty(comboBox2.Text) &&
            !string.IsNullOrEmpty(textBox1.Text) &&
            dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Проверка, открыто ли соединение
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Получаем idDonations из выбранной строки DataGridView
                    int idDonations = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idDonations"].Value);

                    // Создание команды для обновления данных
                    MySqlCommand msCommand = new MySqlCommand("UPDATE fond.donations SET idNeedy_fk=@idNeedy_fk, idSacrificing_fk=@idSacrificing_fk, Sum=@Sum, donations_date=@donations_date WHERE idDonations=@idDonations", myconnect);

                    // Получение ID выбранного нуждающегося и благотворителя
                    string needyId = GetNeedyId(comboBox1.Text);
                    string sacrificingId = GetSacrificingId(comboBox2.Text);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idNeedy_fk", needyId);
                    msCommand.Parameters.AddWithValue("@idSacrificing_fk", sacrificingId);
                    msCommand.Parameters.AddWithValue("@Sum", textBox1.Text);
                    msCommand.Parameters.AddWithValue("@donations_date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    msCommand.Parameters.AddWithValue("@idDonations", idDonations);

                    // Выполнение команды
                    msCommand.ExecuteNonQuery();

                    // Показ сообщения об успешном обновлении
                    MessageBox.Show("Запись успешно изменена!", "UPDATE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Обновление данных и очистка текстовых полей
                    DisplayData();
                    ClearData();
                }
                catch (MySqlException sqlEx)
                {
                    // Обработка исключений базы данных
                    MessageBox.Show("Ошибка базы данных: " + sqlEx.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Обработка других исключений
                    MessageBox.Show("Общая ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Закрытие соединения
                    myconnect.Close();
                }
            }
            else
            {
                // Показ сообщения, если не все поля заполнены
                MessageBox.Show("Для изменения выберите запись и заполните все поля.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayData()
        {
            try
            {
                myconnect.Open();
                string query = @"
            SELECT 
                fond.donations.idDonations,
                fond.donations.Sum,
                fond.needy.idNeedy AS idNeedy_fk,
                needy.surname AS NeedySurname,
                needy.name AS NeedyName,
                needy.patronymic AS NeedyPatronymic,
                fond.sacrificing.idSacrificing AS idSacrificing_fk,
                sacrificing.surname AS SacrificingSurname,
                sacrificing.name AS SacrificingName,
                sacrificing.patronymic AS SacrificingPatronymic,
                fond.donations.donations_date
            FROM 
                fond.donations
            JOIN 
                fond.needy ON fond.donations.idNeedy_fk = needy.idNeedy
            JOIN 
                fond.sacrificing ON fond.donations.idSacrificing_fk = sacrificing.idSacrificing;
        ";
                DataTable dt = new DataTable();
                msDataAdapter = new MySqlDataAdapter(query, myconnect);
                msDataAdapter.Fill(dt);

                // Установка новых данных
                dataGridView1.DataSource = dt;

                // Установка заголовков столбцов
                ColumnName1();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных в DataGridView: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                myconnect.Close();
            }
        }

        private void ClearData()
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Value = DateTime.Now;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idDonations из выбранной строки DataGridView
                    int idDonations = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idDonations"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для удаления данных
                    MySqlCommand msCommand = new MySqlCommand("DELETE FROM fond.donations WHERE idDonations = @idDonations", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idDonations", idDonations);

                    // Выполнение команды
                    msCommand.ExecuteNonQuery();

                    // Показ сообщения об успешном удалении
                    MessageBox.Show("Запись успешно удалена!", "DELETE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Закрытие соединения
                    myconnect.Close();

                    // Обновление данных в DataGridView
                    DisplayData();
                }
                catch (MySqlException sqlEx)
                {
                    // Обработка исключений базы данных
                    MessageBox.Show("Ошибка базы данных: " + sqlEx.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Обработка других исключений
                    MessageBox.Show("Общая ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Показ сообщения, если не выбрана ни одна запись
                MessageBox.Show("Для удаления выберите запись.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Проверка наличия данных в столбцах
                if (dataGridView1.Rows[e.RowIndex].Cells[3].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[5].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[7].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[8].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[9].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[1].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[10].Value != null)
                {
                    // Полное имя нуждающегося
                    string needyFullName = $"{dataGridView1.Rows[e.RowIndex].Cells[3].Value} {dataGridView1.Rows[e.RowIndex].Cells[4].Value} {dataGridView1.Rows[e.RowIndex].Cells[5].Value}";
                    // Полное имя благотворителя
                    string sacrificingFullName = $"{dataGridView1.Rows[e.RowIndex].Cells[7].Value} {dataGridView1.Rows[e.RowIndex].Cells[8].Value} {dataGridView1.Rows[e.RowIndex].Cells[9].Value}";

                    comboBox1.Text = needyFullName;
                    comboBox2.Text = sacrificingFullName;
                    textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                    // Установка значения даты, если возможно, иначе установка текущей даты
                    string dateString = dataGridView1.Rows[e.RowIndex].Cells[10].Value.ToString();
                    DateTime parsedDate;
                    if (DateTime.TryParse(dateString, out parsedDate))
                    {
                        dateTimePicker1.Value = parsedDate;
                    }
                    else
                    {
                        dateTimePicker1.Value = DateTime.Now;
                    }
                }
                else
                {
                    MessageBox.Show("Некоторые данные отсутствуют в выбранной строке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private int GetMaxDonationSum() //максимальное значение
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "SELECT MAX(Sum) FROM fond.donations";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении максимальной суммы пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
            return 0;
        }
        private void DisplayMaxDonationSumRows()
        {
            try
            {
                int maxSum = GetMaxDonationSum();

                if (ConnectionDB())
                {
                    string query = @"
            SELECT 
                fond.donations.idDonations,
                fond.donations.Sum,
                fond.needy.idNeedy AS idNeedy_fk,
                needy.surname AS NeedySurname,
                needy.name AS NeedyName,
                needy.patronymic AS NeedyPatronymic,
                fond.sacrificing.idSacrificing AS idSacrificing_fk,
                sacrificing.surname AS SacrificingSurname,
                sacrificing.name AS SacrificingName,
                sacrificing.patronymic AS SacrificingPatronymic,
                fond.donations.donations_date
            FROM 
                fond.donations
            JOIN 
                fond.needy ON fond.donations.idNeedy_fk = needy.idNeedy
            JOIN 
                fond.sacrificing ON fond.donations.idSacrificing_fk = sacrificing.idSacrificing
            WHERE 
                fond.donations.Sum = @maxSum";

                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@maxSum", maxSum);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    ColumnName1();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении строк с максимальной суммой пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DisplayMaxDonationSumRows();
        }
        private int GetMinDonationSum()
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "SELECT MIN(Sum) FROM fond.donations";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении минимальной суммы пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
            return 0;
        }
        private void DisplayMinDonationSumRows()
        {
            try
            {
                int minSum = GetMinDonationSum();

                if (ConnectionDB())
                {
                    string query = @"
            SELECT 
                fond.donations.idDonations,
                fond.donations.Sum,
                fond.needy.idNeedy AS idNeedy_fk,
                needy.surname AS NeedySurname,
                needy.name AS NeedyName,
                needy.patronymic AS NeedyPatronymic,
                fond.sacrificing.idSacrificing AS idSacrificing_fk,
                sacrificing.surname AS SacrificingSurname,
                sacrificing.name AS SacrificingName,
                sacrificing.patronymic AS SacrificingPatronymic,
                fond.donations.donations_date
            FROM 
                fond.donations
            JOIN 
                fond.needy ON fond.donations.idNeedy_fk = needy.idNeedy
            JOIN 
                fond.sacrificing ON fond.donations.idSacrificing_fk = sacrificing.idSacrificing
            WHERE 
                fond.donations.Sum = @minSum";

                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@minSum", minSum);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    ColumnName1();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении строк с минимальной суммой пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            DisplayMinDonationSumRows();
        }
        private decimal GetAverageDonationSum()
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "SELECT AVG(Sum) FROM fond.donations";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        return Convert.ToDecimal(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении средней суммы пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
            return 0;
        }
        private void DisplayAverageDonationSumRows()
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = @"
            WITH DonationStats AS (
                SELECT 
                    MIN(fond.donations.Sum) AS MinSum,
                    MAX(fond.donations.Sum) AS MaxSum
                FROM 
                    fond.donations
            )
            SELECT 
                AVG(fond.donations.Sum) AS AverageDonation
            FROM 
                fond.donations
            JOIN 
                fond.needy ON fond.donations.idNeedy_fk = fond.needy.idNeedy
            JOIN 
                fond.sacrificing ON fond.donations.idSacrificing_fk = fond.sacrificing.idSacrificing
            WHERE 
                fond.donations.Sum > (SELECT MinSum FROM DonationStats) 
                AND fond.donations.Sum < (SELECT MaxSum FROM DonationStats);";

                    MySqlCommand cmd = new MySqlCommand(query, myconnect);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    AverageSum();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении строк с средней суммой пожертвования: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
        }

        private void AverageSum()
        {
            dataGridView1.Columns[0].HeaderText = "Средняя сумма";
        }


        private void button9_Click(object sender, EventArgs e)
        {
            DisplayAverageDonationSumRows();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DisplayDonationsInRange(500, 5000);
        }
        private void DisplayDonationsInRange(decimal minAmount, decimal maxAmount) //условие between
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = @"
                SELECT 
                    fond.donations.idDonations,
                    fond.donations.Sum,
                    fond.needy.idNeedy AS idNeedy_fk,
                    needy.surname AS NeedySurname,
                    needy.name AS NeedyName,
                    needy.patronymic AS NeedyPatronymic,
                    fond.sacrificing.idSacrificing AS idSacrificing_fk,
                    sacrificing.surname AS SacrificingSurname,
                    sacrificing.name AS SacrificingName,
                    sacrificing.patronymic AS SacrificingPatronymic,
                    fond.donations.donations_date
                FROM 
                    fond.donations
                JOIN 
                    fond.needy ON fond.donations.idNeedy_fk = needy.idNeedy
                JOIN 
                    fond.sacrificing ON fond.donations.idSacrificing_fk = sacrificing.idSacrificing
                WHERE 
                    fond.donations.Sum BETWEEN @minAmount AND @maxAmount";

                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@minAmount", minAmount);
                    cmd.Parameters.AddWithValue("@maxAmount", maxAmount);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    ColumnName1();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отображении пожертвований в заданном диапазоне сумм: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseDB();
            }
        }

    }
}

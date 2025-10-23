using Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fond
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            DGV();
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

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void DGV()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = "SELECT * FROM fond.volunteer";
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, myconnect);
                    System.Data.DataTable dt = new System.Data.DataTable();
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
            }

            ColumnName1();
        }

        public void ColumnName1()
        {
            dataGridView1.Columns[0].HeaderText = "ID";
            dataGridView1.Columns[1].HeaderText = "Имя";
            dataGridView1.Columns[2].HeaderText = "Фамилия";
            dataGridView1.Columns[3].HeaderText = "Отчество";
            dataGridView1.Columns[4].HeaderText = "Номер телефона";
            dataGridView1.Columns[5].HeaderText = "Почта";
            dataGridView1.Columns[6].HeaderText = "Дата рождения";
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].DisplayIndex = 2;
            dataGridView1.Columns[2].DisplayIndex = 1;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SearchAll();
        }

        private void SearchAll()
        {
            try
            {
                // Открываем соединение
                myconnect.Open();

                // Формируем базовый запрос
                string query = "SELECT * FROM fond.volunteer WHERE 1=1";

                // Добавляем условия поиска на основе заполненных текстовых полей
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    query += " AND name = @name";
                }
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    query += " AND surname = @surname";
                }
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    query += " AND patronymic = @patronymic";
                }

                // Создаем MySqlCommand и задаем параметры
                MySqlCommand command = new MySqlCommand(query, myconnect);
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    command.Parameters.AddWithValue("@name", textBox1.Text);
                }
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    command.Parameters.AddWithValue("@surname", textBox2.Text);
                }
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    command.Parameters.AddWithValue("@patronymic", textBox3.Text);
                }
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    command.Parameters.AddWithValue("@phone_number", maskedTextBox1.Text);
                }
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    command.Parameters.AddWithValue("@email", textBox5.Text);
                }

                // Создаем MySqlDataAdapter с использованием MySqlCommand
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);

                // Заполняем DataTable
                System.Data.DataTable dt = new System.Data.DataTable();
                mySqlDataAdapter.Fill(dt);

                // Привязываем DataTable к DataGridView
                dataGridView1.DataSource = dt;

                // Проверка наличия данных в DataTable
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Такой пользователь не найден.", "Результат поиска", MessageBoxButtons.OK);
                }

                // Закрываем соединение
                myconnect.Close();
            }
            catch
            {
                // Обработка ошибок
                MessageBox.Show("Такой пользователь не найден.", "Ошибка", MessageBoxButtons.OK);
            }
            finally
            {
                // Убедимся, что соединение закрыто
                if (myconnect.State == ConnectionState.Open)
                {
                    myconnect.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(maskedTextBox1.Text) && !string.IsNullOrEmpty(textBox5.Text))
            {
                try
                {
                    msCommand = new MySqlCommand("INSERT INTO fond.volunteer(name, surname, patronymic, phone_number, email) " +
                                                 "VALUES (@name, @surname, @patronymic, @phone_number, @email)", myconnect);
                    myconnect.Open();
                    msCommand.Parameters.AddWithValue("@name", textBox1.Text);
                    msCommand.Parameters.AddWithValue("@surname", textBox2.Text);
                    msCommand.Parameters.AddWithValue("@patronymic", textBox3.Text);
                    msCommand.Parameters.AddWithValue("@phone_number", maskedTextBox1.Text);
                    msCommand.Parameters.AddWithValue("@email", textBox5.Text);
                    msCommand.ExecuteNonQuery();
                    myconnect.Close();
                    MessageBox.Show("Запись успешно добавлена!", "INSERT", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DisplayData();
                    ClearData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении записи: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните всю нужную информацию.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DisplayData()
        {
            myconnect.Open();
            System.Data.DataTable dt = new System.Data.DataTable();
            msDataAdapter = new MySqlDataAdapter("select * from fond.volunteer", myconnect);
            msDataAdapter.Fill(dt);
            dataGridView1.DataSource = dt;
            myconnect.Close();
        }

        private void ClearData()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            maskedTextBox1.Text = "";
            textBox5.Text = "";

        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) &&
            !string.IsNullOrEmpty(textBox2.Text) &&
            !string.IsNullOrEmpty(textBox3.Text) &&
            !string.IsNullOrEmpty(maskedTextBox1.Text) &&
            !string.IsNullOrEmpty(textBox5.Text) &&
            dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idNeedy из выбранной строки DataGridView
                    int idVolunteer = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVolunteer"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для обновления данных
                    MySqlCommand msCommand = new MySqlCommand("UPDATE fond.volunteer SET name=@name, surname=@surname, patronymic=@patronymic, phone_number=@phone_number, email=@email WHERE idVolunteer=@idVolunteer", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@name", textBox1.Text);
                    msCommand.Parameters.AddWithValue("@surname", textBox2.Text);
                    msCommand.Parameters.AddWithValue("@patronymic", textBox3.Text);
                    msCommand.Parameters.AddWithValue("@phone_number", maskedTextBox1.Text);
                    msCommand.Parameters.AddWithValue("@email", textBox5.Text);
                    msCommand.Parameters.AddWithValue("@idVolunteer", idVolunteer);

                    // Выполнение команды
                    msCommand.ExecuteNonQuery();

                    // Показ сообщения об успешном обновлении
                    MessageBox.Show("Запись успешно изменена!", "UPDATE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Обновление данных и очистка текстовых полей
                    myconnect.Close();
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
            }
            else
            {
                // Показ сообщения, если не все поля заполнены
                MessageBox.Show("Для изменения выберите запись и заполните все поля.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            maskedTextBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
            textBox5.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idNeedy из выбранной строки DataGridView
                    int idVolunteer = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVolunteer"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для удаления данных
                    MySqlCommand msCommand = new MySqlCommand("DELETE FROM fond.volunteer WHERE idVolunteer=@idVolunteer", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idVolunteer", idVolunteer);

                    // Выполнение команды
                    msCommand.ExecuteNonQuery();

                    // Показ сообщения об успешном удалении
                    MessageBox.Show("Запись успешно удалена!", "DELETE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Обновление данных в DataGridView
                    myconnect.Close();

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
        public void ColumnNameAnniversaries()
        {
            dataGridView1.Columns[1].HeaderText = "Фамилия"; // surname
            dataGridView1.Columns[2].HeaderText = "Имя"; // name
            dataGridView1.Columns[3].HeaderText = "Отчество"; // patronymic
            dataGridView1.Columns[4].HeaderText = "Номер телефона"; // phone_number
            dataGridView1.Columns[5].HeaderText = "Почта"; // email
            dataGridView1.Columns[6].HeaderText = "Дата рождения"; // birth_date
            dataGridView1.Columns[8].HeaderText = "Дата юбилея"; // anniversaryDate
            dataGridView1.Columns[7].HeaderText = "Возраст"; // Age
            dataGridView1.Columns[0].Visible = false; // idNeedy

            dataGridView1.Columns[2].DisplayIndex = 2; // name
            dataGridView1.Columns[1].DisplayIndex = 1; // surname
            dataGridView1.Columns[3].DisplayIndex = 3; // patronymic
            dataGridView1.Columns[4].DisplayIndex = 4; // phone_number
            dataGridView1.Columns[5].DisplayIndex = 5; // email
            dataGridView1.Columns[6].DisplayIndex = 6; // birth_date
            dataGridView1.Columns[8].DisplayIndex = 7; // anniversaryDate
            dataGridView1.Columns[7].DisplayIndex = 8; // Age
        }
        private void FilterVolunteer()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
                        SELECT 
                            idVolunteer,
                            surname,
                            name,
                            patronymic,
                            phone_number,
                            email,
                            birth_date,
                            YEAR(CURDATE()) - YEAR(birth_date) AS 'Age',
                            DATE_FORMAT(
                                ADDDATE(birth_date, INTERVAL YEAR(CURDATE()) - YEAR(birth_date) YEAR), 
                                '%Y-%m-%d'
                            ) AS anniversaryDate
                        FROM 
                            fond.volunteer
                        ORDER BY 
                            surname ASC, 
                            name ASC, 
                            patronymic ASC, 
                            birth_date ASC;

                        ";

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, myconnect);
                    System.Data.DataTable dt = new System.Data.DataTable();
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

                ColumnNameAnniversaries();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FilterVolunteer();
        }
        private void SearchByName()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
                SELECT * FROM fond.volunteer WHERE surname LIKE 'К%';
            ";

                    MySqlCommand command = new MySqlCommand(query, myconnect);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);
                    System.Data.DataTable dt = new System.Data.DataTable();
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
            }
        }


        private void button9_Click(object sender, EventArgs e)
        {
            SearchByName(); 
        }
        private void SearchByName1()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
                SELECT * FROM fond.volunteer WHERE surname LIKE 'К%_';
            ";

                    MySqlCommand command = new MySqlCommand(query, myconnect);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);
                    System.Data.DataTable dt = new System.Data.DataTable();
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
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SearchByName1();
        }
        private void SearchByName2()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
        SELECT * FROM fond.volunteer WHERE surname LIKE 'К%!_' ESCAPE '!';
    ";

                    MySqlCommand command = new MySqlCommand(query, myconnect);

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);
                    System.Data.DataTable dt = new System.Data.DataTable();
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
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SearchByName2();
        }
    }
}

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
    public partial class Form2 : Form
    {
        public Form2()
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

        private void addRow(string name, string surname, string patronymic, string phone_number, string email)
        {
            String[] row = { name, surname, patronymic, phone_number, email };
            dataGridView1.Rows.Add(row);
        }

        public static void CloseDB()
        {
            myconnect.Close();
        }

        public MySqlConnection getConnection()
        {
            return myconnect;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void DGV1()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = "SELECT * FROM fond.sacrificing";
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

            }

            
        }

        public void ColumnName1()
        {
            dataGridView1.Columns[0].HeaderText = "ID";
            dataGridView1.Columns[1].HeaderText = "Имя";
            dataGridView1.Columns[2].HeaderText = "Фамилия";
            dataGridView1.Columns[3].HeaderText = "Отчество";
            dataGridView1.Columns[4].HeaderText = "Номер телефона";
            dataGridView1.Columns[5].HeaderText = "Почта";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[6].HeaderText = "Город";

            dataGridView1.Columns[1].DisplayIndex = 2;
            dataGridView1.Columns[2].DisplayIndex = 1;

        }


        private void button2_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
            Donations form4 = new Donations();
            form4.Show();
        }

        private void button4_Click(object sender, EventArgs e)
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
                string query = "SELECT * FROM fond.sacrificing WHERE 1=1";

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
                DataTable dt = new DataTable();
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

        private void button5_Click(object sender, EventArgs e)
        {
            MySqlCommand msCommand1 = new MySqlCommand("SELECT * FROM fond.sacrificing WHERE name = @name", myconnect);
            msCommand1.Parameters.AddWithValue("@name", textBox1.Text);

            myconnect.Open();
            bool userExist = false;

            using (var dr1 = msCommand1.ExecuteReader())
            {
                if (userExist = dr1.HasRows)
                    MessageBox.Show("Имя пользователя недоступно!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            myconnect.Close();

            if (!userExist)
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrEmpty(maskedTextBox1.Text) && !string.IsNullOrEmpty(textBox5.Text))
                {
                    try
                    {

                        msCommand = new MySqlCommand("INSERT INTO fond.sacrificing(name, surname, patronymic, phone_number, email) " +
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
        }
        private void DisplayData()
        {
            myconnect.Open();
            DataTable dt = new DataTable();
            msDataAdapter = new MySqlDataAdapter("select * from fond.sacrificing", myconnect);
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
                    int idSacrificing = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idSacrificing"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для обновления данных
                    MySqlCommand msCommand = new MySqlCommand("UPDATE fond.sacrificing SET name=@name, surname=@surname, patronymic=@patronymic, phone_number=@phone_number, email=@email WHERE idSacrificing=@idSacrificing", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@name", textBox1.Text);
                    msCommand.Parameters.AddWithValue("@surname", textBox2.Text);
                    msCommand.Parameters.AddWithValue("@patronymic", textBox3.Text);
                    msCommand.Parameters.AddWithValue("@phone_number", maskedTextBox1.Text);
                    msCommand.Parameters.AddWithValue("@email", textBox5.Text);
                    msCommand.Parameters.AddWithValue("@idSacrificing", idSacrificing);

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

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idNeedy из выбранной строки DataGridView
                    int idSacrificing = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idSacrificing"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для удаления данных
                    MySqlCommand msCommand = new MySqlCommand("DELETE FROM fond.sacrificing WHERE idSacrificing=@idSacrificing", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idSacrificing", idSacrificing);

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
        private void NotNullFunction()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
                        SELECT idSacrificing, name, surname, patronymic, phone_number, email, city
                        FROM fond.sacrificing
                        WHERE phone_number IS NOT NULL
                        ORDER BY surname, name, patronymic;
                    ";

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
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            NotNullFunction();
        }
    }
}

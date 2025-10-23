using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.Office.Interop.Word;



namespace fond
{
    
    public partial class Form7 : Form
    {
        private WordExporter wordExporter;
        public Form7()
        {
            InitializeComponent();

            wordExporter = new WordExporter();

            //this.button5.Click += new System.EventHandler(this.button5_Click);

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
                    string query = @"
            SELECT 
                fond.volunteer_help.idVolunteer_help,
                fond.needy.idNeedy,
                fond.needy.surname,
                fond.needy.name,
                fond.needy.patronymic,
                fond.volunteer.idVolunteer,
                fond.volunteer.surname AS volunteer_surname,
                fond.volunteer.name AS volunteer_name,
                fond.volunteer.patronymic AS volunteer_patronymic,
                fond.help_directory.idHelp_director,
                fond.help_directory.help_title
            FROM 
                fond.volunteer_help
            JOIN 
                fond.needy ON fond.volunteer_help.idNeedy_fk = fond.needy.idNeedy
            JOIN 
                fond.volunteer ON fond.volunteer_help.idVolunteer_fk = fond.volunteer.idVolunteer
            JOIN 
                fond.help_directory ON fond.volunteer_help.idHelp_director_fk = fond.help_directory.idHelp_director
            ORDER BY 
                fond.needy.surname ASC, fond.needy.name ASC, fond.needy.patronymic ASC;";
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

                ColumnName1();
                ComboboxChoosing1();
                ComboboxChoosing2();
                ComboboxChoosing3();
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

        private string GetVolunteerId(string fullName)
        {
            try
            {
                string[] nameParts = fullName.Split(' ');
                string surname = nameParts[0];
                string name = nameParts[1];
                string patronymic = nameParts[2];

                if (ConnectionDB())
                {
                    string query = "SELECT idVolunteer FROM fond.volunteer WHERE surname = @surname AND name = @name AND patronymic = @patronymic";
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
        private string GetHelpDirectoryId(string help_title)
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "SELECT idHelp_director FROM fond.help_directory WHERE help_title = @help_title";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@help_title", help_title);
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

        private void InsertDonation(string needyId, string volunteerId, string help_directory_id)
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "INSERT INTO fond.volunteer_help (idNeedy_fk, idVolunteer_fk, idHelp_director_fk) VALUES (@idNeedy, @idVolunteer, @idHelp_directory)";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@idNeedy", needyId);
                    cmd.Parameters.AddWithValue("@idVolunteer", volunteerId);
                    cmd.Parameters.AddWithValue("@idHelp_directory", help_directory_id);

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

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void ComboboxChoosing1()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.needy";
                myconnect.Open();
                MySqlCommand msCommnd = new MySqlCommand(selectQuery, myconnect);
                MySqlDataReader msDataReader = msCommnd.ExecuteReader();

                comboBox3.Items.Clear();

                while (msDataReader.Read())
                {
                    string fullName = $"{msDataReader.GetString("surname")} {msDataReader.GetString("name")} {msDataReader.GetString("patronymic")}";
                    comboBox3.Items.Add(fullName);
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
                string selectQuery = "SELECT * FROM fond.volunteer";
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

        private void ComboboxChoosing3()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.help_directory";
                myconnect.Open();
                MySqlCommand msCommnd = new MySqlCommand(selectQuery, myconnect);
                MySqlDataReader msDataReader = msCommnd.ExecuteReader();

                comboBox1.Items.Clear();

                while (msDataReader.Read())
                {
                    string fullName = $"{msDataReader.GetString("help_title")}";
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
        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }

        public void ColumnName1()
        {
            dataGridView1.Columns[0].HeaderText = "ID";
            dataGridView1.Columns[2].HeaderText = "Фамилия Получателя";
            dataGridView1.Columns[3].HeaderText = "Имя Получателя";
            dataGridView1.Columns[4].HeaderText = "Отчество Получателя";
            dataGridView1.Columns[6].HeaderText = "Фамилия Волонтера";
            dataGridView1.Columns[7].HeaderText = "Имя Волонтера";
            dataGridView1.Columns[8].HeaderText = "Отчество Волонтера";
            dataGridView1.Columns[9].HeaderText = "ID Помощи";
            dataGridView1.Columns[10].HeaderText = "Название помощи";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[9].Visible = false;
            dataGridView1.Columns[1].HeaderText = "ID Получателя";
            dataGridView1.Columns[5].HeaderText = "ID Волонтера";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearData();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedItem != null && comboBox1.SelectedItem != null && comboBox1.SelectedItem != null)
                {
                    // Получить выбранные значения из comboBox2 и comboBox1
                    string selectedNeedy = comboBox3.SelectedItem.ToString();
                    string selectedVolunteer = comboBox2.SelectedItem.ToString();
                    string selectedHelpDirectory = comboBox1.SelectedItem.ToString();

                    // Получить ID для выбранных значения из базы данных
                    string needyId = GetNeedyId(selectedNeedy);
                    string volunteerId = GetVolunteerId(selectedVolunteer);
                    string help_directory_id = GetHelpDirectoryId(selectedHelpDirectory);

                    // Вставить данные в базу данных
                    if (!string.IsNullOrEmpty(needyId) && !string.IsNullOrEmpty(volunteerId) && !string.IsNullOrEmpty(help_directory_id))
                    {
                        InsertDonation(needyId, volunteerId, help_directory_id);

                        // Успешно добавили запись, обновим ListView
                        DGV1();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти соответствующие записи для нуждающегося или диагноза.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите болеющего и диагноз.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayData()
        {
            try
            {
                if (myconnect.State != ConnectionState.Open)
                {
                    myconnect.Open();
                }

                System.Data.DataTable dt = new System.Data.DataTable();
                msDataAdapter = new MySqlDataAdapter("SELECT * FROM fond.volunteer_help", myconnect);
                msDataAdapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                myconnect.Close();
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idDonations из выбранной строки DataGridView
                    int idVolunteer_help = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVolunteer_help"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для удаления данных
                    MySqlCommand msCommand = new MySqlCommand("DELETE FROM fond.volunteer_help WHERE idVolunteer_help = @idVolunteer_help", myconnect);

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idVolunteer_help", idVolunteer_help);

                    // Выполнение команды
                    msCommand.ExecuteNonQuery();

                    // Показ сообщения об успешном удалении
                    MessageBox.Show("Запись успешно удалена!", "DELETE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Закрытие соединения
                    myconnect.Close();

                    // Обновление данных в DataGridView
                    DGV1();
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
        private void button4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text) &&
        !string.IsNullOrEmpty(comboBox2.Text) &&
        !string.IsNullOrEmpty(comboBox3.Text) &&
        dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    // Получаем idVolunteer_help из выбранной строки DataGridView
                    int idVolunteer_help = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVolunteer_help"].Value);

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создание команды для обновления данных
                    MySqlCommand msCommand = new MySqlCommand(@"UPDATE fond.volunteer_help 
                                                        SET idNeedy_fk = @idNeedy_fk, 
                                                            idVolunteer_fk = @idVolunteer_fk, 
                                                            idHelp_director_fk = @idHelp_director_fk 
                                                        WHERE idVolunteer_help = @idVolunteer_help", myconnect);

                    string selectedNeedy = GetNeedyId(comboBox3.Text); // Вам нужно реализовать GetNeedyId
                    string selectedVolunteer = GetVolunteerId(comboBox2.Text); // Вам нужно реализовать GetVolunteerId
                    string selectedHelpTitle = GetHelpDirectoryId(comboBox1.Text);

                    if (selectedNeedy == null || selectedVolunteer == null)
                    {
                        MessageBox.Show("Не удалось найти указанного нуждающегося или волонтера.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Выйти из метода, чтобы избежать выполнения остального кода
                    }

                    // Добавление параметров
                    msCommand.Parameters.AddWithValue("@idNeedy_fk", selectedNeedy);
                    msCommand.Parameters.AddWithValue("@idVolunteer_fk", selectedVolunteer);
                    msCommand.Parameters.AddWithValue("@idHelp_director_fk", selectedHelpTitle); // Если это необходимо для вашей логики
                    msCommand.Parameters.AddWithValue("@idVolunteer_help", idVolunteer_help);

                    // Выполнение команды
                    int rowsAffected = msCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Показ сообщения об успешном обновлении
                        MessageBox.Show("Запись успешно изменена!", "UPDATE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Обновление данных и очистка текстовых полей
                        DGV1();
                        ClearData();
                    }
                    else
                    {
                        // Если ни одна запись не была обновлена
                        MessageBox.Show("Ошибка: запись не обновлена. Убедитесь, что выбранная запись существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

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
                    // Закрытие соединения, если оно открыто
                    if (myconnect.State == ConnectionState.Open)
                    {
                        myconnect.Close();
                    }
                }
            }
            else
            {
                // Показ сообщения, если не все поля заполнены
                MessageBox.Show("Для изменения выберите запись и заполните все поля.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearData()
        {
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox1.Text = "";
        }
        private void dataGridView1_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Проверка наличия данных в столбцах
                if (dataGridView1.Rows[e.RowIndex].Cells[2].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[3].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[4].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[6].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[7].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[8].Value != null &&
                    dataGridView1.Rows[e.RowIndex].Cells[10].Value != null )
                {
                    // Полное имя нуждающегося
                    string selectedNeedy = $"{dataGridView1.Rows[e.RowIndex].Cells[2].Value} {dataGridView1.Rows[e.RowIndex].Cells[3].Value} {dataGridView1.Rows[e.RowIndex].Cells[4].Value}";
                    // Полное имя благотворителя
                    string selectedVolunteer = $"{dataGridView1.Rows[e.RowIndex].Cells[6].Value} {dataGridView1.Rows[e.RowIndex].Cells[7].Value} {dataGridView1.Rows[e.RowIndex].Cells[8].Value}";

                    comboBox3.Text = selectedNeedy;
                    comboBox2.Text = selectedVolunteer;
                    comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[10].Value.ToString();

                }
                else
                {
                    MessageBox.Show("Некоторые данные отсутствуют в выбранной строке.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string additionalText = ""; 
            int[] excludedColumns = new int[] { 0, 1, 5, 9 };

            wordExporter.ExportToWord(dataGridView1, additionalText, excludedColumns);
        }
        private void SortAndFilterData(string helpTitleToExclude, string helpTitleToInclude)
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
               SELECT 
            vh.idVolunteer_help,
            n.idNeedy,
            n.surname AS needy_surname,
            n.name AS needy_name,
            n.patronymic AS needy_patronymic,
            v.idVolunteer,
            v.surname AS volunteer_surname,
            v.name AS volunteer_name,
            v.patronymic AS volunteer_patronymic,
            hd.idHelp_director,
            hd.help_title
        FROM 
            fond.volunteer_help vh
        JOIN 
            fond.needy n ON vh.idNeedy_fk = n.idNeedy
        JOIN 
            fond.volunteer v ON vh.idVolunteer_fk = v.idVolunteer
        JOIN 
            fond.help_directory hd ON vh.idHelp_director_fk = hd.idHelp_director
        WHERE
            hd.help_title NOT IN (
                SELECT hd2.help_title
                FROM fond.help_directory hd2
                WHERE hd2.help_title = @helpTitleToExclude
            )
        INTERSECT
        SELECT 
            vh.idVolunteer_help,
            n.idNeedy,
            n.surname AS needy_surname,
            n.name AS needy_name,
            n.patronymic AS needy_patronymic,
            v.idVolunteer,
            v.surname AS volunteer_surname,
            v.name AS volunteer_name,
            v.patronymic AS volunteer_patronymic,
            hd.idHelp_director,
            hd.help_title
        FROM 
            fond.volunteer_help vh
        JOIN 
            fond.needy n ON vh.idNeedy_fk = n.idNeedy
        JOIN 
            fond.volunteer v ON vh.idVolunteer_fk = v.idVolunteer
        JOIN 
            fond.help_directory hd ON vh.idHelp_director_fk = hd.idHelp_director
        WHERE
            hd.help_title = @helpTitleToInclude
        ORDER BY 
            needy_surname ASC, needy_name ASC, needy_patronymic ASC,
            volunteer_surname ASC, volunteer_name ASC, volunteer_patronymic ASC;
                    ";

                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, myconnect);
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@helpTitleToExclude", helpTitleToExclude);
                    mySqlDataAdapter.SelectCommand.Parameters.AddWithValue("@helpTitleToInclude", helpTitleToInclude);

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

                ColumnName1();
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            string helpTitleToExclude = comboBox1.SelectedItem?.ToString();
            string helpTitleToInclude = "Название помощи, которое вы хотите включить"; // Необходимо определить или получить этот параметр

            if (!string.IsNullOrEmpty(helpTitleToExclude) && !string.IsNullOrEmpty(helpTitleToInclude))
            {
                SortAndFilterData(helpTitleToInclude, helpTitleToExclude);
            }
            else if (string.IsNullOrEmpty(helpTitleToExclude))
            {
                MessageBox.Show("Пожалуйста, выберите название помощи для исключения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите название помощи для включения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

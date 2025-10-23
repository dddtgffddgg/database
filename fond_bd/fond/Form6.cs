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
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace fond
{
    public partial class Form6 : Form
    {
        public Form6()
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
                    string query = @"
                SELECT
                    needy.surname,
                    needy.name,
                    needy.patronymic,
                    GROUP_CONCAT(diagnosis_directory.Diagnosis SEPARATOR ', ') AS Diagnoses
                FROM 
                    fond.diagnosis_records
                JOIN 
                    fond.needy ON fond.diagnosis_records.idNeedy = fond.needy.idNeedy
                JOIN 
                    fond.diagnosis_directory ON fond.diagnosis_records.idDiagnosis_directory = fond.diagnosis_directory.idDiagnosis_directory
                GROUP BY 
                    needy.surname, needy.name, needy.patronymic";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    listView1.Items.Clear();

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = $"{reader.GetString("surname")} {reader.GetString("name")} {reader.GetString("patronymic")}";
                        item.SubItems.Add(reader.GetString("Diagnoses"));
                        listView1.Items.Add(item);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    CloseDB();
                }

                //ColumnName1();
                ComboboxChoosing1();
                ComboboxChoosing2();
            }
        }
        //public void ColumnName1()
        //{
        //    dataGridView1.Columns[0].HeaderText = "ID";
        //    dataGridView1.Columns[1].HeaderText = "ID Получателя";
        //    dataGridView1.Columns[2].HeaderText = "Фамилия Получателя";
        //    dataGridView1.Columns[3].HeaderText = "Имя Получателя";
        //    dataGridView1.Columns[4].HeaderText = "Отчество Получателя";
        //    dataGridView1.Columns[6].HeaderText = "Диагноз";
            
        //    dataGridView1.Columns[0].Visible = false;
        //    dataGridView1.Columns[1].Visible = false;
        //    dataGridView1.Columns[5].Visible = false;

        //}

        private void ComboboxChoosing1()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.needy";
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

        private void ComboboxChoosing2()
        {
            try
            {
                string selectQuery = "SELECT * FROM fond.diagnosis_directory";
                myconnect.Open();
                MySqlCommand msCommnd = new MySqlCommand(selectQuery, myconnect);
                MySqlDataReader msDataReader = msCommnd.ExecuteReader();

                comboBox1.Items.Clear();

                while (msDataReader.Read())
                {
                    string fullName = $"{msDataReader.GetString("Diagnosis")}";
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

        private string GetDiagnosisDirectoryId(string diagnosis)
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "SELECT idDiagnosis_directory FROM diagnosis_directory WHERE Diagnosis = @Diagnosis";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@Diagnosis", diagnosis);
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

        private void InsertDonation(string needyId, string diagnosisId)
        {
            try
            {
                if (ConnectionDB())
                {
                    string query = "INSERT INTO fond.diagnosis_records (idNeedy, idDiagnosis_directory) VALUES (@idNeedy, @idDiagnosis_directory)";
                    MySqlCommand cmd = new MySqlCommand(query, myconnect);
                    cmd.Parameters.AddWithValue("@idNeedy", needyId);
                    cmd.Parameters.AddWithValue("@idDiagnosis_directory", diagnosisId);

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


        private void Form6_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.SelectedItem != null && comboBox1.SelectedItem != null)
                {
                    // Получить выбранные значения из comboBox2 и comboBox1
                    string selectedNeedy = comboBox2.SelectedItem.ToString();
                    string selectedDiagnosis = comboBox1.SelectedItem.ToString();

                    // Получить ID для выбранных значения из базы данных
                    string needyId = GetNeedyId(selectedNeedy);
                    string diagnosisId = GetDiagnosisDirectoryId(selectedDiagnosis);

                    // Вставить данные в базу данных
                    if (!string.IsNullOrEmpty(needyId) && !string.IsNullOrEmpty(diagnosisId))
                    {
                        InsertDonation(needyId, diagnosisId);

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

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                try
                {
                    // Получаем idNeedy из выбранной строки ListView
                    int idNeedy = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Tag); // Предполагается, что второй подэлемент (SubItem) содержит idNeedy

                    // Открытие соединения
                    if (myconnect.State != ConnectionState.Open)
                    {
                        myconnect.Open();
                    }

                    // Создаем транзакцию для группового удаления
                    MySqlTransaction transaction = myconnect.BeginTransaction();

                    try
                    {
                        // Удаляем записи о диагнозах болеющего из таблицы diagnosis_records
                        MySqlCommand deleteDiagnosesCommand = new MySqlCommand("DELETE FROM fond.diagnosis_records WHERE idNeedy=@idNeedy", myconnect, transaction);
                        deleteDiagnosesCommand.Parameters.AddWithValue("@idNeedy", idNeedy);
                        deleteDiagnosesCommand.ExecuteNonQuery();

                        // Удаляем записи о диагнозах из таблицы diagnosis_directory связанные с удаляемым idNeedy
                        MySqlCommand deleteDiagnosisDirectoryCommand = new MySqlCommand("DELETE FROM fond.diagnosis_directory WHERE idDiagnosis_directory IN (SELECT idDiagnosis_directory FROM fond.diagnosis_records WHERE idNeedy=@idNeedy)", myconnect, transaction);
                        deleteDiagnosisDirectoryCommand.Parameters.AddWithValue("@idNeedy", idNeedy);
                        deleteDiagnosisDirectoryCommand.ExecuteNonQuery();

                        // Подтверждаем транзакцию, если удаление прошло успешно
                        transaction.Commit();

                        // Показываем сообщение об успешном удалении
                        MessageBox.Show("Записи успешно удалены!", "DELETE", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Обновление данных в ListView
                        DisplayData();
                    }
                    catch (Exception ex)
                    {
                        // Откатываем транзакцию в случае возникновения ошибки
                        transaction.Rollback();
                        throw ex;
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
                // Показ сообщения, если не выбрана ни одна запись
                MessageBox.Show("Для удаления выберите запись.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                string query = @"
            SELECT 
                fond.diagnosis_records.idDiagnosis_records,
                needy.surname,
                needy.name,
                needy.patronymic,
                GROUP_CONCAT(diagnosis_directory.Diagnosis SEPARATOR ', ') AS Diagnoses,
                needy.idNeedy
            FROM 
                fond.diagnosis_records
            JOIN 
                fond.needy ON fond.diagnosis_records.idNeedy = fond.needy.idNeedy
            JOIN    
                fond.diagnosis_directory ON fond.diagnosis_records.idDiagnosis_directory = fond.diagnosis_directory.idDiagnosis_directory
            GROUP BY 
                fond.diagnosis_records.idDiagnosis_records, needy.surname, needy.name, needy.patronymic";

                MySqlCommand cmd = new MySqlCommand(query, myconnect);
                MySqlDataReader reader = cmd.ExecuteReader();

                listView1.Items.Clear();

                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem($"{reader["surname"]} {reader["name"]} {reader["patronymic"]}");
                    item.SubItems.Add(reader["Diagnoses"].ToString());
                    item.Tag = reader["idDiagnosis_records"]; // Сохраняем idDiagnosis_records в Tag
                    item.SubItems[1].Tag = reader["idNeedy"]; // Сохраняем idNeedy в Tag второго подэлемента
                    listView1.Items.Add(item);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (myconnect.State == ConnectionState.Open)
                {
                    myconnect.Close();
                }
            }
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                // Предполагается, что вы заполняете ListViewItem в порядке: [FullName, Diagnoses]
                string[] fullNameParts = e.Item.Text.Split(' ');
                if (fullNameParts.Length >= 3)
                {
                    string fullName = $"{fullNameParts[0]} {fullNameParts[1]} {fullNameParts[2]}";
                    comboBox2.Text = fullName; // Полное имя
                }
                comboBox1.Text = e.Item.SubItems[1].Text; // Диагнозы
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

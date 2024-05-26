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

namespace logic_machine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox5.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox6.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox7.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox8.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox9.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox10.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox11.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox12.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox13.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            DGV();

        }

        static string DBconnection = "server = localhost; user = root; password = diana@Bakieva_1304; database = diseases";
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

        private void DGV()
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = "SELECT * FROM diagnosis_symptoms";
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, myconnect);
                    DataTable dt = new DataTable();
                    mySqlDataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].HeaderText = "Диагноз";
                    dataGridView1.Columns[1].HeaderText = "Головная боль";
                    dataGridView1.Columns[2].HeaderText = "Артериальное давление";
                    dataGridView1.Columns[3].HeaderText = "Боль";
                    dataGridView1.Columns[4].HeaderText = "Одышка";
                    dataGridView1.Columns[5].HeaderText = "Нехватка воздуха";
                    dataGridView1.Columns[6].HeaderText = "Потеря сознания";
                    dataGridView1.Columns[7].HeaderText = "Нарушение координации";
                    dataGridView1.Columns[8].HeaderText = "Нарушение сознания";
                    dataGridView1.Columns[9].HeaderText = "Пульс";
                    dataGridView1.Columns[10].HeaderText = "Кашель";
                    dataGridView1.Columns[11].HeaderText = "Онемение";
                    dataGridView1.Columns[12].HeaderText = "Слабость";
                    dataGridView1.Columns[13].HeaderText = "Отеки конечностей";

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("headache", comboBox1.SelectedItem.ToString());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("blood_pressure", comboBox2.SelectedItem.ToString());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("pain", comboBox3.SelectedItem.ToString());
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("dyspnea", comboBox4.SelectedItem.ToString());
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("breathlessness", comboBox12.SelectedItem.ToString());
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("fainting", comboBox11.SelectedItem.ToString());
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("coordination_disorder", comboBox10.SelectedItem.ToString());
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("consciousness_disorder", comboBox9.SelectedItem.ToString());
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("pulse", comboBox8.SelectedItem.ToString());
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("cough", comboBox7.SelectedItem.ToString());
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("numbness", comboBox6.SelectedItem.ToString());
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("weakness", comboBox5.SelectedItem.ToString());
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataGridView("limb_swelling", comboBox13.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FilterDataGridView(string columnName, string filterValue)
        {
            try
            {
                if (dataGridView1.DataSource == null || !(dataGridView1.DataSource is DataTable))
                {
                    MessageBox.Show("Данные для фильтрации отсутствуют.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable dataTable = dataGridView1.DataSource as DataTable;

                if (!dataTable.Columns.Contains(columnName))
                {
                    MessageBox.Show($"Столбец '{columnName}' не найден.", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (filterValue == "Отсутствует")
                {
                    dataTable.DefaultView.RowFilter = $"[{columnName}] IS NULL OR [{columnName}] = ''";
                }
                else
                {
                    filterValue = filterValue.Replace("'", "''");
                    dataTable.DefaultView.RowFilter = $"[{columnName}] = '{filterValue}'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации данных: {ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

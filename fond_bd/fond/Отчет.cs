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
using System.Collections;

namespace fond
{
    public partial class Отчет : Form
    {
        public Отчет()
        {
            InitializeComponent();

            SetupListView();
            SetupLabels();
            SetupListView1();
        }

        static string connectionString = "server = 127.0.0.1; user = root; password = diana@Bakieva_1304; database = fond";
        static public MySqlDataAdapter msDataAdapter;
        static MySqlConnection myconnect;
        static public MySqlCommand msCommand;

        public static bool ConnectionDB()
        {
            try
            {
                myconnect = new MySqlConnection(connectionString);
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

        private void SetupListView()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Благотворитель", 200, HorizontalAlignment.Left); // Ширина столбца "Благотворитель" увеличена до 200
            listView1.Columns.Add("Общая сумма", 100, HorizontalAlignment.Left); // Ширина столбца "Общая сумма" установлена в 100
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
        }


        private void LoadDonationsToListView(DateTime startDate, DateTime endDate)
        {
            if (ConnectionDB())
            {
                try
                {
                    string query = @"
                        SELECT 
                            CONCAT(s.surname, ' ', s.name, ' ', s.patronymic) AS FullName,
                            SUM(d.Sum) AS TotalDonations
                        FROM 
                            fond.donations d
                        JOIN 
                            fond.sacrificing s ON d.idSacrificing_fk = s.idSacrificing
                        WHERE 
                            d.donations_date BETWEEN @startDate AND @endDate
                        GROUP BY 
                            s.idSacrificing, FullName;
                    ";

                    MySqlCommand command = new MySqlCommand(query, myconnect);
                    command.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    listView1.Items.Clear();
                    decimal totalSum = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        var listViewItem = new ListViewItem(row["FullName"].ToString());
                        listViewItem.SubItems.Add(row["TotalDonations"].ToString());
                        listView1.Items.Add(listViewItem);
                        totalSum += Convert.ToDecimal(row["TotalDonations"]);
                    }

                    // Добавление итоговой строки
                    var totalItem = new ListViewItem("Итого");
                    totalItem.SubItems.Add(totalSum.ToString("F2"));
                    listView1.Items.Add(totalItem);
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


        private void Form4_Load(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            LoadDonationsToListView(startDate, endDate);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Donations donations = new Donations();
            donations.Show();
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            LoadDonationsToListView(startDate, endDate);
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;
            LoadDonationsToListView(startDate, endDate);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }
        private void SetupLabels()
        {
            label1.Visible = false;
            label3.Visible = false;
            label6.Visible = false;
        }
        private void LoadDonationCounts()
        {
            if (ConnectionDB())
            {
                try
                {
                    int greaterThan = int.Parse(textBox1.Text);
                    int lessThan = int.Parse(textBox2.Text);

                    string query = $@"
                SELECT 
                    CONCAT('more_than_', @greaterThan) AS Category,
                    CONCAT(fond.sacrificing.surname, ' ', fond.sacrificing.name, ' ', fond.sacrificing.patronymic) AS FullName,
                    COUNT(fond.donations.idDonations) AS DonationCount
                FROM 
                    fond.donations
                JOIN 
                    fond.sacrificing ON fond.donations.idSacrificing_fk = fond.sacrificing.idSacrificing
                GROUP BY 
                    fond.sacrificing.idSacrificing, FullName
                HAVING 
                    DonationCount > @greaterThan

                UNION ALL

                SELECT 
                    CONCAT('less_than_', @lessThan) AS Category,
                    CONCAT(fond.sacrificing.surname, ' ', fond.sacrificing.name, ' ', fond.sacrificing.patronymic) AS FullName,
                    COUNT(fond.donations.idDonations) AS DonationCount
                FROM 
                    fond.donations
                JOIN 
                    fond.sacrificing ON fond.donations.idSacrificing_fk = fond.sacrificing.idSacrificing
                GROUP BY 
                    fond.sacrificing.idSacrificing, FullName
                HAVING 
                    DonationCount < @lessThan

                UNION ALL

                SELECT 
                    CONCAT('between_', @greaterThan, '_and_', @lessThan) AS Category,
                    CONCAT(fond.sacrificing.surname, ' ', fond.sacrificing.name, ' ', fond.sacrificing.patronymic) AS FullName,
                    COUNT(fond.donations.idDonations) AS DonationCount
                FROM 
                    fond.donations
                JOIN 
                    fond.sacrificing ON fond.donations.idSacrificing_fk = fond.sacrificing.idSacrificing
                GROUP BY 
                    fond.sacrificing.idSacrificing, FullName
                HAVING 
                    DonationCount BETWEEN @greaterThan AND @lessThan;
            ";

                    MySqlCommand command = new MySqlCommand(query, myconnect);
                    command.Parameters.AddWithValue("@greaterThan", greaterThan);
                    command.Parameters.AddWithValue("@lessThan", lessThan);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    StringBuilder moreThanBuilder = new StringBuilder($"Больше {greaterThan} раз:\n");
                    StringBuilder lessThanBuilder = new StringBuilder($"Меньше {lessThan} раз:\n");
                    StringBuilder betweenBuilder = new StringBuilder($"Между {greaterThan} и {lessThan} разами:\n");

                    foreach (DataRow row in dt.Rows)
                    {
                        string category = row["Category"].ToString();
                        string fullName = row["FullName"].ToString();

                        if (category.StartsWith("more_than_"))
                        {
                            moreThanBuilder.AppendLine(fullName);
                        }
                        else if (category.StartsWith("less_than_"))
                        {
                            lessThanBuilder.AppendLine(fullName);
                        }
                        else if (category.StartsWith("between_"))
                        {
                            betweenBuilder.AppendLine(fullName);
                        }
                    }

                    label1.Text = moreThanBuilder.ToString();
                    label3.Text = lessThanBuilder.ToString();
                    label6.Text = betweenBuilder.ToString();

                    label1.Visible = true;
                    label3.Visible = true;
                    label6.Visible = true;
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


        private void button1_Click(object sender, EventArgs e)
        {
            LoadDonationCounts();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void SetupListView1()
        {
            // Устанавливаем вид списка в виде таблицы (Details)
            listView2.View = View.Details;

            // Добавляем столбцы с указанием имени, ширины и выравнивания текста
            listView2.Columns.Add("Благотворитель", 200, HorizontalAlignment.Left);
            listView2.Columns.Add("Общая сумма", 100, HorizontalAlignment.Left);
            listView2.Columns.Add("Город", 100, HorizontalAlignment.Left);

            // Необязательно, но полезно при отображении данных
            listView2.FullRowSelect = true; // Выделять всю строку при выборе
            listView2.GridLines = true; // Отображать линии сетки между ячейками
        }

        private List<ListViewItem> ExecuteQuery(string donationsDate, decimal minDonationSum, string city)
        {
            List<ListViewItem> items = new List<ListViewItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                CONCAT(s.surname, ' ', s.name, ' ', s.patronymic) AS FullName,
                SUM(d.Sum) AS TotalDonations,
                s.city AS City
            FROM 
                fond.donations d
            LEFT JOIN 
                fond.sacrificing s ON d.idSacrificing_fk = s.idSacrificing
            WHERE 
                d.donations_date > @donationsDate
                AND d.Sum > @minDonationSum
                AND (
                    NOT EXISTS (
                        SELECT 
                            1
                        FROM 
                            fond.sacrificing s
                        WHERE 
                            s.idSacrificing = d.idSacrificing_fk
                            AND s.city = @city
                    )
                    OR d.idSacrificing_fk IS NULL
                )
            GROUP BY 
                s.idSacrificing, FullName, City;
        ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@donationsDate", donationsDate);
                    command.Parameters.AddWithValue("@minDonationSum", minDonationSum);
                    command.Parameters.AddWithValue("@city", city);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fullName = reader["FullName"].ToString();
                            string totalDonations = reader["TotalDonations"].ToString();
                            string cityResult = reader["City"].ToString();

                            ListViewItem item = new ListViewItem(fullName);
                            item.SubItems.Add(totalDonations);
                            item.SubItems.Add(cityResult);
                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                listView2.Items.Clear();

                string donationsDate = dateTimePicker3.Value.ToString("yyyy-MM-dd");
                decimal minDonationSum = decimal.Parse(textBox4.Text);
                string city = textBox3.Text;

                List<ListViewItem> resultItems = ExecuteQuery(donationsDate, minDonationSum, city);

                foreach (ListViewItem item in resultItems)
                {
                    listView2.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

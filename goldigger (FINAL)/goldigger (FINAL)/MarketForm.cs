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



namespace GD_TRADING_APP_beta_
{
   
    public partial class MarketForm : Form
    {
        private MySqlConnection connection;
        public MarketForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }
        private void InitializeDatabase()
        {
            // Establish a connection to the database
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string query = "SELECT coin_rate FROM cryptocurrency";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Assuming you want to display the data in a Label
                        if (dataTable.Rows.Count > 0)
                        {
                            label9.Text = $"PHP {dataTable.Rows[0]["coin_rate"]}";
                            label10.Text = $"PHP {dataTable.Rows[1]["coin_rate"]}";
                            label11.Text = $"PHP {dataTable.Rows[2]["coin_rate"]}";
                            label12.Text = $"PHP {dataTable.Rows[3]["coin_rate"]}";
                            label13.Text = $"PHP {dataTable.Rows[4]["coin_rate"]}";
                            label14.Text = $"PHP {dataTable.Rows[5]["coin_rate"]}";
                            label15.Text = $"PHP {dataTable.Rows[6]["coin_rate"]}";
                            label16.Text = $"PHP {dataTable.Rows[7]["coin_rate"]}";
                            label17.Text = $"PHP {dataTable.Rows[8]["coin_rate"]}";
                            label18.Text = $"PHP {dataTable.Rows[9]["coin_rate"]}";

                        }
                        else
                        {
                            MessageBox.Show("No data found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }
    }
}

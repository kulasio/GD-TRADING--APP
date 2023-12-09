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
using GD_TRADING_APP_beta_;
using Microsoft.Exchange.WebServices.Data;

namespace GD_TRADING_APP_beta_
{
    public partial class DashboardForm : Form
    {
        private MySqlConnection connection;

        public DepositeForm DepositeFormInstance { get; set; }

        public DashboardForm()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadUserData();
            DisplayTradeHistory();
            DisplayUserCoins();

        }
        int userID = UserManager.Instance.CurrentUserID;


        private void InitializeDatabase()
        {
            // Establish a connection to the database
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
        }
        private void LoadUserData()
        {
            int userID = UserManager.Instance.CurrentUserID;

            try
            {
                connection.Open();

                // Fetch user_id
                string queryUserID = $"SELECT user_id FROM users WHERE user_id = {userID}";
                using (MySqlCommand cmdUserID = new MySqlCommand(queryUserID, connection))
                {
                    object resultUserID = cmdUserID.ExecuteScalar();
                    if (resultUserID != null)
                    {
                        label5.Text = $"User ID: {resultUserID.ToString()}";
                    }
                }

                // Fetch username
                string queryUsername = $"SELECT username FROM users WHERE user_id = {userID}";
                using (MySqlCommand cmdUsername = new MySqlCommand(queryUsername, connection))
                {
                    object resultUsername = cmdUsername.ExecuteScalar();
                    if (resultUsername != null)
                    {
                        label1.Text = $"Username: {resultUsername.ToString()}";
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


        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DisplayTradeHistory()
        {
            try
            {
                // Create a DataTable to store the trade history data
                DataTable tradeHistoryTable = new DataTable();

                // Open the database connection
                connection.Open();

                // Fetch the trade history data from the database
                string tradeHistoryQuery = $"SELECT * FROM trade_history WHERE user_id = {userID}";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(tradeHistoryQuery, connection))
                {
                    // Fill the DataTable with the trade history data
                    adapter.Fill(tradeHistoryTable);
                }

                // Bind the DataTable to the DataGridView
                dataGridView1.DataSource = tradeHistoryTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching trade history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the database connection
                connection.Close();
            }
        }
        private void DisplayUserCoins()
        {
            try
            {
                // Create a DataTable to store the user coins data
                DataTable userCoinsTable = new DataTable();

                // Open the database connection
                connection.Open();

                // Fetch the user coins data from the database
                string userCoinsQuery = $@"
                SELECT
                    c.coin_symbol,
                    c.coin_name,
                    uc.amount AS coin_quantity
                FROM
                    user_coins uc
                INNER JOIN
                    Cryptocurrency c ON uc.crypto_id = c.cryptocurrency_id
                WHERE
                    uc.user_id = {userID} ";

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(userCoinsQuery, connection))
                {
                    // Fill the DataTable with the user coins data
                    adapter.Fill(userCoinsTable);
                }

                // Bind the DataTable to the new DataGridView
                dataGridView2.DataSource = userCoinsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching user coins: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the database connection
                connection.Close();
            }
        }

       

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}

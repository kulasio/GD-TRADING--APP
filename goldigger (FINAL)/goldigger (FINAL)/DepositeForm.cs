using System;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GD_TRADING_APP_beta_
{
    public partial class DepositeForm : Form
    {
        private MySqlConnection connection;
        private int currentUserID;
        private MySqlDataAdapter dataAdapter;
        private decimal totalDepositedAmount = 0;

       

        public DepositeForm()
        {
            InitializeComponent();
            InitializeDatabase();
            GetCurrentUserID();
            UpdateBalanceLabel();
            dataGridView1.DataSource = GetTransactionHistory();
        }


        private void InitializeDatabase()
        {
            // Establish a connection to the database
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
        }

        private void GetCurrentUserID()
        {
            // Assuming UserManager.Instance.CurrentUserID is a property or method to get the current user ID
            currentUserID = UserManager.Instance.CurrentUserID;
        }

        private string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
        public decimal GetCurrentBalance()
        {
            decimal currentBalance = 0;
            int userID = UserManager.Instance.CurrentUserID;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Calculate the net balance by subtracting total withdrawals from total deposits
                    string depositQuery = $"SELECT COALESCE(SUM(amount), 0) FROM balance_table WHERE user_id = {userID} AND transaction_type = 'Deposit'";
                    string withdrawalQuery = $"SELECT COALESCE(SUM(amount), 0) FROM balance_table WHERE user_id = {userID} AND transaction_type = 'Withdrawal'";

                    using (MySqlCommand depositCmd = new MySqlCommand(depositQuery, connection))
                    using (MySqlCommand withdrawalCmd = new MySqlCommand(withdrawalQuery, connection))
                    {
                        decimal totalDeposits = Convert.ToDecimal(depositCmd.ExecuteScalar());
                        decimal totalWithdrawals = Convert.ToDecimal(withdrawalCmd.ExecuteScalar());

                        currentBalance = totalDeposits - totalWithdrawals;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching balance: " + ex.Message);
            }

            return currentBalance;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            int userID = UserManager.Instance.CurrentUserID;
            if (decimal.TryParse(textBox1.Text, out decimal depositAmount))
            {
                if (depositAmount == 0)
                {
                    MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    connection.Open();

                    // Insert the deposit into the balance_table
                    string insertQuery = $"INSERT INTO balance_table (transaction_type, amount, user_id) VALUES ('Deposit', '{depositAmount}', '{userID}')";
                    MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                    insertCommand.ExecuteNonQuery();

                    // Update the current balance label
                    UpdateBalanceLabel();
                    InsertTransactionHistory("Deposit", depositAmount);

                    MessageBox.Show("Deposit!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
                
            }
            else
            {
                MessageBox.Show("Invalid deposit amount. Please enter a valid number.");
            }
        }


        private void UpdateBalanceLabel()
        {
            label2.Text = $"Balance: {GetCurrentBalance():C}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int userID = UserManager.Instance.CurrentUserID;
            if (decimal.TryParse(textBox1.Text, out decimal withdrawalAmount))
            {
                if (withdrawalAmount == 0)
                {
                    MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    connection.Open();

                    decimal currentBalance = GetCurrentBalance();
                    if (withdrawalAmount > currentBalance)
                    {
                        MessageBox.Show("Insufficient funds for withdrawal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert the withdrawal into the balance_table with the original withdrawal amount
                    string insertQuery = $"INSERT INTO balance_table (transaction_type, amount, user_id) VALUES ('Withdrawal', '{withdrawalAmount}', '{userID}')";
                    MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                    insertCommand.ExecuteNonQuery();

                   

                    // Update the current balance label
                    UpdateBalanceLabel();

                    InsertTransactionHistory("Withdrawal", withdrawalAmount);
                    MessageBox.Show("Withdrawal successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();                   
                }              
            }
            else
            {
                MessageBox.Show("Invalid withdrawal amount. Please enter a valid number.");
            }
        }
        private void InsertTransactionHistory(string transactionType, decimal amount)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            try
            {
                

                // Insert into transaction_history
                string insertQuery = "INSERT INTO transaction_history (user_id, transaction_type, amount) VALUES (@userID, @transactionType, @amount)";
                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@userID", currentUserID);
                    insertCmd.Parameters.AddWithValue("@transactionType", transactionType);
                    insertCmd.Parameters.AddWithValue("@amount", amount);

                    insertCmd.ExecuteNonQuery();
                }

                // Refresh the DataGridView with updated transaction history
                dataGridView1.DataSource = GetTransactionHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting into transaction history: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        
        private DataTable GetTransactionHistory()
        {
            // Create a DataTable to hold the transaction history data
            DataTable dataTable = new DataTable();

            using (MySqlConnection newConnection = new MySqlConnection(connectionString))
            {
                try
                {
                    newConnection.Open();

                    string query = $"SELECT * FROM transaction_history WHERE user_id = {currentUserID}";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, newConnection))
                    {
                        // Fill the DataTable with the results of the query
                        adapter.Fill(dataTable);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching transaction history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    newConnection.Close();
                }
            }

            return dataTable;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

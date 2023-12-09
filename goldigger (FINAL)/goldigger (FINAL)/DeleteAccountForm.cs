using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GD_TRADING_APP_beta_
{
    public partial class DeleteAccountForm : Form
    {
        private MySqlConnection connection;
        private int currentUserID;
      
        public DeleteAccountForm()
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
        private void SetCurrentUser(int userID)
        {
            currentUserID = userID;
        }

        private void DeleteAllUsers()
        {
            try
            {
                connection.Open();
                string deleteQuery = "DELETE FROM balance_table WHERE user_id = '0';\r\n\r\n-- Then delete from the Users table\r\nDELETE FROM Users WHERE user_id = '0';";
                using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DeleteAllUsers();
                MessageBox.Show("Account deleted successfully.");

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting users: " + ex.Message);
            }
        
           


        }
       
    }
}

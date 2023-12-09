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

namespace GD_TRADING_APP_beta_
{
    public partial class RegistrationForm : Form
    {
        private MySqlConnection connection;
        private MySqlDataAdapter dataAdapter;
        private DataSet dataSet;
        public RegistrationForm()
        {
            InitializeComponent();
            InitializeDatabase();
        }
        private void InitializeDatabase()
        {
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
            dataAdapter = new MySqlDataAdapter("SELECT * FROM Users", connection);
            dataSet = new DataSet();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string email = textBox2.Text;
            string password = textBox3.Text;


            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string insertQuery = $"INSERT INTO Users (Username, Email, Password) VALUES ('{username}', '{email}', '{password}')";

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(insertQuery, connection);
                command.ExecuteNonQuery();
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();



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

        private void RegistrationForm_Load(object sender, EventArgs e)
        {

        }
    }
}

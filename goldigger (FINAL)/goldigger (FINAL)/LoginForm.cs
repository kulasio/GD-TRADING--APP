using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GD_TRADING_APP_beta_
{
    public partial class LoginForm : Form
    {
        private MySqlConnection connection;
        private int currentUserID;
        public LoginForm()
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
        private int SelectQuery1(string email, string password)
        {
            try
            {
                connection.Open();
                string selectQuery = "SELECT user_ID FROM Users WHERE Email = @Email AND Password = @Password";
                MySqlCommand command = new MySqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return -1; // User not found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1; // Handle the error and return -1
            }
            finally
            {
                connection.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text;
            string password = textBox3.Text;

            // Check the entered credentials against the database using parameterized query
            int userID = SelectQuery1(email, password);
            if (userID != -1)
            {
                SetCurrentUser(userID);
                MessageBox.Show("Login successful!");
                UserManager.Instance.SetCurrentUser(userID);
                HompageForm hompageForm = new HompageForm();
                hompageForm.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegistrationForm form2 = new RegistrationForm();
            form2.Show();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

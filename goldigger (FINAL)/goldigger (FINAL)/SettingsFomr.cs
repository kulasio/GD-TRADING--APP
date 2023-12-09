using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GD_TRADING_APP_beta_
{
    public partial class SettingsFomr : Form
    {
        private MySqlConnection connection;
        private string connectionString;
    
        private MySqlDataAdapter dataAdapter;
        private DataSet dataSet;
        public SettingsFomr()
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
     

        private void button4_Click(object sender, EventArgs e)
        {
           DeleteAccountForm deleteAccountForm = new DeleteAccountForm();
            deleteAccountForm.ShowDialog();
        }

        private void SettingsFomr_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The feature is still in beta !!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The feature is still in beta !!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The feature is still in beta !!");
        }
    }
}

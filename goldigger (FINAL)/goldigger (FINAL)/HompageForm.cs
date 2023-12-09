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
namespace GD_TRADING_APP_beta_
{
    public partial class HompageForm : Form
    {
        private MySqlConnection connection;
        public HompageForm()
        {
            InitializeComponent();
            InitializeDatabase();
            
        }
        public class UserManager
        {
            private static UserManager _instance;

            private UserManager() { }

            public static UserManager Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new UserManager();
                    }
                    return _instance;
                }
            }

            public int CurrentUserID { get; private set; }

            public void SetCurrentUser(int userID)
            {
                CurrentUserID = userID;
            }

            internal void SetCurrentUser(object userID)
            {
                throw new NotImplementedException();
            }
        }


        private void InitializeDatabase()
        {
            // Establish a connection to the database
            string connectionString = "server=127.0.0.1; user=root; database=sample; password=";
            connection = new MySqlConnection(connectionString);
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (menuVertical.Width == 250)
            {
                menuVertical.Width = 70;

            }
            else
            {
                menuVertical.Width = 250;

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DashboardForm DashboardForm = new DashboardForm();

            // Set the TopLevel property of the form to false, so it can be hosted inside the panel
            DashboardForm.TopLevel = false;

            // Clear any existing controls from the panel
            content.Controls.Clear();

            // Add the form to the panel's Controls collection
            content.Controls.Add(DashboardForm);

            // Show the form
            DashboardForm.Show();

            // Set the size of the form to match the panel's size
            DashboardForm.Size = content.Size;
            DashboardForm.AutoSize = true;




        }

        private void button2_Click(object sender, EventArgs e)
        {
            MarketForm marketForm = new MarketForm();
            marketForm.TopLevel = false;
            content.Controls.Clear();
            content.Controls.Add(marketForm);
            marketForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TradeForm tradeForm = new TradeForm();  
            tradeForm.TopLevel = false;
            content.Controls.Clear();
            content.Controls.Add(tradeForm);
            tradeForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DepositeForm depositeForm = new DepositeForm();
            depositeForm.TopLevel = false;
            content.Controls.Clear();
            content.Controls.Add(depositeForm);
            depositeForm.Show();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SettingsFomr settings = new SettingsFomr();
            settings.TopLevel = false;
            content.Controls.Clear();
            content.Controls.Add(settings);
            settings.Show();
        }
    }
}

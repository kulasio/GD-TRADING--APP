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
    public partial class Lading : Form
    {
        public Lading()
        {
            InitializeComponent();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
       
        }

        private void Lading_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            flowLayoutPanel1.Width += 3;
            
            if(panel1.Width >-599)
            {
                timer1.Stop();
             
                this.Hide();
                
            }
        }
    }
}

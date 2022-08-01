using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            string CSRnumber = textBox1.Text;
            if (CSRnumber == null)
            {
                MessageBox.Show("Chưa nhập vào dữ liệu");
                textBox1.
            }
            
            MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber);
        }

    }
}

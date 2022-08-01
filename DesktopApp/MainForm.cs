using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TokenService;

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
            if (CSRnumber.Equals(""))
            {
                MessageBox.Show("Chưa nhập vào dữ liệu");
                
            }
            else
            {
                ManageKey generateKeypair = new ManageKey();
                string subjectDN = generateKeypair.createInformation();
                generateKeypair.generateKey(2048);
                string CSR = generateKeypair.generateCSR(subjectDN, null);

                WriteExcelFile writeExcelFile = new WriteExcelFile();
                writeExcelFile.ExportExcel(CSR, null);
                MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber + "\nGenerated CSR and Export Excel Successfully !");


            }
            
            
        }

    }
}

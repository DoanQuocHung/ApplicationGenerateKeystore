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
    public partial class PersonalPanel : Form
    {
        public PersonalPanel()
        {
            InitializeComponent();
        }

        //Button Huy ===
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Button Xac nhan ===
        private void button2_Click(object sender, EventArgs e)
        {
            string tencanhan = textBox1.Text;
            string email = textBox2.Text;
            string sdt = textBox3.Text;
            string diachi = textBox4.Text;
            string thanhpho = textBox5.Text;
            string quocgia = textBox6.Text;
            string giaytocanhan = textBox7.Text;
            string giaytocanhanType = comboBox1.Text;
            
            List<CsrObjects> listCsrObjects = new List<CsrObjects>();
            CsrObjects csrObjects = new CsrObjects();
            

            listCsrObjects.Add(csrObjects);


            MessageBox.Show("Nhập thông tin thành công !", "Thông báo");
            this.Close();
        }
    }
}

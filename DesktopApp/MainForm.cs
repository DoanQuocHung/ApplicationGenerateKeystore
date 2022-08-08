using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            if (!Utils.CheckInputNumber(CSRnumber))
            {
                MessageBox.Show("Chưa nhập dữ liệu hoặc dữ liệu không hợp lệ (kiểu số)", "Thông báo người dùng");
            }
            else
            {
                ManageKey generateKeypair = new ManageKey();
                ManageAlgorithm storePrivateKey = new ManageAlgorithm();

                //Create CSRS
                string subjectDN = generateKeypair.createInformation();
                generateKeypair.generateKey(2048);
                string CSR = generateKeypair.generateCSR(subjectDN, null);

                //Create storage to store PrivateKey
                storePrivateKey.EncryptPrivateKey(generateKeypair.getKey());

                //Write to Excel File
                ExcelExecution writeExcelFile = new ExcelExecution();
                //writeExcelFile.ExportExcel(CSR);
                
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export Excel File";
                saveFileDialog.FileName = "ExportExcelCSR.Dialog";
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx;*.xlsx";
                saveFileDialog.FilterIndex = 2;
                
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    writeExcelFile.ExportExcel(CSR, saveFileDialog.FileName, CSRnumber);
                }

                MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber + "\nGenerated CSR and save excel file successfully !", "Thông báo người dùng");
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (textBox2.Text.Equals("")|| !Utils.CheckInputPath(textBox2.Text))
            {
                MessageBox.Show("Chưa chọn file excel hoặc sai định dạng đường dẫn!!", "Thông báo người dùng");
                return;
                //MessageBox.Show()
            }else {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(textBox2.Text, FileMode.Open, FileAccess.Read);
                }
                catch (FileNotFoundException q)
                {
                    MessageBox.Show("File không tồn tại!!", "Thông báo người dùng");
                    return;
                }
                catch (IOException q)
                {
                    MessageBox.Show("File đang được mở bởi người dùng! Vui lòng tắt file trước khi upload!!", "Thông báo người dùng");
                    return;
                }
            }            
            {
                GenerateP12 generateP12 = new GenerateP12();
                ManageKey generateKeypair = new ManageKey();
                ManageAlgorithm generateAlgorithm = new ManageAlgorithm();
                ExcelExecution excelExecution = new ExcelExecution();

                //byte[] browseFile = File.ReadAllBytes(textBox2.Text);
                //string subjectDN = generateKeypair.createInformation();
                //generateKeypair.generateKey(2048);

                //TEST
                //X509Certificate endEntityCert = Utils.readCertificateFromFile("file/cert.cer");
                //X509Certificate CertChain = Utils.readCertificateFromFile("file/mobileidcert.cer");
                //string EndCertDecoded = Utils.convertCertToBase64(endEntityCert);
                //string CertChainDecoded = Utils.convertCertToBase64(CertChain);

                //LIVE
                string EndCertDecoded = excelExecution.ImportExcel(textBox2.Text, 2);
                string CertChainDecoded = excelExecution.ImportExcel(textBox2.Text, 3);
                X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(EndCertDecoded));
                X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(CertChainDecoded));
                AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
                RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);
                generateP12.pkcs12Keystore(x509cert_1, x509cert_2, privatekey, "file/testP12.p12", "testP12", "12345678");

                MessageBox.Show("Generate PKCS12 Keystore Successfully !", "Thông báo người dùng");
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            string file = "";
            //string browseResult = textBox2.Text;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse Excel File";
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx;*.xlsx";

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                textBox2.Text = file;
                MessageBox.Show("Browse excel file successfully !", "Thông báo người dùng");
            }

            //MessageBox.Show("Browse excel file successfully !\nFile path is: " + file, "Thông báo người dùng");
            //MessageBox.Show("Browse excel file successfully !", "Thông báo người dùng");
        }


    }
}

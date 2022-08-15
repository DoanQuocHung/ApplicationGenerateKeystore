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
                MessageBox.Show("Chưa nhập dữ liệu hoặc dữ liệu không hợp lệ (kiểu số)", "Thông báo");
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

                ExcelExecution writeExcelFile = new ExcelExecution();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export Excel File";
                saveFileDialog.FileName = "ExportExcelCSR.Dialog";
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx;*.xlsx";
                saveFileDialog.FilterIndex = 2;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    writeExcelFile.ExportExcel(CSR, saveFileDialog.FileName, CSRnumber);
                }

                MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber + "\nSinh CSR và lưu file excel thành công !", "Thông báo");
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (textBox2.Text.Equals("") || !Utils.CheckInputPath(textBox2.Text))
            {
                MessageBox.Show("Chưa chọn file excel hoặc sai định dạng đường dẫn !", "Thông báo");
                return;
            }
            else
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(textBox2.Text, FileMode.Open, FileAccess.Read);
                }
                catch (FileNotFoundException q)
                {
                    MessageBox.Show("File không tồn tại !", "Thông báo");
                    return;
                }
                catch (IOException q)
                {
                    MessageBox.Show("File đang được mở bởi người dùng ! Vui lòng tắt file trước khi upload !", "Thông báo");
                    return;
                }
            }
            {
                ExcelExecution excelExecution = new ExcelExecution();

                string EndCertDecoded = excelExecution.ImportExcel(textBox2.Text, 2);
                string CertChainDecoded = excelExecution.ImportExcel(textBox2.Text, 3);

                int level = Utils.CheckLevelOfCertificate(CertChainDecoded);
                //Cert chain 1 layer
                if ( level == 0)
                {
                    this.CreateP12Level1(EndCertDecoded, CertChainDecoded);
                }
                else  //Cert chain 2 layer
                {
                    string[] base64Chain = Utils.convertPEMtoArrayBase64(CertChainDecoded);     //Contain blank
                    int pos = 0;
                    string[] base64Chain_2 = new string[base64Chain.Length];
                    foreach(string s in base64Chain)
                    {
                        if( !Utils.isBlank(s))
                        {
                            base64Chain_2[pos] = s;
                            pos++;
                        }
                    }
                    this.CreateP12LevelGT1(level, EndCertDecoded, base64Chain_2);
                }
            }
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            string file = "";
            //string browseResult = textBox2.Text;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse Excel File";
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx;*.xlsx";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                textBox2.Text = file;
                MessageBox.Show("Browse excel file thành công !", "Thông báo");
            }                                                                             
        }

        private void CreateP12Level1(string base64Cert, string base64Chain)
        {
            GenerateP12 generateP12 = new GenerateP12();
            ManageAlgorithm generateAlgorithm = new ManageAlgorithm();
                      
            X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(base64Cert));
            X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(base64Chain));
            AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
            RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

            string passWord = Utils.CreatePassword(8);
            generateP12.pkcs12Keystore(x509cert_1, x509cert_2, privatekey, "file/KeyStoreP12.p12", "TESTP12", passWord);

            File.WriteAllText("file/passwordP12.txt", passWord);
            MessageBox.Show("Cấp phát Keystore thành công !", "Thông báo");
        }

        private void CreateP12LevelGT1(int level, string base64Cert, string[] base64Chain)
        {
            GenerateP12 generateP12 = new GenerateP12();
            ManageAlgorithm generateAlgorithm = new ManageAlgorithm();

            X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(base64Cert));

            //Create Array CertChain
            List<X509Certificate> certChain = new List<X509Certificate>();
            for (int i = 0; i < level; i++)
            {
                if (base64Chain[i] == null)
                    continue;
                X509Certificate temp = new X509Certificate(Convert.FromBase64String(base64Chain[i]));
                certChain.Add(temp);
            }
            AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
            RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

            string passWord = Utils.CreatePassword(8);
            generateP12.pkcs12Keystore(x509cert_1, certChain, privatekey, "file/KeyStoreP12.p12", "TESTP12", passWord);

            File.WriteAllText("file/passwordP12.txt", passWord);
            MessageBox.Show("Cấp phát Keystore thành công !", "Thông báo");

        }
    }
}

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

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    writeExcelFile.ExportExcel(CSR, saveFileDialog.FileName, CSRnumber);
                }

                MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber + "\nGenerated CSR and save excel file successfully !", "Thông báo người dùng");
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (textBox2.Text.Equals("") || !Utils.CheckInputPath(textBox2.Text))
            {
                MessageBox.Show("Chưa chọn file excel hoặc sai định dạng đường dẫn!!", "Thông báo người dùng");
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

                //GenerateP12 generateP12 = new GenerateP12();
                //ManageKey generateKeypair = new ManageKey();
                //ManageAlgorithm generateAlgorithm = new ManageAlgorithm();
                ExcelExecution excelExecution = new ExcelExecution();

                ////byte[] browseFile = File.ReadAllBytes(textBox2.Text);
                ////string subjectDN = generateKeypair.createInformation();
                ////generateKeypair.generateKey(2048);

                ////TEST
                ////X509Certificate endEntityCert = Utils.readCertificateFromFile("file/cert.cer");
                ////X509Certificate CertChain = Utils.readCertificateFromFile("file/mobileidcert.cer");
                ////string EndCertDecoded = Utils.convertCertToBase64(endEntityCert);
                ////string CertChainDecoded = Utils.convertCertToBase64(CertChain);

                ////LIVE
                string EndCertDecoded = excelExecution.ImportExcel(textBox2.Text, 2);
                string CertChainDecoded = excelExecution.ImportExcel(textBox2.Text, 3);

                int level = Utils.CheckLevelOfCertificate(CertChainDecoded);
                //Level 1
                if ( level == 0)
                {
                    this.CreateP12Level1(EndCertDecoded, CertChainDecoded);
                }
                else //Level >1
                {
                    string[] base64Chain = Utils.convertPEMtoArrayBase64(CertChainDecoded);//Contain blank
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

                //X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(EndCertDecoded));
                //X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(CertChainDecoded));
                //AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
                //RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

                //string passWord = Utils.CreatePassword(8);
                //generateP12.pkcs12Keystore(x509cert_1, x509cert_2, privatekey, "file/KeyStoreP12.p12", "TESTP12", passWord);

                //File.WriteAllText("file/passwordP12.txt", passWord);
                //MessageBox.Show("Generate PKCS12 Keystore Successfully !", "Thông báo người dùng");
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
                MessageBox.Show("Browse excel file successfully !", "Thông báo người dùng");
            }
        }

        private void CreateP12Level1(string base64Cert, string base64Chain)
        {
            //Contructor
            GenerateP12 generateP12 = new GenerateP12();
            ManageAlgorithm generateAlgorithm = new ManageAlgorithm();

            //Handle                        
            X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(base64Cert));
            X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(base64Chain));
            AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
            RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

            string passWord = Utils.CreatePassword(8);
            generateP12.pkcs12Keystore(x509cert_1, x509cert_2, privatekey, "file/KeyStoreP12_1cap.p12", "TESTP12", passWord);

            File.WriteAllText("file/passwordP12.txt", passWord);
            MessageBox.Show("Generate PKCS12 Keystore Successfully !", "Thông báo người dùng 1 cấp");
        }

        private void CreateP12LevelGT1(int level, string base64Cert, string[] base64Chain)
        {
            //Contructor
            GenerateP12 generateP12 = new GenerateP12();
            ManageAlgorithm generateAlgorithm = new ManageAlgorithm();

            //Handle
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
            //Read from hidden file a get PrivateKey            
            AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
            RsaKeyParameters privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

            //
            string passWord = Utils.CreatePassword(8);
            generateP12.pkcs12Keystore(x509cert_1, certChain, privatekey, "file/KeyStoreP12_capnhieu.p12", "TESTP12", passWord);

            File.WriteAllText("file/passwordP12.txt", passWord);
            MessageBox.Show("Generate PKCS12 Keystore Successfully !", "Thông báo người dùng");
        }
    }
}

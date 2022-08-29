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
        public static string subjectCN;
        public static string subjectMST;
        private readonly MainForm mainForm;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        private void combobox1_Click(object sender, System.EventArgs e)
        {

        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            object seclecteCombobox = comboBox1.SelectedItem;
            string resultCombobox = Convert.ToString(seclecteCombobox);

            if (resultCombobox.Equals("PERSONAL"))
            {
                MessageBox.Show("Loại chứng thư số vừa chọn: PERSONAL");
                PersonalPanel personalPanel = new PersonalPanel();
                personalPanel.Show();
            }
            else if (resultCombobox.Equals("ENTERPRISE"))
            {
                MessageBox.Show("Loại chứng thư số vừa chọn: ENTERPRISE");
                EnterprisePanel enterprisePanel = new EnterprisePanel();
                enterprisePanel.Show();
            }
            else if (resultCombobox.Equals("STAFF"))
            {
                MessageBox.Show("Loại chứng thư số vừa chọn: STAFF");
                StaffPanel staffPanel = new StaffPanel();
                staffPanel.Show();
            }
            else
            {
                MessageBox.Show("Chưa chọn loại Chứng thư số");
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            string CSRnumber = textBoxSum.Text;
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
            String filename = saveFileDialog.FileName;
            filename = filename.Remove(filename.Length - 4);
            filename = filename + "_csr.csr";
            using (StreamWriter f = new StreamWriter(filename))
            {
                f.Write(CSR);
            }
            MessageBox.Show("Số lượng Chứng thư số vừa nhập: " + CSRnumber + "\nSinh CSR và lưu file excel thành công !", "Thông báo");
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (textBox2.Text.Equals("") || !Utils.CheckInputPath(textBox2.Text))
            {
                MessageBox.Show("Chưa chọn file excel hoặc sai định dạng đường dẫn!!", "Thông báo người dùng");
                return;
            }

            {
                ExcelExecution excelExecution = new ExcelExecution();

                string EndCertDecoded = excelExecution.ImportExcel(textBox2.Text, 2);
                string CertChainDecoded = excelExecution.ImportExcel(textBox2.Text, 3);
                string[] signer = Utils.convertPEMtoArrayBase64(EndCertDecoded);
                signer[0] = Utils.UsingRegexDeleteNewLine(signer[0]);
                EndCertDecoded = signer[0];
                if (EndCertDecoded == null || CertChainDecoded == null)
                {
                    return;
                }

                int level = Utils.CheckLevelOfCertificate(CertChainDecoded);
                //Level 1
                if (level == 0 || level == 1)
                {
                    string[] base64Chain = Utils.convertPEMtoArrayBase64(CertChainDecoded);   //Contain blank
                    int pos = 0;
                    string[] base64Chain_2 = new string[base64Chain.Length];
                    foreach (string s in base64Chain)
                    {
                        if (!Utils.isBlank(s))
                        {
                            base64Chain_2[pos] = s;
                            pos++;
                        }
                    }
                    this.CreateP12Level1(EndCertDecoded, base64Chain_2[0]);
                    //this.CreateP12Level1(EndCertDecoded, CertChainDecoded);
                }
                else //Level 1
                {
                    string[] base64Chain = Utils.convertPEMtoArrayBase64(CertChainDecoded);   //Contain blank
                    int pos = 0;
                    string[] base64Chain_2 = new string[base64Chain.Length];
                    foreach (string s in base64Chain)
                    {
                        if (!Utils.isBlank(s))
                        {
                            base64Chain_2[pos] = Utils.UsingRegexDeleteNewLine(s);
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
            try
            {
                //Contructor
                GenerateP12 generateP12 = new GenerateP12();
                ManageAlgorithm generateAlgorithm = new ManageAlgorithm();

                //Handle
                X509Certificate x509cert_1 = new X509Certificate(Convert.FromBase64String(base64Cert));
                X509Certificate x509cert_2 = new X509Certificate(Convert.FromBase64String(base64Chain));
                AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
                AsymmetricCipherKeyPair privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

                System.Security.Cryptography.X509Certificates.X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2();
                byte[] rawData = Encoding.ASCII.GetBytes(base64Cert);
                x509.Import(rawData);

                string mark1 = "OID.";
                string mark2 = "CN=";
                string mark3 = "= CMND";
                string subjectCert = x509.Subject;
                string[] words = subjectCert.Split(',');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Contains(mark1))
                    {
                        string[] words_2 = words[i].Split('=');
                        subjectMST = words_2[1].Replace(':', '-');
                    }
                    else if (words[i].Contains(mark2))
                    {
                        string[] words_2 = words[i].Split('=');
                        subjectCN = words_2[1];
                    }
                }

                string passWord = Utils.CreatePassword(8);
                string filePath = "";
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        filePath = fbd.SelectedPath;
                    }
                }

                generateP12.pkcs12Keystore(x509cert_1, x509cert_2, privatekey
                    , filePath + "/" + subjectMST + "_" + passWord + ".p12"
                    , subjectCN
                    , passWord);

                //File.WriteAllText("file/passwordP12.txt", passWord);
                MessageBox.Show("Generate PKCS12 Keystore Successfully!", "Thông báo người dùng 1 cấp");
            }
            catch (Exception e)
            {
                MessageBox.Show("File không thỏa điều kiện để sinh khóa!! (Dữ liệu lỗi)", "Thông báo người dùng");
                return;
            }
        }

        private void CreateP12LevelGT1(int level, string base64Cert, string[] base64Chain)
        {
            try
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
                    String base64Chain_temp = Utils.UsingRegexDeleteNewLine(base64Chain[i]);
                    X509Certificate temp = new X509Certificate(Convert.FromBase64String(base64Chain_temp));
                    certChain.Add(temp);
                }
                //Read from hidden file a get PrivateKey
                AsymmetricKeyParameter publicKey = x509cert_1.GetPublicKey();
                AsymmetricCipherKeyPair privatekey = generateAlgorithm.DecryptPrivateKey(publicKey);

                System.Security.Cryptography.X509Certificates.X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2();
                byte[] rawData = Encoding.ASCII.GetBytes(base64Cert);
                x509.Import(rawData);

                string mark1 = "OID.";
                string mark2 = "CN=";
                string mark3 = "=CMND";
                string subjectCert = x509.Subject;
                string[] words = subjectCert.Split(',');
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Contains(mark1))
                    {
                        string[] words_2 = words[i].Split('=');
                        subjectMST = words_2[1].Replace(':', '-');
                    }
                    else if (words[i].Contains(mark2))
                    {
                        string[] words_2 = words[i].Split('=');
                        subjectCN = words_2[1];
                    }
                }

                string passWord = Utils.CreatePassword(8);
                string filePath = "";
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        filePath = fbd.SelectedPath;
                    }
                }

                generateP12.pkcs12Keystore(x509cert_1, certChain, privatekey.Private
                , filePath + "/" + subjectMST + "_" + passWord + ".p12"
                , subjectCN
                , passWord);

                //File.WriteAllText("file/passwordP12.txt", passWord);
                MessageBox.Show("Generate PKCS12 Keystore Successfully!", "Thông báo người dùng");
            }
            catch (Exception e)
            {
                MessageBox.Show("File không thỏa điều kiện để sinh khóa!! (Dữ liệu lỗi)", "Thông báo người dùng");
                return;
            }
        }
    }
}

﻿using Org.BouncyCastle.X509;
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
            if (CSRnumber.Equals(""))
            {
                MessageBox.Show("Chưa nhập vào dữ liệu", "Thông báo người dùng");
            }
            else
            {
                ManageKey generateKeypair = new ManageKey();
                string subjectDN = generateKeypair.createInformation();
                generateKeypair.generateKey(2048);
                string CSR = generateKeypair.generateCSR(subjectDN, null);

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
            if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("Chưa chọn file excel !", "Thông báo người dùng");
            }
            else
            {
                GenerateP12 generateP12 = new GenerateP12();
                ManageKey generateKeypair = new ManageKey();
                ExcelExecution excelExecution = new ExcelExecution();

                //byte[] browseFile = File.ReadAllBytes(textBox2.Text);
                string subjectDN = generateKeypair.createInformation();
                generateKeypair.generateKey(2048);

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

                generateP12.pkcs12Keystore(x509cert_1, x509cert_2, generateKeypair.getKey(), "file/testP12.p12", "testP12", "12345678");
                
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

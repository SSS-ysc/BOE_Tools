using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using EDID_Form;

namespace WindowsFormsApp_BOE_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label6.Text = System.DateTime.Now.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Blue;
            if (CA.ca210Connect(0) == true)
            {
                CA.ca210SetSyncMode(0);
                CA.ca210SetSpeed(1);
                button1.Text = "Success";

                CA.ca210ZeroCal();
                //btnConnect->Caption = "Connect CA410";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CA210DataStruct BOECA210;

            BOECA210 = CA.ca210Measure();

            label1.Text = BOECA210.CA210fX.ToString();
            label2.Text = BOECA210.CA210fY.ToString();
            label3.Text = BOECA210.CA210fZ.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int a;

            if (int.TryParse(textBox1.Text, out a) == true)//字符转int
            {
                byte[] byte_buffer = new byte[] { 0x85, 0x03, 0xFB, 0x10, 0x00, (byte)a };
 
                DDCCI.BOEDCCCI_Write(byte_buffer);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            byte[] return_buffer = new byte[10];
            byte[] byte_buffer = new byte[] { 0x83, 0x01, 0xFB, 0x10};
            byte i;

            DDCCI.BOEDCCCI_Read(byte_buffer, return_buffer);

            label4.Text = "Read:";
            for (i=0; i < 10; i++)
            {
                label4.Text += Convert.ToString(return_buffer[i], 16) + " ";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"C:\Hupp";
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                textBox2.Text = filePath;
                using (FileStream fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] b = new byte[50];
                    label5.Text = "";
                    while (true)
                    {
                        int r = fsRead.Read(b, 0, b.Length);
                        if (r == 0)
                            break;
                        label5.Text += Encoding.UTF8.GetString(b, 0, r);
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = @"C:\Users\user\Desktop";
            saveFileDialog1.RestoreDirectory= false;
            saveFileDialog1.Filter = ".txt文件(*.txt)|*.txt |.h文件（.h）|*.h";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "EDID_test";
            saveFileDialog1.AddExtension = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            { 
                EDID.OutputNotesEDIDText(saveFileDialog1.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string UnicodeText = "";
            string filePath = textBox2.Text;

            using (FileStream fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] b = new byte[50];

                while (true)
                {
                    int r = fsRead.Read(b, 0, b.Length);
                    if (r == 0)
                        break;
                    UnicodeText += Encoding.UTF8.GetString(b, 0, r);
                }
            }

            EDID.Format(UnicodeText);
        }
    }
}

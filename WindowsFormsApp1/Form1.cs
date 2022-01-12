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
        System.Windows.Forms.TextBox[] EDIDTextBox = new System.Windows.Forms.TextBox[128];
        public Form1()
        {
            InitializeComponent();

            int BoxSizeWeight = 32;
            int BoxSizeHeight = 25;
            int BoxSpaceX = 2;
            int BoxSpaceY = 1;
            for (int i = 0; i < EDIDTextBox.Length; i++)
            {
                EDIDTextBox[i] = new System.Windows.Forms.TextBox();
                EDIDTextBox[i].CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
                EDIDTextBox[i].Location = new System.Drawing.Point(500 + (i % 16) * (BoxSizeWeight + BoxSpaceX), 300 + (i / 16) * (BoxSizeHeight + BoxSpaceY));
                EDIDTextBox[i].MaxLength = 2;
                EDIDTextBox[i].Name = "EDIDTextBox_" + i.ToString();
                EDIDTextBox[i].Size = new System.Drawing.Size(BoxSizeWeight, BoxSizeHeight);
                EDIDTextBox[i].TabIndex = 18;
                EDIDTextBox[i].KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
                Controls.Add(EDIDTextBox[i]);
            }
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

            //label1.Text = BOECA210.CA210fX.ToString();
            //label2.Text = BOECA210.CA210fY.ToString();
            //label3.Text = BOECA210.CA210fZ.ToString();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //int a;

            //if (int.TryParse(textBox1.Text, out a) == true)//字符转int
            {
             //   byte[] byte_buffer = new byte[] { 0x85, 0x03, 0xFB, 0x10, 0x00, (byte)a };
 
             //   DDCCI.BOEDCCCI_Write(byte_buffer);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            byte[] return_buffer = new byte[10];
            byte[] byte_buffer = new byte[] { 0x83, 0x01, 0xFB, 0x10};
            byte i;

            DDCCI.BOEDCCCI_Read(byte_buffer, return_buffer);

            //label4.Text = "Read:";
            for (i=0; i < 10; i++)
            {
            //    label4.Text += Convert.ToString(return_buffer[i], 16) + " ";
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
                textBoxfile.Text = filePath;
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
        private void Form_button_Click(object sender, EventArgs e)
        {
            string UnicodeText = "";
            string filePath = textBoxfile.Text;

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

            string Formatresult;
            Formatresult = EDID.Format(UnicodeText).ToString();

            MessageBox.Show(Formatresult, "EDID解析");

            for (int i = 0; i < 128; i++)
            {
                // 转化输出两位十六进制字符
                EDIDTextBox[i].Text = string.Format("{0:X2}", EDID.EDIDByteData[i]);
            }
        }
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLower(e.KeyChar) && !char.IsUpper(e.KeyChar) && !char.IsNumber(e.KeyChar) && !(e.KeyChar == '\b'))
            {
                //对系统表示事件已处理，及跳过该事件
                e.Handled = true;
            }
        }
   }
}

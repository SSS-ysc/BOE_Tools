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

using Sunny.UI;
using EDID_Form;

namespace WindowsFormsApp_BOE_Tool
{
    public partial class Form1 : UIForm
    {
        Sunny.UI.UITextBox[] EDIDTextBox = new Sunny.UI.UITextBox[128];
        public Form1()
        {
            InitializeComponent();

            //自编的控件
            int BoxSizeWeight = 28;
            int BoxSizeHeight = 22;
            int BoxSpaceX = 3;
            int BoxSpaceY = 3;
            for (int i = 0; i < EDIDTextBox.Length; i++)
            {
                // 
                // uiTextBox
                // 
                EDIDTextBox[i] = new Sunny.UI.UITextBox();
                EDIDTextBox[i].ButtonSymbol = 61761;
                EDIDTextBox[i].Cursor = System.Windows.Forms.Cursors.IBeam;
                EDIDTextBox[i].Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                EDIDTextBox[i].Location = new System.Drawing.Point(22 + (i % 10) * (BoxSizeWeight + BoxSpaceX), 115 + (i / 10) * (BoxSizeHeight + BoxSpaceY));
                EDIDTextBox[i].Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
                EDIDTextBox[i].Maximum = 2147483647D;
                EDIDTextBox[i].Minimum = -2147483648D;
                EDIDTextBox[i].MinimumSize = new System.Drawing.Size(1, 16);
                EDIDTextBox[i].Name = "EDIDTextBox_" + i.ToString();
                EDIDTextBox[i].Size = new System.Drawing.Size(BoxSizeWeight, BoxSizeHeight);
                EDIDTextBox[i].Style = Sunny.UI.UIStyle.Custom;
                EDIDTextBox[i].TabStop = false; // ?
                EDIDTextBox[i].Text = "";
                EDIDTextBox[i].TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                EDIDTextBox[i].KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress_Check);
                Controls.Add(EDIDTextBox[i]);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //labelTime.Text = System.DateTime.Now.ToString();
        }
        private void CA_Connect_Click(object sender, EventArgs e)
        {
            //button1.ForeColor = Color.Blue;
            if (CA.ca210Connect(0) == true)
            {
                CA.ca210SetSyncMode(0);
                CA.ca210SetSpeed(1);
                //button1.Text = "Success";

                CA.ca210ZeroCal();
                //btnConnect->Caption = "Connect CA410";
            }
        }
        private void CA_Measure_Click(object sender, EventArgs e)
        {
            CA210DataStruct BOECA210;

            BOECA210 = CA.ca210Measure();

            //label1.Text = BOECA210.CA210fX.ToString();
            //label2.Text = BOECA210.CA210fY.ToString();
            //label3.Text = BOECA210.CA210fZ.ToString();
        }
        private void DDCCI_Write_Click(object sender, EventArgs e)
        {
            //int a;

            //if (int.TryParse(textBox1.Text, out a) == true)//字符转int
            {
             //   byte[] byte_buffer = new byte[] { 0x85, 0x03, 0xFB, 0x10, 0x00, (byte)a };
 
             //   DDCCI.BOEDCCCI_Write(byte_buffer);
            }
        }
        private void DDCCI_Read_Click(object sender, EventArgs e)
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
        private void Open_File_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"C:\Hupp";
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                uiTextBoxFile.Text = filePath;
            }
        }
        private void Save_File_Click(object sender, EventArgs e)
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
            string filePath = uiTextBoxFile.Text;

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

            DecodeError DecodeResult;
            DecodeResult = EDID.Decode(UnicodeText);

            if(DecodeResult == DecodeError.Success)
                for (int i = 0; i < 128; i++)
                {
                    // 转化输出两位十六进制字符
                    EDIDTextBox[i].Text = string.Format("{0:X2}", EDID.EDIDByteData[i]);
                }

            MessageBox.Show(DecodeResult.ToString(), "EDID解析");


        }
        private void TextBox_KeyPress_Check(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLower(e.KeyChar) && !char.IsUpper(e.KeyChar) && !char.IsNumber(e.KeyChar) && !(e.KeyChar == '\b'))
            {
                //对系统表示事件已处理，及跳过该事件
                e.Handled = true;
            }
        }
    }
}

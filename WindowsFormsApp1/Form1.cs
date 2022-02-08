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
using System.Configuration;

using Sunny.UI;
using EDIDApp;

namespace WindowsFormsApp_BOE_Tool
{
    public partial class Form1 : UIForm
    {
        Sunny.UI.UITextBox[] EDIDTextBox = new Sunny.UI.UITextBox[128];
        EDID FormEDID = new EDID();
        EDIDTable EDIDInfo = new EDIDTable();

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
                EDIDTextBox[i].Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                EDIDTextBox[i].Location = new System.Drawing.Point(22 + (i % 10) * (BoxSizeWeight + BoxSpaceX), 115 + (i / 10) * (BoxSizeHeight + BoxSpaceY));
                EDIDTextBox[i].Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
                EDIDTextBox[i].Maximum = 2147483647D;
                EDIDTextBox[i].Minimum = -2147483648D;
                EDIDTextBox[i].MinimumSize = new System.Drawing.Size(1, 16);
                EDIDTextBox[i].Name = "EDIDTextBox_" + i.ToString();
                EDIDTextBox[i].Size = new System.Drawing.Size(BoxSizeWeight, BoxSizeHeight);
                EDIDTextBox[i].Style = Sunny.UI.UIStyle.Custom;
                EDIDTextBox[i].TabStop = false;
                EDIDTextBox[i].Text = "";
                EDIDTextBox[i].TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                EDIDTextBox[i].ReadOnly = true;
                Controls.Add(EDIDTextBox[i]);
            }
        }
        private void Open_File_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "@" + System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.Filter = "txt files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string UnicodeText = "";
                uiTextBoxFile.Text = filePath;

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

                EDIDInfo = FormEDID.Decode(UnicodeText);

                if (EDIDInfo.Error == DecodeError.Success)
                {
                    uiRadioButtonGroup1.Items.Clear();
                    if (EDIDInfo.Length == 384)
                    {
                        uiRadioButtonGroup1.Items.AddRange(new object[] { "Base", "CEA", "DisplayID" });
                    }
                    if (EDIDInfo.Length == 256)
                    {
                        uiRadioButtonGroup1.Items.AddRange(new object[] { "Base", "CEA" });
                    }
                    if (EDIDInfo.Length == 128)
                    {
                        uiRadioButtonGroup1.Items.AddRange(new object[] { "Base" });
                    }

                    uiRadioButtonGroup1.SelectedIndex = 0;
                    for (int i = 0; i < 128; i++)
                    {
                        EDIDTextBox[i].Text = string.Format("{0:X2}", EDIDInfo.Data[i]);
                    }
                    uiRadioButtonGroup1.Visible = true;
                }
                else
                {
                    uiRadioButtonGroup1.Visible = false;
                    MessageBox.Show(EDIDInfo.Error.ToString(), "EDID解析错误");
                }
                //窗体关闭时，获取文件夹对话框的路径写入配置文件中
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = filePath;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
        private void Save_File_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "@" + ConfigurationManager.AppSettings["EDIDFilePath"];
            saveFileDialog1.RestoreDirectory= false;
            saveFileDialog1.Filter = ".txt文件(.txt)|*.txt |.h文件（.h）|*.h";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "EDID_test";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EDID FormEDID = new EDID();
                if (uiCheckBox1.Checked == true)
                    FormEDID.OutputNotesEDIDText(EDIDInfo, saveFileDialog1.FileName);
                else
                    FormEDID.Output0xEDIDText(EDIDInfo, saveFileDialog1.FileName);

                //窗体关闭时，获取文件夹对话框的路径写入配置文件中
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = saveFileDialog1.FileName;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
        private void uiRadioButtonGroup1_ValueChanged(object sender, int index, string text)
        {
            if (index == 0)
            {
                for (int i = 0; i < 128; i++)
                {
                    EDIDTextBox[i].Text = string.Format("{0:X2}", EDIDInfo.Data[i]);
                }
            }
            else if ((index == 1) && (EDIDInfo.Length >= 128))
            {
                for (int i = 0; i < 128; i++)
                {
                    EDIDTextBox[i].Text = string.Format("{0:X2}", EDIDInfo.Data[128 + i]);
                }
            }
            else if ((index == 2) && (EDIDInfo.Length >= 256))
            {
                for (int i = 0; i < 128; i++)
                {
                    EDIDTextBox[i].Text = string.Format("{0:X2}", EDIDInfo.Data[256 + i]);
                }
            }
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            EDID FormEDID = new EDID();
            FormEDID.Decompile(EDIDInfo);
        }
    }
}

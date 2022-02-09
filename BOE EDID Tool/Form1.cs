using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EDIDApp;

namespace BOE_Tool
{
    public partial class Form1 : Form
    {
        EDID FormEDID = new EDID();
        EDIDTable EDIDInfo = new EDIDTable();

        public Form1()
        {
            InitializeComponent();
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
                textBox1.Text = filePath;

                button4.Text = "解析";

                //窗体关闭时，获取文件夹对话框的路径写入配置文件中
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = filePath;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
        private void Decode_Click(object sender, EventArgs e)
        {
            string UnicodeText = "";
            using (FileStream fsRead = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read))
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

            if (EDIDInfo.Error != DecodeError.Success)
            {
                MessageBox.Show(EDIDInfo.Error.ToString(), "EDID解析错误");
            }
            else
            {
                button4.Text = "解析成功";
            }
        }
        private void Save_File_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = "@" + ConfigurationManager.AppSettings["EDIDFilePath"];
            saveFileDialog1.RestoreDirectory = false;
            saveFileDialog1.Filter = ".txt文件(.txt)|*.txt |.h文件（.h）|*.h";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "EDID_test";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EDID FormEDID = new EDID();
                if (checkBox1.Checked == true)
                    FormEDID.OutputNotesEDIDText(EDIDInfo, saveFileDialog1.FileName);
                else
                    FormEDID.Output0xEDIDText(EDIDInfo, saveFileDialog1.FileName);

                //窗体关闭时，获取文件夹对话框的路径写入配置文件中
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = saveFileDialog1.FileName;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }

        private void Decompile_Click(object sender, EventArgs e)
        {
            EDID FormEDID = new EDID();
            byte[] Decompile;
            Decompile = FormEDID.Decompile(EDIDInfo).Data;

            int i = 0;
            foreach (byte b in Decompile)
            {
                if(EDIDInfo.Data[i]!= Decompile[i])
                    Console.WriteLine("{0}: {1:X2} ,{2:X2}", i, EDIDInfo.Data[i], Decompile[i]);
                i++;
            }
        }
    }
}

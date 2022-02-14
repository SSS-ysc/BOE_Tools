//#define debug

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
#if debug
            button3.Visible = true;
#endif
        }
        private void Open_File_Click(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["EDIDFilePath"] == string.Empty)
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            else
                openFileDialog1.InitialDirectory = "@" + Path.GetDirectoryName(ConfigurationManager.AppSettings["EDIDFilePath"]);
            openFileDialog1.RestoreDirectory = false;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt;|dat files (*.dat)|*.dat;|h files (*.h)|*.h";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                textBox1.Text = filePath;
                button4.Text = "解析";
                EDIDInfo.Error = DecodeError.NoDecode;

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = filePath;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
        private void Decode_Click(object sender, EventArgs e)
        {
            if (Path.GetFileName(textBox1.Text) == string.Empty)
            {
                MessageBox.Show("路径无效", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
                MessageBox.Show(EDIDInfo.Error.ToString(), "解析错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                button4.Text = "解析成功";
            }
        }
        private void Save_File_Click(object sender, EventArgs e)
        {
            if (EDIDInfo.Error != DecodeError.Success)
            {
                MessageBox.Show("未解析", "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            saveFileDialog1.InitialDirectory = "@" + Path.GetDirectoryName(textBox1.Text);
            saveFileDialog1.RestoreDirectory = false;
            if (checkBox1.Checked == true)
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            else
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt;|h files (*.h)|*.h;|dat files (*.dat)|*.dat";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(textBox1.Text) + "_Analysis";
            saveFileDialog1.AddExtension = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EDID FormEDID = new EDID();
                if (checkBox1.Checked == true)
                    FormEDID.OutputNotesEDIDText(EDIDInfo, saveFileDialog1.FileName);
                else
                    FormEDID.Output0xEDIDText(EDIDInfo, saveFileDialog1.FileName);
            }
        }
        private void Decompile_Click(object sender, EventArgs e)
        {
            EDID FormEDID = new EDID();
            byte[] Decompile;
            Decompile = FormEDID.Decompile(EDIDInfo).Data;

#if debug
            int i = 0;
            foreach (byte b in Decompile)
            {
                if (EDIDInfo.Data[i] != Decompile[i])
                    Console.WriteLine("{0}>>{1}:{2:X2} Now:{3:X2}", i / 128, i % 128, EDIDInfo.Data[i], Decompile[i]);
                i++;
            }
            Console.WriteLine("Decompile End");
#endif
        }
        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("可解析任意格式的EDID数据\n选中“厂内格式”可保存为解析后的文本文档，否则保存为十六进制格式", "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

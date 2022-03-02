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
            checkBox1.Checked = ConfigurationManager.AppSettings["checkBox1"] == "1";
            checkBox2.Checked = ConfigurationManager.AppSettings["checkBox2"] == "1";
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
            openFileDialog1.Filter = "txt files (*.txt)|*.txt;|dat files (*.dat)|*.dat;|h files (*.h)|*.h;|rtd files (*.rtd)|*.rtd;|bin files (*.bin)|*.bin";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = textBox1.Text;
                config.Save(ConfigurationSaveMode.Modified);
            }

            if (Path.GetFileName(textBox1.Text) == string.Empty)
            {
                return;
            }
            //解析
            label1.Text = "";
            EDIDInfo.Error = DecodeError.NoDecode;
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
                EDID FormEDID = new EDID();
                if (checkBox1.Checked == true)
                    FormEDID.OutputNotesEDIDText(EDIDInfo, Path.GetDirectoryName(textBox1.Text) + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text) + "_Analysis" + ".txt");
                if (checkBox2.Checked == true)
                    FormEDID.Output0xEDIDText(EDIDInfo, Path.GetDirectoryName(textBox1.Text) + "\\" + Path.GetFileNameWithoutExtension(textBox1.Text) + "_Analysis" + ".c");

                if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
                    MessageBox.Show("解析成功，请选择保存格式", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    label1.Text = "解析成功并保存";
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["checkBox2"].Value = (checkBox2.Checked == true ? "1" : "0");
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["checkBox1"].Value = (checkBox1.Checked == true ? "1" : "0");
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}

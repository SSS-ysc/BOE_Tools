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
        EDIDTable EDIDInfo = new EDIDTable();

        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = ConfigurationManager.AppSettings["checkBox1"] == "1";
            checkBox2.Checked = ConfigurationManager.AppSettings["checkBox2"] == "1";

            openFileDialog1.Filter = "txt files (*.txt)|*.txt;|c files (*.c)|*.c;|h files (*.h)|*.h;|rtd files (*.rtd)|*.rtd;|bin files (*.bin)|*.bin;|dat files (*.dat)|*.dat;|* files(*.*)|*.*";
            if (ConfigurationManager.AppSettings["EDIDFilePath"] == string.Empty)
                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                openFileDialog1.InitialDirectory = "@" + ConfigurationManager.AppSettings["EDIDFilePath"];
            openFileDialog1.FilterIndex = int.Parse(ConfigurationManager.AppSettings["FilterIndex"]);
#if debug
            button3.Visible = true;
#endif
        }
        private void Open_File_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileNames[0];
                openFileDialog1.InitialDirectory = "@" + Path.GetDirectoryName(textBox1.Text);

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = Path.GetDirectoryName(textBox1.Text);
                config.AppSettings.Settings["FilterIndex"].Value = openFileDialog1.FilterIndex.ToString();
                config.Save(ConfigurationSaveMode.Modified);
            }

            foreach (string File in openFileDialog1.FileNames)
            {
                if (Path.GetFileName(File) == string.Empty)
                    return;

                EDIDInfo.Error = DecodeError.NoDecode;

                string UnicodeText = "";
                using (FileStream fsRead = new FileStream(File, FileMode.Open, FileAccess.Read))
                {
                    byte[] b = new byte[50];
                    while (true)
                    {
                        int r = fsRead.Read(b, 0, b.Length);
                        if (r == 0)
                            break;
                        if (Path.GetExtension(File) == ".bin")//纯二进制
                        {
                            for (int i = 0; i < r; i++)
                                UnicodeText += string.Format("0x{0:X2},", b[i]);
                        }
                        else
                            UnicodeText += Encoding.UTF8.GetString(b, 0, r);
                    }
                }

                EDID FormEDID = new EDID();
                EDIDInfo = FormEDID.Decode(UnicodeText);

                if (EDIDInfo.Error != DecodeError.Success)
                {
                    MessageBox.Show(Path.GetFileNameWithoutExtension(File) + "\r" + EDIDInfo.Error.ToString(), "解析错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (checkBox1.Checked == true)
                        FormEDID.OutputNotesEDIDText(EDIDInfo, Path.GetDirectoryName(File) + "\\" + Path.GetFileNameWithoutExtension(File) + "_Analysis" + ".txt");
                    if (checkBox2.Checked == true)
                        FormEDID.Output0xEDIDText(EDIDInfo, Path.GetDirectoryName(File) + "\\" + Path.GetFileNameWithoutExtension(File) + "_Analysis" + ".c");
                }
            }
            label1.Text = "解析成功并保存";
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
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if ((checkBox1.Checked == false) && (checkBox2.Checked == false))
            {
                if (sender == checkBox1)
                    checkBox2.Checked = true;
                else
                    checkBox1.Checked = true;
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["checkBox1"].Value = (checkBox1.Checked == true ? "1" : "0");
            config.AppSettings.Settings["checkBox2"].Value = (checkBox2.Checked == true ? "1" : "0");
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}

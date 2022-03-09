//#define debug

using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using EDIDApp;

namespace BOE_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = ConfigurationManager.AppSettings["checkBox1"] == "1";
            checkBox2.Checked = ConfigurationManager.AppSettings["checkBox2"] == "1";
            checkBox3.Checked = ConfigurationManager.AppSettings["AutoName"] == "1";

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
        private void FormDecodeEDID(string[] Files)
        {
            label1.Text = "";
            textBox1.Text = Files[0];

            foreach (string File in Files)
            {
                if (File.EndsWith(".txt") || File.EndsWith(".h") || File.EndsWith(".c") || File.EndsWith(".dat") || File.EndsWith(".rtd") || File.EndsWith(".bin"))
                {
                    EDID FormEDID = new EDID();
                    EDIDTable EDIDInfo = FormEDID.Decode(File);

                    if (EDIDInfo.Error != DecodeError.Success)
                    {
                        MessageBox.Show(Path.GetFileNameWithoutExtension(File) + "\r" + EDIDInfo.Error.ToString(), "解析错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        string FileName;
                        if (checkBox3.Checked == true)
                        {
                            FileName = Path.GetDirectoryName(File) + "\\" + "EDID for " + EDIDInfo.Base.IDManufacturerName + " " + EDIDInfo.Base.Name;
                        }
                        else
                            FileName = Path.GetDirectoryName(File) + "\\" + Path.GetFileNameWithoutExtension(File);

                        if (checkBox1.Checked == true)
                            FormEDID.OutputNotesEDIDText(EDIDInfo, FileName + "_Analysis.txt");
                        if (checkBox2.Checked == true)
                            FormEDID.Output0xEDIDText(EDIDInfo, FileName + ".c");
                    }
                }
                else
                {
                    MessageBox.Show(File + "\r" + "不支持的格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                label1.Text = "解析成功并保存";
            }
        }
        private void Open_File_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialog1.InitialDirectory = "@" + Path.GetDirectoryName(openFileDialog1.FileNames[0]);

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["EDIDFilePath"].Value = Path.GetDirectoryName(openFileDialog1.FileNames[0]);
                config.AppSettings.Settings["FilterIndex"].Value = openFileDialog1.FilterIndex.ToString();
                config.Save(ConfigurationSaveMode.Modified);
            }

            if (Path.GetFileName(openFileDialog1.FileNames[0]) == string.Empty)
                return;

            FormDecodeEDID(openFileDialog1.FileNames);
        }
        private void Decompile_Click(object sender, EventArgs e)
        {
#if debug
            EDID FormEDID = new EDID();
            byte[] Decompile;
            EDIDTable EDIDInfo;
            EDIDInfo = FormEDID.Decode(textBox1.Text);

            Decompile = FormEDID.Decompile(EDIDInfo).Data;

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
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
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
            config.AppSettings.Settings["AutoName"].Value = (checkBox3.Checked == true ? "1" : "0");
            config.Save(ConfigurationSaveMode.Modified);
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);

            FormDecodeEDID(Files);
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }
}

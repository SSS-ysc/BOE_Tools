using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace BOE_log_viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class Loglist
        {
            public string Log;
            public string LogItem1;
            public string LogItem2;
        }

        List<Loglist> Log = new List<Loglist>();
        List<Loglist> TextLog = new List<Loglist>();
        int VScrollNumber;
        int LogNumber;

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Log.Clear();
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName, Encoding.UTF8))
                {
                    sw.Start();
                    while (true)
                    {                
                        string line = sr.ReadLine();

                        if (line == null)
                            break;
                        else
                        {
                            Loglist log = new Loglist();

                            log.Log = line;
                            // 01-01 08:00:01.527 E/audio_hal( 1942): no pcm error
                            Match itemMatch = Regex.Match(line, @"(?<item1>[A-Z])\/(?<item2>\S*\s*)\(");
                            if (itemMatch.Success)
                            {
                                GroupCollection gc = itemMatch.Groups;
                                log.LogItem1 = gc["item1"].Value;
                                log.LogItem2 = gc["item2"].Value;

                                Log.Add(log);

                                if(comboBox1.Items.Contains(log.LogItem1) == false)
                                    comboBox1.Items.Add(log.LogItem1);
                            }
                        }
                    }
                    sw.Stop();
                    Console.WriteLine("Read:" + sw.ElapsedMilliseconds + "ms");

                    comboBox1.SelectedIndex = 0; 
                }
            }
        }
        private void Item2List_update()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("- - - - -");
            List<Loglist> list = Log.Where(x => x.LogItem1 == comboBox1.Text).ToList();
            List<string> stringlist = list.Select(x => x.LogItem2).ToList();

            foreach (string st in stringlist)
            {
                if (comboBox2.Items.Contains(st) == false)
                    comboBox2.Items.Add(st);
            }
            comboBox2.SelectedIndex = 1; // 间距调用 SelectedIndexChanged_Item2
        }
        private void TextBox_update()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            timer1.Enabled = false;
            VScrollNumber = 0;
            richTextBox1.Text = "";

            sw.Start();
            TextLog = Log.FindAll((Loglist finda) => (finda.LogItem1 == comboBox1.Text));
            if (comboBox2.SelectedIndex != 0)
                TextLog = TextLog.FindAll((Loglist findb) => (findb.LogItem2 == comboBox2.Text));
            LogNumber = TextLog.Count;
            sw.Stop();
            Console.WriteLine("FindAll:" + sw.ElapsedMilliseconds + "ms");

            while (VScrollNumber < 50)
            {
                if (VScrollNumber < LogNumber)
                {
                    richTextBox1.Text += TextLog[VScrollNumber].Log + "\n";
                    VScrollNumber++;
                }
                else
                    break;
            }
            timer1.Enabled = true;
            timer1.Interval = 100;
        }
        private void SelectedIndexChanged_Item1(object sender, EventArgs e)
        {
            Item2List_update();
        }
        private void SelectedIndexChanged_Item2(object sender, EventArgs e)
        {
            TextBox_update();
        }
        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            return;
            int Target = VScrollNumber + 10;
            while (VScrollNumber < Target)
            {
                if (VScrollNumber < LogNumber)
                {
                    richTextBox1.Text += TextLog[VScrollNumber].Log + "\n";
                    VScrollNumber++;
                }
                else
                {
                    timer1.Enabled = false; 
                    break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int Target = VScrollNumber + 10;
            while (VScrollNumber < Target)
            {
                if (VScrollNumber < LogNumber)
                {
                    richTextBox1.Text += TextLog[VScrollNumber].Log + "\n";
                    VScrollNumber++;
                }
                else
                {
                    timer1.Enabled = false;
                    break;
                }
            }
        }
    }
}

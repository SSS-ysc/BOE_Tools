namespace WindowsFormsApp_BOE_Tool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.uiSymbolButtonOpenFile = new Sunny.UI.UISymbolButton();
            this.uiSymbolButtonSaveFile = new Sunny.UI.UISymbolButton();
            this.uiSymbolButtonFarmot = new Sunny.UI.UISymbolButton();
            this.uiStyleManager1 = new Sunny.UI.UIStyleManager(this.components);
            this.uiTextBoxFile = new Sunny.UI.UITextBox();
            this.uiTabControlMenu1 = new Sunny.UI.UITabControlMenu();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.uiRadioButtonGroup1 = new Sunny.UI.UIRadioButtonGroup();
            this.uiTabControlMenu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // uiSymbolButtonOpenFile
            // 
            this.uiSymbolButtonOpenFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButtonOpenFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.Font = new System.Drawing.Font("微软雅黑", 8.64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonOpenFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.Location = new System.Drawing.Point(353, 49);
            this.uiSymbolButtonOpenFile.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButtonOpenFile.Name = "uiSymbolButtonOpenFile";
            this.uiSymbolButtonOpenFile.Radius = 10;
            this.uiSymbolButtonOpenFile.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.Size = new System.Drawing.Size(118, 34);
            this.uiSymbolButtonOpenFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButtonOpenFile.Symbol = 61717;
            this.uiSymbolButtonOpenFile.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.TabIndex = 15;
            this.uiSymbolButtonOpenFile.Text = "浏览文件";
            this.uiSymbolButtonOpenFile.TipsFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonOpenFile.Click += new System.EventHandler(this.Open_File_Click);
            // 
            // uiSymbolButtonSaveFile
            // 
            this.uiSymbolButtonSaveFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButtonSaveFile.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.Font = new System.Drawing.Font("微软雅黑", 8.64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonSaveFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.Location = new System.Drawing.Point(601, 49);
            this.uiSymbolButtonSaveFile.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButtonSaveFile.Name = "uiSymbolButtonSaveFile";
            this.uiSymbolButtonSaveFile.Radius = 10;
            this.uiSymbolButtonSaveFile.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.Size = new System.Drawing.Size(118, 34);
            this.uiSymbolButtonSaveFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButtonSaveFile.Symbol = 61639;
            this.uiSymbolButtonSaveFile.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.TabIndex = 16;
            this.uiSymbolButtonSaveFile.Text = "导出文件";
            this.uiSymbolButtonSaveFile.TipsFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonSaveFile.Click += new System.EventHandler(this.Save_File_Click);
            // 
            // uiSymbolButtonFarmot
            // 
            this.uiSymbolButtonFarmot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButtonFarmot.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.Font = new System.Drawing.Font("微软雅黑", 8.64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonFarmot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.Location = new System.Drawing.Point(477, 49);
            this.uiSymbolButtonFarmot.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButtonFarmot.Name = "uiSymbolButtonFarmot";
            this.uiSymbolButtonFarmot.Radius = 10;
            this.uiSymbolButtonFarmot.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.Size = new System.Drawing.Size(118, 34);
            this.uiSymbolButtonFarmot.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButtonFarmot.Symbol = 61473;
            this.uiSymbolButtonFarmot.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonFarmot.TabIndex = 17;
            this.uiSymbolButtonFarmot.Text = "文件解析";
            this.uiSymbolButtonFarmot.TipsFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonFarmot.Click += new System.EventHandler(this.Form_button_Click);
            // 
            // uiStyleManager1
            // 
            this.uiStyleManager1.DPIScale = true;
            this.uiStyleManager1.Style = Sunny.UI.UIStyle.LightBlue;
            // 
            // uiTextBoxFile
            // 
            this.uiTextBoxFile.ButtonFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiTextBoxFile.ButtonFillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiTextBoxFile.ButtonForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiTextBoxFile.ButtonRectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiTextBoxFile.ButtonSymbol = 61761;
            this.uiTextBoxFile.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiTextBoxFile.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiTextBoxFile.Font = new System.Drawing.Font("微软雅黑", 8.64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBoxFile.Location = new System.Drawing.Point(22, 49);
            this.uiTextBoxFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiTextBoxFile.Maximum = 2147483647D;
            this.uiTextBoxFile.Minimum = -2147483648D;
            this.uiTextBoxFile.MinimumSize = new System.Drawing.Size(1, 16);
            this.uiTextBoxFile.Name = "uiTextBoxFile";
            this.uiTextBoxFile.Radius = 10;
            this.uiTextBoxFile.Size = new System.Drawing.Size(311, 29);
            this.uiTextBoxFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiTextBoxFile.TabIndex = 18;
            this.uiTextBoxFile.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiTabControlMenu1
            // 
            this.uiTabControlMenu1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.uiTabControlMenu1.Controls.Add(this.tabPage1);
            this.uiTabControlMenu1.Controls.Add(this.tabPage2);
            this.uiTabControlMenu1.Controls.Add(this.tabPage3);
            this.uiTabControlMenu1.Controls.Add(this.tabPage4);
            this.uiTabControlMenu1.Controls.Add(this.tabPage5);
            this.uiTabControlMenu1.Controls.Add(this.tabPage6);
            this.uiTabControlMenu1.Controls.Add(this.tabPage7);
            this.uiTabControlMenu1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControlMenu1.FillColor = System.Drawing.Color.Transparent;
            this.uiTabControlMenu1.Font = new System.Drawing.Font("微软雅黑", 8.64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControlMenu1.ItemSize = new System.Drawing.Size(40, 150);
            this.uiTabControlMenu1.Location = new System.Drawing.Point(353, 160);
            this.uiTabControlMenu1.MenuStyle = Sunny.UI.UIMenuStyle.Custom;
            this.uiTabControlMenu1.Multiline = true;
            this.uiTabControlMenu1.Name = "uiTabControlMenu1";
            this.uiTabControlMenu1.SelectedIndex = 0;
            this.uiTabControlMenu1.Size = new System.Drawing.Size(632, 402);
            this.uiTabControlMenu1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControlMenu1.Style = Sunny.UI.UIStyle.Custom;
            this.uiTabControlMenu1.StyleCustomMode = true;
            this.uiTabControlMenu1.TabBackColor = System.Drawing.Color.White;
            this.uiTabControlMenu1.TabIndex = 19;
            this.uiTabControlMenu1.TabSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiTabControlMenu1.TabStop = false;
            this.uiTabControlMenu1.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Location = new System.Drawing.Point(151, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(481, 402);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Info";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Location = new System.Drawing.Point(151, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(481, 402);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Basic Params";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Location = new System.Drawing.Point(151, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(481, 402);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Color";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.Transparent;
            this.tabPage4.Location = new System.Drawing.Point(151, 0);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(481, 402);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Es/St Timing";
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.Color.Transparent;
            this.tabPage5.Location = new System.Drawing.Point(151, 0);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(481, 402);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Main Timing";
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.Color.Transparent;
            this.tabPage6.Location = new System.Drawing.Point(151, 0);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(481, 402);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Range Limits";
            // 
            // tabPage7
            // 
            this.tabPage7.BackColor = System.Drawing.Color.Transparent;
            this.tabPage7.Location = new System.Drawing.Point(151, 0);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(481, 402);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Product Info";
            // 
            // uiRadioButtonGroup1
            // 
            this.uiRadioButtonGroup1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.uiRadioButtonGroup1.ColumnCount = 3;
            this.uiRadioButtonGroup1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiRadioButtonGroup1.Font = new System.Drawing.Font("微软雅黑", 7.68F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiRadioButtonGroup1.Items.AddRange(new object[] {
            "Base",
            "CEA",
            "Display"});
            this.uiRadioButtonGroup1.ItemSize = new System.Drawing.Size(100, 15);
            this.uiRadioButtonGroup1.Location = new System.Drawing.Point(353, 91);
            this.uiRadioButtonGroup1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiRadioButtonGroup1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRadioButtonGroup1.Name = "uiRadioButtonGroup1";
            this.uiRadioButtonGroup1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiRadioButtonGroup1.Size = new System.Drawing.Size(318, 61);
            this.uiRadioButtonGroup1.StyleCustomMode = true;
            this.uiRadioButtonGroup1.TabIndex = 21;
            this.uiRadioButtonGroup1.TabStop = false;
            this.uiRadioButtonGroup1.Text = null;
            this.uiRadioButtonGroup1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1056, 793);
            this.ControlBoxCloseFillHoverColor = System.Drawing.Color.White;
            this.ControlBoxFillHoverColor = System.Drawing.Color.White;
            this.ControlBoxForeColor = System.Drawing.Color.Black;
            this.Controls.Add(this.uiRadioButtonGroup1);
            this.Controls.Add(this.uiTabControlMenu1);
            this.Controls.Add(this.uiTextBoxFile);
            this.Controls.Add(this.uiSymbolButtonFarmot);
            this.Controls.Add(this.uiSymbolButtonSaveFile);
            this.Controls.Add(this.uiSymbolButtonOpenFile);
            this.Font = new System.Drawing.Font("微软雅黑", 6.912F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 31, 0, 0);
            this.ShowRadius = false;
            this.ShowRect = false;
            this.ShowTitleIcon = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.StyleCustomMode = true;
            this.Text = "BOE EDID Tool V0.1";
            this.TitleFont = new System.Drawing.Font("微软雅黑", 6.912F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TitleHeight = 31;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.uiTabControlMenu1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Sunny.UI.UISymbolButton uiSymbolButtonOpenFile;
        private Sunny.UI.UISymbolButton uiSymbolButtonSaveFile;
        private Sunny.UI.UISymbolButton uiSymbolButtonFarmot;
        private Sunny.UI.UIStyleManager uiStyleManager1;
        private Sunny.UI.UITextBox uiTextBoxFile;
        private Sunny.UI.UITabControlMenu uiTabControlMenu1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.TabPage tabPage7;
        private Sunny.UI.UIRadioButtonGroup uiRadioButtonGroup1;
    }
}


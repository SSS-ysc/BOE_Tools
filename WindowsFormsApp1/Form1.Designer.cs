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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.uiSymbolButtonOpenFile = new Sunny.UI.UISymbolButton();
            this.uiSymbolButtonSaveFile = new Sunny.UI.UISymbolButton();
            this.uiStyleManager1 = new Sunny.UI.UIStyleManager(this.components);
            this.uiTextBoxFile = new Sunny.UI.UITextBox();
            this.uiRadioButtonGroup1 = new Sunny.UI.UIRadioButtonGroup();
            this.uiCheckBox1 = new Sunny.UI.UICheckBox();
            this.uiSymbolButton1 = new Sunny.UI.UISymbolButton();
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
            this.uiSymbolButtonOpenFile.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonOpenFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.Location = new System.Drawing.Point(348, 46);
            this.uiSymbolButtonOpenFile.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButtonOpenFile.Name = "uiSymbolButtonOpenFile";
            this.uiSymbolButtonOpenFile.Radius = 10;
            this.uiSymbolButtonOpenFile.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.Size = new System.Drawing.Size(101, 32);
            this.uiSymbolButtonOpenFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButtonOpenFile.Symbol = 61717;
            this.uiSymbolButtonOpenFile.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonOpenFile.TabIndex = 1;
            this.uiSymbolButtonOpenFile.TabStop = false;
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
            this.uiSymbolButtonSaveFile.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonSaveFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.Location = new System.Drawing.Point(455, 46);
            this.uiSymbolButtonSaveFile.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButtonSaveFile.Name = "uiSymbolButtonSaveFile";
            this.uiSymbolButtonSaveFile.Radius = 10;
            this.uiSymbolButtonSaveFile.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.Size = new System.Drawing.Size(101, 32);
            this.uiSymbolButtonSaveFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButtonSaveFile.Symbol = 61639;
            this.uiSymbolButtonSaveFile.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButtonSaveFile.TabIndex = 2;
            this.uiSymbolButtonSaveFile.TabStop = false;
            this.uiSymbolButtonSaveFile.Text = "导出文件";
            this.uiSymbolButtonSaveFile.TipsFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButtonSaveFile.Click += new System.EventHandler(this.Save_File_Click);
            // 
            // uiStyleManager1
            // 
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
            this.uiTextBoxFile.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTextBoxFile.Location = new System.Drawing.Point(19, 46);
            this.uiTextBoxFile.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.uiTextBoxFile.Maximum = 2147483647D;
            this.uiTextBoxFile.Minimum = -2147483648D;
            this.uiTextBoxFile.MinimumSize = new System.Drawing.Size(1, 15);
            this.uiTextBoxFile.Name = "uiTextBoxFile";
            this.uiTextBoxFile.Radius = 10;
            this.uiTextBoxFile.Size = new System.Drawing.Size(323, 27);
            this.uiTextBoxFile.Style = Sunny.UI.UIStyle.Custom;
            this.uiTextBoxFile.TabIndex = 0;
            this.uiTextBoxFile.TabStop = false;
            this.uiTextBoxFile.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiRadioButtonGroup1
            // 
            this.uiRadioButtonGroup1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.uiRadioButtonGroup1.ColumnCount = 3;
            this.uiRadioButtonGroup1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiRadioButtonGroup1.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiRadioButtonGroup1.Items.AddRange(new object[] {
            "Base",
            "CEA",
            "DisplayID"});
            this.uiRadioButtonGroup1.ItemSize = new System.Drawing.Size(90, 18);
            this.uiRadioButtonGroup1.Location = new System.Drawing.Point(348, 106);
            this.uiRadioButtonGroup1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.uiRadioButtonGroup1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiRadioButtonGroup1.Name = "uiRadioButtonGroup1";
            this.uiRadioButtonGroup1.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.uiRadioButtonGroup1.Size = new System.Drawing.Size(300, 58);
            this.uiRadioButtonGroup1.Style = Sunny.UI.UIStyle.Custom;
            this.uiRadioButtonGroup1.StyleCustomMode = true;
            this.uiRadioButtonGroup1.TabIndex = 4;
            this.uiRadioButtonGroup1.TabStop = false;
            this.uiRadioButtonGroup1.Text = null;
            this.uiRadioButtonGroup1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiRadioButtonGroup1.Visible = false;
            this.uiRadioButtonGroup1.ValueChanged += new Sunny.UI.UIRadioButtonGroup.OnValueChanged(this.uiRadioButtonGroup1_ValueChanged);
            // 
            // uiCheckBox1
            // 
            this.uiCheckBox1.Checked = true;
            this.uiCheckBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiCheckBox1.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.uiCheckBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiCheckBox1.Location = new System.Drawing.Point(562, 51);
            this.uiCheckBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiCheckBox1.Name = "uiCheckBox1";
            this.uiCheckBox1.Padding = new System.Windows.Forms.Padding(22, 0, 0, 0);
            this.uiCheckBox1.Size = new System.Drawing.Size(129, 27);
            this.uiCheckBox1.Style = Sunny.UI.UIStyle.Custom;
            this.uiCheckBox1.StyleCustomMode = true;
            this.uiCheckBox1.TabIndex = 3;
            this.uiCheckBox1.TabStop = false;
            this.uiCheckBox1.Text = "厂内格式";
            // 
            // uiSymbolButton1
            // 
            this.uiSymbolButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiSymbolButton1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.Location = new System.Drawing.Point(537, 427);
            this.uiSymbolButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiSymbolButton1.Name = "uiSymbolButton1";
            this.uiSymbolButton1.Radius = 10;
            this.uiSymbolButton1.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.Size = new System.Drawing.Size(111, 32);
            this.uiSymbolButton1.Style = Sunny.UI.UIStyle.Custom;
            this.uiSymbolButton1.Symbol = 61473;
            this.uiSymbolButton1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.uiSymbolButton1.TabIndex = 5;
            this.uiSymbolButton1.TabStop = false;
            this.uiSymbolButton1.Text = "Decompile";
            this.uiSymbolButton1.TipsFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiSymbolButton1.Click += new System.EventHandler(this.uiSymbolButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(678, 478);
            this.ControlBoxCloseFillHoverColor = System.Drawing.Color.White;
            this.ControlBoxFillHoverColor = System.Drawing.Color.White;
            this.ControlBoxForeColor = System.Drawing.Color.Black;
            this.Controls.Add(this.uiSymbolButton1);
            this.Controls.Add(this.uiCheckBox1);
            this.Controls.Add(this.uiRadioButtonGroup1);
            this.Controls.Add(this.uiTextBoxFile);
            this.Controls.Add(this.uiSymbolButtonSaveFile);
            this.Controls.Add(this.uiSymbolButtonOpenFile);
            this.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1851, 1224);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 31, 0, 0);
            this.ShowRadius = false;
            this.ShowRect = false;
            this.ShowTitleIcon = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.StyleCustomMode = true;
            this.Text = "BOE EDID Tool V1.0";
            this.TitleFont = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TitleHeight = 31;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Sunny.UI.UISymbolButton uiSymbolButtonOpenFile;
        private Sunny.UI.UISymbolButton uiSymbolButtonSaveFile;
        private Sunny.UI.UIStyleManager uiStyleManager1;
        private Sunny.UI.UITextBox uiTextBoxFile;
        private Sunny.UI.UIRadioButtonGroup uiRadioButtonGroup1;
        private Sunny.UI.UICheckBox uiCheckBox1;
        private Sunny.UI.UISymbolButton uiSymbolButton1;
    }
}


namespace Architecture_Kursak_WF
{
    partial class SimulatorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulatorForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusSimulatorLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbState = new System.Windows.Forms.ListView();
            this.IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Instruction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tbDecodeToDec = new System.Windows.Forms.TextBox();
            this.tbDecodeToHex = new System.Windows.Forms.TextBox();
            this.lRegDec = new System.Windows.Forms.Label();
            this.lRegHex = new System.Windows.Forms.Label();
            this.Registers = new System.Windows.Forms.GroupBox();
            this.RegistersList = new System.Windows.Forms.ListView();
            this.Register = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.MemoryList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbIndirrectAddressing = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbHexCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbArg3 = new System.Windows.Forms.TextBox();
            this.tbArg2 = new System.Windows.Forms.TextBox();
            this.tbArg1 = new System.Windows.Forms.TextBox();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbSF = new System.Windows.Forms.TextBox();
            this.tbZF = new System.Windows.Forms.TextBox();
            this.tbCF = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.Registers.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.DarkOrange;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusSimulatorLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 464);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(856, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusSimulatorLabel
            // 
            this.statusSimulatorLabel.ForeColor = System.Drawing.SystemColors.Info;
            this.statusSimulatorLabel.Name = "statusSimulatorLabel";
            this.statusSimulatorLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(856, 464);
            this.splitContainer1.SplitterDistance = 246;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbState);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 464);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "State list";
            // 
            // lbState
            // 
            this.lbState.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lbState.AutoArrange = false;
            this.lbState.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.State,
            this.IP,
            this.Instruction});
            this.lbState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbState.FullRowSelect = true;
            this.lbState.GridLines = true;
            this.lbState.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lbState.HideSelection = false;
            this.lbState.LabelWrap = false;
            this.lbState.Location = new System.Drawing.Point(3, 16);
            this.lbState.MultiSelect = false;
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(240, 445);
            this.lbState.TabIndex = 0;
            this.lbState.UseCompatibleStateImageBehavior = false;
            this.lbState.View = System.Windows.Forms.View.Details;
            this.lbState.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lbState_ItemSelectionChanged);
            this.lbState.SelectedIndexChanged += new System.EventHandler(this.lbState_SelectedIndexChanged);
            // 
            // IP
            // 
            this.IP.DisplayIndex = 0;
            this.IP.Text = "IP";
            this.IP.Width = 35;
            // 
            // Instruction
            // 
            this.Instruction.DisplayIndex = 1;
            this.Instruction.Text = "Instruction";
            this.Instruction.Width = 196;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.Registers);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(606, 464);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "State";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tbDecodeToDec);
            this.groupBox6.Controls.Add(this.tbDecodeToHex);
            this.groupBox6.Controls.Add(this.lRegDec);
            this.groupBox6.Controls.Add(this.lRegHex);
            this.groupBox6.Location = new System.Drawing.Point(6, 293);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(188, 86);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Decode number";
            // 
            // tbDecodeToDec
            // 
            this.tbDecodeToDec.Location = new System.Drawing.Point(51, 50);
            this.tbDecodeToDec.Name = "tbDecodeToDec";
            this.tbDecodeToDec.ReadOnly = true;
            this.tbDecodeToDec.Size = new System.Drawing.Size(116, 20);
            this.tbDecodeToDec.TabIndex = 3;
            // 
            // tbDecodeToHex
            // 
            this.tbDecodeToHex.Location = new System.Drawing.Point(51, 21);
            this.tbDecodeToHex.Name = "tbDecodeToHex";
            this.tbDecodeToHex.ReadOnly = true;
            this.tbDecodeToHex.Size = new System.Drawing.Size(116, 20);
            this.tbDecodeToHex.TabIndex = 2;
            // 
            // lRegDec
            // 
            this.lRegDec.AutoSize = true;
            this.lRegDec.Location = new System.Drawing.Point(10, 53);
            this.lRegDec.Name = "lRegDec";
            this.lRegDec.Size = new System.Drawing.Size(27, 13);
            this.lRegDec.TabIndex = 1;
            this.lRegDec.Text = "Dec";
            // 
            // lRegHex
            // 
            this.lRegHex.AutoSize = true;
            this.lRegHex.Location = new System.Drawing.Point(10, 29);
            this.lRegHex.Name = "lRegHex";
            this.lRegHex.Size = new System.Drawing.Size(26, 13);
            this.lRegHex.TabIndex = 0;
            this.lRegHex.Text = "Hex";
            // 
            // Registers
            // 
            this.Registers.Controls.Add(this.RegistersList);
            this.Registers.Dock = System.Windows.Forms.DockStyle.Right;
            this.Registers.Location = new System.Drawing.Point(200, 16);
            this.Registers.Name = "Registers";
            this.Registers.Size = new System.Drawing.Size(195, 445);
            this.Registers.TabIndex = 3;
            this.Registers.TabStop = false;
            this.Registers.Text = "Registers [0..63]";
            // 
            // RegistersList
            // 
            this.RegistersList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Register,
            this.Value});
            this.RegistersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegistersList.FullRowSelect = true;
            this.RegistersList.Location = new System.Drawing.Point(3, 16);
            this.RegistersList.MultiSelect = false;
            this.RegistersList.Name = "RegistersList";
            this.RegistersList.Size = new System.Drawing.Size(189, 426);
            this.RegistersList.TabIndex = 0;
            this.RegistersList.UseCompatibleStateImageBehavior = false;
            this.RegistersList.View = System.Windows.Forms.View.Details;
            this.RegistersList.SelectedIndexChanged += new System.EventHandler(this.RegistersList_SelectedIndexChanged);
            // 
            // Register
            // 
            this.Register.Text = "Register";
            this.Register.Width = 53;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 119;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.MemoryList);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox5.Location = new System.Drawing.Point(395, 16);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(208, 445);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Memory [0..16777215]";
            // 
            // MemoryList
            // 
            this.MemoryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.MemoryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MemoryList.FullRowSelect = true;
            this.MemoryList.Location = new System.Drawing.Point(3, 16);
            this.MemoryList.MultiSelect = false;
            this.MemoryList.Name = "MemoryList";
            this.MemoryList.Size = new System.Drawing.Size(202, 426);
            this.MemoryList.TabIndex = 0;
            this.MemoryList.UseCompatibleStateImageBehavior = false;
            this.MemoryList.View = System.Windows.Forms.View.Details;
            this.MemoryList.SelectedIndexChanged += new System.EventHandler(this.MemoryList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Memory";
            this.columnHeader1.Width = 51;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 135;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 385);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(185, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Report log to file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(314, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(162, 428);
            this.panel1.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbIndirrectAddressing);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.tbHexCode);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.tbArg3);
            this.groupBox4.Controls.Add(this.tbArg2);
            this.groupBox4.Controls.Add(this.tbArg1);
            this.groupBox4.Controls.Add(this.tbCode);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(6, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(188, 187);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Instruction";
            // 
            // tbIndirrectAddressing
            // 
            this.tbIndirrectAddressing.Location = new System.Drawing.Point(119, 157);
            this.tbIndirrectAddressing.Name = "tbIndirrectAddressing";
            this.tbIndirrectAddressing.ReadOnly = true;
            this.tbIndirrectAddressing.Size = new System.Drawing.Size(45, 20);
            this.tbIndirrectAddressing.TabIndex = 11;
            this.tbIndirrectAddressing.Text = "false";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 160);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Indirrect addressing";
            // 
            // tbHexCode
            // 
            this.tbHexCode.Location = new System.Drawing.Point(51, 22);
            this.tbHexCode.Name = "tbHexCode";
            this.tbHexCode.ReadOnly = true;
            this.tbHexCode.Size = new System.Drawing.Size(113, 20);
            this.tbHexCode.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Hex";
            // 
            // tbArg3
            // 
            this.tbArg3.Location = new System.Drawing.Point(51, 130);
            this.tbArg3.Name = "tbArg3";
            this.tbArg3.ReadOnly = true;
            this.tbArg3.Size = new System.Drawing.Size(113, 20);
            this.tbArg3.TabIndex = 7;
            // 
            // tbArg2
            // 
            this.tbArg2.Location = new System.Drawing.Point(51, 104);
            this.tbArg2.Name = "tbArg2";
            this.tbArg2.ReadOnly = true;
            this.tbArg2.Size = new System.Drawing.Size(113, 20);
            this.tbArg2.TabIndex = 6;
            // 
            // tbArg1
            // 
            this.tbArg1.Location = new System.Drawing.Point(51, 78);
            this.tbArg1.Name = "tbArg1";
            this.tbArg1.ReadOnly = true;
            this.tbArg1.Size = new System.Drawing.Size(113, 20);
            this.tbArg1.TabIndex = 5;
            // 
            // tbCode
            // 
            this.tbCode.Location = new System.Drawing.Point(51, 52);
            this.tbCode.Name = "tbCode";
            this.tbCode.ReadOnly = true;
            this.tbCode.Size = new System.Drawing.Size(113, 20);
            this.tbCode.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "arg3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 107);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "arg2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "arg1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Code";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbSF);
            this.groupBox3.Controls.Add(this.tbZF);
            this.groupBox3.Controls.Add(this.tbCF);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(6, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(93, 75);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Flags";
            // 
            // tbSF
            // 
            this.tbSF.Location = new System.Drawing.Point(36, 36);
            this.tbSF.Name = "tbSF";
            this.tbSF.ReadOnly = true;
            this.tbSF.Size = new System.Drawing.Size(17, 20);
            this.tbSF.TabIndex = 4;
            // 
            // tbZF
            // 
            this.tbZF.Location = new System.Drawing.Point(62, 36);
            this.tbZF.Name = "tbZF";
            this.tbZF.ReadOnly = true;
            this.tbZF.Size = new System.Drawing.Size(17, 20);
            this.tbZF.TabIndex = 5;
            // 
            // tbCF
            // 
            this.tbCF.Location = new System.Drawing.Point(10, 36);
            this.tbCF.Name = "tbCF";
            this.tbCF.ReadOnly = true;
            this.tbCF.Size = new System.Drawing.Size(17, 20);
            this.tbCF.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "ZF";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "SF";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CF";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // State
            // 
            this.State.DisplayIndex = 2;
            this.State.Width = 0;
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 486);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimulatorForm";
            this.Text = "SimulatorForm";
            this.Load += new System.EventHandler(this.SimulatorForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.Registers.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripStatusLabel statusSimulatorLabel;
        private System.Windows.Forms.ListView lbState;
        private System.Windows.Forms.ColumnHeader IP;
        private System.Windows.Forms.ColumnHeader Instruction;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListView MemoryList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox Registers;
        private System.Windows.Forms.ListView RegistersList;
        private System.Windows.Forms.ColumnHeader Register;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.TextBox tbZF;
        private System.Windows.Forms.TextBox tbSF;
        private System.Windows.Forms.TextBox tbCF;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbArg3;
        private System.Windows.Forms.TextBox tbArg2;
        private System.Windows.Forms.TextBox tbArg1;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbHexCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbIndirrectAddressing;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lRegDec;
        private System.Windows.Forms.Label lRegHex;
        private System.Windows.Forms.TextBox tbDecodeToDec;
        private System.Windows.Forms.TextBox tbDecodeToHex;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ColumnHeader State;

    }
}
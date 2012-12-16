namespace Architecture_Kursak_WF
{
    partial class AssemblerForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssemblerForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fastSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerPanels = new System.Windows.Forms.SplitContainer();
            this.tbCodeEditor = new System.Windows.Forms.RichTextBox();
            this.tabSymbolList = new System.Windows.Forms.TabControl();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rtbSymbolMap = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.assemblerFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.assemblerFormBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPanels)).BeginInit();
            this.splitContainerPanels.Panel1.SuspendLayout();
            this.splitContainerPanels.Panel2.SuspendLayout();
            this.splitContainerPanels.SuspendLayout();
            this.tabSymbolList.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assemblerFormBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.assemblerFormBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.assembleToolStripMenuItem,
            this.simulateToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.toolStripMenuItem1,
            this.fastSaveToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(770, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.createToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.createToolStripMenuItem.Text = "Create";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "Save as";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // assembleToolStripMenuItem
            // 
            this.assembleToolStripMenuItem.Name = "assembleToolStripMenuItem";
            this.assembleToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.assembleToolStripMenuItem.Text = "Assemble";
            this.assembleToolStripMenuItem.Click += new System.EventHandler(this.assembleToolStripMenuItem_Click);
            // 
            // simulateToolStripMenuItem
            // 
            this.simulateToolStripMenuItem.Name = "simulateToolStripMenuItem";
            this.simulateToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.simulateToolStripMenuItem.Text = "Simulate";
            this.simulateToolStripMenuItem.Click += new System.EventHandler(this.simulateToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Enabled = false;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(22, 20);
            this.toolStripMenuItem1.Text = "|";
            // 
            // fastSaveToolStripMenuItem
            // 
            this.fastSaveToolStripMenuItem.Name = "fastSaveToolStripMenuItem";
            this.fastSaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fastSaveToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.fastSaveToolStripMenuItem.Text = "Fast save";
            this.fastSaveToolStripMenuItem.Click += new System.EventHandler(this.fastSaveToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.DarkOrange;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(770, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.Info;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainerPanels
            // 
            this.splitContainerPanels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPanels.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPanels.Name = "splitContainerPanels";
            this.splitContainerPanels.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPanels.Panel1
            // 
            this.splitContainerPanels.Panel1.Controls.Add(this.tbCodeEditor);
            // 
            // splitContainerPanels.Panel2
            // 
            this.splitContainerPanels.Panel2.Controls.Add(this.tabSymbolList);
            this.splitContainerPanels.Size = new System.Drawing.Size(770, 402);
            this.splitContainerPanels.SplitterDistance = 245;
            this.splitContainerPanels.TabIndex = 3;
            // 
            // tbCodeEditor
            // 
            this.tbCodeEditor.AcceptsTab = true;
            this.tbCodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCodeEditor.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbCodeEditor.Location = new System.Drawing.Point(0, 0);
            this.tbCodeEditor.Name = "tbCodeEditor";
            this.tbCodeEditor.Size = new System.Drawing.Size(770, 245);
            this.tbCodeEditor.TabIndex = 2;
            this.tbCodeEditor.Text = "";
            this.tbCodeEditor.VScroll += new System.EventHandler(this.tbCodeEditor_VScroll);
            this.tbCodeEditor.ClientSizeChanged += new System.EventHandler(this.tbCodeEditor_ClientSizeChanged);
            this.tbCodeEditor.SizeChanged += new System.EventHandler(this.tbCodeEditor_SizeChanged);
            this.tbCodeEditor.TextChanged += new System.EventHandler(this.tbCodeEditor_TextChanged);
            // 
            // tabSymbolList
            // 
            this.tabSymbolList.Controls.Add(this.tabConsole);
            this.tabSymbolList.Controls.Add(this.tabPage2);
            this.tabSymbolList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSymbolList.Location = new System.Drawing.Point(0, 0);
            this.tabSymbolList.Name = "tabSymbolList";
            this.tabSymbolList.SelectedIndex = 0;
            this.tabSymbolList.Size = new System.Drawing.Size(770, 153);
            this.tabSymbolList.TabIndex = 0;
            this.tabSymbolList.Tag = "";
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.rtbConsole);
            this.tabConsole.Location = new System.Drawing.Point(4, 22);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsole.Size = new System.Drawing.Size(762, 127);
            this.tabConsole.TabIndex = 0;
            this.tabConsole.Text = "Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // rtbConsole
            // 
            this.rtbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbConsole.Location = new System.Drawing.Point(3, 3);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(756, 121);
            this.rtbConsole.TabIndex = 0;
            this.rtbConsole.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rtbSymbolMap);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(762, 127);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Symbol map";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtbSymbolMap
            // 
            this.rtbSymbolMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSymbolMap.Location = new System.Drawing.Point(3, 3);
            this.rtbSymbolMap.Name = "rtbSymbolMap";
            this.rtbSymbolMap.ReadOnly = true;
            this.rtbSymbolMap.Size = new System.Drawing.Size(756, 121);
            this.rtbSymbolMap.TabIndex = 0;
            this.rtbSymbolMap.Text = "";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.splitContainerPanels);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(770, 402);
            this.panel1.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Assemble files (*.as)|*.as|Machine code files (*.mc)|*.mc|Text files (*.txt)|*.tx" +
    "t|All files (*.*)|*.*";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "*.as";
            this.saveFileDialog1.Filter = "Assemble files (*.as)|*.as|Machine code files (*.mc)|*.mc|Text files (*.txt)|*.tx" +
    "t|All files (*.*)|*.*";
            // 
            // assemblerFormBindingSource
            // 
            this.assemblerFormBindingSource.DataSource = typeof(Architecture_Kursak_WF.AssemblerForm);
            // 
            // assemblerFormBindingSource1
            // 
            this.assemblerFormBindingSource1.DataSource = typeof(Architecture_Kursak_WF.AssemblerForm);
            // 
            // AssemblerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 448);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AssemblerForm";
            this.Text = "Computer architecture - assembler & simulator - Created by Oleg Chaplya, 2012";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainerPanels.Panel1.ResumeLayout(false);
            this.splitContainerPanels.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerPanels)).EndInit();
            this.splitContainerPanels.ResumeLayout(false);
            this.tabSymbolList.ResumeLayout(false);
            this.tabConsole.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.assemblerFormBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.assemblerFormBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assembleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulateToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainerPanels;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TabControl tabSymbolList;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.RichTextBox rtbConsole;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtbSymbolMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fastSaveToolStripMenuItem;
        private System.Windows.Forms.RichTextBox tbCodeEditor;
        private System.Windows.Forms.BindingSource assemblerFormBindingSource;
        private System.Windows.Forms.BindingSource assemblerFormBindingSource1;
    }
}


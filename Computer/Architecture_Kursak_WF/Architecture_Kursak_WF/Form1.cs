﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assembler;

namespace Architecture_Kursak_WF
{
    public partial class Form1 : Form
    {
        public String filePath = "";
        StreamWriter sw;
        StreamReader sr;
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ( openFileDialog1.ShowDialog() == DialogResult.OK )
                {
                    sr = new StreamReader(openFileDialog1.FileName);
                    tbCodeEditor.Text = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();

                    filePath = openFileDialog1.FileName;
                    toolStripStatusLabel1.Text = "Opened: " + openFileDialog1.FileName;
                    statusStrip1.BackColor = Color.DarkOrange;
                }
            }
            catch ( IOException IOEx )
            {
                toolStripStatusLabel1.Text = "IO Error";
                statusStrip1.BackColor = Color.Red;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
                {
                    sw = new StreamWriter(saveFileDialog1.FileName);
                    sw.Write(tbCodeEditor.Text);
                    sw.Close();

                    filePath = saveFileDialog1.FileName;
                    toolStripStatusLabel1.Text = "Saved to: " + saveFileDialog1.FileName;
                    statusStrip1.BackColor = Color.DarkOrange;
                    sw.Close();
                }
            }
            catch ( IOException IOEx )
            {
                toolStripStatusLabel1.Text = "IO Error";
                statusStrip1.BackColor = Color.Red;
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbCodeEditor.Clear();
        }

        private void assembleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ( filePath != "" )
                {
                    StreamWriter sw = new StreamWriter(filePath);

                    sw.Write(tbCodeEditor.Text);
                    sw.Close();
                    sw.Dispose();
                    toolStripStatusLabel1.Text = "Saved to: " + filePath;

                    assemble();
                }
                else if ( openFileDialog1.ShowDialog() == DialogResult.OK )
                {
                    StreamReader sw = new StreamReader(openFileDialog1.FileName);
                    tbCodeEditor.Text = sw.ReadToEnd();
                    sw.Close();
                    sw.Dispose();
                    filePath = openFileDialog1.FileName;
                    toolStripStatusLabel1.Text = "Opened: " + openFileDialog1.FileName;

                    assemble();
                }
                
            }
            catch ( IOException IOEx )
            {
                toolStripStatusLabel1.Text = "IO Error";
                statusStrip1.BackColor = Color.Red;
            }
        }
        public void assemble() 
        {
            String[] args = new String[2];

            args[0] = filePath;
            args[1] = filePath.Substring(0, filePath.Length - 3) + ".mc";
            rtbConsole.Text = "";

            AssemblerClass assembler = new AssemblerClass(args);
            var a = assembler.readCodeAndCheck();
            if ( a.Count > 0 )
            {
                var b = assembler.createMachineCode();

                foreach ( var i in b )
                {
                    MatchCollection s = new Regex( @"\w+\b" ).Matches(i);
                    rtbConsole.Text += s[0].ToString() + " = " + s[1].ToString() + '\n';
                }
                toolStripStatusLabel1.Text = "Assembled successfully";
                statusStrip1.BackColor = Color.ForestGreen;

                Dictionary<String, int> sml = assembler.symbolMapList;
                rtbSymbolMap.Clear();
                foreach ( var i in sml )
                {
                    rtbSymbolMap.Text += i.Value + " = " + i.Key + '\n';
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Assembled with errors";
                statusStrip1.BackColor = Color.Red;
                foreach ( var i in assembler.getErrorList() )
                    rtbConsole.Text += i + "\n";
            }
            // end assembling
        }

        private void simulateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create new form and simulate machine codes
            SimulatorForm sf = new SimulatorForm(filePath);
            sf.Show();
        }

        private void fastSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ( filePath != "" )
                {

                    sw = new StreamWriter(filePath);

                    sw.Write(tbCodeEditor.Text);
                    sw.Close();

                    toolStripStatusLabel1.Text = "Saved to: " + filePath;
                    statusStrip1.BackColor = Color.DarkOrange;
                    sw.Close();
                }
                else if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
                {
                    sw = new StreamWriter(saveFileDialog1.FileName);
                    sw.Write(tbCodeEditor.Text);
                    sw.Close();

                    filePath = saveFileDialog1.FileName;
                    toolStripStatusLabel1.Text = "Saved to: " + saveFileDialog1.FileName;
                    statusStrip1.BackColor = Color.DarkOrange;
                    sw.Close();
                }
            }
            catch ( IOException IOEx )
            {
                toolStripStatusLabel1.Text = "IO Error";
                statusStrip1.BackColor = Color.Red;
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm hl = new HelpForm();
            hl.Show();
        }
        
    }
}

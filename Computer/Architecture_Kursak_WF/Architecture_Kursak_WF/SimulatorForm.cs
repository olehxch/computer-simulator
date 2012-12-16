using System;
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
using Simulator;

namespace Architecture_Kursak_WF
{
    public partial class SimulatorForm : Form
    {
        public String filePath
        {
            get;
            set;
        }

        private List<Simulator.SimulatorClass.StateClass> stateList;

        public SimulatorForm(String path)
        {
            InitializeComponent();
            if ( path != "" )
            {
                filePath = path.Substring(0, path.Length - 3) + ".mc";
                statusSimulatorLabel.Text = "Opened: " + filePath;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedState = lbState.SelectedItems;
        }

        private void SimulatorForm_Load(object sender, EventArgs e)
        {
            if (( filePath != "") && (filePath != null) )
            {
                String[] arg = new String[1];
                arg[0] = filePath;
                SimulatorClass sml = new SimulatorClass(arg);
                sml.runCode();
                stateList = sml.getStateList();
                int statenum = 0;

                lbState.Items.Clear();
                ListViewItem lvi;
                try
                {
                    if ( stateList.Count < 500 )
                    {
                        lbState.BeginUpdate();
                        foreach ( var i in stateList )
                        {
                            if ( i.instructionLine == "Initial Status" )
                            {
                                lvi = new ListViewItem(new String[] { statenum.ToString(), i.ip.ToString(), i.instructionLine });

                                lbState.Items.Add(lvi);
                                statenum++;
                            }
                            else
                            {
                                Int64 inum = Convert.ToInt64(i.instructionLine);

                                String fhex = String.Format("{0:X}", inum);
                                lvi = new ListViewItem(new String[] { statenum.ToString(), i.ip.ToString(), i.instructionLine });

                                lbState.Items.Add(lvi);
                                statenum++;
                            }
                        }
                        lbState.EndUpdate();
                    }
                } catch( OutOfMemoryException ex )
                {
                    MessageBox.Show("Memory out of bounds", "Memory out of bounds", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
               
            }
        }

        private void lbState_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lbState_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if ( lbState.FocusedItem != null )
            {
                var i = stateList[ Convert.ToInt32(lbState.FocusedItem.Text) ];
                tbCF.Text = i.flags.CF.ToString();
                tbZF.Text = i.flags.ZF.ToString();
                tbSF.Text = i.flags.SF.ToString();
                tbCode.Text = i.instruction.instruction.ToString();
                tbArg1.Text = i.instruction.arg1.ToString();
                tbArg2.Text = i.instruction.arg2.ToString();
                tbArg3.Text = i.instruction.arg3.ToString();
                tbIndirrectAddressing.Text = i.IndirrectAddressing.ToString();

                if ( i.instructionLine != "Initial Status" )
                {
                    Int64 inum = Convert.ToInt64(i.instructionLine);
                    tbHexCode.Text = String.Format("{0:X}", inum);
                }
                int pos = 0;

                RegistersList.Items.Clear();
                foreach ( var ireg in i.reg )
                {
                    RegistersList.Items.Add(new ListViewItem(new String[] { pos.ToString(), ireg.ToString() }));
                    pos++;
                }

                pos = 0;
                MemoryList.Items.Clear();
                foreach ( var ireg in i.mem )
                {
                    MemoryList.Items.Add(new ListViewItem(new String[] { pos.ToString(), ireg.ToString() }));
                    pos++;
                    if ( pos == 64 ) break;
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // save to log file
            try
            {
                if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                    int pos = 0;
                    foreach ( var i in stateList )
                    {
                        sw.WriteLine("------------------------------------------------\nState: " + pos + "\tInstruction pointer = " + i.ip);
                        sw.WriteLine("Instruction: " + i.instructionLine);
                        sw.WriteLine("Flags: CF=" + i.flags.CF + " SF=" + i.flags.SF + " ZF=" + i.flags.ZF);
                        sw.WriteLine("\tRegister\tMemory");
                        int regp = 0;
                        foreach ( var r in i.reg )
                        {
                            sw.WriteLine("\treg[" + regp + "]=" + i.reg[regp] + "\t mem[" + regp + "]=" + i.mem[regp] );
                            regp++;
                        }
                        
                        pos++;
                    }
                    sw.Close();

                    filePath = saveFileDialog1.FileName;
                    statusSimulatorLabel.Text = "Saved to: " + saveFileDialog1.FileName;
                    statusStrip1.BackColor = Color.DarkOrange;
                    sw.Close();
                }
            }
            catch ( IOException IOEx )
            {
                statusSimulatorLabel.Text = "IO Error";
                statusStrip1.BackColor = Color.Red;
            }
        }

        private void RegistersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ( lbState.FocusedItem.SubItems[1].Text != null )
                {
                    long i = Convert.ToInt64(RegistersList.FocusedItem.SubItems[1].Text);
                    long x = convertInt48ToInt64(i);
                    tbDecodeToHex.Text = Regex.Replace(String.Format("{0:X8}", x), "([0-9A-F]{4})(?!$)", "$1 ");
                    tbDecodeToDec.Text = x.ToString();
                }
            }
            catch ( Exception ex ) { }
        }

        private void MemoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ( MemoryList.FocusedItem.SubItems[1].Text != null )
                {
                    long i = Convert.ToInt64(MemoryList.FocusedItem.SubItems[1].Text);
                    long x = convertInt48ToInt64(i);

                    tbDecodeToHex.Text = Regex.Replace(String.Format("{0:X8}", x), "([0-9A-F]{4})(?!$)", "$1 ");
                    tbDecodeToDec.Text = x.ToString();
                }
            }
            catch ( Exception ex ) { }
        }

        private long convertInt48ToInt64(long num)
        {
            bool sign = Convert.ToBoolean(num & 0x800000000000);
            if ( sign ) // -num
            {
                String s = String.Format("{0:X}", num).Insert(0, "FFFF");
                long x = Convert.ToInt64(s, 16);
                return x;
            }
            else // +num
            {
                return Convert.ToInt64(num);
            }
        }
    }
}

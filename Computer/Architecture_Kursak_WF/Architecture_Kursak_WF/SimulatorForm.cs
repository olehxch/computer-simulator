using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
                String x = lbState.FocusedItem.SubItems[2].Text;
                foreach ( var i in stateList )
                {
                    if ( i.instructionLine == x )
                    {
                        tbCF.Text = i.flags.CF.ToString();
                        tbZF.Text = i.flags.ZF.ToString();
                        tbSF.Text = i.flags.SF.ToString();
                        tbCode.Text = i.instruction.instruction.ToString();
                        tbArg1.Text = i.instruction.arg1.ToString();
                        tbArg2.Text = i.instruction.arg2.ToString();
                        tbArg3.Text = i.instruction.arg3.ToString();

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
                        break;
                    }
                }
            }
        }
    }
}

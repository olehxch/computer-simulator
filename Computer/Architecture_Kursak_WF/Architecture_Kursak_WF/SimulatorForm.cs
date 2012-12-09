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
            filePath = path.Substring(0, path.Length - 3) + ".mc";
            statusSimulatorLabel.Text = "Opened: " + filePath;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SimulatorForm_Load(object sender, EventArgs e)
        {
            String[] arg = new String[1];
            arg[0] = filePath;
            SimulatorClass sml = new SimulatorClass( arg );
            sml.runCode();
            stateList = sml.getStateList();
            int statenum = 0;
            //StreamWriter flags = new StreamWriter("log.txt");
            lbState.Items.Clear();
            foreach ( var i in stateList )
            {
                lbState.Items.Add(i.instructionLine); 
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if ( args.Length == 1 )
            {
                SimulatorClass sml = new SimulatorClass(args);
                sml.runCode();
                var st = sml.getStateList();
                int statenum = 0;
                StreamWriter f = new StreamWriter("log.txt");
                foreach ( var i in st )
                {
                    f.WriteLine("{ state=" + statenum);
                    f.WriteLine("ip=" + i.ip);
                    int pos = 0;
                    foreach ( var ri in i.reg )
                    {
                        f.WriteLine( "reg[" + pos + "]=" + ri + ";  ");
                        pos++;
                        if ( pos == 16 ) break;
                    }

                    pos = 0;
                    foreach ( var ri in i.reg )
                    {
                        f.WriteLine("mem[" + pos + "]=" + ri + ";  ");
                        pos++;
                        if ( pos == 16 ) break;
                    }

                    f.WriteLine("\nCF=" + i.f.CF + "\nSF=" + i.f.SF + "\nZF=" + i.f.ZF);
                    f.WriteLine("}");
                    statenum++;
                }

                f.Close();
            }
            else
            {
                // show error
                Console.Out.Write("error: <machine-code-file>\n");
                Console.ReadKey();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Assembler
{
    class Program
    {
        // argv[0] -> input program
        // argv[1] -> output machine code
        static void Main(String[] args)
        {
            // main assembler class
                      
            if (args.Length == 2)
            {
                AssemblerClass assembler = new AssemblerClass(args);
                var a = assembler.readCodeAndCheck();
                if ( a.Count > 0 )
                {
                   
                    foreach ( var i in a )
                        Console.WriteLine(i);
                    var b = assembler.createMachineCode();

                    foreach ( var i in b )
                        Console.WriteLine(i);
                }
                else
                {
                    Console.WriteLine("Assembled with errors: ");
                    foreach ( var i in assembler.getErrorList() )
                        Console.WriteLine(i);
                }
                // end assembling
            }
            else
            {
		        // show error
		        Console.Out.Write("error: <assembly-code-file> <machine-code-file>\n");
                Console.ReadKey();
	        }
        }
    }
}

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
        class AssemblerClass
        {
            const int MAXLINELENGTH = 16777216;
            const int DATABUS = 48;
            const int REGISTERS = 64;
            const int MAXLABELLENGHT = 6;

            // addressing
            // | INSTRUCTION 31-28 | OPERAND1 27-16 | OPERAND2 26-8 | OPERAND3 7-0|
            const int INSTRUCTION_MEM_SHIFT = 28;
            const int OPERAND1_MEM_SHIFT = 16;
            const int OPERAND2_MEM_SHIFT = 8;
            const int OPERAND3_MEM_SHIFT = 0;
            // arithmetic instructions
            const int HALT = 0xF;
            const int DEC = 1;              // DEC regA
            const int DIV = 2;              // DIV regA regB destreg
            const int XIMUL = 3;            // XIMUL regA regB destreg
            // logic instructions
            const int XOR = 4;              // XOR regA regB destreg
            const int SHL = 5;              // SHL regA regB destreg
            const int MOV = 6;              // MOV regA destreg
            // control instructions
            const int JMAE = 7;             // regA regB offset
            const int JMNGE = 8;            // regA regB offset
            // flags
            const int BT = 9;               // regA regB
            const int CMP = 10;             // regA regB
            const int RCL = 11;             // regA regB destreg

            // instruction list
            String[] INSTRUCTIONSARRAY = { "HALT", "DEC", "DIV", "XIMUL", "XOR", "SHL", "MOV", "JMAE", "JMNGE", ".FILL", "BT", "CMP", "RCL" };

            // symbol map list
            Dictionary<String, int> symbolMapList = new Dictionary<String, int>();

            // error states
            public bool ERRORSTATE = false;
            public int COUNTERRORS = 0;

            // path to assembly file
            String pathToAS = "";
            // path to mashine code file
            String pathToMC = "";

            testInstructions iTestInstructions;

            // construction method
            public AssemblerClass(String arg1 = "", String arg2 = "")
            {
                pathToAS = arg1;
                pathToMC = arg2;
                iTestInstructions = new testInstructions();
            }

            // create symbol map
            public bool createSymbolMap(int pos, String label)
            {
                // if there is a label...
                if ( label != "" )
                {
                    // if label is starting with letter...
                    if ( (Char.IsLetter(label, 0)) )
                    {
                        // if label length is smaller than MAXLABELLENGHT...
                        if ( (label.Length <= MAXLABELLENGHT) )
                        {
                            int outpos;
                            // and check if label exists...
                            if ( symbolMapList.TryGetValue(label, out outpos) )
                            {
                                showLineAndReadKey("::Two or more labels have equal names...");
                                return false;
                            }
                            else
                            {
                                // add label and ist position to Symbol Map List
                                symbolMapList[label] = pos;
                                return true;
                            }
                        }
                        else
                        {
                            this.showErrorLine("::Label length \'" + label + "\' has more than " + MAXLABELLENGHT + " letters at position " + pos);
                            return false;
                        }
                    }
                    else
                    {
                        this.showErrorLine("::Label \'" + label + "\' is not starting with letter at position " + pos);
                        return false;
                    }
                }
                return false;
            }

            // check for invalid instruction
            public void checkForInvalidInstruction(int pos, String instruction)
            {
                instruction = instruction.ToUpper();
                if ( !INSTRUCTIONSARRAY.Any(instruction.Contains) )
                {
                    showErrorLine("::Invalid instruction \'" + instruction[1] + "\' at " + pos);
                }
            }

            // check for enough arguments
            public void checkForEnoughtParametrs(int pos, String[] instruction)
            {
                // 3 arguments
                if ( (instruction[1] == "DIV") || (instruction[1] == "XIMUL") || (instruction[1] == "XOR") || (instruction[1] == "RCL") ||
                    (instruction[1] == "SHL") || (instruction[1] == "JMAE") || (instruction[1] == "JMNGE") )
                {
                    if ( instruction.Length >= 5 )
                    {
                        String[] iArgs = { instruction[2], instruction[3], instruction[4] };
                        if ( !iTestInstructions.testArgs(iArgs) || !iTestInstructions.testAddr(iArgs) )
                        {
                            showErrorLine("::Bad arguments at position " + pos);
                        }
                    }
                    else
                    {
                        showErrorLine("::Bad arguments at position " + pos);
                    }
                }
                // 2 arguments
                if ( (instruction[1] == "MOV") || (instruction[1] == "BT") || (instruction[1] == "CMP") )
                {
                    if ( instruction.Length >= 4 )
                    {
                        String[] iArgs = { instruction[2], instruction[3] };
                        if ( !iTestInstructions.testArgs(iArgs) || !iTestInstructions.testAddr(iArgs) )
                        {
                            showErrorLine("::Bad arguments at position " + pos);
                        }
                    }
                    else
                    {
                        showErrorLine("::Bad arguments at position " + pos);
                    }
                }
                // 1 arguments
                if ( (instruction[1] == "DEC") )
                {
                    if ( instruction.Length >= 3 )
                    {
                        String[] iArgs = { instruction[2] };
                        if ( !iTestInstructions.testArgs(iArgs) || !iTestInstructions.testAddr(iArgs) )
                        {
                            showErrorLine("::Bad arguments at position " + pos);
                        }
                    }
                    else
                    {
                        showErrorLine("::Bad arguments at position " + pos);
                    }
                }
            }

            public int createMachineCode(String[] instruction)
            {
                int com = 0;
                if ( instruction[1] == "DEC" )
                    com = (DEC << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT));
                else if ( instruction[1] == "DIV" )
                {
                    if ( new Regex(@"[a-zA-Z]+").IsMatch(instruction[4]) )
                    {
                        return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                    }
                    else return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(instruction[4]) << OPERAND3_MEM_SHIFT);

                }
                else if ( instruction[1] == "XIMUL" )
                    com = (XIMUL << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | (Convert.ToInt32(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "XOR" )
                    com = (XOR << INSTRUCTION_MEM_SHIFT) | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | (Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT) | (Convert.ToInt32(instruction[4]) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "SHL" )
                    com = (SHL << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | (Convert.ToInt32(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "MOV" )
                    com = (MOV << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "JMAE" )
                {
                    int ov;
                    if ( symbolMapList.TryGetValue(instruction[4], out ov) )
                    {
                        if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[4])) )
                        {
                            return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                        }
                        else return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(instruction[4]) << OPERAND3_MEM_SHIFT);
                    }
                    else showErrorLine("There is not label" + instruction[4]);
                }
                else if ( instruction[1] == "JMNGE" )
                {
                    int ov;
                    if ( symbolMapList.TryGetValue(instruction[4], out ov) )
                    {
                        if ( new Regex(@"[a-zA-Z]+").IsMatch(instruction[4]) )
                        {
                            return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                        }
                        else return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(instruction[4]) << OPERAND3_MEM_SHIFT);
                    }
                    else showErrorLine("There is not label" + instruction[4]);
                }
                else if ( instruction[1] == "BT" )
                    com = (BT << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "CMP" )
                    com = (CMP << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "RCL" )
                    com = (RCL << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | (Convert.ToInt32(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == ".FILL" )
                {
                    com = ((Convert.ToInt32(instruction[2])));
                }
                else if ( instruction[1] == "HALT" )
                    com = (HALT << INSTRUCTION_MEM_SHIFT);
                return com;
            }
            #region errors
            // show errors methods
            public void showErrorLine(String e)
            {
                Console.WriteLine(e);
                ERRORSTATE = true;
                COUNTERRORS++;
            }

            public void showLineAndReadKey(String e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            #endregion

            public class testInstructions
            {
                Regex rgx = new Regex(@"[a-zA-z0-9]+");
                Regex rgxAddr = new Regex(@"[0-9]+");
                Regex onlyLeters = new Regex(@"[a-zA-Z]+");

                // test arguments -> is a number/label
                public bool testArgs(String[] args)
                {
                    if ( args.Length == 3 )
                        if ( rgx.IsMatch(args[0]) && rgx.IsMatch(args[1]) && rgx.IsMatch(args[2]) ) return true;
                    if ( args.Length == 2 )
                        if ( rgx.IsMatch(args[0]) && rgx.IsMatch(args[1]) ) return true;
                    if ( args.Length == 1 )
                        if ( rgx.IsMatch(args[0]) ) return true;
                    return false;
                }

                // test arguments range
                public bool testAddr(String[] args)
                {
                    if ( args.Length == 3 )
                    {
                        Regex rgx = new Regex(@"[a-zA-z]+");
                        // when operand 3 is label
                        if ( rgx.IsMatch(args[2]) )
                        {

                            if ( (Convert.ToInt32(args[0]) >= 0 && Convert.ToInt32(args[0]) < 64) && (Convert.ToInt32(args[1]) >= 0 && Convert.ToInt32(args[1]) < 64) )
                            {
                                return true;
                            }
                        }
                        // when operand 3 is register
                        else if ( (Convert.ToInt32(args[0]) >= 0 && Convert.ToInt32(args[0]) < 64) && (Convert.ToInt32(args[1]) >= 0 && Convert.ToInt32(args[1]) < 64) && (Convert.ToInt32(args[2]) >= 0 && Convert.ToInt32(args[2]) < 64) ) return true;
                    }
                    if ( args.Length == 2 )
                        if ( (Convert.ToInt32(args[0]) >= 0 && Convert.ToInt32(args[0]) < 64) && (Convert.ToInt32(args[1]) >= 0 && Convert.ToInt32(args[1]) < 64) ) return true;
                    if ( args.Length == 1 )
                        if ( (Convert.ToInt32(args[0]) >= 0 && Convert.ToInt32(args[0]) < 64) ) return true;
                    return false;
                }

                // create machine code
                public int createMachineCode(String[] instruction, Dictionary<String, int> symbolMapList)
                {
                    if ( onlyLeters.IsMatch(instruction[4]) )
                    {
                        return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                    }
                    else return (JMAE << INSTRUCTION_MEM_SHIFT | (Convert.ToInt32(instruction[2]) << OPERAND1_MEM_SHIFT) | Convert.ToInt32(instruction[3]) << OPERAND2_MEM_SHIFT | Convert.ToInt32(instruction[4]) << OPERAND3_MEM_SHIFT);
                }
            }
        }

        // argv[0] -> input program
        // argv[1] -> output machine code
        static void Main(string[] args)
        {
            bool ERRORSTATE = false;

            // main assembler class
            AssemblerClass assembler = new AssemblerClass();

            if (args.Length == 2)
            {
		        // read instructions from file and parse
                String instructionLine;
                StreamReader fstr = new StreamReader(args[0]);
                int pos = 0;

                Console.WriteLine("-----------------------------\nSource code:\n-----------------------------");
                while ((instructionLine = fstr.ReadLine()) != null)
                {
                    // write source code to a console
                    Console.WriteLine(instructionLine);

                    // instruction[0] = label
                    // instruction[1] = instruction
                    // instruction[2-i] = arguments
                    // instruction[>i+1] = comment
                    String[] instruction = instructionLine.Split(' ', '\t');

                    // create symbol map
                    if(assembler.createSymbolMap(pos, instruction[0]))
                        //symbolMapList[instruction[0]] = pos;
                    pos++;
                }

                // end assembling
                if ( ERRORSTATE || assembler.ERRORSTATE )
                {
                    fstr.Close();
                    Console.WriteLine("----------------------------\nAssembled with " + assembler.COUNTERRORS + " errors");
                    System.Console.ReadKey();
                    return;
                }               

                // write mashine code and replace labels from dictionary with addresses 
                fstr = new StreamReader(args[0]);
                pos = 0;
                while ( (instructionLine = fstr.ReadLine()) != null )
                {
                    String[] instruction = instructionLine.Split(' ', '\t');

                    assembler.checkForInvalidInstruction(pos, instruction[1]);

                    // check for enough arguments
                    assembler.checkForEnoughtParametrs(pos, instruction);                   
                    pos++;
                    //Console.WriteLine(instruction[1]);
                }

                if ( !ERRORSTATE || assembler.ERRORSTATE )
                {
                    // form a hex/dec machine code
                    fstr = new StreamReader(args[0]);
                    StreamWriter fout = new StreamWriter(args[1]);
                    pos = 0;
                    Console.WriteLine("-----------------------------\nMachine code:\n-----------------------------");
                    while ( (instructionLine = fstr.ReadLine()) != null )
                    {
                        String[] instruction = instructionLine.Split(' ', '\t');
                        instruction[1] = instruction[1].ToUpper();
                        int com = assembler.createMachineCode(instruction);

                        // write to file
                        fout.WriteLine(com + " (" + String.Format("0x{0:X}", com) + ")");
                        // write to a console window results
                        Console.WriteLine(com + "\t(" + String.Format("0x{0:X}", com) + ")");
                    }

                    fout.Close();
                    fstr.Close();
                    // end assembling
                    Console.WriteLine("Assembling ended successfully");
                    System.Console.ReadKey();
                } else {
                    Console.WriteLine("-----------------------------\nAssembled with " + assembler.COUNTERRORS + " errors");
                    Console.ReadKey();
                };
            } else {
		        // show error
		        Console.Out.Write("error: <assembly-code-file> <machine-code-file>\n");
                Console.ReadKey();
	        }
        }
    }
}

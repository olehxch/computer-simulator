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
            // for indirrect addressing - @ 1000 0000 - 8th bit set's indirrect addressing
            // |         4              |        3       |       2       |      1      |
            // | INSTRUCTION 31-28 (24) | OPERAND1 27-16 | OPERAND2 26-8 | OPERAND3 7-0|
            // ------------------------------------------------------------------|
            // |         7         |        3       |       2       |      1      |
            // | INSTRUCTION 47-44 | OPERAND1 27-16 | OPERAND2 26-8 | OPERAND3 7-0|
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
            public Dictionary<String, int> symbolMapList = new Dictionary<String, int>();
            public List<String> errorList = new List<String>();
            // error states
            public bool ERRORSTATE = false;
            public int COUNTERRORS = 0;

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
                                addErrorLine(pos + "::Two or more labels have equal names...");
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
                            addErrorLine(pos + "::Label length \'" + label + "\' has more than " + MAXLABELLENGHT + " letters");
                            return false;
                        }
                    }
                    else
                    {
                        addErrorLine(pos + "::Label \'" + label + "\' is not starting with letter");
                        return false;
                    }
                }
                return false;
            }

            // check for invalid instruction
            public bool checkForInvalidInstruction(int pos, String instruction)
            {
                instruction = instruction.ToUpper();
                if ( !INSTRUCTIONSARRAY.Any(instruction.Contains) )
                {
                    addErrorLine(pos + "::Invalid instruction \'" + instruction + "\'");
                    return false;
                }
                return true;
            }

            // check for enough arguments
            public void checkForEnoughtParametrs(int pos, String[] instruction)
            {
                Regex rgx = new Regex(@"[a-zA-Z0-9]+");
                Regex rgxOnlyNum = new Regex(@"[0-9]+");
                Regex onlyLeters = new Regex(@"[a-zA-Z]+");

                // for jumps
                if ( (instruction[1] == "JMAE") || (instruction[1] == "JMNGE") )
                {
                    if ( instruction.Length >= 5 )
                    {
                        String[] iArgs = { instruction[2], instruction[3], instruction[4] };
                        if ( testArgs(iArgs, true) )
                        {
                            if ( testAddr(iArgs) ) return;
                        }
                        else addErrorLine(pos + "::Bad arguments");
                    }
                    else addErrorLine(pos + "::Bad arguments");
                    return;
                }

                // 3 arguments
                if ( (instruction[1] == "DIV") || (instruction[1] == "XIMUL") || (instruction[1] == "XOR") || (instruction[1] == "RCL") ||
                    (instruction[1] == "SHL") || (instruction[1] == "JMAE") || (instruction[1] == "JMNGE") )
                {
                    if ( instruction.Length >= 5 )
                    {
                        String[] iArgs = { instruction[2], instruction[3], instruction[4] };
                        if ( testArgs(iArgs, false) )
                        {
                            if ( testAddr(iArgs) ) return;
                        }
                        else addErrorLine(pos + "::Bad arguments");
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }

                // 2 arguments
                if ( (instruction[1] == "MOV") || (instruction[1] == "BT") || (instruction[1] == "CMP") )
                {
                    if ( instruction.Length >= 4 )
                    {
                        String[] iArgs = { instruction[2], instruction[3] };
                        if ( testArgs(iArgs, false) )
                        {
                            if ( testAddr(iArgs) ) return;
                        }
                        else addErrorLine(pos + "::Bad arguments");
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }

                // 1 arguments
                if ( (instruction[1] == "DEC") )
                {
                    if ( instruction.Length >= 3 )
                    {
                        String[] iArgs = { instruction[2] };
                        if ( testArgs(iArgs, false ) )
                        {
                            if ( testAddr(iArgs) ) return;
                        }
                        else addErrorLine(pos + "::Bad arguments");
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }
            }

            public dynamic convertNumTo(dynamic num)
            {
                //return Convert.ToInt64(num);
                return Convert.ToInt32(num);
            }

            public int createMachineCode(String[] instruction)
            {
                int com = 0;
                if ( instruction[1] == "DEC" )
                    com = (DEC << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT));
                else if ( instruction[1] == "DIV" )
                    com = (JMAE << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "XIMUL" )
                    com = (XIMUL << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "XOR" )
                    com = (XOR << INSTRUCTION_MEM_SHIFT) | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | (convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT) | (convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "SHL" )
                    com = (SHL << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == "MOV" )
                    com = (MOV << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "JMAE" || instruction[1] == "JMNGE" )
                { 
                    int ov;
                    if ( symbolMapList.TryGetValue(instruction[4], out ov) )
                    {
                        if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[4])) )
                        {
                            return (JMAE << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                        }
                        else return (JMAE << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
                    }
                    else addErrorLine("There is not label" + instruction[4]);
                }
                else if ( instruction[1] == "BT" )
                    com = (BT << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "CMP" )
                    com = (CMP << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
                else if ( instruction[1] == "RCL" )
                    com = (RCL << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);
                else if ( instruction[1] == ".FILL" )
                {
                    com = ((convertNumTo(instruction[2])));
                }
                else if ( instruction[1] == "HALT" )
                    com = (HALT << INSTRUCTION_MEM_SHIFT);
                return com;
            }

            // show errors methods
            public void showErrorLine(String e)
            {
                Console.WriteLine(e);
                ERRORSTATE = true;
                COUNTERRORS++;
            }

            public void addErrorLine(String e)
            {
                errorList.Add(e);
                ERRORSTATE = true;
                COUNTERRORS++;
            }

            public void showErrorList()
            {
                foreach ( var s in errorList )
                {
                    Console.WriteLine(s);
                }
            }

            Regex rgx = new Regex(@"[a-zA-Z0-9]+");
            Regex rgxOnlyNum = new Regex(@"[0-9]+");
            Regex onlyLeters = new Regex(@"[a-zA-Z]+");

            // test arguments -> is a number/label
            public bool testArgs(String[] args, bool isJmp)
            {
                if ( isJmp )
                {
                    if ( args.Length == 3 )
                        if ( rgxOnlyNum.IsMatch(args[0]) && rgxOnlyNum.IsMatch(args[1]) && rgx.IsMatch(args[2]) ) return true;
                    return false;
                } else foreach ( var n in args)
                {
                    if ( rgxOnlyNum.IsMatch(n) )
                    {
                        continue;
                    }
                    else return false;
                } return true;
            }

            // test arguments range
            public bool testAddr(String[] args)
            {
                Regex rgx = new Regex(@"[a-zA-z]+");

                if ( args.Length == 3 )
                {  
                    // when operand 3 is label
                    if ( rgx.IsMatch(args[2]) )
                        if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < 64) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < 64) ) return true;
                    // when operand 3 is register
                    if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < 64) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < 64) && (convertNumTo(args[2]) >= 0 && convertNumTo(args[2]) < 64) ) return true;
                }
                if ( args.Length == 2 )
                    if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < 64) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < 64) ) return true;
                if ( args.Length == 1 )
                    if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < 64) ) return true;
                return false;
            }

            // create machine code
            public int createMachineCode(String[] instruction, Dictionary<String, int> symbolMapList)
            {
                if ( onlyLeters.IsMatch(instruction[4]) )
                {
                    return (JMAE << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                }
                else return (JMAE << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
            }
        }

        // argv[0] -> input program
        // argv[1] -> output machine code
        static void Main(string[] args)
        {
            // main assembler class
            AssemblerClass assembler = new AssemblerClass();
             
            if (args.Length == 2)
            {
                // read instructions from file and parse
                String instructionLine;
                StreamReader fstr = new StreamReader(args[0]);
                int pos = 0;

                Console.WriteLine("----------------------------\nSource code:\n----------------------------");
                while ( (instructionLine = fstr.ReadLine()) != null )
                {
                    // write source code to a console
                    Console.WriteLine(pos + " : " + instructionLine);

                    // instruction[0] = label
                    // instruction[1] = instruction
                    // instruction[2-i] = arguments
                    // instruction[>i+1] = comment
                    String[] instruction = instructionLine.Split(' ', '\t');

                    // create symbol map
                    assembler.createSymbolMap(pos, instruction[0]);
                    instruction[1] = instruction[1].ToUpper();
                    // check for invalid instructions and check for enough arguments
                    if ( assembler.checkForInvalidInstruction(pos, instruction[1]) )
                        assembler.checkForEnoughtParametrs(pos, instruction);
                    pos++;
                }

                // check for correct labels
                {
                    fstr = new StreamReader(args[0]);
                    StreamWriter fout = new StreamWriter(args[1]);
                    pos = 0;

                    while ( (instructionLine = fstr.ReadLine()) != null )
                    {
                        String[] instruction = instructionLine.Split(' ', '\t');
                        instruction[1] = instruction[1].ToUpper();

                        if ( instruction[1] == "JMAE" || instruction[1] == "JMNGE" )
                        {
                            int ov;
                            if ( !assembler.symbolMapList.TryGetValue(instruction[4], out ov) )
                                assembler.addErrorLine(pos + "::There is no label \'" + instruction[4] + '\'');
                        }

                        pos++;
                    }
                    fout.Close();
                    fstr.Close();
                }

                // end assembling
                if ( assembler.ERRORSTATE )
                {
                    fstr.Close();
                    Console.WriteLine("----------------------------\nAssembled with " + assembler.COUNTERRORS + " errors");
                    assembler.showErrorList();
                    System.Console.ReadKey();
                    return;
                }
                else
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
                        fout.WriteLine(com);

                        // write to a console window results
                        Console.WriteLine(pos + " : " + com + "\t(" + String.Format("0x{0:X}", com) + ")");
                        pos++;
                    }

                    fout.Close();
                    fstr.Close();
                    // end assembling
                    Console.WriteLine("-----------------------------\nAssembling ended successfully");
                    System.Console.ReadKey();
                }
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

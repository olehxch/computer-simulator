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
    public class AssemblerClass
    {
        const int MAXMEM = 16777216;
        const int DATABUS = 48;
        const int REGISTERS = 64;
        const int MAXLABELLENGHT = 6;

        // addressing
        // |         5         |        4       |       3        | 2 | 1 |      0                   |
        // | INSTRUCTION 47-44 | OPERAND1 39-32 | OPERAND2 31-24 |         OPERAND3 7-0 (mem = 23-0)|
        const int INSTRUCTION_MEM_SHIFT = 44;
        const int OPERAND1_MEM_SHIFT = 32;
        const int OPERAND2_MEM_SHIFT = 24;
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
        // other
        const int LOAD = 12;            // regA mem
        const int SAVE = 13;            // regA mem
        const int CLEAR = 14;           // mem

        // instruction list
        private String[] INSTRUCTIONSARRAY = { "HALT", "DEC", "DIV", "XIMUL", "XOR", "SHL", "MOV", "JMAE", "JMNGE", ".FILL", "BT", "CMP", "RCL", "LOAD", "SAVE", "CLEAR" };

        // symbol map list
        public Dictionary<String, int> symbolMapList = new Dictionary<String, int>();
        public List<String> errorList = new List<String>();

        // error states
        public bool ERRORSTATE = false;
        public int COUNTERRORS = 0;
        private String[] args;

        private Regex rgx = new Regex(@"[a-zA-Z0-9]+");
        private Regex rgxOnlyNum = new Regex(@"^[0-9]+$");
        private Regex onlyLeters = new Regex(@"[a-zA-Z]+");

        public AssemblerClass(String[] args)
        {
            this.args = args;
        }
        // create symbol map
        private bool createSymbolMap(int pos, String label)
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
        private bool checkForInvalidInstruction(int pos, String instruction)
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
        private void checkForEnoughtParametrs(int pos, String[] instruction)
        {
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

            // 2 arguments - LOAD/SAVE
            if ( (instruction[1] == "LOAD") || (instruction[1] == "SAVE") )
            {
                if ( instruction.Length >= 4 )
                {
                    String[] iArgs = { instruction[2], instruction[3] };
                    if ( (rgxOnlyNum.IsMatch(iArgs[0]) && rgxOnlyNum.IsMatch(iArgs[1])) )
                    {
                        if ( iArgs.Length == 2 )
                            if ( (convertNumTo(iArgs[0]) >= 0 && convertNumTo(iArgs[0]) < 64) && (convertNumTo(iArgs[1]) >= 0 && convertNumTo(iArgs[1]) < MAXMEM) ) return;
                    }
                    else if ( (rgxOnlyNum.IsMatch(iArgs[0]) && rgx.IsMatch(iArgs[1])) )
                    {
                        if ( iArgs.Length == 2 )
                            if ( (convertNumTo(iArgs[0]) >= 0 && convertNumTo(iArgs[0]) < 64) ) return;
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }
                else addErrorLine(pos + "::Bad arguments");
            }

            // 1 arguments - CLEAR
            if ( (instruction[1] == "CLEAR") )
            {
                if ( instruction.Length >= 3 )
                {
                    String[] iArgs = { instruction[2] };
                    if ( testArgs(iArgs, false) )
                    {
                        if ( iArgs.Length == 2 )
                            if ( (convertNumTo(iArgs[1]) >= 0 && convertNumTo(iArgs[1]) < MAXMEM) ) return;
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }
                else addErrorLine(pos + "::Bad arguments");
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
                    if ( testArgs(iArgs, false) )
                    {
                        if ( testAddr(iArgs) ) return;
                    }
                    else addErrorLine(pos + "::Bad arguments");
                }
                else addErrorLine(pos + "::Bad arguments");
            }
        }

        private dynamic convertNumTo(dynamic num)
        {

            return Convert.ToUInt64(num);
        }

        private UInt64 translateToMachineCode(String[] instruction)
        {
            UInt64 com = 0;

            // LOAD, SAVE, CLEAR
            if ( instruction[1] == "LOAD" )
            {
                int ov;
                if ( symbolMapList.TryGetValue(instruction[3], out ov) )
                {
                    if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[3])) )
                    {
                        return (convertNumTo(LOAD) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | ( convertNumTo(symbolMapList[instruction[3]]) << OPERAND2_MEM_SHIFT ) );
                    }
                    else return com = (convertNumTo(LOAD) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else addErrorLine("There is not label" + instruction[4]);
            }
                
            else if ( instruction[1] == "SAVE" )
            {
                int ov;
                if ( symbolMapList.TryGetValue(instruction[3], out ov) )
                {
                    if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[3])) )
                    {
                        return (convertNumTo(SAVE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | (convertNumTo(symbolMapList[instruction[3]]) << OPERAND2_MEM_SHIFT));
                    }
                    else return com = (convertNumTo(SAVE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else addErrorLine("There is not label" + instruction[4]);
            }
            else if ( instruction[1] == "CLEAR" )
                com = (convertNumTo(CLEAR) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[2]) << OPERAND3_MEM_SHIFT);

            // DEC, DIV, XIMUL, XOR, SHL, MOV, JMAE, JMNGE, BT, CMP, RCL
            else if ( instruction[1] == "DEC" )
                com = (convertNumTo(DEC) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT));
            else if ( instruction[1] == "DIV" )
                com = (convertNumTo(JMAE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
            else if ( instruction[1] == "XIMUL" )
                com = (convertNumTo(XIMUL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);
            else if ( instruction[1] == "XOR" )
                com = (convertNumTo(XOR) << INSTRUCTION_MEM_SHIFT) | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | (convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT) | (convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
            else if ( instruction[1] == "SHL" )
                com = (convertNumTo(SHL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);
            else if ( instruction[1] == "MOV" )
                com = (convertNumTo(MOV) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
            else if ( instruction[1] == "JMAE" )
            {
                int ov;
                if ( symbolMapList.TryGetValue(instruction[4], out ov) )
                {
                    if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[4])) )
                    {
                        return (convertNumTo(JMAE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                    }
                    else return (convertNumTo(JMAE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
                }
                else addErrorLine("There is not label" + instruction[4]);
            }
            else if ( instruction[1] == "JMNGE" )
            {
                int ov;
                if ( symbolMapList.TryGetValue(instruction[4], out ov) )
                {
                    if ( (new Regex(@"[a-zA-Z]+").IsMatch(instruction[4])) )
                    {
                        return (convertNumTo(JMNGE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(symbolMapList[instruction[4]]) << OPERAND3_MEM_SHIFT);
                    }
                    else return (convertNumTo(JMNGE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[4]) << OPERAND3_MEM_SHIFT);
                }
                else addErrorLine("There is not label" + instruction[4]);
            }
            else if ( instruction[1] == "BT" )
                com = (convertNumTo(BT) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
            else if ( instruction[1] == "CMP" )
                com = (convertNumTo(CMP) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT);
            else if ( instruction[1] == "RCL" )
                com = (convertNumTo(RCL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[2]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[3]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[4])) << OPERAND3_MEM_SHIFT);

            // .FILL, HALT
            else if ( instruction[1] == ".FILL" )
            {
                com = ((convertNumTo(instruction[2])));
            }
            else if ( instruction[1] == "HALT" )
                com = (convertNumTo(HALT) << INSTRUCTION_MEM_SHIFT);
            return com;
        }

        // show errors methods
        private void showErrorLine(String e)
        {
            Console.WriteLine(e);
            ERRORSTATE = true;
            COUNTERRORS++;
        }

        private void addErrorLine(String e)
        {
            errorList.Add(e);
            ERRORSTATE = true;
            COUNTERRORS++;
        }

        public void showErrorListToConsole()
        {
            foreach ( var s in errorList )
            {
                Console.WriteLine(s);
            }
        }

        public List<String> getErrorList()
        {
            return errorList;
        }

        // test arguments -> is a number/label
        private bool testArgs(String[] args, bool isJmp)
        {
            if ( isJmp )
            {
                if ( args.Length == 3 )
                    if ( rgxOnlyNum.IsMatch(args[0]) && rgxOnlyNum.IsMatch(args[1]) && rgx.IsMatch(args[2]) ) return true;
                return false;
            }
            else foreach ( var n in args )
                {
                    if ( rgxOnlyNum.IsMatch(n) )
                    {
                        continue;
                    }
                    else return false;
                } return true;
        }

        // test arguments range
        private bool testAddr(String[] args)
        {
            Regex rgx = new Regex(@"[a-zA-z]+");

            if ( args.Length == 3 )
            {
                // when operand 3 is label
                if ( rgx.IsMatch(args[2]) )
                    if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < REGISTERS) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < REGISTERS) ) return true;
                // when operand 3 is register
                if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < REGISTERS) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < REGISTERS) && (convertNumTo(args[2]) >= 0 && convertNumTo(args[2]) < REGISTERS) ) return true;
            }
            if ( args.Length == 2 )
                if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < REGISTERS) && (convertNumTo(args[1]) >= 0 && convertNumTo(args[1]) < REGISTERS) ) return true;
            if ( args.Length == 1 )
                if ( (convertNumTo(args[0]) >= 0 && convertNumTo(args[0]) < REGISTERS) ) return true;
            return false;
        }

        // read code file and check for errors
        public List<String> readCodeAndCheck()
        {
            // read instructions from file and parse
            String instructionLine;
            StreamReader fstr = new StreamReader(args[0]);
            int pos = 0;

            List<String> res = new List<String>();

            //Console.WriteLine("----------------------------\nSource code:\n----------------------------");
            while ( (instructionLine = fstr.ReadLine()) != null )
            {
                // write source code to a console
                //Console.WriteLine(pos + " : " + instructionLine);
                res.Add(pos + ":" + instructionLine);
                // instruction[0] = label
                // instruction[1] = instruction
                // instruction[2-i] = arguments
                // instruction[>i+1] = comment
                String[] instruction = instructionLine.Split(' ', '\t');

                if ( instruction.Length >= 3 )
                {
                    // create symbol map
                    this.createSymbolMap(pos, instruction[0]);
                    instruction[1] = instruction[1].ToUpper();
                    // check for invalid instructions and check for enough arguments
                    if ( this.checkForInvalidInstruction(pos, instruction[1]) )
                        this.checkForEnoughtParametrs(pos, instruction);
                }
                else this.addErrorLine(pos + "::Unknown instruction \'" );
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
                    if ( instruction.Length >= 3 )
                    {
                        instruction[1] = instruction[1].ToUpper();

                        if ( instruction[1] == "JMAE" || instruction[1] == "JMNGE" )
                        {
                            int ov;
                            if ( !this.symbolMapList.TryGetValue(instruction[4], out ov) )
                                this.addErrorLine(pos + "::There is no label \'" + instruction[4] + '\'');
                        }
                    }
                    else this.addErrorLine(pos + "::Unknown instruction \'");
                    pos++; 
                }
                fout.Close();
                fstr.Close();
                fout.Dispose();
                fstr.Dispose();
            }
            // end assembling
            if ( ERRORSTATE )
            {
                fstr.Close();
                fstr.Dispose();
                //Console.WriteLine("----------------------------\nAssembled with " + assembler.COUNTERRORS + " errors");
                //this.showErrorListToConsole();
                //System.Console.ReadKey();
                res.Clear();
                return res;
            }
            return res;
        }

        public List<String> createMachineCode()
        {
            String instructionLine;
            List<String> res = new List<String>();
            // form a hex/dec machine code
            StreamReader fstr = new StreamReader(args[0]);
            StreamWriter fout = new StreamWriter(args[1]);
            int pos = 0;
            //Console.WriteLine("-----------------------------\nMachine code:\n-----------------------------");
            while ( (instructionLine = fstr.ReadLine()) != null )
            {
                String[] instruction = instructionLine.Split(' ', '\t');
                instruction[1] = instruction[1].ToUpper();
                UInt64 com = this.translateToMachineCode(instruction);

                // write to file
                fout.WriteLine(com);

                // write to a console window results
                //Console.WriteLine(pos + " : " + com + "\t(" + String.Format("0x{0:X}", com) + ")");
                //res.Add(com + "\t(" + String.Format("0x{0:X}", com) + ")");
                res.Add(com.ToString() + " " + String.Format("0x{0:X}", com));
                pos++;
            }

            fout.Close();
            fstr.Close();
            fout.Dispose();
            fstr.Dispose();
            // end assembling
            //Console.WriteLine("-----------------------------\nAssembling ended successfully");
            //System.Console.ReadKey();
            return res;
        }
    }
}
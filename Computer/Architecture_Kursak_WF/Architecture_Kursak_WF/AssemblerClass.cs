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

        // label must end with ':'
        // comments start with '#'
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

        // from assol
        const int ADD = 15;
        const int NAND = 16;
        const int BEQ = 17;
        const int JALR = 18;

        // instruction list
        private String[] INSTRUCTIONSARRAY = { "HALT", "DEC", "DIV", "XIMUL", "XOR", "SHL", "MOV", "JMAE", "JMNGE", ".FILL", "BT", "CMP", "RCL", "LOAD", "SAVE", "CLEAR", "ADD", "NADN", "BEQ", "JARL" };
        
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
        private bool addToSymbolMap(int pos, String label)
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

        // check for invalid instruction
        private bool checkForInvalidInstruction(int pos, String instruction)
        {
            instruction = instruction.ToUpper();
            if ( !INSTRUCTIONSARRAY.Any(instruction.Contains) )
            {
                //addErrorLine(pos + "::Invalid instruction \'" + instruction + "\'");
                return false;
            }
            return true;
        }

        // check for enough arguments
        private void checkForEnoughtParametrs(int pos, String[] instruction)
        {
            String inst = instruction[0].ToUpper();
            
            // JMAE/JMNGE reg1 reg2 offset/label
            if ( (inst == "JMAE") || (inst == "JMNGE") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (rgxOnlyNum.IsMatch(instruction[2])) && (rgx.IsMatch(instruction[3])) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }         

            // LOAD/SAVE reg1 mem
            if ( (inst == "LOAD") || (inst == "SAVE") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1]) && rgxOnlyNum.IsMatch(instruction[2])) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < MAXMEM) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers"); 
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            //  CLEAR/DEC mem
            if ( (inst == "CLEAR") || (inst == "DEC") )
            {
                if ( instruction.Length == 2 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < MAXMEM) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            // DIV/XIMUL/XOR/RCL/SHL/JMAE/JMNGE reg1 reg2 reg3
            if ( (inst == "DIV") || (inst == "XIMUL") || (inst == "XOR") || (inst == "RCL") ||
                (inst == "SHL") || (inst == "JMAE") || (inst == "JMNGE") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (rgxOnlyNum.IsMatch(instruction[2])) && (rgxOnlyNum.IsMatch(instruction[3])) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) && (convertNumTo(instruction[3]) >= 0 && convertNumTo(instruction[3]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            // MOV/BT/CMP reg1 reg2
            if ( (inst == "MOV") || (inst == "BT") || (inst == "CMP") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (rgxOnlyNum.IsMatch(instruction[2])) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
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
                if ( instructionLine != "" )
                {
                    // write source code to a console
                    res.Add(pos + ":" + instructionLine);
                    // instruction[0] = label
                    // instruction[1] = instruction
                    // instruction[2-i] = arguments
                    // instruction[>i+1] = comment

                    if ( instructionLine.IndexOf('#') != -1 )
                        instructionLine = instructionLine.Substring(0, instructionLine.IndexOf('#')).Trim();
                    //instructionLine = instructionLine;
                    String[] instruction = instructionLine.Split(' ', '\t');
                    instruction[0] = instruction[0].TrimEnd(':', ' ', '\t');

                    // if it is instruction...
                    if ( this.checkForInvalidInstruction(pos, instruction[0]) )
                    {
                        // check this instruction
                        this.checkForEnoughtParametrs(pos, instruction);
                    }
                    else
                    {
                        // add label to symbol map
                        if( instruction[0] != "")
                            this.addToSymbolMap(pos, instruction[0]);

                        // and check this instruction
                        String[] inst = new String[instruction.Length - 1];
                        for ( int i = 1 ; i < instruction.Length ; i++ )
                        {
                            inst[i - 1] = instruction[i];
                        }
                        this.checkForEnoughtParametrs(pos, inst);
                    }
                    pos++;
                }
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
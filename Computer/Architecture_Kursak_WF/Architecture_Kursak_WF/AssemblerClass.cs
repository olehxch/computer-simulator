using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;


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
        // |        6          |         5         |        4       |       3        | 2 | 1 |      0                   |
        // | INSTRUCTION 47-44 | INSTRUCTION 43-40 | OPERAND1 39-32 | OPERAND2 31-24 |         OPERAND3 7-0 (mem = 23-0)|
        const int INDIRRECT_ADDRESSING_SHIFT = 47;
        const int INSTRUCTION_MEM_SHIFT = 40;
        const int OPERAND1_MEM_SHIFT = 32;
        const int OPERAND2_MEM_SHIFT = 24;
        const int OPERAND3_MEM_SHIFT = 0;
        // arithmetic instructions
        const int HALT = 0x1F;
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
        const int ADD = 15;             // regA regB destreg
        const int NAND = 16;            // regA regB destreg
        const int BEQ = 17;             // regA regB offset/label
        const int JALR = 18;            // regA regB offset/label

        // instruction list
        private String[] INSTRUCTIONSARRAY = { "HALT", "DEC", "DIV", "XIMUL", "XOR", "SHL", "MOV", "JMAE", "JMNGE", ".FILL", "BT", "CMP", "RCL", "LOAD", "SAVE", "CLEAR", "ADD", "NADN", "BEQ", "JARL" };

        // symbol map list
        public Dictionary<String, int> symbolMapList = new Dictionary<String, int>();
        public List<String> errorList = new List<String>();

        // error states
        public bool ERRORSTATE = false;
        public int COUNTERRORS = 0;
        private String argas;
        private String argmc;

        private Regex rgx = new Regex(@"[a-zA-Z0-9]+");
        private Regex rgxOnlyNum = new Regex(@"^[0-9]+$");
        private Regex onlyLeters = new Regex(@"[a-zA-Z]+");
        private Regex indirrectAddressing = new Regex(@"^@[\w]+");
        private Regex minusNumber = new Regex(@"^-[\d]+");

        public AssemblerClass(String[] args)
        {
            this.argas = args[0];
            this.argmc = args[1];
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
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (onlyLeters.IsMatch(instruction[2])) || (rgx.IsMatch(instruction[3])) || minusNumber.IsMatch( instruction[3]) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            // LOAD/SAVE reg1 mem/@mem
            if ( (inst == "LOAD") || (inst == "SAVE") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( rgxOnlyNum.IsMatch(instruction[1]) && ( onlyLeters.IsMatch(instruction[2]) || indirrectAddressing.IsMatch(instruction[2]) ) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers or have indirrect addressing with '@' or be a label");
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

            // DIV/XIMUL/XOR/SHL/ADD/NAND reg1 reg2 reg3/@mem
            if ( (inst == "DIV") || (inst == "XIMUL") || (inst == "XOR") ||
                 (inst == "SHL") || (inst == "ADD") || (inst == "NAND") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (rgxOnlyNum.IsMatch(instruction[2])) && ( (rgxOnlyNum.IsMatch(instruction[3]) || indirrectAddressing.IsMatch(instruction[2]) ) ) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) && (convertNumTo(instruction[3]) >= 0 && convertNumTo(instruction[3]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers or have indirrect addressing with '@'");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }
            // RCL reg1 reg2 reg3/@mem
            if (  (inst == "RCL") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && (rgxOnlyNum.IsMatch(instruction[2])) && ((rgxOnlyNum.IsMatch(instruction[3]) )) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) && (convertNumTo(instruction[3]) >= 0 && convertNumTo(instruction[3]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }
            // MOV/BT/CMP reg1 reg2/@mem
            if ( (inst == "MOV") || (inst == "BT") || (inst == "CMP") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( (rgxOnlyNum.IsMatch(instruction[1])) && ( (rgxOnlyNum.IsMatch(instruction[2])) ) )
                    {
                        if ( (convertNumTo(instruction[1]) >= 0 && convertNumTo(instruction[1]) < REGISTERS) && (convertNumTo(instruction[2]) >= 0 && convertNumTo(instruction[2]) < REGISTERS) ) return;
                        else addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }
            if ( inst == ".FILL" )
            {
                if ( instruction[1][0] == '-' )
                {
                    long com = 0 - (Convert.ToInt64(instruction[1].Remove(0, 1)));
                    if ( ( com < 0) && ( com >= -140737488355327))  { }
                    else addErrorLine(pos + "::Number is out of range");
                }
                else
                {
                    long com = (Convert.ToInt64(instruction[1]));
                    if ( ( com <= 140737488355327) && (com >= 0)) { }
                    else addErrorLine(pos + "::Number is out of range");
                }
            }
        }

        private dynamic convertNumTo(dynamic num)
        {
            try
            {
                return Convert.ToUInt64(num);
            }
            catch ( Exception e )
            {
                return 0;
            }
        }

        private UInt64 translateToMachineCode(String[] instruction, int ip)
        {
            UInt64 com = 0;
            UInt64 ia = 1;
            String inst = instruction[0];
            
            // LOAD, SAVE, CLEAR
            if ( inst == "LOAD" )
            {
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | convertNumTo(LOAD) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT | convertNumTo(symbolMapList[instruction[2].Remove(0, 1)]) << OPERAND3_MEM_SHIFT;
                }
                else // if mem
                {
                    com = convertNumTo(0) << INDIRRECT_ADDRESSING_SHIFT | convertNumTo(LOAD) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT | convertNumTo(symbolMapList[instruction[2]]) << OPERAND3_MEM_SHIFT;
                }
            }

            else if ( inst == "SAVE" )
            {
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | convertNumTo(SAVE) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT | convertNumTo(symbolMapList[instruction[2].Remove(0, 1)]) << OPERAND3_MEM_SHIFT;
                }
                else // if mem
                {
                    com = convertNumTo(0) << INDIRRECT_ADDRESSING_SHIFT | convertNumTo(SAVE) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT | convertNumTo(symbolMapList[instruction[2]]) << OPERAND3_MEM_SHIFT;
                }
            }
            else if ( inst == "CLEAR" )
                com = (convertNumTo(CLEAR) << INSTRUCTION_MEM_SHIFT | convertNumTo(instruction[1]) << OPERAND3_MEM_SHIFT);

            // DEC, DIV, XIMUL, XOR, SHL, MOV
            else if ( inst == "DEC" )
                if ( indirrectAddressing.IsMatch(instruction[1]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(DEC) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT));
                }
                else // if mem
                {
                    com = (convertNumTo(DEC) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) );
                }
                
            else if ( inst == "DIV" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(DIV) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(DIV) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }               
            else if ( inst == "XIMUL" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(XIMUL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(XIMUL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
            else if ( inst == "XOR" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(XOR) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(XOR) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }  
            else if ( inst == "SHL" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(SHL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(SHL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
            else if ( inst == "MOV" )
                com = (convertNumTo(MOV) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT);
            else if ( inst == "ADD" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(ADD) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(ADD) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                
            else if ( inst == "NAND" )
                if ( indirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                {
                    com = convertNumTo(1) << INDIRRECT_ADDRESSING_SHIFT | (convertNumTo(NAND) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
                else // if mem
                {
                    com = (convertNumTo(NAND) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | convertNumTo(instruction[3]) << OPERAND3_MEM_SHIFT);
                }
            // jumps
            else if ( inst == "JMAE" )
            {
                ulong res = 0;
                // if -number
                if ( minusNumber.IsMatch(instruction[3]) )
                {
                    res = Convert.ToUInt64(ip) - convertNumTo( instruction[3].Remove(0,1) );
                }
                // if +number
                if ( rgxOnlyNum.IsMatch(instruction[3]) )
                {
                    res = Convert.ToUInt64(ip) + convertNumTo(instruction[3]);
                }
                // if label
                else if ( onlyLeters.IsMatch(instruction[3]) )    
                {
                    res = convertNumTo( symbolMapList[instruction[3]] );
                }

                com = (convertNumTo(JMAE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | res << OPERAND3_MEM_SHIFT);
            }
            else if ( inst == "JMNGE" )
            {
                ulong res = 0;
                // if -number
                if ( minusNumber.IsMatch(instruction[3]) )
                {
                    res = Convert.ToUInt64(ip) - convertNumTo(instruction[3].Remove(0, 1));
                }
                // if +number
                if ( rgxOnlyNum.IsMatch(instruction[3]) )
                {
                    res = Convert.ToUInt64(ip) + convertNumTo(instruction[3]);
                }
                // if label
                else if ( onlyLeters.IsMatch(instruction[3]) )
                {
                    res = convertNumTo(symbolMapList[instruction[3]]);
                }

                com = (convertNumTo(JMNGE) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | res << OPERAND3_MEM_SHIFT);
            }

            // flags BT, CMP, RCL
            else if ( inst == "BT" )
                com = (convertNumTo(BT) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT);
            else if ( inst == "CMP" )
                com = (convertNumTo(CMP) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT);
            else if ( inst == "RCL" )
                com = (convertNumTo(RCL) << INSTRUCTION_MEM_SHIFT | (convertNumTo(instruction[1]) << OPERAND1_MEM_SHIFT) | convertNumTo(instruction[2]) << OPERAND2_MEM_SHIFT | (convertNumTo(instruction[3])) << OPERAND3_MEM_SHIFT);

            // .FILL, HALT
            else if ( inst == ".FILL" )
            {
                if ( instruction[1][0] == '-' )
                {
                    com = (convertNumTo(instruction[1].Remove(0, 1)));
                    com = 0x0001000000000000 - com;
                }
                else
                {
                    com = 0x00007FFFFFFFFFFF & convertNumTo(instruction[1]);
                }
            }
            else if ( inst == "HALT" )
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

        // minusNumber arguments range
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
            StreamReader fstr = new StreamReader(argas);
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
                        if ( instruction[0] != "" )
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
                fstr = new StreamReader(argas);
                pos = 0;

                while ( (instructionLine = fstr.ReadLine()) != null )
                {
                    String[] instruction = instructionLine.Split(' ', '\t');
                    instruction[0] = instruction[0].Trim(':', ' ', '\t');

                    try
                    {
                        // if there is a label
                        if ( !this.checkForInvalidInstruction(pos, instruction[0]) )
                        {

                            instruction[1] = instruction[1].ToUpper();

                            // jumps
                            if ( instruction[1] == "JMAE" || instruction[1] == "JMNGE" )
                            {
                                if ( !minusNumber.IsMatch(instruction[3]) )
                                {
                                    if ( !rgxOnlyNum.IsMatch(instruction[3]) )
                                    {
                                        int ov;
                                        if ( !this.symbolMapList.TryGetValue(instruction[4], out ov) )
                                            this.addErrorLine(pos + "::There is no label \'" + instruction[3] + '\'');
                                    }
                                }
                            }
                            // save/load
                            if ( instruction[1] == "SAVE" || instruction[1] == "LOAD" )
                            {
                                // if @mem
                                if ( indirrectAddressing.IsMatch(instruction[3]) )
                                {
                                    int ov;
                                    instruction[3] = instruction[3].Remove(0, 1);
                                    if ( this.symbolMapList.TryGetValue(instruction[3], out ov) ) { }
                                    else addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                                }
                                else
                                // if mem
                                {
                                    int ov;
                                    if ( symbolMapList.TryGetValue(instruction[3], out ov) ) { }
                                    else addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                                }
                            }

                            // DIV/XIMUL/XOR/RCL/SHL/JMAE/JMNGE/ADD/NAND reg1 reg2 reg3/@mem
                            if ( (instruction[1] == "DIV") || (instruction[1] == "XIMUL") || (instruction[1] == "XOR") ||
                                  (instruction[1] == "SHL") || (instruction[1] == "ADD") || (instruction[1] == "NAND") )
                            {
                                if( !rgxOnlyNum.IsMatch( instruction[4]) )
                                {
                                    // if @mem
                                    if ( indirrectAddressing.IsMatch(instruction[4]) )
                                    {
                                        int ov;
                                        if ( this.symbolMapList.TryGetValue(instruction[4].Remove(0, 1), out ov) ) { }
                                        else addErrorLine(pos + "::There is not label \'" + instruction[4] + '\'');
                                    }
                                    else
                                    // if mem
                                    {
                                        int ov;
                                        if ( this.symbolMapList.TryGetValue(instruction[4], out ov) ) { }
                                        else addErrorLine(pos + "::There is not label \'" + instruction[4] + '\'');
                                    }
                                }
                            }
                        }
                        // if there is no label
                        else
                        {
                            instruction[0] = instruction[0].ToUpper();

                            if ( instruction[0] == "JMAE" || instruction[0] == "JMNGE"  )
                            {
                                if( !minusNumber.IsMatch(instruction[3]))
                                {
                                    if ( !rgxOnlyNum.IsMatch(instruction[3]))
                                    {
                                        int ov;
                                        if ( !this.symbolMapList.TryGetValue(instruction[3], out ov) )
                                            this.addErrorLine(pos + "::There is no label \'" + instruction[3] + '\'');
                                    }
                                }
                            }
                            // save/load
                            if ( instruction[0] == "SAVE" || instruction[0] == "LOAD" )
                            {
                                // if @mem
                                if ( indirrectAddressing.IsMatch(instruction[2]) )
                                {
                                    int ov;
                                    if ( this.symbolMapList.TryGetValue(instruction[2].Remove(0, 1), out ov) ) { }
                                    else addErrorLine(pos + "::There is not label \'" + instruction[2] + '\'');
                                }
                                else
                                // if mem
                                {
                                    int ov;
                                    if ( symbolMapList.TryGetValue(instruction[2], out ov) ) { }
                                    else addErrorLine(pos + "::There is not label \'" + instruction[2] + '\'');
                                }
                            }

                            // DIV/XIMUL/XOR/RCL/SHL/JMAE/JMNGE/ADD/NAND reg1 reg2 reg3/@mem
                            if ( (instruction[0] == "DIV") || (instruction[0] == "XIMUL") || (instruction[0] == "XOR") ||
                                  (instruction[0] == "SHL") || (instruction[0] == "ADD") || (instruction[0] == "NAND") )
                            {
                                if ( !rgxOnlyNum.IsMatch(instruction[4]) )
                                {
                                    // if @mem
                                    if ( indirrectAddressing.IsMatch(instruction[3]) )
                                    {
                                        int ov;
                                        instruction[3] = instruction[4].Remove(0, 1);
                                        if ( this.symbolMapList.TryGetValue(instruction[3], out ov) ) { }
                                        else addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                                    }
                                    else
                                    // if mem
                                    {
                                        int ov;
                                        if ( symbolMapList.TryGetValue(instruction[3], out ov) ) { }
                                        else addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                                    }
                                }
                            }

                        }
                        pos++;
                    }
                    catch ( Exception e ) { }
                }
                fstr.Close();
                fstr.Dispose();
            }

            // check for correct argument labels

            // end assembling
            if ( ERRORSTATE )
            {
                fstr.Close();
                fstr.Dispose();
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
            StreamReader fstr = new StreamReader(argas);
            StreamWriter fout = new StreamWriter(argmc);
            int pos = 0;

            while ( (instructionLine = fstr.ReadLine()) != null )
            {
                String[] instruction = instructionLine.Split(' ', '\t');
                String[] inst;
                // if there is label
                if ( !this.checkForInvalidInstruction(pos, instruction[0].ToUpper()) )
                {
                    if ( instruction.Length >= 2 )
                    {
                        instruction[1] = instruction[1].ToUpper();
                        inst = new String[instruction.Length - 1];
                        // and check this instruction
                        for ( int i = 1 ; i < instruction.Length ; i++ )
                        {
                            inst[i - 1] = instruction[i];
                        }
                        inst[0] = inst[0].ToUpper();

                        UInt64 com = this.translateToMachineCode(inst, pos);
                        fout.WriteLine(com);
                        res.Add(com.ToString() + " " + String.Format("0x{0:X}", com));

                    }
                }
                else
                {
                    inst = new String[instruction.Length];
                    inst = instruction;
                    inst[0] = inst[0].ToUpper();

                    UInt64 com = this.translateToMachineCode(inst, pos);
                    fout.WriteLine(com);
                    res.Add(com.ToString() + " " + String.Format("0x{0:X}", com));
                }

                pos++;
            }

            fout.Close();
            fstr.Close();
            fout.Dispose();
            fstr.Dispose();
            // end assembling
            return res;
        }
    }
}
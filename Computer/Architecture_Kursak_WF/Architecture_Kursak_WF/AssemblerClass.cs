using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Assembler
{
    public class AssemblerClass
    {
        const int MAX_MEM = 16777216;
        const int DATABUS = 48;
        const int MAX_REGISTERS = 64;
        const int MAX_LABELLENGHT = 6;

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

        const UInt64 INDIRRECT = 1;
        const UInt64 DIRECT = 0;
        // arithmetic instructions
        const UInt64 HALT = 0x1F;
        const UInt64 DEC = 1;              // DEC regA
        const UInt64 DIV = 2;              // DIV regA regB destreg
        const UInt64 XIMUL = 3;            // XIMUL regA regB destreg
        // logic instructions
        const UInt64 XOR = 4;              // XOR regA regB destreg
        const UInt64 SHL = 5;              // SHL regA regB destreg
        const UInt64 MOV = 6;              // MOV regA destreg
        // control instructions
        const UInt64 JMAE = 7;             // regA regB offset
        const UInt64 JMNGE = 8;            // regA regB offset
        // flags
        const UInt64 BT = 9;               // regA regB
        const UInt64 CMP = 10;             // regA regB
        const UInt64 RCL = 11;             // regA regB destreg
        // other
        const UInt64 LOAD = 12;            // regA mem
        const UInt64 SAVE = 13;            // regA mem
        const UInt64 CLEAR = 14;           // mem

        // from assol
        const UInt64 ADD = 15;             // regA regB destreg
        const UInt64 NAND = 16;            // regA regB destreg
        const UInt64 BEQ = 17;             // regA regB offset/label
        const UInt64 JALR = 18;            // regA regB offset/label

        // instruction list
        private String[] INSTRUCTION_ARRAY = { "halt", "dec", "div", "ximul", "xor", "shl", "mov", "jmae", "jmnge", ".fill", "bt", "cmp", "rcl", "load", "save", "clear", "add", "nand", "beq", "jalr" };

        // symbol map list
        public Dictionary<String, int> symbolMapList = new Dictionary<String, int>();
        public List<String> errorList = new List<String>();
        private List<String> instructionList = new List<String>();

        // error states
        private bool ERROR_STATE = false;
        private String arg1;
        private String arg2;

        private Regex rgxOnlyNumbers = new Regex(@"^[0-9]+$");
        private Regex rgxOnlyLeters = new Regex(@"[a-zA-Z]+");
        private Regex rgxIndirrectAddressing = new Regex(@"^@[\w]+");
        private Regex rgxSignedNumber = new Regex(@"^-[\d]+");

        public AssemblerClass(String arg1, String arg2)
        {
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
        
        private void addErrorLine(String e)
        {
            errorList.Add(e);
            ERROR_STATE = true;
        }

        public List<String> getErrorList()
        {
            return errorList;
        }

        private UInt64 toUInt64(dynamic num)
        {
            return Convert.ToUInt64(num);
        }

        private int toInt(String n)
        {
            return Convert.ToInt32(n);
        }

        private bool testRegisterBounds(int r)
        {
            if ( r >= 0 && r < MAX_REGISTERS )
                return true;
            return false;
        }

        private bool testMemoryBounds(int r)
        {
            if ( toUInt64(r) >= 0 && toUInt64(r) < MAX_MEM )
                return true;
            return false;
        }

        private bool testGetLabelFromSymbolList(String s)
        {
            int ov;
            if ( !this.symbolMapList.TryGetValue(s, out ov) )
                return false;
            return true;
        }

        private bool testGetIndirrectLabelFromSymbolList(String s)
        {
            int ov;
            // remove '@' from label and test
            if ( !this.symbolMapList.TryGetValue(s.Remove(0, 1), out ov) )
                return false;
            return true;
        }
        
        private bool checkForInvalidInstruction(int pos, String instruction)
        {
            foreach ( String s in INSTRUCTION_ARRAY )
            {
                if ( instruction.Equals(s) )
                {
                    return true;
                }
            }
            addErrorLine(pos + "::Invalid instruction \'" + instruction + "\'");
            return false;
        }

        private bool addToSymbolMap(int pos, String label)
        {
            // if label is starting with letter...
            if ( (Char.IsLetter(label, 0)) )
            {
                // if label length is smaller than MAX_LABELLENGHT...
                if ( (label.Length <= MAX_LABELLENGHT) )
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
                    addErrorLine(pos + "::Label length \'" + label + "\' has more than " + MAX_LABELLENGHT + " letters");
                    return false;
                }
            }
            else
            {
                addErrorLine(pos + "::Label \'" + label + "\' is not starting with letter");
                return false;
            }
        }

        private void checkForCorrectParametrs(int pos, String[] instruction)
        {
            String inst = instruction[0];

            // JMAE/JMNGE reg1 reg2 offset/label
            if ( (inst == "jmae") || (inst == "jmnge") || (inst == "beq") )
            {
                if ( (rgxOnlyNumbers.IsMatch(instruction[1])) && (rgxOnlyNumbers.IsMatch(instruction[2])) && (rgxOnlyLeters.IsMatch(instruction[3]) || rgxSignedNumber.IsMatch(instruction[3]) || rgxOnlyNumbers.IsMatch(instruction[3]) ) )
                {
                    if ( !testRegisterBounds(toInt(instruction[1])) || !testRegisterBounds(toInt(instruction[2])) )
                        addErrorLine(pos + "::Arguments has incorrect bounds");

                    if ( rgxOnlyLeters.IsMatch(instruction[3]) )
                    {
                        if ( !testGetLabelFromSymbolList(instruction[3]) )
                            addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                    }
                    else if ( rgxSignedNumber.IsMatch(instruction[3]) )
                    {
                        if ( ( pos - toInt(instruction[3].Remove(0, 1)) ) < 0 )
                            addErrorLine(pos + "::Bias out of range \'" + instruction[3] + '\'');
                    }
                }
                else addErrorLine(pos + "::Bad arguments");
             }

            // LOAD/SAVE reg1 mem/@mem
            if ( (inst == "load") || (inst == "save") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( rgxOnlyNumbers.IsMatch(instruction[1]) && (rgxOnlyLeters.IsMatch(instruction[2]) || rgxIndirrectAddressing.IsMatch(instruction[2])) )
                    {
                        if ( !testRegisterBounds( toInt(instruction[1])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");

                        if( rgxOnlyLeters.IsMatch(instruction[2]))
                        {
                            if(!testGetLabelFromSymbolList(instruction[2]) )
                                addErrorLine(pos + "::There is not label \'" + instruction[2] + '\'');
                        } 
                        else if( rgxIndirrectAddressing.IsMatch(instruction[2]) )
                        {
                            if ( !testGetIndirrectLabelFromSymbolList(instruction[2]) )
                                addErrorLine(pos + "::There is not label \'" + instruction[2] + '\'');
                        }
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers or have indirrect addressing with '@' or be a label");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            //  CLEAR/DEC mem
            if ( (inst == "clear") || (inst == "dec") )
            {
                if ( instruction.Length == 2 )
                {
                    if ( (rgxOnlyNumbers.IsMatch(instruction[1])) )
                    {
                        if ( !testRegisterBounds(toInt(instruction[1])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            // DIV/XIMUL/XOR/SHL/ADD/NAND reg1 reg2 reg3/@mem
            if ( (inst == "div") || (inst == "ximul") || (inst == "xor") ||
                 (inst == "shl") || (inst == "add") || (inst == "nand") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNumbers.IsMatch(instruction[1])) && (rgxOnlyNumbers.IsMatch(instruction[2])) && ( rgxOnlyNumbers.IsMatch(instruction[3]) || rgxIndirrectAddressing.IsMatch(instruction[2]) ) )
                    {
                        if ( !testRegisterBounds(toInt(instruction[1])) || !testRegisterBounds(toInt(instruction[2])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");

                        if ( rgxOnlyNumbers.IsMatch(instruction[3]) )
                        {
                            if ( !testRegisterBounds(toInt(instruction[3])) )
                                addErrorLine(pos + "::Arguments has incorrect bounds");
                        }
                        else if ( rgxIndirrectAddressing.IsMatch(instruction[3]) )
                        {
                            if ( !testGetIndirrectLabelFromSymbolList(instruction[3]) )
                                addErrorLine(pos + "::There is not label \'" + instruction[3] + '\'');
                        }
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers or have indirrect addressing with '@'");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }
            // RCL reg1 reg2 reg3
            if (  (inst == "rcl") )
            {
                if ( instruction.Length == 4 )
                {
                    if ( (rgxOnlyNumbers.IsMatch(instruction[1])) && (rgxOnlyNumbers.IsMatch(instruction[2])) && (rgxOnlyNumbers.IsMatch(instruction[3]) ) )
                    {
                        if ( !testRegisterBounds(toInt(instruction[1])) || !testRegisterBounds(toInt(instruction[2])) || !testRegisterBounds(toInt(instruction[3])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            // MOV/BT/CMP/JALR reg1 reg2
            if ( (inst == "mov") || (inst == "cmp") || (inst == "jalr") || (inst == "bt") )
            {
                if ( instruction.Length == 3 )
                {
                    if ( (rgxOnlyNumbers.IsMatch(instruction[1])) && ((rgxOnlyNumbers.IsMatch(instruction[2]))) )
                    {
                        if ( !testRegisterBounds(toInt(instruction[1])) || !testRegisterBounds(toInt(instruction[2])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");
                    }
                    else addErrorLine(pos + "::Arguments must consists only with numbers");
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }

            if ( inst == ".fill" )
            {
                if ( instruction.Length == 2 )
                {
                    if ( rgxOnlyLeters.IsMatch(instruction[1]) )
                    {
                        if ( !testRegisterBounds(toInt(instruction[1])) )
                            addErrorLine(pos + "::Arguments has incorrect bounds");
                    }  
                    else if ( instruction[1][0] == '-' )
                    {
                        long com = 0 - (Convert.ToInt64(instruction[1].Remove(0, 1)));
                        if ( (com < 0) && (com >= -140737488355327) ) { }
                        else addErrorLine(pos + "::Number is out of range");
                    }
                    else
                    {
                        long com = (Convert.ToInt64(instruction[1]));
                        if ( (com <= 140737488355327) && (com >= 0) ) { }
                        else addErrorLine(pos + "::Number is out of range");
                    }
                }
                else addErrorLine(pos + "::Bad arguments");
                return;
            }
        }

        private UInt64 translateToMachineCode(int ip, String[] instruction)
        {
            UInt64 com = 0;
            
            // load reg1 mem/@mem
            if ( instruction[0] == "load" )
            {
                // if @mem
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | LOAD << INSTRUCTION_MEM_SHIFT | toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT | toUInt64(symbolMapList[instruction[2].Remove(0, 1)]) << OPERAND3_MEM_SHIFT;
                else // if mem
                    com = LOAD << INSTRUCTION_MEM_SHIFT | toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT | toUInt64(symbolMapList[instruction[2]]) << OPERAND3_MEM_SHIFT;
            }

            // save reg1 mem/@mem
            else if ( instruction[0] == "save" )
            {
                // if @mem
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | SAVE << INSTRUCTION_MEM_SHIFT | toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT | toUInt64(symbolMapList[instruction[2].Remove(0, 1)]) << OPERAND3_MEM_SHIFT;
                else // if mem
                    com = SAVE << INSTRUCTION_MEM_SHIFT | toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT | toUInt64(symbolMapList[instruction[2]]) << OPERAND3_MEM_SHIFT;
            }
            
            // clear reg1
            else if ( instruction[0] == "clear" )
                com = CLEAR << INSTRUCTION_MEM_SHIFT | toUInt64(instruction[1]) << OPERAND3_MEM_SHIFT;

            // DEC reg1
            else if ( instruction[0] == "dec" )
                    com = DEC << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT);
            // div reg1 reg2 reg3/@mem
            else if ( instruction[0] == "div" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (DIV << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = DIV << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
            
            // ximul reg1 reg2 reg3/@mem
            else if ( instruction[0] == "ximul" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (toUInt64(XIMUL) << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = XIMUL << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
           
            // xor reg1 reg2 reg3/@mem
            else if ( instruction[0] == "xor" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (toUInt64(XOR) << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = XOR << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
           
            // shl reg1 reg2 reg3/@mem
            else if ( instruction[0] == "shl" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (toUInt64(SHL) << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = SHL << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
            
            // mov reg1 reg2
            else if ( instruction[0] == "mov" )
                com = MOV << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT;

            // add div reg1 reg2 reg3/@mem
            else if ( instruction[0] == "add" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (toUInt64(ADD) << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = ADD << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
            
            // nand reg1 reg2 reg3/@mem
            else if ( instruction[0] == "nand" )
                if ( rgxIndirrectAddressing.IsMatch(instruction[2]) )  // if @mem
                    com = INDIRRECT << INDIRRECT_ADDRESSING_SHIFT | (toUInt64(NAND) << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(symbolMapList[instruction[3].Remove(0, 1)]) << OPERAND3_MEM_SHIFT);
                else // if mem
                    com = NAND << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | toUInt64(instruction[3]) << OPERAND3_MEM_SHIFT;
            
            // jmae reg1 reg2 offset/label
            else if ( instruction[0] == "jmae" )
            {
                ulong res = 0;
                // if -number
                if ( rgxSignedNumber.IsMatch(instruction[3]) )
                    res = toUInt64(ip) - toUInt64( instruction[3].Remove(0,1) );
                
                // if +number
                if ( rgxOnlyNumbers.IsMatch(instruction[3]) )
                    res = toUInt64(ip) + toUInt64(instruction[3]);
                
                // if label
                else if ( rgxOnlyLeters.IsMatch(instruction[3]) )    
                    res = toUInt64( symbolMapList[instruction[3]] );

                com = JMAE << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | res << OPERAND3_MEM_SHIFT;
            }

            // jmnge reg1 reg2 offset/label
            else if ( instruction[0] == "jmnge" )
            {
                ulong res = 0;
                
                // if -number
                if ( rgxSignedNumber.IsMatch(instruction[3]) )
                    res = toUInt64(ip) - toUInt64(instruction[3].Remove(0, 1));
                
                // if +number
                if ( rgxOnlyNumbers.IsMatch(instruction[3]) )
                    res = toUInt64(ip) + toUInt64(instruction[3]);
                
                // if label
                else if ( rgxOnlyLeters.IsMatch(instruction[3]) )
                    res = toUInt64(symbolMapList[instruction[3]]);

                com = JMNGE << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | res << OPERAND3_MEM_SHIFT;
            }

            // beq reg1 reg2
            else if ( instruction[0] == "beq" )
            {
                ulong res = 0;
                
                // if -number
                if ( rgxSignedNumber.IsMatch(instruction[3]) )
                    res = toUInt64(ip) - toUInt64(instruction[3].Remove(0, 1));
                
                // if +number
                if ( rgxOnlyNumbers.IsMatch(instruction[3]) )
                    res = toUInt64(ip) + toUInt64(instruction[3]);
                
                // if label
                else if ( rgxOnlyLeters.IsMatch(instruction[3]) )
                    res = toUInt64(symbolMapList[instruction[3]]);

                com = BEQ << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | res << OPERAND3_MEM_SHIFT;
            }

            // jalr reg1 reg2
            else if ( instruction[0] == "jalr" )
                com = JALR << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT;

            // bt reg1 reg2
            else if ( instruction[0] == "bt" )
                com = BT << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT;
            
            // cmp reg1 reg2
            else if ( instruction[0] == "cmp" )
                com = CMP << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT;

            // cmp reg1 reg2
            else if ( instruction[0] == "rcl" )
                com = RCL << INSTRUCTION_MEM_SHIFT | (toUInt64(instruction[1]) << OPERAND1_MEM_SHIFT) | toUInt64(instruction[2]) << OPERAND2_MEM_SHIFT | (toUInt64(instruction[3])) << OPERAND3_MEM_SHIFT;

            // .FILL number
            else if ( instruction[0] == ".fill" )
            {
                if ( rgxOnlyLeters.IsMatch(instruction[1]) )
                    com = toUInt64(symbolMapList[instruction[1]]);
                else 
                {
                    if ( instruction[1][0] == '-' )
                    {
                        com = (toUInt64(instruction[1].Remove(0, 1)));
                        com = 0x0001000000000000 - com;
                    }
                    else
                        com = 0x00007FFFFFFFFFFF & toUInt64(instruction[1]);
                }
            }

            // halt
            else if ( instruction[0] == "halt" )
                com = (toUInt64(HALT) << INSTRUCTION_MEM_SHIFT);
            return com;
        }

        public bool readCodeAndCheck()
        {
            // read instructions from file and parse
            String instructionLine;
            String[] instruction;
            StreamReader fstr = new StreamReader(arg1);
            int pos = 0;

            // create symbol map
            while ( (instructionLine = fstr.ReadLine()) != null )
            {
                instructionLine = new Regex(@"[\s\t:]+").Replace(instructionLine, " ").Trim().ToLower();
                if ( instructionLine.IndexOf('#') != -1 )
                    instructionLine = instructionLine.Substring(0, instructionLine.IndexOf('#')).Trim();
                
                if ( instructionLine != "" )
                {
                    String test;
                    if ( instructionLine.IndexOf(' ') != -1 )
                        test = instructionLine.Substring(0, instructionLine.IndexOf(' '));
                    else test = instructionLine;

                    if ( INSTRUCTION_ARRAY.Any(test.Contains) )
                    {
                        instructionList.Add(instructionLine);
                    }
                    else
                    {
                        addToSymbolMap(pos, instructionLine.Substring(0, instructionLine.IndexOf(' ')) );
                        instructionList.Add( instructionLine.Substring( instructionLine.IndexOf(' ')+1, (instructionLine.Length - 1) - instructionLine.IndexOf(' ')) );
                    }
                }
                pos++;
            }

            fstr.DiscardBufferedData();
            fstr.Close();
            pos = 0;
            // check instructions and their arguments
            foreach ( String i in instructionList )
            {
                instruction = i.Split(' ');

                // if there is correct instruction...
                if ( checkForInvalidInstruction(pos, instruction[0]) )
                {
                    checkForCorrectParametrs(pos, instruction);
                }
                pos++;
            }

            if ( ERROR_STATE )
                return false;
            return true;
        }

        public List<String> createMachineCode()
        {
            UInt64 com = 0;
            int pos = 0;
            List<String> machineCodeList = new List<String>();
            StreamWriter fout = new StreamWriter(arg2);

            foreach ( String i in instructionList )
            {
                String[] instruction = i.Split(' ');
                com = translateToMachineCode(pos, instruction);
                fout.WriteLine(com);
                machineCodeList.Add(com.ToString());
            }

            fout.Close();
            return machineCodeList;
         }
    }
}

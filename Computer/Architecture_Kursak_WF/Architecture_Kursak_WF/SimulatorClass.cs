using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Simulator
{
    class SimulatorClass
    {
        const int MAXMEM = 16777216;
        const int DATABUS = 48;
        const int REGISTERS = 64;
        const int MAXLABELLENGHT = 6;

        // addressing
        // |         5         |        4       |       3        | 2 | 1 |      0                   |
        // | INSTRUCTION 47-44 | OPERAND1 39-32 | OPERAND2 31-24 |         OPERAND3 7-0 (mem = 23-0)|
        const int INDIRECT_ADDRESSING_SHIFT = 47;
        const int INSTRUCTION_MEM_SHIFT = 40;
        const int OPERAND1_MEM_SHIFT = 32;
        const int OPERAND2_MEM_SHIFT = 24;
        const int OPERAND3_MEM_SHIFT = 0;

        const long OPERAND1_MASK =    0x000000FF00000000;
        const long OPERAND2_MASK =    0x00000000FF000000;
        const long OPERAND3_MASK =    0x0000000000FFFFFF;
        const long INSTRUCTION_MASK = 0x00007F0000000000;
        const long INDIRECT_ADDRESSING_MASK = 0x0000800000000000;
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

        // standart instructions
        const int ADD = 15;             // regA regB destreg
        const int NAND = 16;            // regA regB destreg
        const int BEQ = 17;             // regA regB offset
        const int JALR = 18;

        // instruction list
        private String[] INSTRUCTIONSARRAY = { "HALT", "DEC", "DIV", "XIMUL", "XOR", "SHL", "MOV", "JMAE", "JMNGE", ".FILL", "BT", "CMP", "RCL", "LOAD", "SAVE", "CLEAR", "ADD", "NADN", "BEQ", "JARL" };
        String[] args;

        // init system
        long[] registers = new long[64];    // registers [0..63]
        long[] memory = new long[256]; // memory 0..16777215
        Flags f = new Flags();
        Flags stateFlags;
        // flags [3]
        public class Flags
        {
            public long CF;    // carry flag
            public long SF;    // sign flag
            public long ZF;    // zero flag
            public Flags(Flags o)
            {
                this.CF = o.CF;
                this.ZF = o.ZF;
                this.SF = o.SF;
            }
            public Flags() { }
        }

        private List<StateClass> states = new List<StateClass>();
        public List<StateClass> getStateList() { return states; }

        public class InstructionClass
        {
            public long instruction = 0;
            public long arg1 = 0;
            public long arg2 = 0;
            public long arg3 = 0;
        }

        public class StateClass 
        {
            public long ip = 0; // instruction pointer
            public String instructionLine;
            public long[] reg = new long[64];       // registers [0..63]
            public long[] mem = new long[256]; // memory 0..16777215 
            public Flags flags;      // CF, SF, ZF
            public InstructionClass instruction;
            public bool IndirrectAddressing = false;
            public StateClass(long ip, bool inAddr, String instructionLine, long[] memory, long[] registers, Flags f, InstructionClass i)
            {
                this.instructionLine = instructionLine;
                this.ip = ip;

                memory.CopyTo(this.mem, 0);
                registers.CopyTo(this.reg, 0);
                this.flags = new Flags();
                this.flags = f;
                this.instruction = new InstructionClass();
                this.instruction = i;
                this.IndirrectAddressing = inAddr;
            }
        }

        public SimulatorClass(String[] args)
        {
            if( args[0] != null )
            {
                this.args = args;
            
                // open file for reading
                try
                {
                    StreamReader fstr = new StreamReader(args[0]);
                    String instructionLine;     //  line with instructions readed from file
                    uint pos = 0;
                    while ( (instructionLine = fstr.ReadLine()) != null )
                    {
                        if ( pos > Convert.ToUInt32(memory.Length) )
                        {
                            MessageBox.Show("Memory overflow", "Memory overflow", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            break;
                        }
                        memory[pos] = Convert.ToInt64(instructionLine);
                        pos++;
                    }
                    fstr.Close();
                }
                catch ( FileNotFoundException e )
                {
                }
            }
        }

        private long convertInt48ToInt64(long num)
        {
            bool sign = Convert.ToBoolean( num & 0x800000000000 );          
            if ( sign ) // -num
            {
                String s = String.Format("{0:X}", num).Insert(0, "FFFF");
                long x = Convert.ToInt64(s, 16);
                return x;
            }
            else // +num
            {
                return Convert.ToInt64( num );
            }
        }

        private long convertInt64ToInt48(long num)
        {
            if ( num < 0 )
            {
                return 0x0001000000000000 - Math.Abs(num);
            }
            else
            {
                return 0x00007FFFFFFFFFFF & num;
            }
        }

        public void runCode()
        {
            long arg1 = 0, arg2 = 0, arg3 = 0, inst_ind_addr = 0;
            Int64 instruction;
            long ip = 0;
            long ip_old = 0;
            states.Add(new StateClass(ip, false, "Initial Status", memory, registers, new Flags(), new InstructionClass()));

            while ( true )
            {
                try
                {
                    instruction = Convert.ToInt64(memory[ip]);
                    String instructionLine = instruction.ToString();
                    // init system
                    ip++;
                    ip_old = ip;
                    InstructionClass ic = new InstructionClass();
                    // get arguments
                    // [ instruction | arg1 | arg2 | arg3 ]
                    arg3 = (instruction & OPERAND3_MASK) >> OPERAND3_MEM_SHIFT;
                    arg2 = (instruction & OPERAND2_MASK) >> OPERAND2_MEM_SHIFT;
                    arg1 = (instruction & OPERAND1_MASK) >> OPERAND1_MEM_SHIFT;
                    inst_ind_addr = (instruction & INDIRECT_ADDRESSING_MASK);
                    instruction = (instruction & INSTRUCTION_MASK) >> INSTRUCTION_MEM_SHIFT;
                    bool ind_addr = Convert.ToBoolean(inst_ind_addr);
                    ic.instruction = instruction;
                    ic.arg1 = arg1;
                    ic.arg2 = arg2;
                    ic.arg3 = arg3;

                    if ( instruction == LOAD )
                    {
                        if ( ind_addr )
                            registers[arg1] = memory[memory[arg3]];
                        else registers[arg1] = memory[arg3];
                    }
                    else if ( instruction == SAVE )
                    {
                        if ( ind_addr )
                            memory[memory[arg3]] = registers[arg1];
                        else memory[arg3] = registers[arg1];
                    }
                    else if ( instruction == CLEAR )
                    { memory[arg3] = 0; }

                    // DEC, DIV, XIMUL, XOR, SHL, MOV, JMAE, JMNGE, BT, CMP, RCL
                    else if ( instruction == DEC )
                    {
                        long x = convertInt48ToInt64(registers[arg1]);
                        registers[arg1] = x-1; 
                    }
                    else if ( instruction == DIV )
                    {
                        if ( ind_addr )
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(memory[memory[arg3]]);
                            registers[arg3] = convertInt64ToInt48(r1 / r2); 
                        }
                        else
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(registers[arg2]);
                            registers[arg3] = convertInt64ToInt48(r1 / r2); 
                        }
                    }
                    else if ( instruction == XIMUL )
                    {
                        if ( ind_addr )
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(memory[memory[arg3]]);
                            registers[arg3] = convertInt64ToInt48(r1 * r2);
                        }
                        else
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(registers[arg2]);
                            registers[arg3] = convertInt64ToInt48(r1 * r2);
                        }
                    }
                    else if ( instruction == XOR )
                    {
                        if ( ind_addr )
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(memory[memory[arg3]]);
                            registers[arg3] = convertInt64ToInt48(r1 ^ r2);
                        }
                        else
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(registers[arg2]);
                            registers[arg3] = convertInt64ToInt48(r1 ^ r2);
                        }
                    }
                    else if ( instruction == SHL )
                    {
                        if ( ind_addr )
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(memory[memory[arg3]]);
                            registers[arg3] = convertInt64ToInt48( r1 << Convert.ToInt32(registers[arg2]) ); 
                        }
                        else
                        {
                            long r1 = convertInt48ToInt64(registers[arg1]);
                            long r2 = convertInt48ToInt64(registers[arg2]);
                            registers[arg3] = convertInt64ToInt48( r1 << Convert.ToInt32(registers[arg2]) ); 
                        }
                    }
                    else if ( instruction == MOV )
                    { registers[arg2] = registers[arg1]; }
                    else if ( instruction == JMAE )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);
                        if ( ind_addr )
                        {
                            if ( Math.Abs(r1) >= Math.Abs(r2) )
                                ip = arg3;
                        }
                        else if ( Math.Abs(r1) >= Math.Abs(r2) )
                        {
                            ip = arg3;
                        }
                        ip_old = ip;
                    }
                    else if ( instruction == JMNGE )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);
                        if ( ind_addr )
                        {
                            if ( r1 <= r2 )
                                ip = arg3;
                        }
                        else if ( r1 <= r2 )
                        {
                            ip = arg3;
                        }
                        ip_old = ip;
                    }
                    else if ( instruction == BEQ )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);
                        if ( ind_addr )
                        {
                            if ( r1 == r2 )
                                ip = arg3;
                        }
                        else if ( r1 == r2 )
                        {
                            ip = arg3;
                        }
                        ip_old = ip;
                    }
                    else if ( instruction == JALR )
                    {
                        registers[arg2] = ip;
                        ip = registers[arg1];
                        ip_old = ip;
                    }
                    else if ( instruction == BT )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        int r2 = Convert.ToInt32(registers[arg2]);
                        f.CF = Convert.ToInt64((r1 & (1 << r2 )) != 0);
                    }
                    else if ( instruction == CMP )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);

                        if ( r1 == r2 ) 
                        { 
                            f.CF = 0; f.SF = 0; f.ZF = 1; 
                        }
                        else if ( r1 > r2 ) 
                        { 
                            f.CF = 0; f.SF = 0; f.ZF = 0; 
                        }
                        else if ( r1 < r2 ) 
                        { 
                            f.CF = 1; f.SF = 1; f.ZF = 0; 
                        }
                    }
                    else if ( instruction == ADD )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);
                        registers[arg3] = convertInt64ToInt48(r1 + r2);
                    }
                    else if ( instruction == NAND )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        long r2 = convertInt48ToInt64(registers[arg2]);
                        registers[arg3] = convertInt64ToInt48 ( ~(r1 & r2) );
                    }
                    else if ( instruction == RCL )
                    {
                        long r1 = convertInt48ToInt64(registers[arg1]);
                        int r2 = Convert.ToInt32(convertInt48ToInt64(registers[arg2]));
                        long t1 = r1;
                        for( int i=0; i<=r2; i++)
                        {
                            f.CF = Convert.ToInt32(Convert.ToBoolean(t1 & 0x800000000000));
                            t1 = r1 << i;
                            t1 += f.CF;
                        }
                        if ( Convert.ToBoolean(t1 & 0x800000000000) )
                            registers[arg3] = convertInt64ToInt48( (t1 & 0x0000FFFFFFFFFFFF) - 281474976710656 );
                        else registers[arg3] = convertInt64ToInt48( t1 & 0x0000FFFFFFFFFFFF);
                    }

                    // HALT   
                    else if ( instruction == HALT )     // checked
                    {
                        stateFlags = new Flags(f);
                        states.Add(new StateClass(ip_old, ind_addr, instructionLine, memory, registers, stateFlags, ic));
                        break;
                    }
                    // instructionLine
                    stateFlags = new Flags(f);
                    states.Add(new StateClass(ip_old, ind_addr, instructionLine, memory, registers, stateFlags, ic));
                }
                catch ( Exception e )
                {
                    MessageBox.Show("Memory out of bounds", "Memory out of bounds", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

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
        const int INSTRUCTION_MEM_SHIFT = 44;
        const int OPERAND1_MEM_SHIFT = 32;
        const int OPERAND2_MEM_SHIFT = 24;
        const int OPERAND3_MEM_SHIFT = 0;

        const long OPERAND1_MASK =    0x000000FF00000000;
        const long OPERAND2_MASK =    0x00000000FF000000;
        const long OPERAND3_MASK =    0x0000000000FFFFFF;
        const long INSTRUCTION_MASK = 0x0000F00000000000;
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
        String[] args;

        // init system
        int[] registers = new int[64];    // registers [0..63]
        int[] memory = new int[16777215]; // memory 0..16777215
                
        // flags [3]
        public class Flags
        {
            public int CF;    // carry flag
            public int SF;    // sign flag
            public int ZF;    // zero flag
        }

        public class InstructionClass
        {
            public long instruction = 0;
            public long arg1 = 0;
            public long arg2 = 0;
            public long arg3 = 0;
        }

        private List<StateClass> states = new List<StateClass>();
        public List<StateClass> getStateList() { return states; }
        public Dictionary<ulong, long> symbolMapList = new Dictionary<ulong, long>(); // symbol map

        public class StateClass 
        {
            public int ip = 0; // instruction pointer
            String instructionLine;
            public int[] reg = new int[64];       // registers [0..63]
            public int[] mem = new int[16777216]; // memory 0..16777215 
            public Flags f;      // CF, SF, ZF
            public InstructionClass instruction;
            public StateClass(int ip, String instructionLine, int[] memory, int[] registers, Flags f, InstructionClass i)
            {
                this.instructionLine = instructionLine;
                this.ip = ip;
                
                memory.CopyTo(this.mem, 0);
                registers.CopyTo(this.reg, 0);
                this.f = new Flags();
                this.f = f;
                this.instruction = new InstructionClass();
                this.instruction = i;
            }
        }

        public SimulatorClass(String[] args)
        {
            this.args = args;
            
            // create symbol map
            // open file for reading
            StreamReader fstr = new StreamReader(args[0]);
            String instructionLine;     //  line with instructions readed from file
            long instruction;
            ulong pos = 0;
            while ( (instructionLine = fstr.ReadLine()) != null )
            {
                instruction = Convert.ToInt64(instructionLine);
                long inst = (instruction & INSTRUCTION_MASK) >> INSTRUCTION_MEM_SHIFT;
                if ( inst == 0 ) symbolMapList.Add( pos, instruction);
                pos++;
            }
            fstr.Close();
        }

        public void runCode()
        {
            long arg1 = 0, arg2 = 0, arg3 = 0;
            Int64 instruction;
            int ip = 0;

            // open file for reading
            StreamReader fstr = new StreamReader(args[0]);
            String instructionLine;     //  line with instructions readed from file

            // initial state
            states.Add(new StateClass(ip,"Initial Status", memory, registers, new Flags(), new InstructionClass() ));

            while ( (instructionLine = fstr.ReadLine()) != null )
            {             
                instruction = Convert.ToInt64(instructionLine);
                // init system
                Flags f = new Flags();
                InstructionClass ic = new InstructionClass();
                // get arguments
                // [ instruction | arg1 | arg2 | arg3 ]
                arg3 = (instruction & OPERAND3_MASK) >> OPERAND3_MEM_SHIFT;
                arg2 = (instruction & OPERAND2_MASK) >> OPERAND2_MEM_SHIFT;
                arg1 = (instruction & OPERAND1_MASK) >> OPERAND1_MEM_SHIFT;
                instruction = (instruction & INSTRUCTION_MASK) >> INSTRUCTION_MEM_SHIFT;
                
                ic.instruction = instruction;
                ic.arg1 = arg1;
                ic.arg2 = arg2;
                ic.arg3 = arg3;

                if ( instruction == LOAD )
                {
                    ulong x = Convert.ToUInt32( arg2 );
                    registers[arg1] = Convert.ToInt32(symbolMapList[x]); 
                }
                else if ( instruction == SAVE )
                {
                    ulong x = Convert.ToUInt32( arg2 );
                    symbolMapList[x] = memory[arg1];
                }
                else if ( instruction == CLEAR )
                { memory[arg3] = 0; }

                // DEC, DIV, XIMUL, XOR, SHL, MOV, JMAE, JMNGE, BT, CMP, RCL
                else if ( instruction == DEC )
                { registers[arg1]--; }
                else if ( instruction == DIV )
                { registers[arg3] = registers[arg1] / registers[arg2]; }
                else if ( instruction == XIMUL )
                { registers[arg3] = registers[arg1] * registers[arg2]; }
                else if ( instruction == XOR )
                { registers[arg3] = registers[arg1] ^ registers[arg2]; }
                else if ( instruction == SHL )
                { registers[arg3] = registers[arg1] << registers[arg2]; }
                else if ( instruction == MOV )
                { registers[arg3] = registers[arg2]; }
                else if ( instruction == JMAE)
                { if ( registers[arg1] >= registers[arg2] ) ip = ip + 1 + registers[arg3]; }
                else if ( instruction == JMNGE )
                { if (registers[arg1] <= registers[arg2]) ip=ip+1+registers[arg3]; }
                else if ( instruction == BT )
                {
                    int t = registers[arg1];
                    f.CF = (registers[arg1] << registers[arg2]) ^ registers[arg2]+1;
                }
                else if ( instruction == CMP )
                { 
                    int res = registers[arg1] - registers[arg2];
                    if( res > 0 ) { f.CF = 0; f.SF = 0; f.ZF = 0; }
                    else if( res == 0 ) { f.CF = 0; f.SF = 0; f.ZF = 1; }
                    else if( res < 0  ) { f.CF = 1; f.SF = 1; f.ZF = 0; }
                }
                else if ( instruction == RCL )
                {
                //    int t1, t2;
                //    int n = registers[arg1];
                //    registers[arg2] = registers[arg2] % (sizeof( int ) * 8);                       // нормализуем n
                //    t1 = registers[arg1] << registers[arg2];                      // двигаем а влево на n бит, теряя старшие биты
                //    t2 = registers[arg1] >> (sizeof( int ) * 8 - registers[arg2] );    // перегоняем старшие биты в младшие
                //    registers[arg3] = t1 | t2;                     // объединяем старшие и младшие биты
                    int value = registers[arg1];
                    int count = registers[arg2];
                    registers[arg3] = (value >> count) + (((value << (32 - count)) >> (32 - count)) << count);
                }

                // HALT   
                else if ( instruction == HALT )
                { break; }

                states.Add(new StateClass(ip, instructionLine, memory, registers, f, ic));
                ip++;
            }
            fstr.Close();
        }

    }
}

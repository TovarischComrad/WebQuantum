using System;
using System.Collections.Generic;
using System.Text;
using QuantumCore.Quantum;

namespace BL
{
    public static class QLogic
    {
        public static string Operator = "I";

        public static List<List<string>> Table = new List<List<string>>();  

        public static int Find(string temp, int type, int j)
        {
            int result = 0;
            for (int i = 0; i < Table.Count; i++) 
            { 
                if (Table[i][j] == temp + type.ToString())
                {
                    result = i; break;
                }
            }
            return result;  
        }

        public static List<int> MakeCircuit()
        {
            // Ранняя версия - ПЕРЕДЕЛАТЬ!!!
            Template template = new Template(Table.Count);
            for (int j = 0; j < Table[0].Count; j++) 
            { 
                for (int i = 0; i < Table.Count; i++)
                {
                    if (Table[i][j] != "CX0" && Table[i][j] != "CX1" && Table[i][j] != "I")
                    {
                        template.Add(Table[i][j], new double[1] { i });
                    }
                    if (Table[i][j] == "CX0")
                    {
                        int k = Find("CX", 1, j);
                        template.Add("CNOT", new double[2] { i, k });
                        break;
                    }
                    if (Table[i][j] == "CX1")
                    {
                        int k = Find("CX", 0, j);
                        template.Add("CNOT", new double[2] { k, i });
                        break;
                    }
                }
            }
            Circuit circuit = new Circuit(template);
            QubitReg qreg = new QubitReg(Table.Count);
            Simulator simulator = new Simulator(qreg, circuit); 
            simulator.Run();
            List<int> lst = simulator.Result;
            return lst;     
        }
    }
}

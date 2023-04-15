using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumCore.Quantum
{
    internal class Algorithms
    {
        public static void QAOA()
        {
            Template t = new Template(4);
            t.Add("H", new double[1] { 0 });
            t.Add("H", new double[1] { 1 });
            t.Add("H", new double[1] { 2 });
            t.Add("H", new double[1] { 3 });

            double p = 12.0;
            double h = System.Math.PI / p;
            double beta = 0.0;
            double gamma = 0.0;

            for (int i = 0; i < p; i++)
            {
                // Вершины графа
                t.Add("RX", new double[2] { 0, 2 * beta });
                t.Add("RX", new double[2] { 1, 2 * beta });
                t.Add("RX", new double[2] { 2, 2 * beta });
                t.Add("RX", new double[2] { 3, 2 * beta });

                // Ребра графа
                t.Add("CNOT", new double[2] { 0, 1 });
                t.Add("RZ", new double[2] { 1, 2 * gamma });
                t.Add("CNOT", new double[2] { 0, 1 });

                t.Add("CNOT", new double[2] { 0, 3 });
                t.Add("RZ", new double[2] { 3, 2 * gamma });
                t.Add("CNOT", new double[2] { 0, 3 });

                t.Add("CNOT", new double[2] { 1, 2 });
                t.Add("RZ", new double[2] { 2, 2 * gamma });
                t.Add("CNOT", new double[2] { 1, 2 });

                t.Add("CNOT", new double[2] { 2, 3 });
                t.Add("RZ", new double[2] { 3, 2 * gamma });
                t.Add("CNOT", new double[2] { 2, 3 });

                beta += h;
                gamma += h;
            }

            Circuit circ = new Circuit(t);
            QubitReg qreg = new QubitReg(4);
            Simulator sim = new Simulator(qreg, circ);
            sim.Run();

            double s = 0.0;
            List<double> prob = qreg.Probability();
            for (int i = 0; i < prob.Count; i++)
            {
                Console.WriteLine(prob[i]);
                s += prob[i];
            }
            Console.WriteLine("___________________________________________________");
            Console.WriteLine(s);
        }

        public static void Grover()
        {
            Template t = new Template(3);
            t.Add("Hn", new double[2] { 0, 2 });
            t.Add("GroverOracle", new double[2] { 0, 2 });
            t.Add("GroverDiffuser", new double[2] { 0, 2 });

            Circuit circ = new Circuit(t);
            QubitReg qreg = new QubitReg(3);
            Simulator sim = new Simulator(qreg, circ);
            sim.Run();

            double s = 0.0;
            List<double> prob = qreg.Probability();
            for (int i = 0; i < prob.Count; i++)
            {
                Console.WriteLine(prob[i]);
                s += prob[i];
            }
            Console.WriteLine("___________________________________________________");
            Console.WriteLine(s);
        }
    }
}

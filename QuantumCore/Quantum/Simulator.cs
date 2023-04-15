using QuantumCore.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumCore.Quantum
{
    public class Simulator
    {
        public QubitReg QubitReg { get; }
        public Circuit Circuit { get; }
        public List<int> Result { get; }

        public Simulator(QubitReg qubitReg, Circuit circuit)
        {
            QubitReg = qubitReg;
            Circuit = circuit;
            Result = new List<int>();
            for (int i = 0; i < qubitReg.Size; i++)
            {
                Result.Add(-1);  
            } 
        }

        public void Run()
        {
            for (int i = 0; i < Circuit.Operators.Count; i++)
            {
                // Измерение состояния
                if (Circuit.Operators[i].Width == 1)
                {
                    // Нахождение длины блоков
                    int n = (int)Circuit.Operators[i][0][0].Re;
                    int k = (int)System.Math.Pow(2, QubitReg.Size - 1 - n);

                    // Определение вероятности измерения состояния 0 или 1
                    double p = 0.0;
                    int j = 0; // указатель на i компоненту вектора
                    int t = 0; // указатель на номер блока
                    while (j < QubitReg.Amplitude.Size)
                    {
                        while (j < QubitReg.Amplitude.Size && j < t * k + k)
                        {
                            double amp = QubitReg.Amplitude[j].abs();
                            amp *= amp;
                            p += amp;
                            j++;
                        }
                        t += 2;
                        j += k;
                    }

                    // Проведение испытания Бернулли
                    Random random = new Random();
                    double seed = random.NextDouble();
                    if (seed < p)
                    {
                        Result[n] = 0;
                    }
                    else
                    {
                        Result[n] = 1;
                    }

                    // Изменение состояния вектора
                    j = k * (1 - Result[n]); // указатель на i компоненту вектора
                    t = 1 - Result[n]; // указатель на номер блока
                    while (j < QubitReg.Amplitude.Size)
                    {
                        while (j < QubitReg.Amplitude.Size && j < t * k + k)
                        {
                            QubitReg.Amplitude[j] = new Complex(0);
                            j++;
                        }
                        t += 2;
                        j += k;
                    }
                    QubitReg.Amplitude = QubitReg.Amplitude.Norm();
                }
                // Применение оператора
                else
                {
                    Matrix m = Circuit.Operators[0];
                    QubitReg.Amplitude = QubitReg.Amplitude.MatrixProduct(m);
                }
            }

            // Очистка регистра
            bool fl = true;
            for (int i = 0; i < Result.Count; i++)
            {
                if (Result[i] == -1) {
                    fl = false;
                    break;
                }
            }
            if (fl)
            {
                for (int i = 0; i < QubitReg.Amplitude.Size; i++)
                {
                    QubitReg.Amplitude[i] = new Complex(0);
                }
            }
        }
    }
}

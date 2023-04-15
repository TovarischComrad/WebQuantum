using QuantumCore.Math;
using System.Collections.Generic;


namespace QuantumCore.Quantum
{
    public class Circuit
    {
        internal List<Matrix> Operators { get; }
        public List<bool> Types { get; }
        private Template Template;

        // Преобразование квантовой схемы в набор матричных операторов
        public Circuit(Template template)
        {
            Operators = new List<Matrix>();
            Template = template;

            List<int> types = Template.Type;

            int size = (int)System.Math.Pow(2, Template.Size);
            int j = 0; // Указатель на операторы

            // Перебор операторов
            while (j < types.Count)
            {
                // ОПЕРАТОР ИЗМЕРЕНИЯ
                if (types[j] == 0)
                {
                    int num = 0;
                    for (int i = 0; i < Template.Size; i++)
                    {
                        if (Template._Template[i][j] == "M")
                        {
                            num = i;
                            break;
                        }
                    }
                    Operators.Insert(0, new Matrix(1, 1, new Complex(num)));
                    j++;
                    continue;
                }

                Matrix Op = new Matrix();
                // УНАРНЫЕ ОПЕРАТОРЫ
                if (types[j] == 1)
                {
                    // Накопитель для тензорного произведения матриц
                    Op = new Matrix(1, 1, new Complex(1));

                    // Перебор кубитов
                    //for (int i = Template.Size - 1; i >= 0; i--)
                    //{
                    //    Op = Op.TensorProduct(Operator.OperatorsDict[Template._Template[i][j]]);
                    //}
                    for (int i = 0; i < Template.Size; i++)
                    {
                        if (Template._Template[i][j] == "R")
                        {
                            Matrix R = Operator.Rphi(Template.Parameters[j]);
                            Op = Op.TensorProduct(R);
                        }
                        else if (Template._Template[i][j] == "RX")
                        {
                            Matrix RX = Operator.RX(Template.Parameters[j]);
                            Op = Op.TensorProduct(RX);
                        }
                        else if (Template._Template[i][j] == "RY")
                        {
                            Matrix RY = Operator.RY(Template.Parameters[j]);
                            Op = Op.TensorProduct(RY);
                        }
                        else if (Template._Template[i][j] == "RZ")
                        {
                            Matrix RZ = Operator.RZ(Template.Parameters[j]);
                            Op = Op.TensorProduct(RZ);
                        }
                        else
                        {
                            Op = Op.TensorProduct(Operator.OperatorsDict[Template._Template[i][j]]);
                        }
                    }
                }

                // БИНАРНЫЕ ОПЕРАТОРЫ
                if (j < types.Count && (types[j] == 2 || types[j] == 4))
                {
                    List<int> ind = new List<int>();
                    int i0 = 0;
                    int i1 = 0;
                    string name = "";

                    for (int i = 0; i < Template.Size; i++)
                    {
                        if (Template._Template[i][j] == "CNOT0") {
                            i0 = i;
                            name = "CNOT";
                        }
                        if (Template._Template[i][j] == "CNOT1")
                        {
                            i1 = i;
                            name = "CNOT";
                        }

                        if (Template._Template[i][j] == "CR0")
                        {
                            i0 = i;
                            name = "CR";
                        }
                        if (Template._Template[i][j] == "CR1")
                        {
                            i1 = i;
                            name = "CR";
                        }

                        if (Template._Template[i][j] == "SWAP")
                        {
                            ind.Add(i);
                            name = "SWAP";
                        }

                        if (Template._Template[i][j] == "QFT")
                        {
                            ind.Add(i);
                            name = "QFT";
                        }

                        if (Template._Template[i][j] == "GroverOracle")
                        {
                            ind.Add(i);
                            name = "GroverOracle";
                        }

                        if (Template._Template[i][j] == "Hn")
                        {
                            ind.Add(i);
                            name = "Hn";
                        }

                        if (Template._Template[i][j] == "GroverDiffuser")
                        {
                            ind.Add(i);
                            name = "GroverDiffuser";
                        }
                    }

                    Matrix S = new Matrix();
                    if (name == "SWAP") {
                        i0 = ind[0];
                        i1 = ind[1];
                        S = Operator.SWAP(System.Math.Abs(i1 - i0));
                    }
                    if (name == "QFT")
                    {
                        i0 = ind[0];
                        i1 = ind[1];
                        S = Operator.QFT(System.Math.Abs(i1 - i0 + 1));
                    }
                    if (name == "GroverOracle")
                    {
                        i0 = ind[0];
                        i1 = ind[1];
                        S = Operator.GroverOracle(System.Math.Abs(i1 - i0 + 1));
                    }
                    if (name == "Hn")
                    {
                        i0 = ind[0];
                        i1 = ind[1];
                        S = Operator.Hn(System.Math.Abs(i1 - i0 + 1));
                    }
                    if (name == "GroverDiffuser")
                    {
                        i0 = ind[0];
                        i1 = ind[1];
                        S = Operator.GroverDiffuser(System.Math.Abs(i1 - i0 + 1));
                    }
                    if (name == "CNOT") { S = Operator.CNOT(i1 - i0); }
                    if (name == "CR") { S = Operator.CR(i1 - i0, Template.Parameters[j]); }

                    //Matrix Op = new Matrix(1, 1, new Complex(1));
                    //for (int i = Template.Size - 1; i > System.Math.Max(i0, i1); i--)
                    //{
                    //    Op = Op.TensorProduct(Operator.I);
                    //}
                    //Op = Op.TensorProduct(S);
                    //for (int i = System.Math.Min(i0, i1) - 1; i >= 0; i--)
                    //{
                    //    Op = Op.TensorProduct(Operator.I);
                    //}

                    Op = new Matrix(1, 1, new Complex(1));
                    for (int i = 0; i < System.Math.Min(i0, i1); i++)
                    {
                        Op = Op.TensorProduct(Operator.I);
                    }
                    Op = Op.TensorProduct(S);
                    for (int i = System.Math.Max(i0, i1) + 1; i < Template.Size; i++)
                    {
                        Op = Op.TensorProduct(Operator.I);
                    }
                }

                //if (Types.Count == 0 || !Types[Types.Count - 1])
                //{
                //    Operators.Add(Op);
                //}
                //else
                //{
                //    // Operators[Operators.Count - 1] = Op.Product(Operators[Operators.Count - 1]);
                //    Operators[Operators.Count - 1] = Operators[Operators.Count - 1].Product(Op);
                //}

                // Operators.Add(Op);
                Operators.Insert(0, Op);
                j++;
            }

            List<Matrix> result = new List<Matrix>();
            int t = 0;
            while (t < Operators.Count)
            {
                Matrix res = new Matrix(size);

                bool fl = false;
                while (t < Operators.Count && Operators[t].Height != 1)
                {
                    res = res.Product(Operators[t]);
                    t++;
                    fl = true;
                }
                if (fl) { result.Add(res); }
                
                if (t < Operators.Count && Operators[t].Height == 1)
                {
                    result.Add(Operators[t]);
                    t++;
                }
            }

            result.Reverse();
            Operators = result;
        } 
    }
}
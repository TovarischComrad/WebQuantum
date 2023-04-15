using System;
using System.Collections.Generic;
using System.Text;
using QuantumCore.Math;

namespace QuantumCore.Quantum
{
    internal class Qubit
    {
        public Vector Amplitude { get; }
        public Qubit()
        {
            Amplitude = new Vector { new Complex(1), new Complex(0) };
        }

        public override string ToString()
        {
            return Amplitude.ToString();
        }
    }

    public class QubitReg
    {
        internal Vector Amplitude { get; set; }
        public int Size { get; }

        public QubitReg()
        {
            Qubit q = new Qubit();
            Amplitude = q.Amplitude;
            Size = 1;
        }

        public QubitReg(int size)
        {
            Vector res = new Vector(1, new Complex(1));  
            for (int i = 0; i < size; i++)
            {
                Qubit q = new Qubit();
                res = res.TensorProduct(q.Amplitude);
            }
            Amplitude = res;
            Size = size;
        }

        public List<double> Probability()
        {
            List<double> res = new List<double>();
            for (int i = 0; i < Amplitude.Size; i++)
            {
                double r = Amplitude[i].abs();
                res.Add(r * r);
            }
            return res;
        }

        public override string ToString()
        {
            return Amplitude.ToString();
        }
    }

    internal class Operator
    {
        public static readonly Matrix Zero
            = new Matrix { new Vector { new Complex(1), new Complex(0) },
                           new Vector { new Complex(0), new Complex(0) } };

        public static readonly Matrix One
            = new Matrix { new Vector { new Complex(0), new Complex(0) },
                           new Vector { new Complex(0), new Complex(1) } };

        public static readonly Matrix I
            = new Matrix { new Vector { new Complex(1), new Complex(0) },
                           new Vector { new Complex(0), new Complex(1) } };

        public static readonly Matrix X
            = new Matrix { new Vector { new Complex(0), new Complex(1) },
                           new Vector { new Complex(1), new Complex(0) } };

        public static readonly Matrix Y
            = new Matrix { new Vector { new Complex(0), new Complex(0, -1) },
                           new Vector { new Complex(0, 1), new Complex(0) } };

        public static readonly Matrix Z
            = new Matrix { new Vector { new Complex(1), new Complex(0) },
                           new Vector { new Complex(0), new Complex(-1) } };

        public static readonly Matrix H
            = new Matrix { new Vector { new Complex(1), new Complex(1) },
                           new Vector { new Complex(1), new Complex(-1) } }
                         .ScalarProduct(new Complex(1.0 / System.Math.Sqrt(2)));

        public static readonly Matrix S
            = new Matrix { new Vector { new Complex(1), new Complex(0) },
                           new Vector { new Complex(0), new Complex(0, 1) } };

        public static readonly Matrix T
            = new Matrix { new Vector { new Complex(1), new Complex(0) },
                           new Vector { new Complex(0), new Complex(1, System.Math.PI / 4, true) } };

        public static readonly Dictionary<string, Matrix> OperatorsDict = new Dictionary<string, Matrix>()
        {
            { "Zero", Zero },
            { "One", One },
            { "I", I },
            { "H", H },
            { "X", X },
            { "Y", Y },
            { "Z", Z },
            { "S", S },
            { "T", T }
        };

        public static Matrix Rphi(double phi)
        {
            Matrix R = new Matrix(2);
            Complex z = new Complex(1, phi, true);
            R[1][1] = z;
            return R;
        }

        public static Matrix RX(double theta)
        {
            Matrix RX = new Matrix(2);
            Complex z1 = new Complex(System.Math.Cos(theta / 2.0));
            Complex z2 = new Complex(0.0, -System.Math.Sin(theta / 2.0));
            RX[0][0] = z1;
            RX[0][1] = z2;
            RX[1][0] = z2;
            RX[1][1] = z1;
            return RX;
        }

        public static Matrix RY(double theta)
        {
            Matrix RY = new Matrix(2);
            Complex z1 = new Complex(System.Math.Cos(theta / 2.0));
            Complex z2 = new Complex(0.0, -System.Math.Sin(theta / 2.0));
            Complex z3 = new Complex(0.0, System.Math.Sin(theta / 2.0));
            RY[0][0] = z1;
            RY[0][1] = z2;
            RY[1][0] = z3;
            RY[1][1] = z1;
            return RY;
        }

        public static Matrix RZ(double theta)
        {
            Matrix RZ = new Matrix(2);
            Complex z1 = new Complex(1.0, -theta / 2.0, true);
            Complex z2 = new Complex(1.0, theta / 2.0, true);
            RZ[0][0] = z1;
            RZ[1][1] = z2;
            return RZ;
        }

        public static Matrix Controlled(int n, Matrix Op)
        {
            Matrix first = new Matrix();
            Matrix second = new Matrix();
            if (n > 0)
            {
                first = Zero;
                second = One;
                for (int i = 0; i < n - 1; i++)
                {
                    first = first.TensorProduct(I);
                    second = second.TensorProduct(I);
                }
                first = first.TensorProduct(I);
                second = second.TensorProduct(Op);
            }
            else
            {
                first = I;
                second = Op;
                for (int i = 0; i < -1 - n; i++)
                {
                    first = first.TensorProduct(I);
                    second = second.TensorProduct(I);
                }
                first = first.TensorProduct(Zero);
                second = second.TensorProduct(One);
            }
            Matrix S = first.Plus(second);
            return S;
        }

        public static Matrix CNOT(int n)
        {
            return Controlled(n, X);
        }

        public static Matrix CR(int n, double phi)
        {
            Matrix R = Rphi(phi);
            return Controlled(n, R);
        }

        public static Matrix SWAP(int n)
        {
            Matrix M1 = CNOT(n);
            Matrix M2 = CNOT(-n);
            Matrix Res = M1.Product(M2);
            Res = Res.Product(M1);
            return Res;
        }

        public static Matrix QFT(int n)
        {
            Template t = new Template(n);
            for (int i = 0; i < n; i++)
            {
                t.Add("H", new double[1] { i });
                double phi = 2.0;
                for (int j = 2; j < n - i + 1; j++)
                {
                    t.Add("CR", new double[3] { i + j - 1, i, System.Math.PI / phi });
                    phi *= 2.0;
                }
            }
            Circuit circ = new Circuit(t);
            return circ.Operators[0];
        }

        public static int f(int n)
        {
            if (n == 5) { return -1; }
            else { return 1; }
        }

        public static Matrix GroverOracle(int n)
        {
            int N = (int)System.Math.Pow(2, n);
            Matrix M = new Matrix(N);
            for (int i = 0; i < N; i++)
            {
                M[1][1] = new Complex(-1);
                M[4][4] = new Complex(-1);
                // M[i][i] = new Complex(System.Math.Pow(-1.0, f(i)));
            }
            return M;
        }

        public static Matrix Hn(int n)
        {
            Matrix M = H;
            for (int i = 1; i < n; i++)
            {
                M = H.TensorProduct(M);
            }
            return M;
        }

        public static Matrix GroverDiffuser(int n)
        {
            Matrix M = Hn(n);
            int N = (int)System.Math.Pow(2, n);

            Vector v = new Vector(N);
            v[0] = new Complex(1);
            v = v.MatrixProduct(M);

            Matrix s1 = new Matrix(N, 1);
            Matrix s2 = new Matrix(1, N);
            for (int i = 0; i < N; i++)
            {
                s1[i][0] = v[i];
                s2[0][i] = v[i];
            }
            Matrix res = s1.TensorProduct(s2);
            res = res.ScalarProduct(new Complex(2));
            Matrix identity = new Matrix(N);
            res = res.Minus(identity);

            return res;
        }
    }
}

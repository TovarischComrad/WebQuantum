using System;
using System.Collections;
using System.Text;

namespace QuantumCore.Math
{
    public class Complex
    {
        public double Re { get; set; }
        public double Im { get; set; }
        public Complex()
        {
            Re = 0;
            Im = 0;
        }
        public Complex(double a)
        {
            Re = a;
            Im = 0;
        }
        public Complex(double a, double b, bool polar = false)
        {
            if (!polar)
            {
                Re = a;
                Im = b;
            }
            else
            {
                Re = a * System.Math.Cos(b);
                Im = a * System.Math.Sin(b);
            }
        }
        public Complex(Complex z)
        {
            Re = z.Re;
            Im = z.Im;
        }
        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Re + c2.Re, c1.Im + c2.Im);
        }
        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Re - c2.Re, c1.Im - c2.Im);
        }
        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Re * c2.Re - c1.Im * c2.Im, c1.Re * c2.Im + c1.Im * c2.Re);
        }
        public static Complex operator /(Complex c1, Complex c2)
        {
            double den = c2.Re * c2.Re + c2.Im * c2.Im;
            return new Complex((c1.Re * c2.Re + c1.Im * c2.Im) / den, (c2.Re * c1.Im - c1.Re * c2.Im) / den);
        }
        public double abs()
        {
            return System.Math.Sqrt(Re * Re + Im * Im);
        }
        public override string ToString()
        {
            if (Im >= 0)
            {
                return string.Format("{0:F2}", Re) + "+" + string.Format("{0:F2}", Im) + "i";
            }
            else
            {
                return string.Format("{0:F2}", Re) + "-" + string.Format("{0:F2}", -1 * Im) + "i";
            }
        }
    }

    // Определение комплексного числа, как перечисляемого элемента
    internal class ComplexEnum : IEnumerator
    {
        public Complex[] _Complex;
        int position = -1;

        public ComplexEnum(Complex[] arr)
        {
            _Complex = arr;
        }

        public bool MoveNext()
        {
            position++;
            return position < _Complex.Length;
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get => Current;
        }

        public Complex Current
        {
            get
            {
                try
                {
                    return _Complex[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

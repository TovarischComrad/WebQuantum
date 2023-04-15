using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QuantumCore.Math
{
    internal class Vector : IEnumerable
    {
        public int Size { get; protected set; }
        protected List<Complex> _Vector;

        // Конструкторы
        public Vector()
        {
            Size = 0;
            _Vector = new List<Complex>();
        }
        public Vector(int size, bool random = false)
        {
            Size = size;
            _Vector = new List<Complex>();
            for (int i = 0; i < size; i++)
            {
                if (random)
                {
                    double maximum = 10;
                    double minimum = -10;
                    Random rand = new Random();
                    double re = rand.NextDouble() * (maximum - minimum) + minimum;
                    double im = rand.NextDouble() * (maximum - minimum) + minimum;
                    _Vector.Add(new Complex(re, im));
                }
                else
                {
                    _Vector.Add(new Complex(0));
                }
            }
        }
        public Vector(int size, Complex z)
        {
            Size = size;
            _Vector = new List<Complex>();
            for (int i = 0; i < size; i++)
            {
                _Vector.Add(new Complex(z));
            }
        }
        public Vector(Complex[] arr)
        {
            Size = arr.Length;
            _Vector = new List<Complex>();
            for (int i = 0; i < Size; i++)
            {
                _Vector.Add(new Complex(arr[i]));
            }
        }

        // Вставка и удаление элемента
        public void Add(Complex z)
        {
            Size++;
            _Vector.Add(z);
        }
        public void Delete()
        {
            Size--;
            _Vector.RemoveAt(Size);
        }

        // Операции над векторами
        public Vector Plus(Vector vec)
        {
            try
            {
                Vector res = new Vector(Size);
                for (int i = 0; i < Size; i++)
                {
                    res[i] = this[i] + vec[i];
                }
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }

        public Vector Minus(Vector vec)
        {
            try
            {
                Vector res = new Vector(Size);
                for (int i = 0; i < Size; i++)
                {
                    res[i] = this[i] - vec[i];
                }
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }

        public Vector ScalarProd(Complex c)
        {
            Vector res = new Vector(Size);
            for (int i = 0; i < Size; i++)
            {
                res[i] = c * this[i];
            }
            return res;
        }

        public Vector TensorProduct(Vector vec)
        {
            Vector res = new Vector(Size * vec.Size);
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < vec.Size; j++)
                {
                    res[i * vec.Size + j] = this[i] * vec[j];
                }
            }
            return res;
        }

        public Vector MatrixProduct(Matrix m)
        {
            try
            {
                Vector res = new Vector(m.Height);
                for (int i = 0; i < res.Size; i++)
                {
                    Complex sum = new Complex();
                    for (int j = 0; j < Size; j++)
                    {
                        sum = sum + m[i][j] * this[j];
                    }
                    res[i] = sum;
                }
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }

        public Vector Norm()
        {
            Vector res = new Vector(Size);
            double den = 0.0;
            for (int i = 0; i < Size; i++)
            {
                den += (this[i] * this[i]).abs();
            }
            Complex c = new Complex(System.Math.Sqrt(den));
            double eps = 0.000001;
            bool fl = System.Math.Abs(c.Re) < eps && System.Math.Abs(c.Im) < eps;
            for (int i = 0; i < Size; i++)
            {
                if (fl) { res[i] = new Complex(0); }
                else { res[i] = this[i] / c; }
            }
            return res;
        }

        // Технические методы
        public override string ToString()
        {
            string s = "{";
            for (int i = 0; i < Size; i++)
            {
                s += " " + _Vector[i].ToString() + " ";
            }
            s += "}";
            return s;
        }

        public Complex this[int key]
        {
            get => _Vector[key];
            set => _Vector[key] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ComplexEnum GetEnumerator()
        {
            return new ComplexEnum(_Vector.ToArray());
        }
    }

    internal class VectorEnum : IEnumerator
    {
        public Vector[] _Vector;
        int position = -1;

        public VectorEnum(Vector[] arr)
        {
            _Vector = arr;
        }

        public bool MoveNext()
        {
            position++;
            return position < _Vector.Length;
        }
        public void Reset()
        {
            position = -1;
        }
        object IEnumerator.Current
        {
            get => Current;
        }
        public Vector Current
        {
            get
            {
                try
                {
                    return _Vector[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
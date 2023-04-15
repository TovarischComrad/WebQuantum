using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuantumCore.Math
{
    internal class Matrix : IEnumerable
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        protected List<Vector> _Matrix;

        // Конструкторы
        public Matrix()
        {
            Width = 0;
            Height = 0;
            _Matrix = new List<Vector>();
        }
        public Matrix(int n)
        {
            Width = n;
            Height = n;
            _Matrix = new List<Vector>();
            for (int i = 0; i < Height; i++)
            {
                _Matrix.Add(new Vector(Width));
            }
            Complex z = new Complex(1);
            for (int i = 0; i < n; i++)
            {
                _Matrix[i][i] = z;
            }
        }
        public Matrix(int height, int width, bool random = false)
        {
            Width = width;
            Height = height;
            _Matrix = new List<Vector>();
            for (int i = 0; i < Height; i++)
            {
                if (random)
                {
                    _Matrix.Add(new Vector(Width, true));
                }
                else
                {
                    _Matrix.Add(new Vector(Width));
                }
            }
        }
        public Matrix(int height, int width, Complex z)
        {
            Width = width;
            Height = height;
            _Matrix = new List<Vector>();
            for (int i = 0; i < Height; i++)
            {
                _Matrix.Add(new Vector(Width, z));
            }
        }
        public Matrix(Complex[][] arr)
        {
            Height = arr.Length;
            Width = arr[0].Length;
            _Matrix = new List<Vector>();
            for (var i = 0; i < Height; i++)
            {
                _Matrix.Add(new Vector(arr[i]));
            }
        }

        // Вставка и удаление
        public void Add(Vector vec)
        {
            try
            {
                if (Width > 0 && vec.Size != Width) { throw new Exception(); }
                Height++;
                Width = vec.Size;
                _Matrix.Add(vec);
            }
            catch (Exception)
            {
                throw new Exception("Incorrect vector size.");
            }
        }
        public void Delete()
        {
            _Matrix.RemoveAt(Height - 1);
            Height = System.Math.Max(Height - 1, 0);
        }
        public void AddColumn(Vector vec)
        {
            try
            {
                if (Height > 0 && vec.Size != Height) { throw new Exception(); }
                Width++;
                Height = vec.Size;
                for (int i = 0; i < vec.Size; i++)
                {
                    _Matrix[i].Add(vec[i]);
                }
            }
            catch (Exception)
            {
                throw new Exception("Incorrect vector size.");
            }
        }
        public void DeleteColumn()
        {
            for (int i = 0; i < Height; i++)
            {
                _Matrix[i].Delete();
            }
            Width = System.Math.Max(Width - 1, 0);
        }

        // Операции над векторами
        public Matrix Plus(Matrix m)
        {
            try
            {
                Matrix res = new Matrix(Height, Width);
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        res[i][j] = this[i][j] + m[i][j];
                    }
                }
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }
        public Matrix Minus(Matrix m)
        {
            try
            {
                Matrix res = new Matrix(Height, Width);
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        res[i][j] = this[i][j] - m[i][j];
                    }
                }
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }
        public Matrix ScalarProduct(Complex z)
        {
            Matrix res = new Matrix(Height, Width);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    res[i][j] = this[i][j] * z;
                }
            }
            return res;
        }
        public Matrix Product(Matrix m)
        {
            try
            {
                Matrix res = new Matrix(Height, m.Width);
                Parallel.For(0, Height, i =>
                {
                    Parallel.For(0, m.Width, j =>
                    {
                        Complex sum = new Complex();
                        for (int k = 0; k < Width; k++)
                        {
                            sum = sum + this[i][k] * m[k][j];
                        }
                        res[i][j] = sum;
                    });
                });
                return res;
            }
            catch (Exception)
            {
                return this;
            }
        }
        public Matrix TensorProduct(Matrix m)
        {
            Matrix res = new Matrix(Height * m.Height, Width * m.Width);
            Parallel.For(0, Height, i =>
            {
                Parallel.For(0, Width, j =>
                {
                    Parallel.For(0, m.Height, k =>
                    {
                        Parallel.For(0, m.Width, l =>
                        {
                            res[m.Height * i + k][m.Width * j + l] = this[i][j] * m[k][l];
                        });
                    });
                });
            });
            return res;
        }

        // Технические методы
        public override string ToString()
        {
            string s = "{";
            for (int i = 0; i < Height; i++)
            {
                s = i > 0 ? s + " " : s;
                s += _Matrix[i].ToString();
                s = i == Height - 1 ? s : s + "\n";
            }
            s += "}";
            return s;
        }

        public Vector this[int key]
        {
            get => _Matrix[key];
            set => _Matrix[key] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public VectorEnum GetEnumerator()
        {
            return new VectorEnum(_Matrix.ToArray());
        }
    }
}
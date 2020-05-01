using System;
using System.Collections.Generic;

namespace TestFormsApp
{
    public class ElectricalField
    {
        public ElectricalField(int widht, int height)
        {
            _field = new double[widht, height];
            _rgb_field = new int[widht, height];
            _charges = new List<int[]>();
            _dicon = 1D / (4 * pi * eps);
            _rgba_cache = new byte[] {0, 0, 0, 255};
            _random = new Random();
        }

        private const double distanceScale = 50E-7;
        private const double e = 1.6022E-19;
        private const double pi = Math.PI;
        private const double eps = 8.8541E-12;
        private double _dicon;
        private double[,] _field;
        private int[,] _rgb_field;
        private List<int[]> _charges;
        private byte[] _rgba_cache;
        private Random _random;
        private double _maxValue = 0;

        public void changeCharge(int index, int charge)
        {
            _charges[index][2] = charge;
        }

        public void placeElectricalCharge(int xPos, int yPos, int amountOfE)
        {
            _charges.Add(new[] {xPos, yPos, amountOfE});
        }

        public void changePosition(int index, int x, int y)
        {
            _charges[index][0] = x;
            _charges[index][1] = y;
        }

        public double[,] generateElectricPotentialField()
        {
            for (int x = 0; x < _field.GetLength(0); x++)
            {
                for (int y = 0; y < _field.GetLength(1); y++)
                {
                    double c0 = 0;
                    foreach (int[] charge in _charges)
                    {
                        c0 += getField(charge, x, y);
                    }

                    _field[x, y] = c0;
                }
            }

            return _field;
        }

        private double getField(int[] charge, int xPos, int yPos)
        {
            double cx = (charge[0] - xPos) * distanceScale;
            double cy = (charge[1] - yPos) * distanceScale;
            double Q = charge[2] * e;
            double rsq = (cx * cx) + (cy * cy);
            double append = Q / rsq;
            double value = _dicon * append;
            return value;
        }

        public int[,] getElectricalFieldAsRGB()
        {
            generateElectricPotentialField();
            for (int x = 0; x < _field.GetLength(0); x++)
            {
                for (int y = 0; y < _field.GetLength(1); y++)
                {
                    double c0 = _field[x, y];
                    double c0A = c0 / 50;

                    if (c0 < 0)
                    {
                        if (c0 < -1)
                        {
                            c0 = -1;
                        }
                        
                        if (c0A < -1)
                        {
                            c0A = -1;
                        }

                        _rgba_cache[0] = (byte) (255 * -c0);
                        _rgba_cache[3] = (byte) (255 - (255.0 * -c0A));
                    }
                    else
                    {
                        if (c0 > 1)
                        {
                            c0 = 1;
                        }
                        
                        if (c0A > 1)
                        {
                            c0A = 1;
                        }

                        _rgba_cache[2] = (byte) (255.0 * c0);
                        _rgba_cache[3] = (byte) (255.0 - (255.0 * c0A));
                    }

                    _rgb_field[x, y] = BitConverter.ToInt32(_rgba_cache, 0);
                    _rgba_cache[2] = 0;
                    _rgba_cache[0] = 0;
                }
            }

            return _rgb_field;
        }
    }
}
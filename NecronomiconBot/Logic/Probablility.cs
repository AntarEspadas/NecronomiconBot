using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NecronomiconBot.Logic
{
    [TypeConverter(typeof(TypeConverterProbablillityConverter))]
    class Probablility
    {
        public float Percentage { get { return percentage; } set => SetPercentage(value); }
        private float percentage;
        private static Random random = new Random();

        public Probablility(float percentage)
        {
            Percentage = percentage;
        }
        private void SetPercentage(float value)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException();
            }
            percentage = value;
        }
        public bool Roll()
        {
            return random.NextDouble() < (Percentage / 100);
        }
    }
}

using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace Calculator_OOP
{
    class MemoryFeature
    {
        Queue<double> q;

        public MemoryFeature()
        {
            q = new Queue<double>();
        }
        public void Record(double _in)
        {
            q.Enqueue(_in);
        }
        public double Copy()
        {
            double ret;
            ret = q.Dequeue();
            q.Enqueue(ret);

            return ret;
        }
    }
}
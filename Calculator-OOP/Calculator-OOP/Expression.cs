using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace Calculator_OOP
{
    interface Expression
    {
        public double solve();
    }

    abstract class BinaryExpression: Expression
    {
        protected Expression x;
        protected Expression y;

        public BinaryExpression(Expression x_, Expression y_)
        {
            x = x_;
            y = y_;
        }

        abstract public double solve();
    }

    abstract class UnaryExpression: Expression
    {
        protected Expression x;

        public UnaryExpression(Expression x_)
        {
            x = x_;
        }
        abstract public double solve();
    }

    class TerminalExpression: Expression
    {
        protected double x;

        public TerminalExpression(double x_)
        {
            x = x_;
        }
        public double solve()
        {
            return x;
        }
    }
    
    //Binary Expression
    class AddExpression: BinaryExpression
    {
        public AddExpression(Expression x_, Expression y_): base(x_,y_)
        {
        }
        public override double solve()
        {
            return x.solve() + y.solve();
        }
    }

    class SubstractExpression: BinaryExpression
    {
        public SubstractExpression(Expression x_, Expression y_): base(x_, y_)
        {
        }
        public override double solve()
        {
            return x.solve() - y.solve();
        }
    }

    class MultiplyExpression: BinaryExpression
    {
        public MultiplyExpression(Expression x_, Expression y_): base(x_,y_)
        {
        }
        public override double solve()
        {
            return x.solve() * y.solve();
        }
    }

    class DivisionExpression: BinaryExpression
    {
        public DivisionExpression(Expression x_, Expression y_): base(x_, y_)
        {
        }
        public override double solve()
        {
            if(y.solve() == 0)
            {
                if(x.solve() == 0)
                {
                    throw new Exception("0/0 division is undefined");
                }
                throw new Exception("Division by zero is not allowed");
            }
            return x.solve() / y.solve();
        }
    }

    //Unary Expression
    class NegativeExpression: UnaryExpression
    {
        public NegativeExpression(Expression x_): base(x_)
        {
        }
        public override double solve()
        {
            return -1 * x.solve();
        }
    }

    class RootExpression: UnaryExpression
    {
        public RootExpression(Expression x_): base(x_)
        {
        }
        public override double solve()
        {
            if(x.solve() < 0)
            {
                throw new Exception("Square root of a negative number is not allowed");
            } 
            return Math.Sqrt(x.solve());
        }
    }
    
    public class MainProgram
    {
        static List<TokenCategory> li;
        public static void Main(string[] args)
        {
            try
            {
                li = new List<TokenCategory>();
                var input = Console.ReadLine();
                using (var read = new StringReader(input))
                {
                    Parser p = new Parser();
                    double result = p.parse(read);
                    Console.WriteLine(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
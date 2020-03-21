using System;
using System.Text;
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
            return Math.Sqrt(x.solve());
        }
    }

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
    enum TokenCategory
    {
        Number,
        Operator,
        Dot
    }
    class Parser
    {
        Queue<Expression> q;
        Queue<char> operatorsq;
        HashSet<char> listOfOperators = new HashSet<char>()
        {
            '+',
            '-',
            '*',
            '/',
            'r'
        };
        public Parser()
        {
            q = new Queue<Expression>();
            operatorsq = new Queue<char>();
        }
        public TokenCategory type(char c)
        {
            if(char.IsDigit(c))
            {
                return TokenCategory.Number;
            }
            else if(c == '.')
            {
                return TokenCategory.Dot;
            }
            else if(listOfOperators.Contains(c))
            {
                return TokenCategory.Operator;
            }
            else
            {
                throw new Exception("Character\'s not available");
            }
        }
        public double parse(TextReader text)
        {
            bool prevNumber = false, prevOperator = false, prevDot = false;
            int current, dotCounter = 0, unaryCounter = 0;
            char op;
            StringBuilder token = new StringBuilder();

            while((current = text.Read()) != -1)
            {
                char c = (char)current;
                TokenCategory categ = type(c);

                if(categ == TokenCategory.Number)
                {
                    token.Append(c);
                    int next = text.Peek();
                    TokenCategory nextType = type((char)next);
                    if(nextType == TokenCategory.Operator)
                    {
                        q.Enqueue(new TerminalExpression(Convert.ToDouble(token.ToString())));
                        dotCounter = 0;
                        unaryCounter = 0;
                        token.Clear();
                    }
                    else if(next == -1)
                    {
                        q.Enqueue(new TerminalExpression(Convert.ToDouble(token.ToString())));
                        dotCounter = 0;
                        unaryCounter = 0;
                        token.Clear();
                    }
                    prevDot = false;
                    prevOperator = false;
                    prevNumber = true;
                }
                else if(categ == TokenCategory.Operator)
                {
                    if(prevNumber)
                    {
                        operatorsq.Enqueue(c);
                        if(q.Count >= 2)
                        {
                            Expression exp1 = q.Dequeue();
                            Expression exp2 = q.Dequeue();
                            op = operatorsq.Dequeue();

                            if(op == '+')
                            {
                                q.Enqueue(new AddExpression(exp1, exp2));
                            }
                            else if(op == '-')
                            {
                                q.Enqueue(new SubstractExpression(exp1, exp2));
                            }
                            else if(op == '*')
                            {
                                q.Enqueue(new MultiplyExpression(exp1, exp2));
                            }
                            else if(op == '/')
                            {
                                q.Enqueue(new DivisionExpression(exp1, exp2));
                            }
                        }
                    }
                    else
                    {
                        if(prevOperator && unaryCounter == 0 && c == '-')
                        {
                            token.Append(c);
                            unaryCounter++;
                        }
                        else if(prevDot)
                        {
                            throw new Exception("Illegal use of operator after decimal separator");
                        }
                        else if(unaryCounter == 1)
                        {
                            throw new Exception("Too much unary operator");
                        }
                        else if(prevOperator && c != '-')
                        {
                            throw new Exception("Missing operand");
                        }
                    }
                    prevDot = false;
                    prevNumber = false;
                    prevOperator = true;
                }
                else // Dot
                {
                    if(dotCounter == 0 && prevNumber)
                    {
                        token.Append(c);
                        dotCounter++;
                        prevNumber = false;
                        prevDot = true;
                    } 
                    else
                    {
                        throw new Exception("Illegal use of dot as decimal separator");
                    }
                }
            }
            if(q.Count == 2 && operatorsq.Count == 1)
            {
                Expression ret;
                Expression ex1 = q.Dequeue();
                Expression ex2 = q.Dequeue();
                op = operatorsq.Dequeue();

                if(op == '+')
                {
                   ret = new AddExpression(ex1, ex2);
                }
                else if(op == '-')
                {
                    ret = new SubstractExpression(ex1, ex2);
                }
                else if(op == '*')
                {
                   ret = new MultiplyExpression(ex1, ex2);
                }
                else //op == '/'
                {
                    ret = new DivisionExpression(ex1, ex2);
                }
                return ret.solve();
            } 
            else
            {
                throw new Exception("Illegal expression");
            }
        }
    }
    public class MainProgram
    {
        public static void Main(string[] args)
        {
            try
            {
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
            //int current;
            //var input = Console.ReadLine();
            //using (var read = new StringReader(input))
            //{
            //    while((current = read.Read()) != -1)
            //    {
            //        if(char.IsDigit((char)current))
            //        {
            //            Console.WriteLine("Ini digit");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Ini bukan digit");
            //        }
            //    }
            //}
        }
    }
}

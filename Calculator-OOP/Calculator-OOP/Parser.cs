using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace Calculator_OOP
{
    enum TokenCategory
    {
        Number,
        Operator,
        Dot,
        Ans,
        End
    }
    class Parser
    {
        static Queue<Expression> q;
        static Queue<char> operatorsq;
        static HashSet<char> listOfOperators = new HashSet<char>()
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
            if (char.IsDigit(c))
            {
                return TokenCategory.Number;
            }
            else if (c == '.')
            {
                return TokenCategory.Dot;
            }
            else if (listOfOperators.Contains(c))
            {
                return TokenCategory.Operator;
            }
            else if (c == 'A')
            {
                return TokenCategory.Ans;
            }
            else
            {
                throw new Exception("Character\'s not available");
            }
        }
        public double parse(TextReader text)
        {
            bool prevNumber = false, prevOperator = false, prevDot = false, ans_popped = false;
            int current, dotCounter = 0, unaryMinCounter = 0, unaryRootCounter = 0;
            char op;
            StringBuilder token = new StringBuilder();

            while ((current = text.Read()) != -1)
            {
                char c = (char)current;
                TokenCategory categ = type(c);
                int next = text.Peek();
                TokenCategory nextType;
                if (next != -1)
                {
                    nextType = type((char)next);
                }
                else
                {
                    nextType = TokenCategory.End;
                }
                if (categ == TokenCategory.Ans)
                {
                    if (!ans_popped)
                    {
                        //q.Enqueue(Ans);
                        ans_popped = true;
                    }
                    continue;
                }
                else if (categ == TokenCategory.Number)
                {
                    token.Append(c);
                    Expression tmp;
                    if (nextType == TokenCategory.Operator || nextType == TokenCategory.End)
                    {
                        Expression temp = new TerminalExpression(Convert.ToDouble(token.ToString(), CultureInfo.InvariantCulture));
                        if (unaryRootCounter > 0)
                        {
                            List<Expression> tmpList = new List<Expression>();

                            for (int i = 0; i < unaryRootCounter; i++)
                            {
                                if (i == 0)
                                {
                                    tmpList.Add(temp);
                                }
                                else
                                {
                                    tmpList.Add(new RootExpression(tmpList[i - 1]));
                                }
                            }
                            temp = new RootExpression(tmpList[unaryRootCounter - 1]);
                            if (unaryMinCounter == 1)
                            {
                                tmp = new NegativeExpression(temp);
                            }
                            else
                            {
                                tmp = temp;
                            }
                        }
                        else if (unaryMinCounter == 1)
                        {
                            tmp = new NegativeExpression(temp);
                        }
                        else
                        {
                            tmp = temp;
                        }
                        q.Enqueue(tmp);
                        dotCounter = 0;
                        unaryMinCounter = 0;
                        unaryRootCounter = 0;
                        token.Clear();
                    }
                    prevDot = false;
                    prevOperator = false;
                    prevNumber = true;
                }
                else if (categ == TokenCategory.Operator)
                {
                    ans_popped = false;
                    if (prevNumber)
                    {
                        operatorsq.Enqueue(c);
                        if (q.Count >= 2)
                        {
                            Expression exp1 = q.Dequeue();
                            Expression exp2 = q.Dequeue();
                            op = operatorsq.Dequeue();

                            if (op == '+')
                            {
                                q.Enqueue(new AddExpression(exp1, exp2));
                            }
                            else if (op == '-')
                            {
                                q.Enqueue(new SubstractExpression(exp1, exp2));
                            }
                            else if (op == '*')
                            {
                                q.Enqueue(new MultiplyExpression(exp1, exp2));
                            }
                            else if (op == '/')
                            {
                                q.Enqueue(new DivisionExpression(exp1, exp2));
                            }
                        }
                    }
                    else
                    {
                        if (!prevDot && !prevOperator)
                        {
                            if (c == 'r')
                            {
                                unaryRootCounter++;
                            }
                            else if (c == '-')
                            {
                                unaryMinCounter++;
                            }
                            else
                            {
                                throw new Exception("Wrong operator at the start of expression");
                            }
                        }
                        else if (prevOperator && unaryMinCounter == 0 && (c == '-' || c == 'r'))
                        {
                            if (c == '-')
                            {
                                unaryMinCounter++;
                            }
                            else
                            {
                                unaryRootCounter++;
                            }
                        }
                        else if (prevDot)
                        {
                            throw new Exception("Illegal use of operator after decimal separator");
                        }
                        else if (unaryMinCounter == 1)
                        {
                            throw new Exception("Too much negative unary operator");
                        }
                        else if (prevOperator && (c != '-' && c != 'r'))
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
                    if (dotCounter == 0 && prevNumber)
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
            if (q.Count == 2 && operatorsq.Count == 1)
            {
                Expression ret;
                Expression ex1 = q.Dequeue();
                Expression ex2 = q.Dequeue();
                op = operatorsq.Dequeue();

                if (op == '+')
                {
                    ret = new AddExpression(ex1, ex2);
                }
                else if (op == '-')
                {
                    ret = new SubstractExpression(ex1, ex2);
                }
                else if (op == '*')
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
                if (q.Count == 1 && operatorsq.Count == 0)
                {
                    return q.Dequeue().solve();
                }
                else
                {
                    throw new Exception("Illegal expression");
                }
            }
        }
    }
}
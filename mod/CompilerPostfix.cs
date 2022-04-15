using System;
using System.Collections.Generic;

public static class CompilerPostfix
{
    private static int DoOperation(string signOperator, int operand1, int operand2)
    {
        int result = 0;

        switch (signOperator)
        {
            case "+":
                result = operand1 + operand2;
                break;
            case "-":
                result = operand1 - operand2;
                break;
            case "*":
                result = operand1 * operand2;
                break;
            case "/":
                result = operand1 / operand2;
                break;
            case "%":
                result = operand1 % operand2;
                break;
        }

        return result;
    }

    private static string[] SplitExpression(string sourceExpression)
    {
        List<string> result = new List<string>();

        string[] arrayOperation = new string[] {"(", ")", "+", "-", "*", "/", "%"};
        char[] whitespace = new char[] {' ', '\t', '\v', '\n', '\f'};

        string expression = "";

        for (int index = 0; index < sourceExpression.Length; index++)
        {
            if (!(Array.Exists(whitespace, element => element == sourceExpression[index])))
            {
                expression += sourceExpression[index];
            }
        }

        string tempExpression;

        int position = 0;
        int lastPositionElement = 0;

        while (position < expression.Length)
        {
            for (int indexOperation = 0; indexOperation < arrayOperation.Length; indexOperation++)
            {
                tempExpression = expression.Substring(position, arrayOperation[indexOperation].Length);

                if (tempExpression.Equals(arrayOperation[indexOperation]))
                {
                    if (position != lastPositionElement)
                    {
                        result.Add(expression.Substring(lastPositionElement, position - lastPositionElement));
                    }

                    result.Add(tempExpression);

                    position += arrayOperation[indexOperation].Length;
                    lastPositionElement = position;
                    position--;
                    break;
                }
            }

            position++;
        }

        if (position != lastPositionElement)
        {
            result.Add(expression.Substring(lastPositionElement, position - lastPositionElement));
        }

        return result.ToArray();
    }

    public static int CompileExpression(string expression)
    {
        string[] splitedExpression = SplitExpression(expression);

        if (splitedExpression.Length == 0)
        {
            return 0;
        }

        Stack<int> stackE = new Stack<int>();
        Stack<string> stackT = new Stack<string>();

        string signOperator;
        int operand1;
        int operand2;

        int index = 0;

        while (index < splitedExpression.Length)
        {
            switch (splitedExpression[index])
            {
                case "(":
                    stackT.Push(splitedExpression[index]);
                    index++;
                    break;
                case ")":
                    if (stackT.Count == 0)
                    {
                        throw new ArithmeticException("Error with brackets \")\".");
                    }
                    else if (stackT.Peek() == "(")
                    {
                        stackT.Pop();
                        index++;
                    }
                    else
                    {
                        signOperator = stackT.Pop();
                        operand2 = stackE.Pop();
                        operand1 = stackE.Pop();

                        stackE.Push(DoOperation(signOperator, operand1, operand2));
                    }
                    break;
                case "+": case "-":
                    if (stackT.Count == 0 || stackT.Peek() == "(")
                    {
                        stackT.Push(splitedExpression[index]);
                        index++;
                    }
                    else if (stackT.Peek() == "+" || stackT.Peek() == "-")
                    {
                        signOperator = stackT.Pop();
                        operand2 = stackE.Pop();
                        operand1 = stackE.Pop();

                        stackE.Push(DoOperation(signOperator, operand1, operand2));
                        stackT.Push(splitedExpression[index]);

                        index++;
                    }
                    else if (stackT.Peek() == "*" || stackT.Peek() == "/")
                    {
                        signOperator = stackT.Pop();
                        operand2 = stackE.Pop();
                        operand1 = stackE.Pop();

                        stackE.Push(DoOperation(signOperator, operand1, operand2));
                    }
                    break;
                case "*": case "/": case "%":
                    if (stackT.Count == 0 || stackT.Peek() == "(" || stackT.Peek() == "+" || stackT.Peek() == "-")
                    {
                        stackT.Push(splitedExpression[index]);
                        index++;
                    }
                    else if (stackT.Peek() == "*" || stackT.Peek() == "/")
                    {
                        signOperator = stackT.Pop();
                        operand2 = stackE.Pop();
                        operand1 = stackE.Pop();

                        stackE.Push(DoOperation(signOperator, operand1, operand2));
                        stackT.Push(splitedExpression[index]);

                        index++;
                    }
                    break;
                default:
                    stackE.Push(Int32.Parse(splitedExpression[index]));
                    index++;
                    break;
            }
        }

        while (stackT.Count != 0)
        {
            if (stackT.Peek() == "(")
            {
                throw new ArithmeticException("Error with brackets \"(\".");
            }

            signOperator = stackT.Pop();
            operand2 = stackE.Pop();
            operand1 = stackE.Pop();
            
            stackE.Push(DoOperation(signOperator, operand1, operand2));
        }

        return stackE.Pop();
    }
}

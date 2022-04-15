using System;
class Program
{
    static void Main(string[] args)
    {
        string expression = "";

        if (args.Length > 0)
        {
            expression += args[0];
        }

        for (int index = 1; index < args.Length; index++)
        {
            expression += " " + args[index];
        }

        Console.Write("Source expression: ");
        Console.WriteLine(expression);
        Console.Write("Result expression: ");
        Console.WriteLine(CompilerPostfix.CompileExpression(expression));
    }
}

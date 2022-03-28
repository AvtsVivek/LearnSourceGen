using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Types in this assembly:");
            foreach (Type t in typeof(Program).Assembly.GetTypes())
            {
                Console.WriteLine(t.FullName);
            }
        }
    }

    [Log]
    public class HereWeGo : ISomeInterface
    { }

    public interface ISomeInterface { }

    //public class LogAttribute : Attribute { }
}

namespace ConsoleAppWithDecorators;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello!!");
    }
}

[Log]
public class ClassWithAnAttribute { }

public class ClassImplimentingInterface : ISomeInterface
{ }

public class LogAttribute : Attribute { }

public interface ISomeInterface { }

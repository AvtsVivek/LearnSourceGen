namespace SourceGenTrial;

partial class Program
{
    static void Main(string[] args)
    {
        HelloFrom("Generated Code");
        HelloWorldGenerated.HelloWorld.SayHello();
    }

    static partial void HelloFrom(string name);
}
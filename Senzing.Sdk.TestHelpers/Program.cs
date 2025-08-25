using System.Text;

using Senzing.Sdk.TestHelpers;

if (args.Length < 1)
{
    Console.Error.WriteLine("A test class name must be specified");
    Environment.Exit(1);
}

string typeName = "Senzing.Sdk.TestHelpers." + args[0];

Type? type = Type.GetType(typeName);

if (type == null)
{
    Console.Error.WriteLine("Type not found: " + typeName);
    Environment.Exit(1);
    throw new ArgumentException("Bad type name argument: " + args[0]);
}

if (!(type.IsAssignableTo(typeof(TestHelper))))
{
    Console.Error.WriteLine("Type does not extend TestHelper: " + typeName);
    Environment.Exit(1);
}

TestHelper? testHelper = (TestHelper?)Activator.CreateInstance(type);

if (testHelper == null)
{
    Console.Error.WriteLine("Failed to create instance of helper: " + type);
    Environment.Exit(1);
}

try
{
    testHelper.Execute(args[1..]);
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
    Console.Error.WriteLine(e.StackTrace);
    Environment.Exit(1);
}

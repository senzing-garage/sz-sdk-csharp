using System.Reflection;
using System.Text;

using Senzing.Sdk.TestHelpers;

using static System.StringComparison;

if (args.Length < 1)
{
    Console.Error.WriteLine("A log file path must be specified");
    Environment.Exit(1);
}
logFilePath = args[0];
log("GOT HERE 1");
if (args.Length < 2)
{
    log("A test class name must be specified");
    Environment.Exit(1);
}

string typeName = "Senzing.Sdk.TestHelpers." + args[1];

Type? type = Type.GetType(typeName);
log("GOT HERE 2");

if (type == null)
{
    log("Type not found: " + typeName);
    Environment.Exit(1);
    throw new ArgumentException("Bad type name argument: " + args[0]);
}

log("GOT HERE 3");
if (!(type.IsAssignableTo(typeof(TestHelper))))
{
    log("Type does not extend TestHelper: " + typeName);
    Environment.Exit(1);
}

log("GOT HERE 4");
TestHelper? testHelper = (TestHelper?)Activator.CreateInstance(type);

if (testHelper == null)
{
    log("Failed to create instance of helper: " + type);
    Environment.Exit(1);
}

log("GOT HERE 5");
try
{
    testHelper.Execute(args[2..], (msg) => log(msg));
    log("GOT HERE 6");
}
catch (Exception e)
{
    log(e);
    log(e.StackTrace);
    Environment.Exit(1);
}

public partial class Program
{
    private static string? logFilePath = null;

    public static void log(object? msg)
    {
        Console.Error.WriteLine(msg);
        Console.Error.Flush();

        if (logFilePath == null)
        {
            return;
        }

        using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(msg);
            sw.Flush();
            sw.Close();
        }
    }
}

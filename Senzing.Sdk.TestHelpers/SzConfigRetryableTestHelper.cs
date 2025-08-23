namespace Senzing.Sdk.TestHelpers;

using System.Text;
using System.Text.Unicode;

using Senzing.Sdk;
using Senzing.Sdk.Core;

using static Senzing.Sdk.Tests.Core.SzConfigRetryableTest;

/// <summary>
/// Helper for the <c>SzConfigRetryableTest</c>.
/// </summary>
public class SzConfigRetryableTestHelper : TestHelper
{
    private static readonly Encoding UTF8 = new UTF8Encoding();

    public void Execute(string[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Must specify the following command-line arguments:");
                Console.Error.WriteLine("  1: Path to setting JSON file for the repository");
                Console.Error.WriteLine("  2: Path to the output file for the results");
                Environment.Exit(1);
            }

            string initFilePath = args[0];
            string outputFilePath = args[1];
            FileInfo initFile = new FileInfo(initFilePath);
            FileInfo outputFile = new FileInfo(outputFilePath);

            if (!initFile.Exists)
            {
                Console.Error.WriteLine("Settings file does not exist: " + initFilePath);
                Environment.Exit(1);
            }

            string initJson = File.ReadAllText(initFile.FullName, UTF8).Trim();

            SzEnvironment env
                = SzCoreEnvironment.NewBuilder()
                                   .Settings(initJson)
                                   .InstanceName("SzConfigRetryableTestHelper")
                                   .Build();

            try
            {
                SzConfigManager configMgr = env.GetConfigManager();

                SzConfig config = configMgr.CreateConfig(configMgr.GetDefaultConfigID());
                config.RegisterDataSource(Customers);
                config.RegisterDataSource(Passengers);
                long configID = configMgr.SetDefaultConfig(config.Export());
                env.Reinitialize(configID);

                SzEngine engine = env.GetEngine();

                engine.AddRecord(Customers, CustomerABC123.recordID, RecordABC123);
                engine.AddRecord(Customers, CustomerDEF456.recordID, RecordDEF456);

                string network = engine.FindNetwork(
                    new SortedSet<(string, string)> { CustomerABC123, CustomerDEF456 },
                    2, 0, 0, SzFlags.SzFindNetworkAllFlags);

                FileStream fs = new FileStream(outputFile.FullName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, UTF8);
                sw.WriteLine(network);
                sw.Flush();
            }
            finally
            {
                env.Destroy();
            }

        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            Console.Error.WriteLine(e.StackTrace);
            Environment.Exit(1);
        }
    }
}
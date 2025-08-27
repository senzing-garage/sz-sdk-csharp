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

    public void Execute(string[] args, Action<object?> logger)
    {
        try
        {
            if (args.Length < 2)
            {
                logger("Must specify the following command-line arguments:");
                logger("  1: Path to setting JSON file for the repository");
                logger("  2: Path to the output file for the results");
                Environment.Exit(1);
            }

            string initFilePath = args[0];
            string outputFilePath = args[1];
            FileInfo initFile = new FileInfo(initFilePath);
            FileInfo outputFile = new FileInfo(outputFilePath);

            if (!initFile.Exists)
            {
                logger("Settings file does not exist: " + initFilePath);
                Environment.Exit(1);
            }
            logger("Init file exists");

            string initJson = File.ReadAllText(initFile.FullName, UTF8).Trim();

            logger("Initializing Senzing");
            SzEnvironment env
                = SzCoreEnvironment.NewBuilder()
                                   .Settings(initJson)
                                   .InstanceName("SzConfigRetryableTestHelper")
                                   .Build();

            logger("Initialized Senzing");
            try
            {
                SzConfigManager configMgr = env.GetConfigManager();

                SzConfig config = configMgr.CreateConfig(configMgr.GetDefaultConfigID());
                config.RegisterDataSource(Customers);
                config.RegisterDataSource(Passengers);
                logger("Setting default config...");
                long configID = configMgr.SetDefaultConfig(config.Export());
                logger("Setting default config: " + configID);
                env.Reinitialize(configID);
                logger("Reinitialized");


                SzEngine engine = env.GetEngine();

                logger("Adding records...");
                engine.AddRecord(Customers, CustomerABC123.recordID, RecordABC123);
                engine.AddRecord(Customers, CustomerDEF456.recordID, RecordDEF456);
                logger("Added records.");

                logger("Finding network...");
                string network = engine.FindNetwork(
                    new SortedSet<(string, string)> { CustomerABC123, CustomerDEF456 },
                    2, 0, 0, SzFlags.SzFindNetworkAllFlags);

                logger("Found network.");
                logger("Opening output file: " + outputFile.FullName);
                using (FileStream fs = new FileStream(outputFile.FullName, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs, UTF8);
                    logger("Writing network to output file: " + outputFile.FullName);
                    sw.WriteLine(network);
                    sw.Flush();
                    logger("Wrote network to output file: " + outputFile.FullName);
                }
                logger("Closed output file: " + outputFile.FullName);
            }
            finally
            {
                logger("Destroying SzEnvironment...");
                env.Destroy();
                logger("Destroyed SzEnvironment.");
            }

        }
        catch (Exception e)
        {
            logger(e);
            logger(e.StackTrace);
            Environment.Exit(1);
        }
    }
}
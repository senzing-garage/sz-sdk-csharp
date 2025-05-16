namespace Senzing.Sdk.Demo;

using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;

using NUnit.Framework;

using static System.StringComparison;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
internal class SzExampleExtractor
{
    protected static void LogError(string message, Exception e)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("**********************************");
        Console.Error.WriteLine("FAILURE: " + message);
        Console.Error.WriteLine(e);
        Console.Error.WriteLine();
        throw e;
    }

    public static List<object?[]> GetExtractParameters()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        DirectoryInfo dirInfo = new DirectoryInfo(currentDirectory);

        // find the bin directory
        DirectoryInfo? binDir = dirInfo.Parent;
        while (binDir != null && !"bin".Equals(binDir?.Name, Ordinal))
        {
            binDir = binDir?.Parent;
        }
        DirectoryInfo? moduleDir = binDir?.Parent;
        DirectoryInfo? repoDir = moduleDir?.Parent;
        if (moduleDir != null && repoDir != null)
        {
            Assert.That(moduleDir.Name, Is.EqualTo("Senzing.Sdk.Demo"),
                        "Bad module directory.  Current directory: "
                        + currentDirectory);
            Assert.That(repoDir.Name, Is.EqualTo("sz-sdk-csharp"),
                        "Bad repository directory name.  Current directory: "
                        + currentDirectory);

            string demoPath = Path.Combine(moduleDir.FullName, "demo");
            DirectoryInfo demoDir = new DirectoryInfo(demoPath);

            string targetPath = Path.Combine(repoDir.FullName, "target", "examples");
            DirectoryInfo targetDir = new DirectoryInfo(targetPath);

            FileInfo[] filesArray = demoDir.GetFiles("*Demo.cs");
            List<object?[]> result = new List<object?[]>(filesArray.Length);
            foreach (FileInfo fileInfo in filesArray)
            {
                result.Add(new object?[] { fileInfo, targetDir });
            }
            return result;
        }
        Assert.That(moduleDir, Is.Not.Null,
            "Failed to get module directory.  Current directory: "
            + currentDirectory);
        Assert.That(repoDir, Is.Not.Null,
            "Failed to get the repository directory.  Current directory: "
            + currentDirectory);
        throw new Exception(
            "Failed to get directories.  Current directory: " + currentDirectory);
    }

    [Test, TestCaseSource(nameof(GetExtractParameters))]
    public void ExtractExamples(FileInfo fileInfo, DirectoryInfo targetDir)
    {
        // ensure the target directory exists
        targetDir.Create();

        // get the snippet prefix
        string prefix = fileInfo.Name.Substring(0, fileInfo.Name.Length - 3) + "_";

        // read the input file
        FileStream fs = fileInfo.OpenRead();
        StreamReader sr = new StreamReader(fs, Encoding.UTF8);

        StreamWriter? sw = null;
        bool omitting = false;
        int lineNumber = 0;
        string? snippet = null;
        for (string? line = sr.ReadLine(); line != null; line = sr.ReadLine())
        {
            lineNumber++;
            int index = 0;
            // check if we are in-between snippets
            if (sw == null)
            {
                // check for snippet start
                index = line.IndexOf("// @start ");
                if (index >= 0)
                {
                    snippet = line.Substring(index + "// @start".Length);
                    snippet = snippet.Trim();
                    string snippetPath = Path.Combine(
                        targetDir.FullName, prefix + snippet + ".xml");
                    FileStream fs2 = new FileStream(snippetPath, FileMode.Create, FileAccess.Write);
                    sw = new StreamWriter(fs2, Encoding.UTF8);
                    sw.WriteLine("<code><![CDATA[");
                }
                continue;
            }

            // check if starting a new block without ending the previous
            index = line.IndexOf("// @start ");
            if (index >= 0)
            {
                Assert.Fail(fileInfo.FullName + ":" + lineNumber
                            + " - Found @start directive when previous @end for "
                            + snippet + " not found: " + line.Substring(index));
            }

            // check for the end of an omission block
            if (omitting)
            {
                index = line.IndexOf("// @omit-end");
                if (index >= 0)
                {
                    omitting = false;
                }

                index = line.IndexOf("// @");
                if (index >= 0)
                {
                    Assert.Fail(fileInfo.FullName + ":" + lineNumber
                                + " - Found directive within omission block: "
                                + line.Substring(index));
                }
                continue;
            }

            // check for the start of omission block
            index = line.IndexOf("// @omit-start");
            if (index >= 0)
            {
                omitting = true;
                continue;
            }

            // check for single-line omission
            index = line.IndexOf("// @omit");
            if (index >= 0)
            {
                continue;
            }

            // check for snippet end
            index = line.IndexOf("// @end");
            if (index >= 0)
            {
                sw.WriteLine("]]></code>");
                sw.Close();
                sw = null;
                snippet = null;
                continue;
            }

            // check for replacement text
            index = line.IndexOf("// @replace ");
            if (index >= 0)
            {
                string replacement = line.Substring(index + "// @replace".Length);
                replacement = replacement.Trim();
                int length = line.Length - line.TrimStart(null).Length;
                sw.WriteLine(line.Substring(0, length) + replacement);
            }
            else
            {
                sw.WriteLine(line);
            }
        }
    }

}

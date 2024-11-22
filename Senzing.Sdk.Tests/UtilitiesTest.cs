namespace Senzing.Sdk.Tests;

using System.Text;
using System.Text.Json.Nodes;

using NUnit.Framework;

using Senzing.Sdk;
using Senzing.Sdk.Tests.Core;

[TestFixture]
[FixtureLifeCycle(LifeCycle.SingleInstance)]
[Parallelizable(scope: ParallelScope.All)]
internal class UtilitiesTest : AbstractTest
{
    [OneTimeSetUp]
    public void Initialze()
    {
        this.BeginTests();
    }

    [OneTimeTearDown]
    public void Complete()
    {
        this.EndTests();
    }

    [Test]
    [TestCase(0L)]
    [TestCase(1L)]
    [TestCase(2L)]
    [TestCase(20L)]
    [TestCase(40L)]
    [TestCase(80L)]
    [TestCase(160L)]
    [TestCase(3200L)]
    [TestCase(64000L)]
    [TestCase(128345789L)]
    public void TestHexFormat(long value)
    {
        string text = Utilities.HexFormat(value);
        string[] tokens = text.Split(' ');
        StringBuilder sb = new StringBuilder();
        foreach (string token in tokens)
        {
            sb.Append(token);
        }
        string formatted = sb.ToString();
        long reversedValue = Convert.ToInt64(formatted, 16);
        Assert.That(value, Is.EqualTo(reversedValue), "Value was not as expected");
    }

    [Test]
    public void testJsonEscapeNull()
    {
        string text = Utilities.JsonEscape(null);
        Assert.That(
            text, Is.EqualTo("null"), "The JSON-escaped null value was not \"null\"");
    }

    [Test]
    [TestCase("Hello")]
    [TestCase("Hello,\nWorld")]
    [TestCase("\f\b\\\tHey!\r\n")]
    [TestCase("Bell \u0007!")]
    public void TestJsonEscape(string value)
    {
        string text = Utilities.JsonEscape(value);
        string objText = "{\"value\": " + text + "}";

        JsonObject? obj = JsonNode.Parse(objText)?.AsObject();

        string? parsedValue = obj?["value"]?.GetValue<string>();
        Assert.That(parsedValue, Is.EqualTo(value),
                    "The reverse parsed value is not as expected");
    }

}

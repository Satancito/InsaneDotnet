using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    internal static class TestSerializationAssertions
    {
        public static void AssertJsonEquals(JsonObject expected, JsonObject actual)
        {
            Assert.AreEqual(expected.ToJsonString(), actual.ToJsonString());
        }
    }
}

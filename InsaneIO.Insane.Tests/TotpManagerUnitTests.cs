using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    
    public class TotpManagerUnitTests
    {

        public readonly List<(string Code, long EpochMilliseconds)> Codes = new List<(string, long)>
        {
            ("528272",1676334453222),
            ("221152", 1676334549854),
            ("143989", 1676334957195),
            ("479754", 1676335321240),
            ("737759",1676341598474)
        };

        private readonly TotpManager manager = new()
        {
            Secret = "insaneiosecret".ToByteArrayUtf8(),
            Issuer = "InsaneIO",
            Label = "insane@insaneio.com"
        };

        [TestMethod]
        public void TestVerifyCodes()
        {
            foreach (var element in Codes)
            {
                manager.VerifyCode(element.Code, DateTimeOffset.FromUnixTimeMilliseconds(element.EpochMilliseconds)).Should().BeTrue();
            }
        }

        [TestMethod]
        public void TestComputeCodes()
        {
            foreach (var element in Codes)
            {
                var computed = manager.ComputeCode(DateTimeOffset.FromUnixTimeMilliseconds(element.EpochMilliseconds));
                computed.Should().BeEquivalentTo(element.Code);
            }
        }

        [TestMethod]
        public void TestSerializeDeserialize()
        {
            string json = manager.Serialize();
            JsonObject jsonObject = manager.ToJsonObject();
            TotpManager deserialized = TotpManager.Deserialize(json);
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
        }

    }
}

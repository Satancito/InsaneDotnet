using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class TotpManagerUnitTests
    {
        public readonly List<(string Code, long EpochMilliseconds)> Codes =
        [
            ("528272", 1676334453222),
            ("221152", 1676334549854),
            ("143989", 1676334957195),
            ("479754", 1676335321240),
            ("737759", 1676341598474)
        ];

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
                string computed = manager.ComputeCode(DateTimeOffset.FromUnixTimeMilliseconds(element.EpochMilliseconds));
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

        [TestMethod]
        public void Deserialize_ShouldRejectMismatchedAssemblyName()
        {
            string json = HexEncoder.DefaultInstance.Serialize();

            FluentActions.Invoking(() => TotpManager.Deserialize(json)).Should().Throw<DeserializeException>();
        }

        [TestMethod]
        public void Deserialize_ShouldRejectUndefinedEnums()
        {
            JsonObject json = manager.ToJsonObject();
            json[nameof(TotpManager.CodeLength)] = 999;
            json[nameof(TotpManager.HashAlgorithm)] = 999;

            FluentActions.Invoking(() => TotpManager.Deserialize(json.ToJsonString())).Should().Throw<DeserializeException>();
        }
    }
}

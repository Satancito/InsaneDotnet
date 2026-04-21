using FluentAssertions;
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;

namespace InsaneIO.Insane.Tests
{
    
    [TestClass]
    public class HexEncoderUnitTests
    {
        private readonly byte[] testTytes = new byte[] { 0xff, 0xa, 1, 0x22 };
        private const string hexStringUppercase = "FF0A0122";
        private const string hexStringLowercase = "ff0a0122";
        private static readonly HexEncoder encoderToUpper = new() { ToUpper = true };
        private static readonly HexEncoder encoderToLower = new() { ToUpper = false };

        [TestMethod]
        public void TestDecodeUppercase()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpper.Decode(hexStringUppercase), testTytes));
        }

        [TestMethod]
        public void TestDecodeLowercase()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLower.Decode(hexStringLowercase), testTytes));
        }

        [TestMethod]
        public void TestEncodeUpper()
        {
            Assert.AreEqual(hexStringUppercase, encoderToUpper.Encode(testTytes));
        }

        [TestMethod]
        public void TestEncodeLower()
        {
            Assert.AreEqual(hexStringLowercase, encoderToLower.Encode(testTytes));
        }

        [TestMethod]
        public void TestSerializeDeserialize()
        {
            IEncoder encoder = HexEncoder.DefaultInstance;
            string json = encoder.Serialize();
            JsonObject jsonObject = encoder.ToJsonObject();
            IEncoder deserialized = HexEncoder.Deserialize(json);
            Assert.AreEqual(encoder.GetType().FullName, deserialized.GetType().FullName);
            Assert.IsInstanceOfType(deserialized, typeof(HexEncoder));
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
        }

    }


}

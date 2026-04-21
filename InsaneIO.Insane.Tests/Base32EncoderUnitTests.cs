using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsaneIO.Insane.Extensions;
using InsaneIO.Insane.Cryptography;
using System.Runtime.Versioning;
using System.Text.Json.Nodes;
using FluentAssertions;

namespace InsaneIO.Insane.Tests
{

    [TestClass]
    
    public class Base32EncoderUnitTests
    {
        public readonly static byte[] TestBytes = new byte[] { 104, 101, 108, 108, 111, 119, 111, 114, 108, 100 };

        public readonly static string TestString = "helloworld";
        public readonly static string UpperBase32Result = "NBSWY3DPO5XXE3DE";
        public readonly static string LowerBase32Result = "nbswy3dpo5xxe3de";

        public readonly static string TestString2 = "A";
        public readonly static byte[] TestBytes2 = new byte[] { 65 };
        public readonly static string UpperBase32Result2 = "IE======";
        public readonly static string LowerBase32Result2 = "ie======";
        public readonly static string UpperBase32Result2NoPadding = "IE";
        public readonly static string LowerBase32Result2NoPadding = "ie";

        public readonly static Base32Encoder encoderToLowerWithPadding = new Base32Encoder { ToLower = true, RemovePadding = false };
        public readonly static Base32Encoder encoderToLowerNoPadding = new Base32Encoder { ToLower = true, RemovePadding = true };
        public readonly static Base32Encoder encoderToUpperWithPadding = new Base32Encoder {  RemovePadding = false };
        public readonly static Base32Encoder encoderToUpperNoPadding = new Base32Encoder { RemovePadding = true };

        [TestMethod]
        public void TestEncodeUppercaseWithPadding()
        {
            Assert.AreEqual(encoderToUpperWithPadding.Encode(TestBytes), UpperBase32Result);
        }

        [TestMethod]
        public void TestEncodeLowercaseWithPadding()
        {
            Assert.AreEqual(encoderToLowerWithPadding.Encode(TestBytes), LowerBase32Result);
        }

        [TestMethod]
        public void TestDecodeUpperCaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(UpperBase32Result), TestBytes));
        }

        [TestMethod]
        public void TestDecodeLowerCaseWithPadding()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(LowerBase32Result), TestBytes));
        }

        [TestMethod]
        public void TestEncodeUppercaseWithPadding2()
        {
            Assert.AreEqual(encoderToUpperWithPadding.Encode(TestBytes2), UpperBase32Result2);
        }

        [TestMethod]
        public void TestEncodeLowercaseWithPadding2()
        {
            Assert.AreEqual(encoderToLowerWithPadding.Encode(TestBytes2), LowerBase32Result2);
        }

        [TestMethod]
        public void TestDecodeUpperCaseWithPadding2()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperWithPadding.Decode(UpperBase32Result2), TestBytes2));
        }

        [TestMethod]
        public void TestDecodeLowerCaseWithPadding2()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLowerWithPadding.Decode(LowerBase32Result2), TestBytes2));
        }


        [TestMethod]
        public void TestEncodeUppercaseNoPadding2()
        {
            Assert.AreEqual(encoderToUpperNoPadding.Encode(TestBytes2), UpperBase32Result2NoPadding);
        }

        [TestMethod]
        public void TestEncodeLowercaseNoPadding2()
        {
            var x = encoderToLowerNoPadding.Encode(TestBytes2);
            Assert.AreEqual(x, LowerBase32Result2NoPadding);
        }

        [TestMethod]
        public void TestDecodeUpperCaseNoPadding2()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToUpperNoPadding.Decode(UpperBase32Result2NoPadding), TestBytes2));
        }

        [TestMethod]
        public void TestDecodeLowerCaseNoPadding2()
        {
            Assert.IsTrue(Enumerable.SequenceEqual(encoderToLowerNoPadding.Decode(LowerBase32Result2NoPadding), TestBytes2));
        }

        [TestMethod]
        public void TestSerializeDeserialize()
        {
            IEncoder encoder = Base32Encoder.DefaultInstance;
            string json = encoder.Serialize();
            JsonObject jsonObject = encoder.ToJsonObject();
            IEncoder deserialized = Base32Encoder.Deserialize(json);
            Assert.AreEqual(encoder.GetType().FullName, deserialized.GetType().FullName);
            Assert.IsInstanceOfType(deserialized, typeof(Base32Encoder));
            TestSerializationAssertions.AssertJsonEquals(jsonObject, deserialized.ToJsonObject());
        }
    }
}

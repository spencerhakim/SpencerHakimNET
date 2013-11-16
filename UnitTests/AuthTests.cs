using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpencerHakim;
using SpencerHakim.Auth;
using SpencerHakim.Extensions;

namespace UnitTests
{
    [TestClass]
    public class AuthTests
    {
        [TestMethod]
        public void Base32_RegressionValues()
        {
            byte[] INPUT1 = Encoding.UTF8.GetBytes("foo");
            byte[] INPUT2 = Encoding.UTF8.GetBytes("foob");
            byte[] INPUT3 = Encoding.UTF8.GetBytes("fooba");
            byte[] INPUT4 = Encoding.UTF8.GetBytes("foobar");

            string OUTPUT1 = "MZXW6";
            string OUTPUT2 = "MZXW6YQ";
            string OUTPUT3 = "MZXW6YTB";
            string OUTPUT4 = "MZXW6YTBOI";

            // check encoding
            Assert.AreEqual(OUTPUT1, Convert.ToBase32String(INPUT1));
            Assert.AreEqual(OUTPUT2, Convert.ToBase32String(INPUT2));
            Assert.AreEqual(OUTPUT3, Convert.ToBase32String(INPUT3));
            Assert.AreEqual(OUTPUT4, Convert.ToBase32String(INPUT4));

            // check decoding
            CollectionAssert.AreEqual(INPUT1, Convert.FromBase32String(OUTPUT1));
            CollectionAssert.AreEqual(INPUT2, Convert.FromBase32String(OUTPUT2));
            CollectionAssert.AreEqual(INPUT3, Convert.FromBase32String(OUTPUT3));
            CollectionAssert.AreEqual(INPUT4, Convert.FromBase32String(OUTPUT4));

        }

        [TestMethod]
        public void Base32_Ambiguous()
        {
            byte[] b16 = Convert.FromBase32String("7777777777777777"); // 16 7s.
            byte[] b17 = Convert.FromBase32String("77777777777777777"); // 17 7s.
            CollectionAssert.AreEqual(b16, b17);
        }

        [TestMethod]
        public void Base32_Small()
        {
            // decoded, but not enough to return any bytes.
            Assert.AreEqual(0, Convert.FromBase32String("A").Length);
            Assert.AreEqual(0, Convert.FromBase32String("").Length);
            Assert.AreEqual(0, Convert.FromBase32String(" ").Length);

            // decoded successfully and returned 1 byte.
            Assert.AreEqual(1, Convert.FromBase32String("AA").Length);
            Assert.AreEqual(1, Convert.FromBase32String("AAA").Length);

            // decoded successfully and returned 2 bytes.
            Assert.AreEqual(2, Convert.FromBase32String("AAAA").Length);

            // acceptable separators " " and "-" which should be ignored
            //Assert.AreEqual(2, Convert.FromBase32String("AA AA").Length);
            //Assert.AreEqual(2, Convert.FromBase32String("AA-AA").Length);
            CollectionAssert.AreEqual(Convert.FromBase32String("AA-AA"), Convert.FromBase32String("AA AA"));
            //CollectionAssert.AreEqual(Convert.FromBase32String("AAAA"), Convert.FromBase32String("AA AA"));

            // 1, 8, 9, 0 are not a valid character, decoding should fail
            Assert.IsNull(Convert.FromBase32String("11"));
            Assert.IsNull(Convert.FromBase32String("A1"));
            Assert.IsNull(Convert.FromBase32String("AAA8"));
            Assert.IsNull(Convert.FromBase32String("AAA9"));
            Assert.IsNull(Convert.FromBase32String("AAA0"));

            // non-alphanumerics (except =) are not valid characters and decoding should fail
            Assert.IsNull(Convert.FromBase32String("AAA,"));
            Assert.IsNull(Convert.FromBase32String("AAA;"));
            Assert.IsNull(Convert.FromBase32String("AAA."));
            Assert.IsNull(Convert.FromBase32String("AAA!"));

        }

        [TestMethod]
        public void TOTP_Ctor()
        {
            new TOTP<HMACSHA1>("").SafeDispose();
            new TOTP<HMACSHA256>("").SafeDispose();
            new TOTP<HMACSHA512>("").SafeDispose();
            new TOTP<HMACMD5>("").SafeDispose();
        }

        [TestMethod]
        public void TOTP_Calc()
        {
            using( var totp = new TOTP<HMACSHA1>("7777777777777777") )
            {
                Assert.AreEqual("724477", totp.Calculate(Environment.UnixEpochDateTime));
                Assert.AreEqual("683298", totp.Calculate(30));
                Assert.AreEqual("891123", totp.Calculate(60));
            }

            using( var totp = new TOTP<HMACSHA1>("JBSWY3DPK5XXE3DE", 60) )
            {
                Assert.AreEqual("495334", totp.Calculate(Environment.UnixEpochDateTime));
                Assert.AreEqual("495334", totp.Calculate(30));
                Assert.AreEqual("052202", totp.Calculate(60));
            }
        }
    }
}

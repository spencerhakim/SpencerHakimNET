using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using SpencerHakim.Drawing;

namespace UnitTests
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void CIE94()
        {
            Assert.AreEqual(100, ColorDeltaAlgorithms.CIE94(Color.White, Color.Black), 0.01);
            Assert.AreEqual(100, ColorDeltaAlgorithms.CIE94(Color.Black, Color.White), 0.01);
            Assert.AreEqual(13.928, ColorDeltaAlgorithms.CIE94(Color.Cyan, Color.LightCyan), 0.01);
            Assert.AreEqual(0, ColorDeltaAlgorithms.CIE94(Color.White, Color.White));
        }
    }
}

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
        public void TestMethod1()
        {
            Console.WriteLine("RGB");
            Console.WriteLine("White:Black - " + DeltaAlgorithms.DeltaRGB(Color.White, Color.Black) );
            Console.WriteLine("Cyan:LightCyan - " + DeltaAlgorithms.DeltaRGB(Color.Cyan, Color.LightCyan) );

            Console.WriteLine("\nDeltaE2000");
            var white = CIELab_Color.FromXYZ( XYZ_Color.FromColor(Color.White) );
            var black = CIELab_Color.FromXYZ( XYZ_Color.FromColor(Color.Black) );
            var cyan = CIELab_Color.FromXYZ( XYZ_Color.FromColor(Color.Cyan) );
            var lightcyan = CIELab_Color.FromXYZ( XYZ_Color.FromColor(Color.LightCyan) );

            Console.WriteLine("White:Black - " + DeltaAlgorithms.DeltaE2000(white, black) );
            Console.WriteLine("Cyan:LightCyan - " + DeltaAlgorithms.DeltaE2000(cyan, lightcyan) );
        }
    }
}

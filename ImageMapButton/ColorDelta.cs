using System;
using System.Drawing;

namespace SpencerHakim.Drawing
{
    #region Color types
    public class CIELab_Color
    {
        public double L { get; set; }
        public double a { get; set; }
        public double b { get; set; }

        /// <summary>
        /// instantiates a CIELab_Color instance from the supplied 
        /// CIE Lab components
        /// </summary>
        /// <param name="L">
        /// CIE Lightnes
        /// </param>
        /// <param name="a">
        /// CIE color component a
        /// </param>
        /// <param name="b">
        /// CIE color component b
        /// </param>
        /// <see>
        /// http://en.wikipedia.org/wiki/Lab_color_space
        /// </see>
        public CIELab_Color(double L, double a, double b)
        {
            this.L = Utilities.round(L, 3);
            this.a = Utilities.round(a, 3);
            this.b = Utilities.round(b, 3);
        }

        #region Conversion statics
        /// <summary>
        /// Converts a standard Color to a CIELab_Color
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <returns>Converted color</returns>
        public static CIELab_Color FromColor(Color color)
        {
            return FromXYZ( XYZ_Color.FromColor(color) );
        }

        /// <summary>
        /// instantiates a CIELab_Color instance from the supplied 
        /// CIE XYZ_Color
        /// </summary>
        /// <param name="xyz">
        /// XYZ_Color that is to be converted to a CIELab_Color
        /// </param>
        /// <returns>
        /// the CIELab_Color derived from the XYZ_Color
        /// </returns>
        /// <see>
        /// http://en.wikipedia.org/wiki/Lab_color_space
        /// </see>
        internal static CIELab_Color FromXYZ(XYZ_Color xyz)
        {
            // constants using 
            // Observer= 2°, 
            // Illuminant= D65
            double additive_constant = 16.0 / 116.0;
            double one_third_constant = 1.0 / 3.0;

            double X = xyz.X / 95.047;  // [0.0,1.0]
            double Y = xyz.Y / 100.0;   // [0.0,1.0]
            double Z = xyz.Z / 108.883; // [0.0,1.0]

            double L;
            double a;
            double b;

            if( X > 0.008856 )
                X = Math.Pow(X, one_third_constant);
            else
                X = (7.787 * X) + additive_constant;

            if( Y > 0.008856 )
                Y = Math.Pow(Y, one_third_constant);
            else
                Y = (7.787 * Y) + additive_constant;

            if( Z > 0.008856 )
                Z = Math.Pow(Z, one_third_constant);
            else
                Z = (7.787 * Z) + additive_constant;

            L = (116.0 * Y) - 16.0;
            a = 500.0 * (X - Y);
            b = 200.0 * (Y - Z);

            return new CIELab_Color(L, a, b);
        }
        #endregion
    }

    internal class XYZ_Color
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// instantiates a XYZ_Color instance from the supplied XYZ 
        /// components
        /// </summary>
        /// <param name="X">
        /// double precision X component for the XYZ_Color
        /// </param>
        /// <param name="Y">
        /// double precision Y component for the XYZ_Color
        /// </param>
        /// <param name="Z">
        /// double precision Z component for the XYZ_Color
        /// </param>
        public XYZ_Color(double X, double Y, double Z)
        {

            this.X = Utilities.round(X, 3);
            this.Y = Utilities.round(Y, 3);
            this.Z = Utilities.round(Z, 3);
        }

        #region Conversion statics
        /// <summary>
        /// creates the XYZ_Color from the supplied double precision 
        /// RGB color components
        /// </summary>
        /// <param name="r">
        /// double precision value containing the the red component of 
        /// the color in the range [0.0,1.0]
        /// </param>
        /// <param name="g">
        /// double precision value containing the the green component 
        /// of the color in the range [0.0,1.0]
        /// </param>
        /// <param name="b">
        /// double precision value containing the the blue component 
        /// of the color in the range [0.0,1.0]
        /// </param>
        /// <returns>
        /// the XYZ_Color derived from the given double precision RGB 
        /// color components
        /// </returns>
        /// <see>
        /// http://www.easyrgb.com/index.php?X=MATH&H=02#text2
        /// http://forum.doom9.org/showthread.php?t=150145
        /// </see>
        public static XYZ_Color FromRGB(double R, double G, double B)
        {
            double r0;
            double g0;
            double b0;

            double X;
            double Y;
            double Z;

            if( R > 0.04045 )
                r0 = Math.Pow(((R + 0.055) / 1.055), 2.4);
            else
                r0 = R / 12.92;

            if( G > 0.04045 )
                g0 = Math.Pow(((G + 0.055) / 1.055), 2.4);
            else
                g0 = G / 12.92;

            if( B > 0.04045 )
                b0 = Math.Pow(((B + 0.055) / 1.055), 2.4);
            else
                b0 = B / 12.92;

            r0 = r0 * 100.0;
            g0 = g0 * 100.0;
            b0 = b0 * 100.0;

            // Observer = 2°, 
            // Illuminant = D65

            // from http://forum.doom9.org/showthread.php?t=150145
            X = r0 * 0.4124564 + g0 * 0.3575761 + b0 * 0.1804375;
            Y = r0 * 0.2126729 + g0 * 0.7151522 + b0 * 0.0721750;
            Z = r0 * 0.0193339 + g0 * 0.1191920 + b0 * 0.9503041;

            // from http://www.easyrgb.com/index.php?X=MATH&H=02#text2
            //X = r0 * 0.4124 + g0 * 0.3576 + b0 * 0.1805;
            //Y = r0 * 0.2126 + g0 * 0.7152 + b0 * 0.0722;
            //Z = r0 * 0.0193 + g0 * 0.1192 + b0 * 0.9505;

            // http://software.intel.com/sites/products/documentation/hpc/ipp/ippi/ippi_ch6/ch6_color_models.html#fig6-9
            //X = 0.412453 * r0 + 0.35758 * g0 + 0.180423 * b0;
            //Y = 0.212671 * r0 + 0.71516 * g0 + 0.072169 * b0;
            //Z = 0.019334 * r0 + 0.119193 * g0 + 0.950227 * b0;

            return new XYZ_Color(X, Y, Z);
        }

        /// <summary>
        /// creates the XYZ_Color from the supplied byte RGB color 
        /// components
        /// </summary>
        /// <param name="R">
        /// byte value containing the red component of the color 
        /// (e.g., ( color & 0x00FF0000 ) >> 16 ) or color.R) in the 
        /// range [0,255]
        /// </param>
        /// <param name="G">
        /// byte value containing the green component of the color 
        /// (e.g., ( color & 0x0000FF00 ) >> 8 ) or color.G) in the 
        /// range [0,255]
        /// </param>
        /// <param name="B">
        /// byte value containing the blue component of the color  
        /// (e.g., ( color & 0x000000F ) or color.B) in the 
        /// range [0,255]
        /// </param>
        /// <returns>
        /// the XYZ_Color derived from the given byte RGB color 
        /// components
        /// </returns>
        public static XYZ_Color FromRGB(Byte R, Byte G, Byte B)
        {
            double r = (double)R / 255d;
            double g = (double)G / 255d;
            double b = (double)B / 255d;

            return FromRGB(r, g, b);
        }

        /// <summary>
        /// creates the XYZ_Color values from the supplied RGB color 
        /// </summary>
        /// <param name="color">
        /// the color to be converted 
        /// </param>
        /// <returns>
        /// <returns>
        /// the XYZ_Color derived from the given RGB color
        /// </returns>
        public static XYZ_Color FromColor(Color color)
        {
            return FromRGB(color.R, color.G, color.B);
        }

        /// <summary>
        /// creates the XYZ_Color from the supplied double precision 
        /// CIE-Lab color components
        /// </summary>
        /// <param name="CIE_L">
        /// CIE-Lab L component 
        /// </param>
        /// <param name="CIE_a">
        /// CIE-Lab a component 
        /// </param>
        /// <param name="CIE_b">
        /// CIE-Lab b component 
        /// </param>
        /// <returns>
        /// the XYZ_Color derived from the given CIE-Lab color 
        /// components
        /// </returns>
        public static XYZ_Color FromCIE(double CIE_L, double CIE_a, double CIE_b)
        {
            double x;
            double y;
            double z;

            double x_cubed;
            double y_cubed;
            double z_cubed;
            // Observer= 2°
            // Illuminant= D65
            double x_offset = 95.047;
            double y_offset = 100.000;
            double z_offset = 108.883;

            double t = 16.0 / 116.0;

            double X;
            double Y;
            double Z;

            y = (CIE_L + 16.0) / 116.0;
            x = (CIE_a / 500.0) + y;
            z = y - (CIE_b / 200.0);

            x_cubed = x * x * x;
            y_cubed = y * y * y;
            z_cubed = z * z * z;

            if( y_cubed > 0.008856 )
                y = y_cubed;
            else
                y = (y - t) / 7.787;

            if( x_cubed > 0.008856 )
                x = x_cubed;
            else
                x = (x - t) / 7.787;

            if( z_cubed > 0.008856 )
                z = z_cubed;
            else
                z = (z - t) / 7.787;

            X = x_offset * x;
            Y = y_offset * y;
            Z = z_offset * z;

            return new XYZ_Color(X, Y, Z);
        }

        /// <summary>
        /// creates the XYZ_Color values from the supplied CIELab 
        /// color 
        /// </summary>
        /// <param name="color">
        /// the CIELab color to be converted 
        /// </param>
        /// <returns>
        /// <returns>
        /// the XYZ_Color derived from the given CIE-Lab color 
        /// </returns>
        public static XYZ_Color FromCIELab(CIELab_Color color)
        {
            return FromCIE(color.L, color.a, color.b);
        }
        #endregion
    }
    #endregion

    #region MathUtilities
    internal static class Utilities
    {
        // http://www.codeproject.com/info/cpol10.aspx
        public static double round(double number, int decimal_places)
        {
            int[] powers = new int[10]{
                          1,    // ^0
                         10,    // ^1
                        100,    // ^2
                       1000,    // ^3
                      10000,    // ^4
                     100000,    // ^5
                    1000000,    // ^6
                   10000000,    // ^7
                  100000000,    // ^8
                 1000000000     // ^9
            };

            if( decimal_places < 0 || decimal_places > 9 )
                throw new ArgumentException("Precision out of range [0,9]", "decimal_places");

            int power = powers[decimal_places];
            double t = (number * (double)power) + 0.5;

            return t / (double)power;
        }

        public static double[] ToRGBArray(this Color color)
        {
            return new double[]{ color.R, color.G, color.B };
        }
    }
    #endregion

    #region Delta algorithms
    public class ColorDeltaAlgorithms
    {
        /// <summary>
        /// Computes the difference (really the distance) between two Colors using the CIE94 color difference algorithm
        /// </summary>
        /// <param name="x">The first color</param>
        /// <param name="y">The second color</param>
        /// <returns>Difference between the colors [0,100]</returns>
        /// <see cref="http://en.wikipedia.org/wiki/Color_difference#CIE94"/>
        public static double CIE94(Color x, Color y)
        {
            return CIE94(CIELab_Color.FromColor(x), CIELab_Color.FromColor(y));
        }

        /// <summary>
        /// Computes the difference (really the distance) between two CIELab_Colors using the CIE94 color difference algorithm
        /// </summary>
        /// <param name="x">The first color</param>
        /// <param name="y">The second color</param>
        /// <returns>Difference between the colors [0,100]</returns>
        /// <see cref="http://en.wikipedia.org/wiki/Color_difference#CIE94"/>
        public static double CIE94(CIELab_Color x, CIELab_Color y)
        {
            //these are apparently always 1, and usually in unity with K_L ( [K_L:K_C:K_H] == [1:1:1] or [2:1:1] )
            const double K_C = 1.0;
            const double K_H = 1.0;

            //weighting factors for graphic arts
            const double K_L = 1.0;
            const double K_1 = 0.045;
            const double K_2 = 0.015;

            double ΔL = x.L - y.L;
            double Δa = x.a - y.a, Δa_2 = Math.Pow(Δa, 2);
            double Δb = x.b - y.b, Δb_2 = Math.Pow(Δb, 2);

            double C_1 = Math.Sqrt(Math.Pow(x.a, 2) + Math.Pow(x.b, 2));
            double C_2 = Math.Sqrt(Math.Pow(y.a, 2) + Math.Pow(y.b, 2));
            double ΔC_ab = C_1 - C_2, ΔC_ab_2 = Math.Pow(ΔC_ab, 2);

            // avoid imaginary ΔH_ab
            double ΔH_ab = 0.0;
            if( (Δa_2 + Δb_2) >= ΔC_ab_2 )
                ΔH_ab = Math.Sqrt(Δa_2 + Δb_2 - ΔC_ab_2);

            const double S_L = 1.0;
            double S_C = 1.0 + (K_1 * C_1);
            double S_H = 1.0 + (K_2 * C_1);

            return Math.Sqrt(
                Math.Pow(ΔL/ (K_L * S_L), 2) +
                Math.Pow(ΔC_ab / (K_C * S_C), 2) +
                Math.Pow(ΔH_ab / (K_H * S_H), 2)
            );
        }
    }
    #endregion
}

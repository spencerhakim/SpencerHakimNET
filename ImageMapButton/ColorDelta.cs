using System;
using System.Drawing;

namespace SpencerHakim.Drawing
{
    #region Color types
    public class CIELab_Color
    {
        public double CIE_L { get; set; }
        public double CIE_a { get; set; }
        public double CIE_b { get; set; }

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
            CIE_L = MathUtilities.round(L, 3);
            CIE_a = MathUtilities.round(a, 3);
            CIE_b = MathUtilities.round(b, 3);
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
        public static CIELab_Color FromXYZ(XYZ_Color xyz)
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
    }

    public class XYZ_Color
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

            this.X = MathUtilities.round(X, 3);
            this.Y = MathUtilities.round(Y, 3);
            this.Z = MathUtilities.round(Z, 3);
        }

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
            return FromCIE(color.CIE_L, color.CIE_a, color.CIE_b);
        }

    }
    #endregion

    #region MathUtilities
    public static class MathUtilities
    {
        /// <summary>
        /// rounds a double precision number to the specified number 
        /// of decimal places
        /// </summary>
        /// <param name="number">
        /// double precision value to round
        /// </param>
        /// <param name="decimal_places">
        /// number of decimal points to maintain
        /// </param>
        /// <returns>
        /// double precision value rounded to the specified decimal 
        /// places
        /// </returns>
        /// <exception>
        /// ArgumentException if decimal places not in the range [0,9]
        /// </exception>
        /// <remarks>
        /// uses round half up rule for tie-breaking
        /// </remarks>
        /// <see cref="http://en.wikipedia.org/wiki/Rounding"/>
        /// <algorithm>
        /// 1. Multiple the original number by 10^decimal_places
        /// 2. Add 0.5 and round the result (truncate to an integer)
        /// 3. Divide result by 10^decimal_places
        /// </algorithm>
        /// <copyright>
        /// Distributed under the Code Project Open License
        /// http://www.codeproject.com/info/cpol10.aspx
        /// </copyright>
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

        /// <summary>
        /// converts radians to degrees
        /// </summary>
        /// <param name="radians">
        /// double precision radians value to be converted
        /// </param>
        /// <returns>
        /// double precision degrees obtained by converting radians
        /// </returns>
        /// <see>
        /// http://en.wikipedia.org/wiki/Radian
        /// </see>
        public static double rad2deg(double radians)
        {
            return radians / Math.PI * 180d;
        }

        /// <summary>
        /// converts degrees to radians
        /// </summary>
        /// <param name="degrees">
        /// double precision degrees value to be converted
        /// </param>
        /// <returns>
        /// double precision radians obtained by converting degrees
        /// </returns>
        /// <see>
        /// http://en.wikipedia.org/wiki/Radian
        /// </see>
        public static double deg2rad(double degrees)
        {
            return degrees * Math.PI / 180d;
        }
    }
    #endregion

    #region Delta algorithms
    public class DeltaAlgorithms
    {
        /// <summary>
        /// compute the difference between two Colors using a
        /// Euclidian distance algorithm
        /// </summary>
        /// <param name="color_1">
        /// first RGB Color to form the difference
        /// </param>
        /// <param name="color_2">
        /// second RGB Color to form the difference
        /// </param>
        /// <returns>
        /// difference between the two RGB Color instances
        /// </returns>
        public static double DeltaRGB(Color color_1, Color color_2)
        {
            double delta_R = (double)(color_1.R - color_2.R);
            double delta_G = (double)(color_1.G - color_2.G);
            double delta_B = (double)(color_1.B - color_2.B);

            // Eudlidean distance
            return Math.Sqrt((delta_R * delta_R) + 
                             (delta_G * delta_G) + 
                             (delta_B * delta_B));
        }

        /// <summary>
        /// compute the difference between two CIELab_Color using 
        /// Euclidian distance algorithm
        /// </summary>
        /// <param name="cielab_1">
        /// first Color to form the difference
        /// </param>
        /// <param name="cielab_2">
        /// second Color to form the difference
        /// </param>
        /// <returns>
        /// difference between the two CIELab_Color instances
        /// </returns>
        public static double DeltaE1976(CIELab_Color cielab_1, CIELab_Color cielab_2)
        {
            double delta_CIE_a = cielab_1.CIE_a - cielab_2.CIE_a;
            double delta_CIE_b = cielab_1.CIE_b - cielab_2.CIE_b;
            double delta_CIE_L = cielab_1.CIE_L - cielab_2.CIE_L;

            // Eudlidean distance
            return Math.Sqrt((delta_CIE_L * delta_CIE_L) +
                             (delta_CIE_a * delta_CIE_a) +
                             (delta_CIE_b * delta_CIE_b));
        }

        /// <summary>
        /// compute the difference between two CIE Lab colors using 
        /// the CIE 1994 delta E algorithm
        /// </summary>
        /// <param name="cielab_1">
        /// first CIELab_Color to form the difference
        /// </param>
        /// <param name="cielab_2">
        /// second CIELab_Color to form the difference
        /// </param>
        /// <returns>
        /// difference between the two CIELab_Color instances
        /// </returns>
        /// <see>
        /// http://en.wikipedia.org/wiki/Color_difference
        /// </see>
        public static double DeltaE1994(CIELab_Color cielab_1, CIELab_Color cielab_2)
        {
            double C1;
            double C2;
            double CIE_1_a_squared = cielab_1.CIE_a * cielab_1.CIE_a;
            double CIE_1_b_squared = cielab_1.CIE_b * cielab_1.CIE_b;
            double CIE_2_a_squared = cielab_2.CIE_a * cielab_2.CIE_a;
            double CIE_2_b_squared = cielab_2.CIE_b * cielab_2.CIE_b;
            double delta_a;
            double delta_a_squared;
            double delta_b;
            double delta_b_squared;
            double delta_C_ab;
            double delta_C_ab_divisor;
            double delta_C_ab_squared;
            double delta_E_Lab;
            double delta_H_ab;
            double delta_H_ab_divisor;
            double delta_L;
            double delta_L_squared;
            double K_1;
            double K_2;

            delta_L = cielab_1.CIE_L - cielab_2.CIE_L;
            delta_L_squared = delta_L * delta_L;

            delta_a = cielab_1.CIE_a - cielab_2.CIE_a;
            delta_a_squared = delta_a * delta_a;

            delta_b = cielab_1.CIE_b - cielab_2.CIE_b;
            delta_b_squared = delta_b * delta_b;

            delta_E_Lab = Math.Sqrt(delta_L_squared + delta_a_squared + delta_b_squared);

            C1 = Math.Sqrt(CIE_1_a_squared + CIE_1_b_squared);
            C2 = Math.Sqrt(CIE_2_a_squared + CIE_2_b_squared);
            delta_C_ab = C1 - C2;
            delta_C_ab_squared = delta_C_ab * delta_C_ab;

            // avoid imaginary delta_H_ab
            if( (delta_a_squared + delta_b_squared) >= delta_C_ab_squared )
                delta_H_ab = Math.Sqrt(delta_a_squared + delta_b_squared - delta_C_ab_squared);
            else
                delta_H_ab = 0.0;

            // weighting factors for 
            // graphic arts
            // K_L = 1.0;               // => no delta_L division
            K_1 = 0.045;
            K_2 = 0.015;

            delta_C_ab_divisor = 1.0 + (K_1 * C1);
            delta_H_ab_divisor = 1.0 + (K_2 * C1);

            delta_C_ab /= delta_C_ab_divisor;
            delta_H_ab /= delta_H_ab_divisor;

            return Math.Sqrt(delta_L_squared + 
                             (delta_C_ab * delta_C_ab) + 
                             (delta_H_ab * delta_H_ab));
        }

        public static double DeltaE2000(CIELab_Color cielab_1, CIELab_Color cielab_2)
        {
            double c = Math.Pow(25, 7);
            double CIE_1_a_squared = cielab_1.CIE_a * cielab_1.CIE_a;
            double CIE_1_b_squared = cielab_1.CIE_b * cielab_1.CIE_b;
            double CIE_2_a_squared = cielab_2.CIE_a * cielab_2.CIE_a;
            double CIE_2_b_squared = cielab_2.CIE_b * cielab_2.CIE_b;
            double E00;
            double t;
            double weighting_factor_C = 1.0;
            double weighting_factor_H = 1.0;
            double weighting_factor_L = 1.0;
            double xC1;
            double xC2;
            double xCX;
            double xCY;
            double xDC;
            double xDH;
            double xDL;
            double xGX;
            double xH1;
            double xH2;
            double xHX;
            double xLX;
            double xNN;
            double xPH;
            double xRC;
            double xRT;
            double xSC;
            double xSH;
            double xSL;
            double xTX;

            xC1 = Math.Sqrt(CIE_1_a_squared + CIE_1_b_squared);
            xC2 = Math.Sqrt(CIE_2_a_squared + CIE_2_b_squared);
            xCX = (xC1 + xC2) / 2.0;
            t = Math.Pow(xCX, 7);
            xGX = 0.5 * (1.0 - Math.Sqrt(t / (t + c)));

            xNN = (1.0 + xGX) * cielab_1.CIE_a;
            xC1 = Math.Sqrt(xNN * xNN + CIE_1_b_squared);
            xH1 = CieLab2Hue(xNN, cielab_1.CIE_b);

            xNN = (1.0 + xGX) * cielab_2.CIE_a;
            xC2 = Math.Sqrt(xNN * xNN + CIE_2_b_squared);
            xH2 = CieLab2Hue(xNN, cielab_2.CIE_b);

            xDL = cielab_2.CIE_L - cielab_1.CIE_L;
            xDC = xC2 - xC1;
            if( (xC1 * xC2) == 0 )
            {
                xDH = 0.0;
            }
            else
            {
                t = xH2 - xH1;
                xNN = Math.Round(t, 12);
                if( Math.Abs(xNN) <= 180 )
                {
                    xDH = t;
                }
                else
                {
                    if( xNN > 180 )
                    {
                        xDH = t - 360.0;
                    }
                    else
                    {
                        xDH = t + 360.0;
                    }
                }
            }
            xDH = 2.0 * Math.Sqrt(xC1 * xC2) * Math.Sin(MathUtilities.deg2rad(xDH / 2.0));
            xLX = (cielab_1.CIE_L - cielab_2.CIE_L) / 2.0;
            xCY = (xC1 + xC2) / 2.0;
            t = xH1 + xH2;
            if( (xC1 *  xC2) == 0 )
            {
                xHX = t;
            }
            else
            {
                xNN = Math.Abs(Math.Round((xH1 - xH2), 12));
                if( xNN > 180 )
                {
                    if( t < 360.0 )
                    {
                        xHX = t + 360.0;
                    }
                    else
                    {
                        xHX = t - 360.0;
                    }
                }
                else
                {
                    xHX = t;
                }
                xHX /= 2;
            }
            xTX = 1.0 - 0.17 * Math.Cos(MathUtilities.deg2rad(xHX - 30.0)) + 
                        0.24 * Math.Cos(MathUtilities.deg2rad(2.0 * xHX)) + 
                        0.32 * Math.Cos(MathUtilities.deg2rad(3.0 * xHX + 6.0)) - 
                        0.20 * Math.Cos(MathUtilities.deg2rad(4.0 * xHX - 63.0));
            t = (xHX  - 275.0) / 25.0;
            xPH = 30.0 * Math.Exp(-(t * t));

            t = Math.Pow(xCY, 7);
            xRC = 2.0 * Math.Sqrt(t / (t + c));
            t = xLX - 50.0;
            xSL = 1.0 + (0.015 * (t * t)) / Math.Sqrt(20.0 + (t * t));
            xSC = 1.0 + 0.045 * xCY;
            xSH = 1.0 + 0.015 * xCY * xTX;
            xRT = -Math.Sin(MathUtilities.deg2rad(2.0 * xPH)) * xRC;

            xDL /= (weighting_factor_L * xSL);
            xDC /= (weighting_factor_C * xSC);
            xDH /= (weighting_factor_H * xSH);

            E00 = Math.Sqrt((xDL * xDL) + 
                            (xDC * xDC) + 
                            (xDH * xDH) + 
                            (xRT * xDC * xDH));

            return E00;
        }

        /// <summary>
        /// helper function to return the CIE-H° value
        /// </summary>
        private static double CieLab2Hue(double a, double b)
        {
            double bias = 0.0;

            if( (a >= 0.0) && (b == 0.0) )
                return 0.0;

            if( (a < 0.0) && (b == 0.0) )
                return 180.0;

            if( (a == 0.0) && (b > 0.0) )
                return 90.0;

            if( (a == 0.0) && (b < 0.0) )
                return 270.0;

            if( (a > 0.0) && (b > 0.0) )
                bias = 0.0;

            if( a < 0.0 )
                bias = 180.0;

            if( (a > 0.0) && (b < 0.0) )
                bias = 360.0;

            return MathUtilities.rad2deg(Math.Atan(b / a)) + bias;
        }
    }
    #endregion
}

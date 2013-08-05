using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods for Images
    /// </summary>
    public static class ImageMethods
    {
        /// <summary>
        /// Saves an Image to a Stream as a JPEG with an optional quality (defaults to 85%)
        /// </summary>
        /// <param name="image">Image to encode/save</param>
        /// <param name="stream">Stream to save to</param>
        /// <param name="quality">JPEG quality level</param>
        public static void ToJpeg(this Image image, Stream stream, byte quality=(byte)(0.85*255))
        {
            var ici_ep = getEncoder(ImageFormat.Jpeg, new[]{
                new EncoderParameter(Encoder.Quality, (long)(quality/255 * 100))
            });

            image.Save(stream, ici_ep.Item1, ici_ep.Item2);
        }

        /// <summary>
        /// Saves an Image to a Stream as a PNG
        /// </summary>
        /// <param name="image">Image to encode/save</param>
        /// <param name="stream">Stream to save to</param>
        public static void ToPng(this Image image, Stream stream)
        {
            var ici_ep = getEncoder(ImageFormat.Png, new EncoderParameter[]{});

            image.Save(stream, ici_ep.Item1, ici_ep.Item2);
        }

        private static Tuple<ImageCodecInfo, EncoderParameters> getEncoder(ImageFormat format, params EncoderParameter[] encParams)
        {
            ImageCodecInfo ici = ImageCodecInfo.GetImageEncoders().Where( v => v.FormatID == format.Guid ).FirstOrDefault();
            EncoderParameters ep = new EncoderParameters(encParams.Length);
            
            for(int i=0; i < encParams.Length; i++)
                ep.Param[i] = encParams[i];

            return Tuple.Create(ici, ep);
        }
    }
}

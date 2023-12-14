using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleBauen
{
    public class BlackWhite
    {
        public Mat blackImage;
        public Mat whiteImage;
        public Mat diffSource()
        {
            Mat difSourceGray = new Mat();
            Mat difSource = whiteImage - blackImage;
            Cv2.CvtColor(difSource, difSourceGray, ColorConversionCodes.BGR2GRAY);
            return difSourceGray;
        }
        public BlackWhite(Mat black, Mat white)
        {
            blackImage = black;
            whiteImage = white;
        }
        public static BlackWhite loadFromStream(Stream stream)
        {
            Mat black = Serializer.getMat(stream);
            Mat white = Serializer.getMat(stream);
            return new BlackWhite(black, white);
        }
        public Stream writeToStream()
        {
            MemoryStream ret = new MemoryStream();
            Serializer.Add(blackImage, ret);
            Serializer.Add(whiteImage, ret);
            return ret;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;
using Point = System.Windows.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;

#pragma warning disable
namespace PuzzleBauen
{
    public class PTDraw
    {
        public static void drawMe(Mat image, PuzzleTeil pt, double scale, OpenCvSharp.Point puzzlePoint, double puzzleDirection, OpenCvSharp.Point targetPoint, double targetDirection)
        {
            int quadratSize = pt.mask.Width;
            if (pt.mask.Height > quadratSize) quadratSize = pt.mask.Height;


            //Das Bild wird verschoben, damit eine Drehung nichts abschneidet. 
            Mat translatedImage = new Mat();
            Mat trans = Transformer.getTransmat(quadratSize * 3 - puzzlePoint.X, quadratSize * 3 - puzzlePoint.Y);            
            Cv2.WarpAffine(pt.getImage(), translatedImage, trans, new OpenCvSharp.Size(6 * quadratSize, 6 * quadratSize));
            //Der ursprüngliche "puzzlePoint" ist jetzt bei (quadratSize * 3/quadratSize * 3)


            //Das Bild wird um den aktuellen Puzzlepoint gedreht.
            Mat rotatedImage = new Mat();
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(new OpenCvSharp.Point(quadratSize * 3, quadratSize * 3), targetDirection - puzzleDirection, 1);                          
            Cv2.WarpAffine(translatedImage, rotatedImage, rotationMatrix, translatedImage.Size());
            //Der ursprüngliche "puzzlePoint" ist immer noch bei (quadratSize * 3/quadratSize * 3)

            //Jetzt wird scaliert um den Faktor scale
            Mat scaledImage = new Mat();
            Cv2.Resize(rotatedImage, scaledImage, new OpenCvSharp.Size(0, 0),scale,scale);
            //Der ursprüngliche "puzzlePoint" ist jetzt bei (quadratSize * 3*scale/quadratSize * 3*scale)


            //jetzt wird so verschoben, dass der puzzlepoint auf targetpoint zu liegen kommt
            Mat retImage = new Mat();
            Mat retranslateMatrix = Transformer.getTransmat(targetPoint.X - (float)(quadratSize * 3 * scale), (float)(targetPoint.Y - quadratSize * 3 * scale));
            //Mat retranslateMatrix = Transformer.getTransmat(targetPoint.X - (float)(quadratSize * 3*1), (float)(targetPoint.Y - quadratSize * 3*1));          
            Cv2.WarpAffine(scaledImage, retImage, retranslateMatrix, image.Size());

            //Aha
            retImage.CopyTo(image, retImage);
        }

        public static int NeighbourDrawDistance = 15;
        public static Mat drawWithNeighbours2(OrientiertesTeil ptCenter, OrientiertesTeil ptLeft, OrientiertesTeil ptDown, OrientiertesTeil ptRight, OrientiertesTeil ptUp)
        {
            Mat mat = new Mat(1000, 1000, MatType.CV_8UC3);
            if (ptCenter == null || ptCenter.PuzzleTeil == null) return mat;
            OpenCvSharp.Point targetpoint = new OpenCvSharp.Point(400, 400);

            OpenCvSharp.Point rotPoint = ptCenter.PuzzleTeil.getSeite(0 + ptCenter.Orientierung).contour.First();
            double wink = Transformer.getWinkel(ptCenter.PuzzleTeil.getSeite(0 + ptCenter.Orientierung).contour.First(), ptCenter.PuzzleTeil.getSeite(0 + ptCenter.Orientierung).contour.Last());
            PTDraw.drawMe(mat, ptCenter.PuzzleTeil, 1, rotPoint, wink, targetpoint, -90);

            OpenCvSharp.Point centerEck0 = ptCenter.PuzzleTeil.getSeite(0 + ptCenter.Orientierung).contour.First();
            centerEck0 = Transformer.rotate(centerEck0, rotPoint, wink + 90);
            centerEck0 += targetpoint;
            centerEck0 -= rotPoint;

            OpenCvSharp.Point centerEck1 = ptCenter.PuzzleTeil.getSeite(1 + ptCenter.Orientierung).contour.First();
            centerEck1 = Transformer.rotate(centerEck1, rotPoint, wink + 90);
            centerEck1 += targetpoint;
            centerEck1 -= rotPoint;

            OpenCvSharp.Point centerEck2 = ptCenter.PuzzleTeil.getSeite(2 + ptCenter.Orientierung).contour.First();
            centerEck2 = Transformer.rotate(centerEck2, rotPoint, wink + 90);
            centerEck2 += targetpoint;
            centerEck2 -= rotPoint;

            OpenCvSharp.Point centerEck3 = ptCenter.PuzzleTeil.getSeite(3 + ptCenter.Orientierung).contour.First();
            centerEck3 = Transformer.rotate(centerEck3, rotPoint, wink + 90);
            centerEck3 += targetpoint;
            centerEck3 -= rotPoint;


            Cv2.Circle(mat, centerEck0, 10, Scalar.Red, 3);
            Cv2.Circle(mat, centerEck1, 10, Scalar.Green, 3);
            Cv2.Circle(mat, centerEck2, 10, Scalar.Blue, 3);
            Cv2.Circle(mat, centerEck3, 10, Scalar.White, 3);

            //int NeighbourDrawDistance = 15;
            OpenCvSharp.Point leftEck = centerEck0 + new OpenCvSharp.Point(-NeighbourDrawDistance, 0);
            OpenCvSharp.Point downEck = centerEck1 + new OpenCvSharp.Point(0, NeighbourDrawDistance);
            OpenCvSharp.Point rightEck = centerEck2 + new OpenCvSharp.Point(NeighbourDrawDistance, 0);
            OpenCvSharp.Point topEck = centerEck0 + new OpenCvSharp.Point(0, -NeighbourDrawDistance);


            if (ptLeft != null && ptLeft.PuzzleTeil != null)
            {
                wink = Transformer.getWinkel(ptLeft.PuzzleTeil.getSeite(2 + ptLeft.Orientierung).contour.First(), ptLeft.PuzzleTeil.getSeite(2 + ptLeft.Orientierung).contour.Last());
                rotPoint = ptLeft.PuzzleTeil.getSeite(2 + ptLeft.Orientierung).contour.Last();
                double tarwi = Transformer.getWinkel(centerEck0, centerEck1);
                //PTDraw.drawMe(mat, ptLeft.PuzzleTeil, 1, rotPoint, wink, centerEck0, -tarwi);
                PTDraw.drawMe(mat, ptLeft.PuzzleTeil, 1, rotPoint, wink, leftEck, -tarwi);
            }
            if (ptDown != null && ptDown.PuzzleTeil != null)
            {
                wink = Transformer.getWinkel(ptDown.PuzzleTeil.getSeite(3 + ptDown.Orientierung).contour.First(), ptDown.PuzzleTeil.getSeite(3 + ptDown.Orientierung).contour.Last());
                rotPoint = ptDown.PuzzleTeil.getSeite(3 + ptDown.Orientierung).contour.Last();
                double tarwi = Transformer.getWinkel(centerEck2, centerEck1);
                //PTDraw.drawMe(mat, ptDown.PuzzleTeil, 1, rotPoint, wink, centerEck1, tarwi);
                PTDraw.drawMe(mat, ptDown.PuzzleTeil, 1, rotPoint, wink, downEck, tarwi);
            }
            if (ptRight != null && ptRight.PuzzleTeil != null)
            {
                wink = Transformer.getWinkel(ptRight.PuzzleTeil.getSeite(0 + ptRight.Orientierung).contour.First(), ptRight.PuzzleTeil.getSeite(0 + ptRight.Orientierung).contour.Last());
                rotPoint = ptRight.PuzzleTeil.getSeite(0 + ptRight.Orientierung).contour.Last();
                double tarwi = Transformer.getWinkel(centerEck3, centerEck2);
                //PTDraw.drawMe(mat, ptRight.PuzzleTeil, 1, rotPoint, wink, centerEck2, tarwi);
                PTDraw.drawMe(mat, ptRight.PuzzleTeil, 1, rotPoint, wink, rightEck, tarwi);
            }
            if (ptUp != null && ptUp.PuzzleTeil != null)
            {
                wink = Transformer.getWinkel(ptUp.PuzzleTeil.getSeite(1 + ptUp.Orientierung).contour.First(), ptUp.PuzzleTeil.getSeite(1 + ptUp.Orientierung).contour.Last());
                rotPoint = ptUp.PuzzleTeil.getSeite(1 + ptUp.Orientierung).contour.First();
                double tarwi = Transformer.getWinkel(centerEck0, centerEck3);
                //PTDraw.drawMe(mat, ptUp.PuzzleTeil, 1, rotPoint, wink, centerEck0, tarwi);
                PTDraw.drawMe(mat, ptUp.PuzzleTeil, 1, rotPoint, wink, topEck, tarwi);
            }
            return mat;
        }
    }
}

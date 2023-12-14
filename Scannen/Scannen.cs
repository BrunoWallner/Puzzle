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


using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;
using Point = System.Windows.Point;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;


using WIA;
using System.Drawing.Imaging;
using Xceed.Wpf.Toolkit;
using OpenCvSharp;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Xml.Linq;

using PuzzleBauen;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace ScannenServer
{
    public class Scannen : Window
    {
        public static BlackWhite scan(Dispatcher dis)
        {

            //if (index < 2)
            //{
            //    try
            //    {
            //        string fileb = "c:\\Users\\heigl\\Desktop\\WeltPuzzleBilder\\" + "Scan" + index.ToString() + "_Black.png";
            //        string filew = "c:\\Users\\heigl\\Desktop\\WeltPuzzleBilder\\" + "Scan" + index.ToString() + "_White.png";
            //        BlackWhite ret = new BlackWhite(Cv2.ImRead(fileb), Cv2.ImRead(filew));
            //        return ret;
            //    }
            //    catch
            //    {
            //        return null;
            //    }
            //}



            var deviceManager = new DeviceManager();
            DeviceInfo firstScannerAvailable = null;
            // Loop through the list of devices to choose the first available
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Skip the device if it's not a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }
                firstScannerAvailable = deviceManager.DeviceInfos[i];
                break;
            }
            if (firstScannerAvailable == null)
            {
                dis.Invoke(new Action(() =>
                {
                    MessageBox.Show("kein Scanner angeschlossen");
                }));
                return null;
            }
            var device = firstScannerAvailable.Connect();
            var scannerItem = device.Items[1];

            dis.Invoke(new Action(() =>
            {
                MessageBox.Show("Licht AUS!", "Licht AUS!", MessageBoxButton.OK);
            }));
          //  if (MessageBox.Show("Licht AUS!", "Licht AUS!", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return null;
            adjust(scannerItem);
            Mat black = scan(scannerItem);
            dis.Invoke(new Action(() =>
            {
                MessageBox.Show("Licht AUS!", "Licht AUS!", MessageBoxButton.OK);
            }));

            //if (MessageBox.Show("Licht EIN!", "Licht EIN!", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return null;
            Mat white = scan(scannerItem);
            if (white.Width != black.Width || white.Height != black.Height)
            {
                MessageBox.Show("unterschiedliche größe????");
                return null;
            }
            return new BlackWhite(black, white);
        }
        static Mat scan(Item scannerItem)
        {
            var path = @"temptemptemp";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            //FormatID.
            ImageFile imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatPNG);
            //ImageFile imageFile = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatTIFF);
            imageFile.SaveFile(path);
            return Cv2.ImRead(path);
        }
        static void adjust(IItem scannnerItem)
        {
            int colorMode = 1;
            int scanRes = 300;
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanRes);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanRes);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }
        private static string WIA_SCAN_COLOR_MODE = "6146";
        private static string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
        private static string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
        private static string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
        private static string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
        private static string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
        private static string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
        private static string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
        private static string WIA_SCAN_CONTRAST_PERCENTS = "6155";

        private void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {

            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }
        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
        }
    }
}

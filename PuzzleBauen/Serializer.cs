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
using System.ComponentModel;
using System;

#pragma warning disable 

namespace PuzzleBauen
{

    //public interface ISer<T> where T: class
    //{
    //    public static T get()
    //    {

    //    }
    //    public void load();
    //    //public T get();
    //}
    public static class Serializer
    {
        public static void Add(Int32 i, Stream fs)
        {
            fs.Write(BitConverter.GetBytes(i));            
        }
        public static Int32 getInt32(Stream fs)
        {
            byte[] buf = new byte[sizeof(Int32)];
            fs.Read(buf, 0, sizeof(int));
            return BitConverter.ToInt32(buf, 0);
        }
        public static void Add(OpenCvSharp.Rect rect, Stream fs)
        {
            Add((Int32)rect.Left, fs);
            Add((Int32)rect.Top, fs);
            Add((Int32)rect.Width, fs);
            Add((Int32)rect.Height, fs);
        }
        public static OpenCvSharp.Rect getRect(Stream fs)
        {
            Int32 left = getInt32(fs);
            Int32 top = getInt32(fs);
            Int32 width = getInt32(fs);
            Int32 height = getInt32(fs);
            return new OpenCvSharp.Rect(left, top, width, height);
        }
        //public static void Add<T>(T ar, Stream fs) where T:ISer<T>
        //{            

        //}
        //public static object get<T>(Stream fs) where T:ISer<T>
        //{
        //    T dd;
        //    return dd.get();
        //}

        public static void Add(Mat m, Stream fs)
        {
            if (m.Type() == MatType.CV_8UC3) fs.WriteByte(3);
            else fs.WriteByte(1);
            byte[] buf = m.ToBytes();
            Add((Int32)buf.Length, fs);
            fs.Write(buf);
        }
        public static Mat getMat(Stream fs)
        {
            byte typ = (byte)fs.ReadByte();
            Int32 siz = getInt32(fs);
            byte[] buf = new byte[siz];
            fs.Read(buf, 0, siz);
            if(typ == 1) return Mat.FromImageData(buf, ImreadModes.Grayscale);
            else return Mat.FromImageData(buf, ImreadModes.Color);
        }
    }

}

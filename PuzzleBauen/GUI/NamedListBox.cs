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

using OpenCvSharp;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;

#pragma warning disable 
namespace PuzzleBauen
{
    public class NamedListBox:StackPanel
    {
        public Label label = new Label();
        public ListBox list = new ListBox();
        public NamedListBox(String desc)
        {
            label.Content = desc;
            this.Children.Add(label);
            this.Children.Add(list);
        }
    }
}

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
    public class NamedIntUpDown : StackPanel
    {
        public event EventHandler ValueChanged;
        IntegerUpDown udInt;
        Label l = new Label();
        public NamedIntUpDown(String desc)
        {
            this.Orientation = System.Windows.Controls.Orientation.Horizontal;
            l.Content = desc;
            this.Children.Add(l);            
            udInt = new IntegerUpDown();
            this.Children.Add(udInt);
            udInt.ValueChanged += UDValueChanged;
            return;           
        }
       
        private void UDValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ValueChanged != null) ValueChanged(this, null);
        }
        public int val
        {
            get { return udInt.Value.GetValueOrDefault(); }
            set { udInt.Value = value as int?; }
        }

    }
}


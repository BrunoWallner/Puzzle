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
using MessageBox = System.Windows.MessageBox;

#pragma warning disable 
namespace PuzzleBauen
{

    public class AutoActionButton : Button
    {
        Action act;
        public AutoActionButton(string description, Action action)
        {
            this.act = action;
            this.Content = description;
            this.Click += click;
            this.FontSize = 14;
            this.Focusable = false;
        }
        //public void setEnable(bool enabled)
        //{
        //    this.IsEnabled = 
        //}
        void click(object s, EventArgs e)
        {
            act();
        }
    }
}

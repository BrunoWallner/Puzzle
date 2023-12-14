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

#pragma warning disable 
namespace PuzzleBauen
{
    public class EnumCombo<T> : StackPanel
    {
        public event EventHandler ValueChanged;
        public ComboBox combo = new ComboBox();
        public T Value
        {
            get
            {
                if (combo.SelectedItem != null)
                {
                    return (T)Enum.Parse(typeof(T), (String)combo.SelectedItem, true);
                }
                else
                {
                    return (T)Enum.Parse(typeof(T), (String)combo.SelectedItem, true);
                    //return null;
                }
            }
            set
            {
                combo.SelectedItem = Enum.GetName(typeof(T), value);
            }
        }
       // Type enumType;
        Label label = new Label();
        public EnumCombo(String desc)//, Type en)
        {
            this.Orientation = System.Windows.Controls.Orientation.Horizontal;
            label.Content = desc;
            this.Children.Add(label);
            this.Children.Add(combo);
            //enumType = en;            
            String[] s = Enum.GetNames(typeof(T));// en);
            foreach (String ss in s)
            {
                combo.Items.Add(ss);
            }
            combo.SelectionChanged += Combo_SelectionChanged;
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValueChanged != null) ValueChanged(this, null);
        }
    }
}

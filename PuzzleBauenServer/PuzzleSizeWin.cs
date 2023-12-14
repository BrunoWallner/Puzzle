using PuzzleBauen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Grid = System.Windows.Controls.Grid;
using System.Diagnostics.Eventing.Reader;

namespace PuzzleBauenServer
{
    class PuzzleSizeWin : Window
    {
        public static OpenCvSharp.Point Show()
        {
            while (true)
            {
                PuzzleSizeWin ps = new PuzzleSizeWin();
                if (ps.ShowDialog() == true)
                {
                    return ps.size;
                }
            }
        }

        OpenCvSharp.Point size;
        Grid g = new Grid();
        TextBox breiteText = new TextBox();
        TextBox höheText = new TextBox();

        PuzzleSizeWin()
        {
            this.Title = "Puzzlegröße";
            this.Content = g;
            Label breiteLabel = new Label();
            breiteLabel.Content = "Breite";
            Label höheLabel = new Label();
            höheLabel.Content = "Höhe";

            g.Children.Add(breiteText);
            g.Children.Add(höheText);
            g.Children.Add(breiteLabel);
            g.Children.Add(höheLabel);
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetColumn(breiteLabel, 0);
            Grid.SetRow(breiteLabel, 0);

            Grid.SetColumn(höheLabel, 0);
            Grid.SetRow(höheLabel, 1);

            Grid.SetColumn(breiteText, 1);
            Grid.SetRow(breiteText, 0);

            Grid.SetColumn(höheText, 1);
            Grid.SetRow(höheText, 1);



            AutoActionButton okB = new AutoActionButton("OK", ok);
            //AutoActionButton nokB = new AutoActionButton("Nein", notOk);
            g.Children.Add(okB);
            //g.Children.Add(nokB);

            Grid.SetColumn(okB, 0);
            Grid.SetRow(okB, 2);
            Grid.SetColumnSpan(okB, 2);

            this.SizeToContent = SizeToContent.Height;
            this.Width = 400;

            this.Closing += PuzzleSizeWin_Closing;
            
        }

        private void PuzzleSizeWin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            int höhe = -1;
            int breite = -1;
            if (int.TryParse(höheText.Text, out höhe))
            {
                size.Y = höhe;
            }
            else
            {
                this.DialogResult = false;
            }
            if (int.TryParse(breiteText.Text, out breite))
            {
                size.X = breite;
            }
            else
            {
                this.DialogResult = false;
            }

        }

        void ok()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}

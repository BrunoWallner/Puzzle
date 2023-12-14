//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;

//using PuzzleBauen;
//namespace PuzzleBauenServer
//{
//    internal class ClientSelection:Window
//    {
//        public static PuzzleBauClient selectClient(List<PuzzleBauClient> kunden)
//        {
//            ClientSelection cs = new ClientSelection(kunden);
//            cs.ShowDialog();
//            return cs.selectedClient;

//        }
//        PuzzleBauClient selectedClient = null;
//        StackPanel sp = new StackPanel();
//        ListBox lb = new ListBox();
//        public ClientSelection(List<PuzzleBauClient> kunden)
//        {            
//            this.Content = sp;
//            this.Title = "Baumeister wählen";
//            foreach (PuzzleBauClient client in kunden)
//            {
//                lb.Items.Add(client);
//            }
//            lb.MouseDoubleClick += Lb_MouseDoubleClick;
//            sp.Children.Add(lb);
//            sp.Children.Add(new AutoActionButton("OK", ok));

//            this.SizeToContent = SizeToContent.WidthAndHeight;
//        }
//        void ok()
//        {
//            selectedClient = (lb.SelectedItem as PuzzleBauClient);
//            this.Close();
//        }

//        private void Lb_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
            
//        }
//    }
//}

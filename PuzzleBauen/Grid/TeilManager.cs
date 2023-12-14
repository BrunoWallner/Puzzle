using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PuzzleBauen
{
    public class TeilManager
    {
        public event EventHandler teileGeändertEvent;
        void teileGeändert()
        {
            if (teileGeändertEvent == null) return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                teileGeändertEvent(this, null);
            }));
            
        }
        static TeilManager instance = null;
        public static TeilManager getInstance()
        {
            if (instance == null)
            {
                instance = new TeilManager();
            }
            return instance;
        }
        public void reload()
        {
            alleTeile = new List<int>(PuzzleScanCollection.getInstance().getAllIDs());
            verfügbareTeiles = new List<int>(alleTeile);
            teileGeändert();
        }
        TeilManager()
        {
            alleTeile = new List<int>(PuzzleScanCollection.getInstance().getAllIDs());
            verfügbareTeiles = new List<int>(alleTeile);
            teileGeändert();
        }        
        List<int> alleTeile;
        List<int> verfügbareTeiles;
        public void teilVerbaut(int id)
        {
            for(int i = 0; i<verfügbareTeiles.Count; i++)
            {
                if (verfügbareTeiles[i] == id)              
                {                    
                    verfügbareTeiles.RemoveAt(i);
                    break;
                }
            }
            teileGeändert();
        }
        public void insert(int id)
        {
            verfügbareTeiles.Add(id);
            teileGeändert();
        }
        public void teileVerbaut(List<int> ids)
        {
            foreach (int id in ids)
            {
                verfügbareTeiles.Remove(id);
            }
            teileGeändert();
        }
        public List<int> getVerfügbareTeile()
        {
            return verfügbareTeiles;
        }
        public List<int> getVerbauteTeile()
        {
            List<int> ret = new List<int>();
            for(int i = 0; i<alleTeile.Count; i++)
            {
                if (!verfügbareTeiles.Contains(alleTeile[i]))
                {
                    ret.Add(alleTeile[i]);
                }
            }
            return ret;
        }
    }
}

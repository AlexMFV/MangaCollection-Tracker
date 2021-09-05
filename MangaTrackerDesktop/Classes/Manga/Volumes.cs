using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MangaTrackerDesktop
{
    public class Volumes : CollectionBase
    {
        public void Add(Volume vol)
        {
            List.Add(vol);
        }

        public void Remove(Volume vol)
        {
            List.Remove(vol);
        }

        public List<Volume> ToList()
        {
            List<Volume> clone = new List<Volume>();

            for (int i = 0; i < List.Count; i++)
                clone.Add(List[i] as Volume);

            return clone;
        }

        public bool Contains(int _id)
        {
            foreach(Volume vol in List)
                if (vol.Id == _id)
                    return true;
            return false;
        }

        public Volumes ToObject(List<Volume> list)
        {
            Volumes vols = new Volumes();
            for (int i = 0; i < list.Count; i++)
            {
                Volume vol = (Volume)list[i];
                vols.Add(vol);
            }
            return vols;
        }

        public void Update(Volume vol)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (((Volume)List[i]).Id == vol.Id)
                {
                    List[i] = vol;
                    return;
                }
            }
        }

        public Volume GetByID(int _id)
        {
            return ((Volumes)List).ToList().FirstOrDefault(x => x.Id == _id);
        }

        public Volume this[int index]
        {
            get { return (Volume)List[index]; }
            set { List[index] = value; }
        }
    }
}

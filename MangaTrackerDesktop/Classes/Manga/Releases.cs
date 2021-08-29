using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public class Releases : CollectionBase
    {
        public void Add(Release manga)
        {
            List.Add(manga);
        }

        public void Remove(Release manga)
        {
            List.Remove(manga);
        }

        public List<Release> ToList()
        {
            List<Release> clone = new List<Release>();

            for (int i = 0; i < List.Count; i++)
                clone.Add(List[i] as Release);

            return clone;
        }

        public Releases ToObject(List<Release> list)
        {
            Releases mangas = new Releases();
            for (int i = 0; i < list.Count; i++)
            {
                Release manga = (Release)list[i];
                mangas.Add(manga);
            }
            return mangas;
        }

        public Release GetByID(int _id)
        {
            return ((Releases)List).ToList().First(x => x.Id == _id);
        }

        public List<Releases> Partition(int number)
        {
            List<Releases> toReturn = new List<Releases>();
            for (int i = 0; i < this.Count; i += number)
            {
                Releases manga = new Releases();
                for (int j = 0; j < number; j++)
                {
                    if (i + j >= this.Count)
                        break;

                    manga.Add(this[i + j]);
                }
                toReturn.Add(manga);
            }
            return toReturn;
        }

        public Release this[int index]
        {
            get { return (Release)List[index]; }
            set { List[index] = value; }
        }
    }
}

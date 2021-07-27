using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public class Mangas : CollectionBase
    {
        public void Add(Manga manga)
        {
            List.Add(manga);
        }

        public void Remove(Manga manga)
        {
            List.Remove(manga);
        }

        public List<Manga> ToList()
        {
            List<Manga> clone = new List<Manga>();

            for (int i = 0; i < List.Count; i++)
                clone.Add(List[i] as Manga);

            return clone;
        }

        public Mangas ToObject(List<Manga> list)
        {
            Mangas mangas = new Mangas();
            for(int i = 0; i < list.Count; i++)
            {
                Manga manga = (Manga)list[i];
                mangas.Add(manga);
            }
            return mangas;
        }

        public List<Mangas> Partition(int number)
        {
            List<Mangas> toReturn = new List<Mangas>();
            for (int i = 0; i < this.Count; i += number)
            {
                Mangas manga = new Mangas();
                for (int j = 0; j < number; j++)
                {
                    if (i + j >= this.Count)
                        break;

                    manga.Add(this[i+j]);
                }
                toReturn.Add(manga);
            }
            return toReturn;
        }

        //public Manga GetNoteFromGUID(Guid id)
        //{
        //    return List.Cast<Manga>().First(note => note.Note_ID == id);
        //}

        //public int GetNoteIndex(Guid id)
        //{
        //    return List.IndexOf(List.Cast<Manga>().First(note => note.Note_ID == id));
        //}

        public Manga this[int index]
        {
            get { return (Manga)List[index]; }
            set { List[index] = value; }
        }
    }
}

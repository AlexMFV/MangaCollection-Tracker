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
                clone.Add((List[i] as Manga));

            return clone;
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

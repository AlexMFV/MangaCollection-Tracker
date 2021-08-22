using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MangaTrackerDesktop
{
    public class FavMangas : CollectionBase
    {
        public void Add(FavManga manga)
        {
            List.Add(manga);
        }

        public void Remove(FavManga manga)
        {
            List.Remove(manga);
        }

        public List<FavManga> ToList()
        {
            List<FavManga> clone = new List<FavManga>();

            for (int i = 0; i < List.Count; i++)
                clone.Add(List[i] as FavManga);

            return clone;
        }

        public FavMangas ToObject(List<FavManga> list)
        {
            FavMangas mangas = new FavMangas();
            for (int i = 0; i < list.Count; i++)
            {
                FavManga manga = (FavManga)list[i];
                mangas.Add(manga);
            }
            return mangas;
        }

        public List<FavMangas> Partition(int number)
        {
            List<FavMangas> toReturn = new List<FavMangas>();
            for (int i = 0; i < this.Count; i += number)
            {
                FavMangas manga = new FavMangas();
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

        //public Manga GetNoteFromGUID(Guid id)
        //{
        //    return List.Cast<Manga>().First(note => note.Note_ID == id);
        //}

        //public int GetNoteIndex(Guid id)
        //{
        //    return List.IndexOf(List.Cast<Manga>().First(note => note.Note_ID == id));
        //}

        public FavManga this[int index]
        {
            get { return (FavManga)List[index]; }
            set { List[index] = value; }
        }
    }
}

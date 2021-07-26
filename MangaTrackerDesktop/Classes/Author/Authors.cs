using System.Collections;
using System.Collections.Generic;

namespace MangaTrackerDesktop
{
    class Authors : CollectionBase
    {
        public void Add(Author author)
        {
            List.Add(author);
        }

        public void Remove(Author author)
        {
            List.Remove(author);
        }

        public List<Author> ToList()
        {
            List<Author> clone = new List<Author>();

            for (int i = 0; i < List.Count; i++)
                clone.Add((List[i] as Author));

            return clone;
        }

        //public int GetNoteIndex(Guid id)
        //{
        //    return List.IndexOf(List.Cast<Author>().First(note => note.Note_ID == id));
        //}

        public Author this[int index]
        {
            get { return (Author)List[index]; }
            set { List[index] = value; }
        }
    }
}

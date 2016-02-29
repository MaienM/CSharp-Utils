using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSharpUtils.Utils
{
    class SizeLimitedCollection<T> : Collection<T>
    {
        public int Limit { get; set; }

        public SizeLimitedCollection(int limit) : base()
        {
            Limit = limit;
            Fit();
        }
        public SizeLimitedCollection(int limit, IList<T> list) : base(list)
        {
            Limit = limit;
            Fit();
        }

        public new void Add(T item)
        {
            base.Add(item);
            Fit();
        }

        /// <summary>
        /// Make the collection fit.
        /// </summary>
        private void Fit()
        {
            while (Count > Limit)
            {
                RemoveAt(0);
            }
        }
    }
}

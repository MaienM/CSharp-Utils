using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSharpUtils.Utils
{
    class SizeLimitedCollection<T> : Collection<T>
    {
        public int Limit { get; set; }

        public SizeLimitedCollection(int limit, IList<T> list) : base(list)
        {
            this.Limit = limit;

            // Make sure the current list fits in the limit.
            while (this.Count > this.Limit)
            {
                this.RemoveAt(0);
            }
        }
    }
}

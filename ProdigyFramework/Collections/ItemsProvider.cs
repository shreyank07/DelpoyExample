using Prodigy.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prodigy.Framework.Collections
{
    public class ItemsProvider<T> : IItemsProvider<T>
    {
        Action<IList<T>, int, int> updateRange;

        public ItemsProvider(Action<IList<T>, int, int> updateRange)
        {
            this.updateRange = updateRange;
        }

        public void UpdateRange(IList<T> items, int startIndex, int count)
        {
            updateRange(items, startIndex, count);
        }
    }
}

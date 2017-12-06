using System.Collections.Generic;
using Android.Widget;
using Android.Support.V7.Widget;

namespace GLNLabelPrint
{
    public class ContactListAdapter<T>
    {
        readonly List<T> mItems;
        RecyclerView.Adapter mAdapter;
        public ContactListAdapter()
        {
            mItems = new List<T>();
        }
        public RecyclerView.Adapter Adapter
        {
            get
            {
                return mAdapter;
            }
            set
            {
                mAdapter = value;
            }
        }
        public void Add(T item)
        {
            mItems.Add(item);
            if (Adapter != null)
            {
                Adapter.NotifyItemInserted(0);
            }
        }
        public void Remove(int position)
        {
            mItems.RemoveAt(position);
            if (Adapter != null)
            {
                Adapter.NotifyItemRemoved(0);
            }
        }
        public T this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return mItems.Count;
            }
        }
    }
}
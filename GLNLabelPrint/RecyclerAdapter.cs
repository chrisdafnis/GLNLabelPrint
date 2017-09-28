using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace GLNLabelPrint
{
    public class RecyclerAdapter : RecyclerView.Adapter
    {
        private ContactListAdapter<GLNListRow> Mitems;
        Context context;
        public RecyclerAdapter(ContactListAdapter<GLNListRow> Mitems)
        {
            this.Mitems = Mitems;
            NotifyDataSetChanged();
        }
        public class MyView : RecyclerView.ViewHolder
        {
            public View mainview
            {
                get;
                set;
            }
            public TextView mtxtcontactname
            {
                get;
                set;
            }
            public TextView mtxtcontactnumber
            {
                get;
                set;
            }
            public MyView(View view) : base(view)
            {
                mainview = view;
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ListRow, parent, false);
            CheckedTextView txtcontactname = listitem.FindViewById<CheckedTextView>(Resource.Id.simpleCheckedListItem);
            MyView view = new MyView(listitem)
            {
                mtxtcontactname = txtcontactname
                //,
                //mtxtcontactnumber = txtnumber
            };
            return view;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;
            myholder.mtxtcontactname.Text = Mitems[position].ContactName;
            myholder.mtxtcontactnumber.Text = Mitems[position].Number;
        }
        public override int ItemCount
        {
            get
            {
                return Mitems.Count;
            }
        }
    }
}
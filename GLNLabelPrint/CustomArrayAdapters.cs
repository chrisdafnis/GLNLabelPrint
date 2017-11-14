
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Android.Runtime;
using Android.OS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

namespace DakotaIntegratedSolutions
{
    class ViewHolder : Java.Lang.Object
    {
        public TextView Text { get; set; }
        public bool Selected { get; set; }
        public bool Printed { get; set; }
        public IGLNLocation Location { get; set; }
    }

    class ListCustomArrayAdapter : ArrayAdapter
    {
        private IList objectList;
        private int layoutResourceId;
        private int selectedIndex;
        private Dictionary<int, bool> printedItems;
        private int[] colors = new int[] { Color.LightBlue, Color.White };

        public ListCustomArrayAdapter(Context context, int layout, IList objects) : base(context, layout, objects)
        {
            objectList = objects;
            layoutResourceId = layout;
            printedItems = new Dictionary<int, bool>();
        }

        public override int Count
        {
            get
            {
                return base.Count;
            }
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
            NotifyDataSetChanged();
        }

        public void SetPrintedIndex(int index)
        {
            try
            {
                if (printedItems.ContainsKey(index))
                {
                    printedItems.Remove(index);
                }
                printedItems.Add(index, true);
                NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
                IFileUtil fileUtility = new FileUtilImplementation();
                //call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                //fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }
        }

        public override int GetItemViewType(int position)
        {
            return base.GetItemViewType(position);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return base.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return base.GetItemId(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;
            convertView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(GLNLabelPrint.Resource.Layout.ListRow, null);

            holder = new ViewHolder();
            var inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            int colorPos = position % colors.Length;
            Color color = new Color(colors[colorPos]);
            convertView.SetBackgroundColor(color);

            TextView text = convertView.FindViewById<TextView>(GLNLabelPrint.Resource.Id.ListItemRowText);
            text.Text = objectList[position].ToString();
            holder.Text = text;
            holder.Selected = false;
            holder.Printed = false;
            holder.Location = (IGLNLocation)objectList[position];
            convertView.Tag = holder;

            if (selectedIndex != -1 && position == selectedIndex)
            {
                HighlightCurrentRow(holder.Text);
            }
            else
            {
                UnhighlightCurrentRow(holder.Text);
                HighlightPrintedRow(holder.Text, position);
            }

            return convertView;
        }

        public void HighlightCurrentRow(View rowView)
        {
            rowView.SetBackgroundColor(Color.DarkGray);
            TextView textView = (TextView)rowView.FindViewById(GLNLabelPrint.Resource.Id.ListItemRowText);
            if (textView != null)
                textView.SetTextColor(Color.Yellow);
        }

        public void UnhighlightCurrentRow(View rowView)
        {
            rowView.SetBackgroundColor(Color.Transparent);
            TextView textView = (TextView)rowView.FindViewById(GLNLabelPrint.Resource.Id.ListItemRowText);
            if (textView != null)
                textView.SetTextColor(Color.Black);
        }

        public void HighlightPrintedRow(View rowView, int position)
        {
            if (printedItems.ContainsKey(position))
            {
                rowView.SetBackgroundColor(Color.ParseColor("#0A64A2"));
                TextView textView = (TextView)rowView.FindViewById(GLNLabelPrint.Resource.Id.ListItemRowText);
                if (textView != null)
                    textView.SetTextColor(Color.White);
            }
        }
    }

    class ListAlternateRowAdapter : ArrayAdapter
    {
        private int[] colors = new int[] { Color.LightBlue, Color.White };
        private IList objectList;

        public ListAlternateRowAdapter(Context context, int layout, System.Collections.IList objects) : base(context, layout, objects)
        {
            objectList = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;
            convertView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(GLNLabelPrint.Resource.Layout.ListRow, null);
            holder = new ViewHolder();
            var inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            int colorPos = position % colors.Length;
            Color color = new Color(colors[colorPos]);
            convertView.SetBackgroundColor(color);

            TextView textView = convertView.FindViewById<TextView>(GLNLabelPrint.Resource.Id.ListItemRowText);
            textView.Text = objectList[position].ToString();

            if (textView != null)
                textView.SetTextColor(Color.Black);

            return convertView;
        }
    }

    class CheckListCustomArrayAdapter : ArrayAdapter
    {
        private IList objectList;
        private int layoutResourceId;
        private int selectedIndex;
        private Dictionary<int, bool> printedItems;
        private int[] colors = new int[] { Color.LightBlue, Color.White };
        bool[] checkBoxState;
        private IList<IGLNLocation> locationList;
        //Button printButton = null;

        public CheckListCustomArrayAdapter(Context context, int layout, IList objects) : base(context, layout, objects)
        {
            objectList = objects;
            layoutResourceId = layout;
            printedItems = new Dictionary<int, bool>();
            checkBoxState = new bool[objectList.Count];
        }

        public override int Count
        {
            get
            {
                return base.Count;
            }
        }

        public int GetSelectedIndex()
        {
            return selectedIndex;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = index;
            NotifyDataSetChanged();
        }

        public bool GetChecked(View view, int index)
        {
            ///View convertView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(GLNLabelPrint.Resource.Layout.ListRowWithCheck, null);
            //CheckBox checkBox = convertView.FindViewById<CheckBox>(GLNLabelPrint.Resource.Id.PrintCheckbox);

            var simpleCheckedTextView = view as CheckedTextView;
            Boolean checkedValue = simpleCheckedTextView.Checked; // check current state of CheckedTextView
            ////checkBoxState[index] = 
            return checkBoxState[index];
        }

        public void SetPrinted(View view, int position)
        {
            //var checkView = view as CheckedTextView;
            //OnViewTouch(checkView, position);
        }

        public void SetChecked(int index, bool check)
        {
            checkBoxState[index] = check;
            ((IGLNLocation)objectList[index]).ToPrint = check;
        }

        public void SetPrintedIndex(int index)
        {
            try
            {
                if (printedItems.ContainsKey(index))
                {
                    printedItems.Remove(index);
                }
                printedItems.Add(index, true);
                ((IGLNLocation)objectList[index]).ToPrint = false;
                ((IGLNLocation)objectList[index]).Selected = false;

                locationList[index].ToPrint = false;
            }
            catch (Exception ex)
            {
                IFileUtil fileUtility = new FileUtilImplementation();
                //call LogFile method and pass argument as Exception message, event name, control name, error line number, current form name
                //fileUtility.LogFile(ex.Message, ex.ToString(), MethodBase.GetCurrentMethod().Name, ExceptionHelper.LineNumber(ex), Class.SimpleName);
            }
        }

        internal void SetRowPrinted(CheckedTextView view, int pos)
        {
            view.SetBackgroundColor(Color.ParseColor("#0A64A2"));
            view.SetTextColor(Color.White);
            view.Invalidate();
        }

        public override int GetItemViewType(int position)
        {
            return base.GetItemViewType(position);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return base.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return base.GetItemId(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView as CheckedTextView;
            if (view == null)
            {
                view = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(global::Android.Resource.Layout.SimpleListItemChecked, null) as CheckedTextView;
                view.Click += delegate
                {
                    OnViewClick(view, position);
                };
            }
            view.SetText(((IGLNLocation)objectList[position]).Code + ", " + ((IGLNLocation)objectList[position]).GLN, TextView.BufferType.Normal);
            view.SetTextColor(Color.Black);
            view.Checked = ((IGLNLocation)objectList[position]).Selected;

            var inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            int colorPos = position % colors.Length;
            Color color = new Color(colors[colorPos]);
            view.SetBackgroundColor(color);

            if (((IGLNLocation)objectList[position]).Printed)
            {
                view.SetBackgroundColor(Color.ParseColor("#0A64A2"));
                view.SetTextColor(Color.White);
            }

            return view;
        }

        public bool CheckForItemsToPrint()
        {
            bool print = false;
            foreach (IGLNLocation loc in objectList)
            {
                if (loc.ToPrint)
                {
                    print = true;
                    break;
                }
            }
            return print;
        }

        private void OnViewClick(object sender, int position)
        {
            var view = sender as CheckedTextView;
            view.Checked = !view.Checked;
            
            for (int i=0; i<objectList.Count; i++)
            {
                IGLNLocation loc = (IGLNLocation)objectList[i];
                if (view.Text.Contains(loc.GLN))
                {
                    position = i;
                    break;
                }
            }
            //locationList.
            //objectList.IndexOf.IndexOf(text);
            checkBoxState[position] = view.Checked;
            ((IGLNLocation)objectList[position]).Selected = view.Checked;
            ((IGLNLocation)objectList[position]).ToPrint = view.Checked;
        }

        public void HighlightCurrentRow(View rowView)
        {
            //rowView.SetBackgroundColor(Color.DarkGray);
            //TextView textView = (TextView)rowView.FindViewById(GLNLabelPrint.Resource.Id);
            //if (textView != null)
            //    textView.SetTextColor(Color.Yellow);
        }

        public void UnhighlightCurrentRow(View rowView)
        {
            //rowView.SetBackgroundColor(Color.Transparent);
            //TextView textView = (TextView)rowView.FindViewById(GLNLabelPrint.Resource.Id.ListWithCheckItemRowText);
            //if (textView != null)
            //    textView.SetTextColor(Color.Black);
        }

        public void HighlightAllPrintedRows(/*int position,*/ View convertView/*, ViewGroup parent*/)
        {
            foreach (object obj in objectList)
            {
                if (((IGLNLocation)obj).Printed)
                {
                    CheckedTextView textView = convertView as CheckedTextView;
                    if (textView == null)
                    {
                        textView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(global::Android.Resource.Layout.SimpleListItemChecked, null) as CheckedTextView;
                    }
                    textView.SetTextColor(Color.White);
                    textView.SetBackgroundColor(Color.ParseColor("#0A64A2"));
                }
            }
        }

        public void HighlightPrintedRow(View rowView, int position)
        {
            if (printedItems.ContainsKey(position))
            {
                rowView.SetBackgroundColor(Color.ParseColor("#0A64A2"));
                CheckedTextView textView = rowView as CheckedTextView;
                if (textView == null)
                {
                    textView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(global::Android.Resource.Layout.SimpleListItemChecked, null) as CheckedTextView;
                }
                if (textView != null)
                    textView.SetTextColor(Color.White);
            }
        }

        internal void RefreshList(ObservableCollection<IGLNLocation> locationList)
        {
            this.objectList.Clear();
            foreach (IGLNLocation loc in locationList)
            {
                this.objectList.Add(loc);
            }
            NotifyDataSetChanged();
        }
    }

    class CheckListAlternateRowAdapter : ArrayAdapter
    {
        private int[] colors = new int[] { Color.LightBlue, Color.White };
        private IList objectList;

        public CheckListAlternateRowAdapter(Context context, int layout, System.Collections.IList objects) : base(context, layout, objects)
        {
            objectList = objects;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;
            convertView = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService)).Inflate(Android.Resource.Layout.SimpleListItemChecked, null);
            holder = new ViewHolder();
            var inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            int colorPos = position % colors.Length;
            Color color = new Color(colors[colorPos]);
            convertView.SetBackgroundColor(color);

            //CheckBox checkBox = convertView.FindViewById<CheckBox>(GLNLabelPrint.Resource.Id.PrintCheckbox);
            CheckedTextView checkTextView = convertView as CheckedTextView;//convertView.FindViewById<CheckedTextView>(GLNLabelPrint.Resource.Id.simpleCheckedListItem) as CheckedTextView;
            checkTextView.Text = objectList[position].ToString();
            checkTextView.Click += (sender, args) =>
            {
                var pos = ((View)sender).Tag;
                //do the work
                //checkBox.Checked = !checkBox.Checked;
            };

            if (checkTextView != null)
                checkTextView.SetTextColor(Color.Black);

            return convertView;
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Widget.AdapterView;

namespace Web_DevTools.utils
{
    public class DynamicListView
    {
        public LinearLayout BaseView { get; private set; }
        public Context Context { get => BaseView.Context; }
        public DynamicListView(LinearLayout baseView)
        {
            this.BaseView = baseView;
        }

        public void Clear()
        {
            BaseView.RemoveAllViews();
        }

        private IAdapter _adapter = null; // IListAdapter
        public IAdapter Adapter
        {
            get => _adapter;
            set
            {
                _adapter = value;

                Clear();

                for (int i = 0; i < Adapter.Count; i++)
                {
                    LinearLayout parent = new LinearLayout(Context);
                    if (IsSelectable)
                        parent.Background = Context.Resources.GetDrawable(Resource.Drawable.ripple_background);
                    View view = Adapter.GetView(i, null, parent);
                    parent.AddView(view);

                    int index = i;

                    BaseView.AddView(parent);
                    parent.Click += delegate
                    {
                        if (IsSelectable)
                            ItemClick?.Invoke(null, new ItemClickEventArgs(null, view, index, Adapter.GetItemId(index)));
                    };

                }
            }
        }
        public bool IsSelectable { get; set; } = false;
        public event EventHandler<ItemClickEventArgs> ItemClick;
    }
}
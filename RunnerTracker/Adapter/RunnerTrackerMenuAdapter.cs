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
using RunnerTracker.Core.Model;

namespace RunnerTracker.Adapter
{
    public class RunnerTrackerMenuAdapter : BaseAdapter<RunnerTrackerMenu>
    {
        List<RunnerTrackerMenu> menuItems;
        Activity context;

        public RunnerTrackerMenuAdapter(Activity context, List<RunnerTrackerMenu> menuItems) : base()
        {
            this.menuItems = menuItems;
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override RunnerTrackerMenu this[int position]
        {
            get
            {
                return menuItems[position];
            }
        }

        public override int Count
        {
            get
            {
                return menuItems.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = menuItems[position];

            if (convertView == null)
            {
                convertView = context.LayoutInflater.Inflate(Resource.Layout.RowStyleView, null);
            }

            convertView.FindViewById<TextView>(Resource.Id.IdListView).Text = item.Index.ToString();
            convertView.FindViewById<TextView>(Resource.Id.DateTextView).Text = item.RecordDate.ToString("MM/dd/yyyy");
            convertView.FindViewById<TextView>(Resource.Id.DetailTextView).Text = item.Details;

            return convertView;
        }
    }
}
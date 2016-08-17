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

namespace BleSample.Droid
{
    public class SimpleListItem2_Adapter : ArrayAdapter<BleDeviceData>
    {
        Activity context;

        public SimpleListItem2_Adapter(Activity context, IList<BleDeviceData> objects)
            : base(context, Android.Resource.Id.Text1, objects)
        {
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            var item = GetItem(position);

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Name;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Id;

            return view;
        }
    }
}
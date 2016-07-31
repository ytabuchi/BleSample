using Android.App;
using Android.Widget;
using Android.OS;

namespace BleSample.Droid
{
    [Activity(Label = "BleSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += (sender, e) =>
            {
                
            };
        }
    }
}



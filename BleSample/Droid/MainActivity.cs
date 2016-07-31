using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;

namespace BleSample.Droid
{
    [Activity(Label = "BleSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        // UUID of LBT-VRU01
        const string uuid = "10B568BD-5CED-10E6-D467-EB3FBA189A95";

        BluetoothAdapter mBluetoothAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Use this check to determine whether BLE is supported on the device.  Then you can
            // selectively disable BLE-related features.
            if (!PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureBluetoothLe))
            {
                Toast.MakeText(this, Resource.String.ble_not_supported, ToastLength.Short).Show();
                Finish();
            }

            // BluetoothManager
            BluetoothManager manager = (BluetoothManager)GetSystemService(Context.BluetoothService); // using Android.Content;
            mBluetoothAdapter = manager.Adapter;

            var button = FindViewById<Button>(Resource.Id.myButton);
            button.Click += (sender, e) =>
            {
                
            };
        }
    }
}



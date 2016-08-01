using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Bluetooth.LE;

namespace BleSample.Droid
{
    [Activity(Label = "BleSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        // UUID of LBT-VRU01
        const string uuid = "10B568BD-5CED-10E6-D467-EB3FBA189A95";

        BluetoothLeScanner scanner;

        // Stops scanning after 10 seconds.
        static readonly long SCAN_PERIOD = 10000;
        // Use handler to timeout.
        Handler mHandler;
        bool mScanning;

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
            BluetoothAdapter adapter = manager.Adapter;
            scanner = adapter.BluetoothLeScanner;

            // Checks if Bluetooth is supported on the device.
            if (adapter == null)
            {
                Toast.MakeText(this, Resource.String.error_bluetooth_not_supported, ToastLength.Short).Show();
                Finish();
                return;
            }

            var startButton = FindViewById<Button>(Resource.Id.startButton);
            startButton.Click += (sender, e) =>
            {
                
            };

            var stopButton = FindViewById<Button>(Resource.Id.stopButton);
            stopButton.Click += (sender, e) => 
            {
                
            };

        }

        void ScanLeDevice(bool enable)
        {
            // Start Scan
            if (enable)
            {

            }
            else
            {

            }
        }
    }
}



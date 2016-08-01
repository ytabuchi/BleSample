using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Runtime;

namespace BleSample.Droid
{
    [Activity(Label = "BleSample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        // UUID of LBT-VRU01
        const string uuid = "10B568BD-5CED-10E6-D467-EB3FBA189A95";

        BluetoothLeScanner scanner;

        // Use handler to timeout.
        //Handler mHandler;
        //bool mScanning;

        BleScanCallback scanCallback = new BleScanCallback();

        string deviceAddress;
        TextView deviceLabel;

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

            deviceLabel = FindViewById<TextView>(Resource.Id.deviceLabel);

            var startButton = FindViewById<Button>(Resource.Id.startButton);
            startButton.Click += (sender, e) =>
            {
                ScanLeDevice(true);
            };

            var stopButton = FindViewById<Button>(Resource.Id.stopButton);
            stopButton.Click += (sender, e) =>
            {
                ScanLeDevice(false);
            };

            var connectButton = FindViewById<Button>(Resource.Id.connectButton);
            connectButton.Click += (sender, e) =>
            {

            };

        }

        void ScanLeDevice(bool enable)
        {
            // Start and stop Scan
            if (enable)
            {
                // Scan filter and scan mode setting.
                // http://blog.techfirm.co.jp/2015/11/30/android-5-0-ble%E3%81%AEbluetoothlescanner%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6/
                // see this article for Android

                // Build scan filter and add it to list.
                ScanFilter scanFilter = new ScanFilter.Builder()
                    .SetDeviceName("LBT-VRU01")
                    .Build();
                List<ScanFilter> filterList = new List<ScanFilter>();
                filterList.Add(scanFilter);

                // Set scan mode.
                ScanSettings scanSettings = new ScanSettings.Builder()
                    .SetScanMode(Android.Bluetooth.LE.ScanMode.Balanced)
                    .Build();

                // Regist event of OnScanResult.
                scanCallback.ScanResultEvent += ScanCallback_ScanResultEvent;

                // Start scan.
                scanner.StartScan(filterList, scanSettings, scanCallback);
                //scanner.StartScan(scanCallback);


            }
            else
            {
                scanner.StopScan(scanCallback);
            }
        }

        private void ScanCallback_ScanResultEvent(BluetoothDevice device, int rssi, ScanRecord record)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceName: {device.Name}, Level: {rssi}");
            if (deviceAddress == null)
            {
                deviceAddress = device.Address;
                deviceLabel.Text = $"Device: {device.Name}, Level: {rssi}";
                scanner.StopScan(scanCallback);
            }
            //deviceLabel.Text = $"Device: {device.Name}, Level: {rssi}";
        }
    }


    // http://engineer.recruit-lifestyle.co.jp/techblog/2015-01-15-using-ibeacon-on-android5/
    // see this article for Android callback.
    // http://www.shaga-workshop.net/diary/20150602.html
    // http://www.shaga-workshop.net/diary/20150608.html
    // http://www.shaga-workshop.net/diary/20150617.html
    // see these articles for Xamarin.Android imprementation.
    public class BleScanCallback : ScanCallback
    {
        public event Action<BluetoothDevice, int, ScanRecord> ScanResultEvent;

        /// <summary>
        /// When getting scan result, this method is called.
        /// </summary>
        /// <param name="callbackType"></param>
        /// <param name="result"></param>
        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            // result.Device     : 発見したBluetoothDevice
            // result.Rssi       : RSSI値
            // result.ScanRecord : スキャンレコード(byte[]で取得するなら、result.ScanRecord.GetBytes()で)
            System.Diagnostics.Debug.WriteLine($"{result.Device}");

            ScanResultEvent(result.Device, result.Rssi, result.ScanRecord);
        }

        /// <summary>
        /// When getting scan error, this method is called.
        /// </summary>
        /// <param name="errorCode"></param>
        public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);

            System.Diagnostics.Debug.WriteLine($"Error has occurred: {errorCode}");
        }
    }
}



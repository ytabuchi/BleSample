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
        BluetoothLeScanner scanner;

        // Use handler to timeout.
        //Handler mHandler;
        //bool mScanning;

        BleScanCallback scanCallback = new BleScanCallback();
        BleGattCallback gattCallback = new BleGattCallback();

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

            // Set BluetoothManager, BluetoothAdapter, BluetoothLeScanner
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
                BluetoothDevice device = adapter.GetRemoteDevice(deviceAddress);
                var mBluetoothGatt = device.ConnectGatt(this, false, gattCallback);
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

                // Build scan filter and add it to list. (ぶるタグ)
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
                //scanner.StartScan(filterList, scanSettings, scanCallback);
                scanner.StartScan(scanCallback);


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


    public class BleGattCallback : BluetoothGattCallback
    {
        /// <summary>
        /// This method is called when BluetoothGatt connection status is changed.
        /// </summary>
        /// <param name="gatt"></param>
        /// <param name="status"></param>
        /// <param name="newState"></param>
        public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            System.Diagnostics.Debug.WriteLine(newState); // If Connected, get enum connected.

            // If Connected, newState will be "ProfileState.Connected".
            if (newState == ProfileState.Connected)
            {
                // Will recieve service discovered return.
                gatt.DiscoverServices();
            }
            else if (newState == ProfileState.Disconnected)
            {
                // do something when disconnected.
            }
        }

        /// <summary>
        /// Called when service on connected device will be found.
        /// </summary>
        /// <param name="gatt"></param>
        /// <param name="status"></param>
        public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            // Immediate Alert service UUID of LBT-VRU01
            const string uuidAlertService = "00001802-0000-1000-8000-00805f9b34fb";
            const string uuidAlertCharcteristics = "00002a06-0000-1000-8000-00805f9b34fb";

            // Serviceを取得して
            BluetoothGattService service = gatt.GetService(Java.Util.UUID.FromString(uuidAlertService));
            // キャラクタリスティックを取得
            BluetoothGattCharacteristic characteristic = service.GetCharacteristic(Java.Util.UUID.FromString(uuidAlertCharcteristics));
            //// Notify/Indicateを有効に
            //gatt.SetCharacteristicNotification(characteristic, true);

            foreach (var x in service.Characteristics)
            {
                System.Diagnostics.Debug.WriteLine($"【Characteristics】: {x}, {x.Uuid}, {x.Permissions}");
            }



            //characteristic.SetValue("02");
            //gatt.WriteCharacteristic(characteristic);
        }

        /// <summary>
        /// Called when charcteristics has read.
        /// </summary>
        /// <param name="gatt"></param>
        /// <param name="characteristic"></param>
        /// <param name="status"></param>
        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);

            System.Diagnostics.Debug.WriteLine($"【read】: {characteristic.Uuid}");
        }
    }
}



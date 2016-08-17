using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using System.Threading;

namespace BleSample.Droid
{
    [Activity(Label = "Ble Search", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        BluetoothLeScanner scanner;
        BleScanCallback scanCallback = new BleScanCallback();
        ListView listView;
        List<BleDeviceData> bleDevices;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // Bluetooth Low Energyがサポートされているかのチェック。
            if (!PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureBluetoothLe))
            {
                Toast.MakeText(this, Resource.String.ble_not_supported, ToastLength.Short).Show();
                Finish();
            }

            // BluetoothManager,BluetoothAdapter,BluetoothLeScannerをインスタンス化。
            BluetoothManager manager = (BluetoothManager)GetSystemService(BluetoothService);
            BluetoothAdapter adapter = manager.Adapter;
            scanner = adapter.BluetoothLeScanner;

            // BluetoothのAdapterが取得できているか＝Bluetoothがサポートされているかのチェック。
            if (adapter == null)
            {
                Toast.MakeText(this, Resource.String.error_bluetooth_not_supported, ToastLength.Short).Show();
                Finish();
                return;
            }

            var scanButton = FindViewById<Button>(Resource.Id.scanButton);
            scanButton.Click += (sender, e) =>
            {
                bleDevices = new List<BleDeviceData>();

                scanCallback.ScanResultEvent += ScanCallback_ScanResultEvent;
                scanner.StartScan(scanCallback);
                Thread.Sleep(5000);
                scanner.StopScan(scanCallback);
            };

            listView = FindViewById<ListView>(Resource.Id.deviceList);
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var intent = new Intent(this, typeof(ServiceListActivity));
                var sendData = new string[] { bleDevices[e.Position].Name, bleDevices[e.Position].Id };
                intent.PutExtra("data", sendData);
                StartActivity(intent);
            };
        }


        private void ScanCallback_ScanResultEvent(BluetoothDevice device, int rssi, ScanRecord record)
        {
            // 同じアイテムならListに追加しない。
            if (bleDevices.Count == 0)
                bleDevices.Add(new BleDeviceData { Name = device.Name, Id = device.Address });
            else
            {
                foreach (var item in bleDevices)
                {
                    if (!item.Id.Contains(device.Address))
                        bleDevices.Add(new BleDeviceData { Name = device.Name ?? "Unknown", Id = device.Address });
                }
            }

            // MainThreadで処理する。
            RunOnUiThread(() =>
            {
                listView.Adapter = new SimpleListItem2_Adapter(this, bleDevices);
            });
        }
    }
}



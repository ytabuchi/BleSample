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
using Plugin.Permissions;
using Plugin.CurrentActivity;
using Android.Support.Design.Widget;

namespace BleSample.Droid
{
    [Activity(Label = "Ble Search", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        BluetoothLeScanner scanner;
        BleScanCallback scanCallback = new BleScanCallback();
        ListView listView;
        List<BleDeviceData> bleDevices = new List<BleDeviceData>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            CrossCurrentActivity.Current.Init(this, bundle);

            var scanButton = FindViewById<Button>(Resource.Id.scanButton);
            scanButton.Click += (sender, e) =>
            {
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

        protected override async void OnResume()
        {
            base.OnResume();

            // Bluetooth Low Energyがサポートされているかのチェック。
            if (!PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureBluetoothLe))
            {
                Toast.MakeText(this, Resource.String.ble_not_supported, ToastLength.Long).Show();
                Finish();
                return;
            }

            // Bluetoothを使うため、位置情報のPermissionのチェックとリクエスト
            var status = await Core.PermissionManager.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);

            if (status.Count == 0)
            {
                Toast.MakeText(this, "Permission denied.", ToastLength.Long).Show();
                Finish();
                return;
            }

            // もう一回（必要？）
            if (status[Plugin.Permissions.Abstractions.Permission.Location] != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                await Core.PermissionManager.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);

            // BluetoothManager,BluetoothAdapter,BluetoothLeScannerをインスタンス化。
            var manager = (BluetoothManager)GetSystemService(BluetoothService);
            var adapter = manager.Adapter;
            scanner = adapter.BluetoothLeScanner;

            // BluetoothのAdapterが取得できているか＝Bluetoothがサポートされているかのチェック。
            if (adapter == null)
            {
                Toast.MakeText(this, Resource.String.error_bluetooth_not_supported, ToastLength.Long).Show();
                Finish();
                return;
            }
        }

        


        private void ScanCallback_ScanResultEvent(BluetoothDevice device, int rssi, ScanRecord record)
        {
            // 同じアイテムならListに追加しない。
            if (bleDevices.Where(x => x.Id.Contains(device.Address)).Count() == 0)
                bleDevices.Add(new BleDeviceData { Name = device.Name ?? "Unknown", Id = device.Address });

            // MainThreadで処理する。
            RunOnUiThread(() =>
            {
                listView.Adapter = new SimpleListItem2_Adapter(this, bleDevices);
            });
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}



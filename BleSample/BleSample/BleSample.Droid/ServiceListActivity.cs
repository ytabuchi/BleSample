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
using Android.Bluetooth;

namespace BleSample.Droid
{
    [Activity(Label = "Services")]
    public class ServiceListActivity : Activity
    {
        ListView listView;
        string deviceName;
        string deviceAddress;
        List<string> gattServices = new List<string>();
        BleGattCallback gattCallback = new BleGattCallback();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ServiceList);

            // Intentを受け取れたらTextViewに反映させる
            var recievedData = Intent.GetStringArrayExtra("data") ?? null;
            if (recievedData != null)
            {
                deviceName = recievedData[0];
                deviceAddress = recievedData[1];

                var deviceNameLabel = FindViewById<TextView>(Resource.Id.DeviceNameLabel);
                deviceNameLabel.Text = deviceName;
                var deviceAddressLabel = FindViewById<TextView>(Resource.Id.DeviceAddressLabel);
                deviceAddressLabel.Text = deviceAddress;
            }

            listView = FindViewById<ListView>(Resource.Id.serviceList);
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var intent = new Intent(this, typeof(CharacteristicsListActivity));
                var sendData = new string[]{ deviceAddress, gattServices[e.Position]};
                intent.PutExtra("data", sendData);
                StartActivity(intent);
            };

            // 再度BLuetoothManager,BluetoothAdapterを用意する（つらい
            BluetoothManager manager = (BluetoothManager)GetSystemService(BluetoothService);
            BluetoothAdapter adapter = manager.Adapter;

            // deviceAddressが取得出来ていればデバイスに接続しサービスを取得する
            if (deviceAddress != null)
            {
                BluetoothDevice device = adapter.GetRemoteDevice(deviceAddress);

                gattCallback.ServicesDiscoveredEvent += GattCallback_ServicesDiscoveredEvent;
                var mBluetoothGatt = device.ConnectGatt(this, false, gattCallback);
            }
        }


        private void GattCallback_ServicesDiscoveredEvent(BluetoothGatt gatt)
        {
            foreach (var item in gatt.Services)
            {
                gattServices.Add(item.Uuid.ToString());

                System.Diagnostics.Debug.WriteLine($"【Service】UUID:{item.Uuid}");
            }
            var arry = gattServices.ToArray();

            // Main Threadで処理させる。
            RunOnUiThread(() =>
            {
                listView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, arry);
            });

        }
    }
}
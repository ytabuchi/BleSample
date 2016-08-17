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
    [Activity(Label = "Characteristics")]
    public class CharacteristicsListActivity : Activity
    {
        BleGattCallback gattCallback = new BleGattCallback();
        ListView listView;
        List<TableItem> gattCharacteristics = new List<TableItem>();
        // Text1 = Name,Text2 = UUID,Text3 = Properties,Text4 = DataType
        string deviceAddress;
        string serviceUuid;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.CharacteristicsList);

            // Intentを受け取れたらTextViewに反映させる。
            var recievedData = Intent.GetStringArrayExtra("data") ?? null;
            if (recievedData != null)
            {
                deviceAddress = recievedData[0];
                serviceUuid = recievedData[1];
            }

            listView = FindViewById<ListView>(Resource.Id.serviceList);
            listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var intent = new Intent(this, typeof(CharacteristicsActivity));
                var sendData = new string[] { deviceAddress, serviceUuid, gattCharacteristics[e.Position].Text2 };
                intent.PutExtra("data", sendData);
                StartActivity(intent);
            };

            // 再度BLuetoothManager,BluetoothAdapterを用意する
            BluetoothManager manager = (BluetoothManager)GetSystemService(BluetoothService);
            BluetoothAdapter adapter = manager.Adapter;

            BluetoothDevice device = adapter.GetRemoteDevice(deviceAddress);

            gattCallback.ServicesDiscoveredEvent += GattCallback_ServicesDiscoveredEvent;
            var mBluetoothGatt = device.ConnectGatt(this, false, gattCallback);
        }

        private void GattCallback_ServicesDiscoveredEvent(BluetoothGatt gatt)
        {
            // 受け取ったUUIDに接続してキャラクタリスティックを取得
            var service = gatt.GetService(Java.Util.UUID.FromString(serviceUuid));
            var characteristics = service.Characteristics;

            foreach (var item in characteristics)
            {
                gattCharacteristics.Add(new TableItem {
                    Text1 = "CharacteristicName",
                    Text2 = item.Uuid.ToString(),
                    Text3 = item.Properties.ToString(),
                    Text4 = item.WriteType.ToString() });
                //gattCharacteristics.Add(item.Uuid.ToString());
            }
            //var arry = gattCharacteristics.ToArray();

            // Main Threadで処理させる。
            RunOnUiThread(() =>
            {
                //listView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, arry);
                listView.Adapter = new CharacteristicsListAdapter(this, gattCharacteristics);
            });


        }
    }
}
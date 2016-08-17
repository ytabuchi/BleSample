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
    [Activity(Label = "Characteristic")]
    public class CharacteristicsActivity : Activity
    {
        BleGattCallback gattCallback = new BleGattCallback();
        BluetoothGattCharacteristic _characteristics;
        BluetoothGatt _gatt;
        string deviceAddress;
        string serviceUuid;
        string characteristic;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Characteristic);

            var characteristicNameLabel = FindViewById<TextView>(Resource.Id.CharacteristicNameLabel);
            var characteristicUuidLabel = FindViewById<TextView>(Resource.Id.CharacteristicUuidLabel);

            var readButton = FindViewById<Button>(Resource.Id.ReadButton);
            readButton.Click += (sender, e) =>
            {
                _gatt.ReadCharacteristic(_characteristics);

            };

            var writeText = FindViewById<EditText>(Resource.Id.WriteText);
            var writeButton = FindViewById<Button>(Resource.Id.WriteButton);
            writeButton.Click += (sender, e) =>
            {
                // 書き込みフォーマットにより、SpinnerでGattFormatを選択させるなどしてもよいでしょう。
                // https://developer.xamarin.com/guides/android/user_interface/spinner/
                int i;
                var res = int.TryParse(writeText.Text, out i);
                if (res)
                {
                    _characteristics.SetValue(i, GattFormat.Sint8, 0);
                    _gatt.WriteCharacteristic(_characteristics);
                }
            };

            // Intentを受け取れたらTextViewに反映させる。
            var recievedData = Intent.GetStringArrayExtra("data") ?? null;
            if (recievedData != null)
            {
                deviceAddress = recievedData[0];
                serviceUuid = recievedData[1];
                characteristic = recievedData[2];

                characteristicNameLabel.Text = "Characteristic";
                characteristicUuidLabel.Text = characteristic;
            }

            // 再度BLuetoothManager,BluetoothAdapterを用意する
            BluetoothManager manager = (BluetoothManager)GetSystemService(BluetoothService);
            BluetoothAdapter adapter = manager.Adapter;

            BluetoothDevice device = adapter.GetRemoteDevice(deviceAddress);

            gattCallback.ServicesDiscoveredEvent += GattCallback_ServicesDiscoveredEvent;
            gattCallback.CharacteristicReadEvent += GattCallback_CharacteristicReadEvent;
            var mBluetoothGatt = device.ConnectGatt(this, false, gattCallback);
        }

        private void GattCallback_CharacteristicReadEvent(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            System.Diagnostics.Debug.WriteLine($"【GattCallback_CharacteristicReadEvent】:{gatt},{characteristic},{status}");

            // 必要に応じてステータスにより処理を分岐
            switch (status)
            {
                case GattStatus.ConnectionCongested:
                    break;
                case GattStatus.Failure:
                    break;
                case GattStatus.InsufficientAuthentication:
                    break;
                case GattStatus.InsufficientEncryption:
                    break;
                case GattStatus.InvalidAttributeLength:
                    break;
                case GattStatus.InvalidOffset:
                    break;
                case GattStatus.ReadNotPermitted:
                    break;
                case GattStatus.RequestNotSupported:
                    break;
                case GattStatus.Success:
                    System.Diagnostics.Debug.WriteLine($"【Success】:{gatt},{characteristic},{status}");

                    var descs = characteristic.Descriptors;
                    foreach (var item in descs)
                    {
                        System.Diagnostics.Debug.WriteLine($"{item.Uuid},{item.Permissions}");
                    }

                    break;
                case GattStatus.WriteNotPermitted:
                    break;
                default:
                    break;
            }
        }

        private void GattCallback_ServicesDiscoveredEvent(BluetoothGatt gatt)
        {
            _gatt = gatt;
            var service = gatt.GetService(Java.Util.UUID.FromString(serviceUuid));
            _characteristics = service.GetCharacteristic(Java.Util.UUID.FromString(characteristic));

            System.Diagnostics.Debug.WriteLine($@"
{_characteristics.Uuid},
{_characteristics.Properties},
{_characteristics.WriteType}
");
        }


    }
}
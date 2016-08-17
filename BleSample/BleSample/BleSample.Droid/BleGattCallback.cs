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
    public class BleGattCallback : BluetoothGattCallback
    {
        public event Action<BluetoothGatt> ServicesDiscoveredEvent;
        public event Action<BluetoothGatt, BluetoothGattCharacteristic, GattStatus> CharacteristicReadEvent;

        // 接続状況が変化した時に発火するメソッド
        public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            if (newState == ProfileState.Connected)
            {
                // 接続状態になったらサービスを検索する
                // サービスが見つかったらOnServicesDiscoveredが発火
                gatt.DiscoverServices();
            }
            else if(newState == ProfileState.Disconnected)
            {
                //ポップアップなどを出して、任意のActivityに移動するなどしてください
            }
        }

        // 接続したデバイスのサービスが見つかった時に発火するメソッド
        public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            ServicesDiscoveredEvent(gatt);
        }

        /// <summary>
        /// キャラクタリスティック読み込み時のイベント
        /// </summary>
        /// <param name="gatt"></param>
        /// <param name="characteristic"></param>
        /// <param name="status"></param>
        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);

            CharacteristicReadEvent(gatt, characteristic, status);

        }
    }
}
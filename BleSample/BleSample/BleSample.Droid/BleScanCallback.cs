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
using Android.Bluetooth.LE;
using Android.Bluetooth;

namespace BleSample.Droid
{
    public class BleScanCallback : ScanCallback
    {
        // 外部から拾えるようにイベントを用意
        public event Action<BluetoothDevice, int, ScanRecord> ScanResultEvent;

        // スキャン結果を得ると発火するメソッド
        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            // result.Device     : 発見したBluetoothDevice
            // result.Rssi       : RSSI値
            // result.ScanRecord : スキャンレコード(byte[]で取得するなら、result.ScanRecord.GetBytes()で)
            ScanResultEvent(result.Device, result.Rssi, result.ScanRecord);
        }

        // スキャンエラー時に発火するメソッド
        public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);

            // スキャンが失敗した時の処理を記述
        }

        // 必要に応じて記述
        //public override void OnBatchScanResults(IList<ScanResult> results)
        //{
        //    base.OnBatchScanResults(results);

        //}
    }
}
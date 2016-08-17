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
        // �O������E����悤�ɃC�x���g��p��
        public event Action<BluetoothDevice, int, ScanRecord> ScanResultEvent;

        // �X�L�������ʂ𓾂�Ɣ��΂��郁�\�b�h
        public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);

            // result.Device     : ��������BluetoothDevice
            // result.Rssi       : RSSI�l
            // result.ScanRecord : �X�L�������R�[�h(byte[]�Ŏ擾����Ȃ�Aresult.ScanRecord.GetBytes()��)
            ScanResultEvent(result.Device, result.Rssi, result.ScanRecord);
        }

        // �X�L�����G���[���ɔ��΂��郁�\�b�h
        public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);

            // �X�L���������s�������̏������L�q
        }

        // �K�v�ɉ����ċL�q
        //public override void OnBatchScanResults(IList<ScanResult> results)
        //{
        //    base.OnBatchScanResults(results);

        //}
    }
}
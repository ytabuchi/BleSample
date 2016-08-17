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

        // �ڑ��󋵂��ω��������ɔ��΂��郁�\�b�h
        public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            if (newState == ProfileState.Connected)
            {
                // �ڑ���ԂɂȂ�����T�[�r�X����������
                // �T�[�r�X������������OnServicesDiscovered������
                gatt.DiscoverServices();
            }
            else if(newState == ProfileState.Disconnected)
            {
                //�|�b�v�A�b�v�Ȃǂ��o���āA�C�ӂ�Activity�Ɉړ�����Ȃǂ��Ă�������
            }
        }

        // �ڑ������f�o�C�X�̃T�[�r�X�������������ɔ��΂��郁�\�b�h
        public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            ServicesDiscoveredEvent(gatt);
        }

        /// <summary>
        /// �L�����N�^���X�e�B�b�N�ǂݍ��ݎ��̃C�x���g
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
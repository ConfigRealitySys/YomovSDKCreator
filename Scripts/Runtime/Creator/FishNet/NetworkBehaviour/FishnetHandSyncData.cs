using System;
using UnityEngine;
using static Yomov.HandTrackingSync;

namespace Yomov
{
    //Fishnet同步手指传输的数据
    [Serializable]
    public struct FingersData
    {
        public Finger thumb;
        public Finger index;
        public Finger middle;
        public Finger ring;
        public Finger little;
        public void SetData(in HandTrackingData_hand data)
        {
            thumb.SetData(data.Thumb);
            index.SetData(data.Index);
            middle.SetData(data.Middle);
            ring.SetData(data.Ring);
            little.SetData(data.Little);
        }
        public void GetData(ref HandTrackingData_hand data)
        {
            thumb.GetData(ref data.Thumb);
            index.GetData(ref data.Index);
            middle.GetData(ref data.Middle);
            ring.GetData(ref data.Ring);
            little.GetData(ref data.Little);
        }
    }

    [Serializable]
    public struct Finger
    {
        public int metacarpal;
        public int proximal;
        public int intermediate;
        public int distal;
        private Vector3 _Temp;
        public void SetData(in HandTrackingData_finger finger)
        {
            Vector3ToInt(finger.Metacarpal.eulerAngles, ref metacarpal);
            Vector3ToInt(finger.Proximal.eulerAngles, ref proximal);
            Vector3ToInt(finger.Intermediate.eulerAngles, ref intermediate);
            Vector3ToInt(finger.Distal.eulerAngles, ref distal);
        }
        public void GetData(ref HandTrackingData_finger finger)
        {
            IntToVector3(metacarpal, ref _Temp);
            finger.Metacarpal = Quaternion.Euler(_Temp);

            IntToVector3(proximal, ref _Temp);
            finger.Proximal = Quaternion.Euler(_Temp);

            IntToVector3(intermediate, ref _Temp);
            finger.Intermediate = Quaternion.Euler(_Temp);

            IntToVector3(distal, ref _Temp);
            finger.Distal = Quaternion.Euler(_Temp);
        }

        private void Vector3ToInt(in Vector3 v, ref int i)
        {
            //每个数只保留10位
            i = ((int)v.x & 0x3FF) << 20 |
                ((int)v.y & 0x3FF) << 10 |
                (int)v.z & 0x3FF;
        }

        private void IntToVector3(in int i, ref Vector3 v)
        {
            v.x = i >> 20 & 0x3FF;
            v.y = i >> 10 & 0x3FF;
            v.z = i & 0x3FF;
        }
    }
}

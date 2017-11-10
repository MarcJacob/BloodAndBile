using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float X, float Y, float Z, float W)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }

        public static implicit operator UnityEngine.Quaternion(SerializableQuaternion sV)
        {
            return new UnityEngine.Quaternion(sV.x, sV.y, sV.z, sV.w);
        }

        public static implicit operator SerializableQuaternion(UnityEngine.Quaternion v)
        {
            return new SerializableQuaternion(v.x, v.y, v.z, v.w);
        }
    }
}

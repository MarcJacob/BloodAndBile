using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloodAndBileEngine
{
    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static implicit operator UnityEngine.Vector3(SerializableVector3 sV)
        {
            return new UnityEngine.Vector3(sV.x, sV.y, sV.z);
        }

        public static implicit operator SerializableVector3(UnityEngine.Vector3 v)
        {
            return new SerializableVector3(v.x, v.y, v.z);
        }
    }
}

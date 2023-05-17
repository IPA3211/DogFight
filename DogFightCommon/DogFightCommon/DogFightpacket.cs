using System;
using UnityEngine;

namespace DogFightCommon
{
    namespace Packet
    {
        [Serializable]
        class SerializableVector3
        {
            [SerializeField] float x, y, z;

            SerializableVector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            Vector3 GetVector3 ()
            {
                return new Vector3(x, y, z);
            }
        }

        [Serializable]
        class SerializableQuaternion
        {
            [SerializeField] float x, y, z, w;

            SerializableQuaternion(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            Quaternion GetQuaternion()
            {
                return new Quaternion(x, y, z, w);
            }
        }
    }
}

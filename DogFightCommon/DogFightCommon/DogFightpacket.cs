using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DogFightCommon
{
    namespace Packet
    {
        [Serializable]
        public class SerializableVector3
        {
            [SerializeField] float x, y, z;

            void Init(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public SerializableVector3(float x, float y, float z)
            {
                Init(x, y, z);
            }
            public SerializableVector3(Vector3 vector)
            {
                Init(vector.x, vector.y, vector.z);
            }

            public Vector3 GetVector3()
            {
                return new Vector3(x, y, z);
            }
        }

        [Serializable]
        public class SerializableQuaternion
        {
            [SerializeField] float x, y, z, w;

            void Init(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
            public SerializableQuaternion(float x, float y, float z, float w)
            {
                Init(x, y, z, w);
            }
            public SerializableQuaternion(Quaternion quaternion)
            {
                Init(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
            }

            public Quaternion GetQuaternion()
            {
                return new Quaternion(x, y, z, w);
            }
        }

        [Serializable]
        public class SerializableTransform
        {
            [SerializeField] SerializableVector3 position;
            [SerializeField] SerializableQuaternion rotation;
            [SerializeField] SerializableVector3 scale;

            public SerializableTransform(Transform transform)
            {
                position = new SerializableVector3(transform.position);
                rotation = new SerializableQuaternion(transform.rotation);
                scale = new SerializableVector3(transform.localScale);
            }

            public void SetTransform(Transform transform)
            {
                transform.position = position.GetVector3();
                transform.rotation = rotation.GetQuaternion();
                transform.localScale = scale.GetVector3();
            }
        }
    }
}

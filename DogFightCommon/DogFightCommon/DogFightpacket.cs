using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace DogFightCommon
{
    public class ByteUtil
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return Compress(ms.ToArray());
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            byte[] decompressByte = Decompress(arrBytes);
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(decompressByte, 0, decompressByte.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }

    namespace WebSocketPacket
    {
        [Serializable]
        public class WebSocketPacket
        {
            [SerializeField] int index;
            [SerializeField] bool isAns;
            [SerializeField] int ansTo;
            [SerializeField] string order;

            public int Index => index;
            public bool IsAns => isAns;
            public int AnsTo => ansTo;
            public string Order => order;

            public WebSocketPacket()
            {
                index = this.GetHashCode();
                order = this.GetType().Name;
                isAns = false;
            }

            public void AnswerTo(int ansTo)
            {
                isAns = true;
                this.ansTo = ansTo;
            }
        }

        // 네트워크에 요청하면 대답을 기다림 Duration이 지나면 TimeOutException
        public class NetworkRequest
        {
            WebSocketPacket packet;
            TaskCompletionSource<WebSocketPacket> tsc;
            float duration;

            public WebSocketPacket Packet => packet;

            public NetworkRequest(WebSocketPacket pack, TaskCompletionSource<WebSocketPacket> inTsc, float dur)
            {
                packet = pack;
                tsc = inTsc;
                duration = dur;
            }

            public bool UpdateDuration(float delta)
            {
                duration -= delta;

                if (duration < 0)
                {
                    return true;
                }
                return false;
            }

            public void OnAnswerArrive(WebSocketPacket packet)
            {
                tsc.SetResult(packet);
            }

            public void OnRemove()
            {
                TimeoutException e = new TimeoutException();
                tsc.SetException(e);
            }
        }
    }
    namespace Util 
    {
        public class ByteUtil
        {
            public static byte[] ObjectToByteArray(object obj)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return Compress(ms.ToArray());
                }
            }

            public static object ByteArrayToObject(byte[] arrBytes)
            {
                byte[] decompressByte = Decompress(arrBytes);
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(decompressByte, 0, decompressByte.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    var obj = binForm.Deserialize(memStream);
                    return obj;
                }
            }

            public static byte[] Compress(byte[] data)
            {
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(output, System.IO.Compression.CompressionLevel.Optimal))
                {
                    dstream.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }

            public static byte[] Decompress(byte[] data)
            {
                MemoryStream input = new MemoryStream(data);
                MemoryStream output = new MemoryStream();
                using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    dstream.CopyTo(output);
                }
                return output.ToArray();
            }
        }
    }
    namespace UDPpacket
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

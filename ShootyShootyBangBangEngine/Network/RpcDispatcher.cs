using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using ShootyShootyBangBangEngine.Network;
using ShootyShootyBangBangEngine.Helpers;

namespace ShootyShootyBangBangEngine.Network
{
    public delegate void RPCDelegate(RPCData rpcData);
    public delegate void OnPacket(Type packetType);

    public struct RPCData
    {
        public NetWrapIncomingMessage OriginalMessage;
        public object DeserializedObject;
    }

    public class RPCDispatcher
    {
        public Dictionary<Type, RPCDelegate> Functions = new Dictionary<Type, RPCDelegate>();
    }

    public class RPCStream
    {
        static BinaryFormatter formatter = new BinaryFormatter();
        public static OnPacket packetDelegate;
        public static void Execute(Stream stream, RPCDispatcher dispatcher, RPCData dataTemplate)
        {
            object deserializedObject;
            do
            {
                if (stream.Position >= stream.Length)
                    break;

                deserializedObject = Serialization.GetGlobalSerializer().Deserialize(stream);

                Type t = deserializedObject.GetType();
                if (dispatcher.Functions.ContainsKey(t))
                {
                    if (packetDelegate != null)
                        packetDelegate.Invoke(t);
                    dataTemplate.DeserializedObject = deserializedObject;
                    dispatcher.Functions[t](dataTemplate);
                }
                else
                    throw new Exception("RPC: Unrecognized type: " + t.ToString());
            } while (deserializedObject != null);

        }
        public static void AddCall(Stream stream, object argumentObject)
        {
            Serialization.GetGlobalSerializer().Serialize(stream, argumentObject);
        }

        public static void Test()
        {
            RPCDispatcher dispatch = new RPCDispatcher();
            dispatch.Functions[typeof(Serialization.TestStruct)] = (RPCData data) =>
            {
                Serialization.TestStruct obj = (Serialization.TestStruct)data.DeserializedObject;
                Console.WriteLine("RPC Test Successful.");
            };

            // add a rpc call
            Serialization.TestStruct serTest = new Serialization.TestStruct();
            serTest.a = 10;
            serTest.b = 20;
            serTest.c = "hello";
            MemoryStream memStream = new MemoryStream(100);
            RPCStream.AddCall(memStream, serTest);

            // execute rpc calls
            MemoryStream memStreamIn = new MemoryStream(memStream.ToArray());

            RPCData template = new RPCData();

            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 100000; i++)
            {
                memStreamIn.Position = 0;
                try
                {
                    RPCStream.Execute(memStreamIn, dispatch, template);
                }
                catch
                {

                }

            }
            st.Stop();
            Console.WriteLine("RPC performance: " + st.ElapsedMilliseconds);
        }
    }
}

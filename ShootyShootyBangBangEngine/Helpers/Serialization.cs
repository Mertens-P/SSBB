using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using NetSerializer;

namespace ShootyShootyBangBangEngine.Helpers
{
    // Summary:
    //     Indicates that a class can be serialized. This class cannot be inherited.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    public sealed class NetSerializable : System.Attribute
    {
        // Summary:
        //     Initializes a new instance of the System.SerializableAttribute class.
        public NetSerializable() { }

        static public IEnumerable<Type> GetTypesWithAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(NetSerializable), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }

    #region SortedDictionary Serializer
    /*
    public class SortedDictionarySerializer : IStaticTypeSerializer
    {
        public bool Handles(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var genTypeDef = type.GetGenericTypeDefinition();

            return genTypeDef == typeof(SortedDictionary<,>);
        }


        public IEnumerable<Type> GetSubtypes(Type type)
        {
            // Dictionary<K,V> is stored as KeyValuePair<K,V>[]

            var genArgs = type.GetGenericArguments();

            var serializedType = typeof(KeyValuePair<,>).MakeGenericType(genArgs).MakeArrayType();

            yield return serializedType;
        }

        public void GetStaticMethods(Type type, out MethodInfo writer, out MethodInfo reader)
        {
            Debug.Assert(type.IsGenericType);

            if (!type.IsGenericType)
                throw new Exception();

            var genTypeDef = type.GetGenericTypeDefinition();

            Debug.Assert(genTypeDef == typeof(SortedDictionary<,>));

            var containerType = this.GetType();

            writer = GetGenWriter(containerType, genTypeDef);
            reader = GetGenReader(containerType, genTypeDef);

            var genArgs = type.GetGenericArguments();

            writer = writer.MakeGenericMethod(genArgs);
            reader = reader.MakeGenericMethod(genArgs);
        }

        static MethodInfo GetGenWriter(Type containerType, Type genType)
        {
            var mis = containerType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(mi => mi.IsGenericMethod && mi.Name == "WritePrimitive");

            foreach (var mi in mis)
            {
                var p = mi.GetParameters();

                if (p.Length != 2)
                    continue;

                if (p[0].ParameterType != typeof(Stream))
                    continue;

                var paramType = p[1].ParameterType;

                if (paramType.IsGenericType == false)
                    continue;

                var genParamType = paramType.GetGenericTypeDefinition();

                if (genType == genParamType)
                    return mi;
            }

            return null;
        }

        static MethodInfo GetGenReader(Type containerType, Type genType)
        {
            var mis = containerType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(mi => mi.IsGenericMethod && mi.Name == "ReadPrimitive");

            foreach (var mi in mis)
            {
                var p = mi.GetParameters();

                if (p.Length != 2)
                    continue;

                if (p[0].ParameterType != typeof(Stream))
                    continue;

                var paramType = p[1].ParameterType;

                if (paramType.IsByRef == false)
                    continue;

                paramType = paramType.GetElementType();

                if (paramType.IsGenericType == false)
                    continue;

                var genParamType = paramType.GetGenericTypeDefinition();

                if (genType == genParamType)
                    return mi;
            }

            return null;
        }

        public static void WritePrimitive<TKey, TValue>(Stream stream, SortedDictionary<TKey, TValue> value)
        {
            var kvpArray = new KeyValuePair<TKey, TValue>[value.Count];

            int i = 0;
            foreach (var kvp in value)
                kvpArray[i++] = kvp;

            Game.Framework.Serialization.GlobalSerializer.Serialize(stream, kvpArray);
        }

        public static void ReadPrimitive<TKey, TValue>(Stream stream, out SortedDictionary<TKey, TValue> value)
        {
            var kvpArray = (KeyValuePair<TKey, TValue>[])Game.Framework.Serialization.GlobalSerializer.Deserialize(stream);

            value = new SortedDictionary<TKey, TValue>();

            foreach (var kvp in kvpArray)
                value.Add(kvp.Key, kvp.Value);
        }

        public System.Reflection.MethodInfo GetStaticWriter(Type type)
        {
            return GetGenWriter(type, type);
        }

        public System.Reflection.MethodInfo GetStaticReader(Type type)
        {
            return GetGenReader(type, type);
        }
    }
    */
    #endregion

    public class Serialization
    {
        [Serializable, NetSerializable]
        public struct TestStruct
        {
            public TestStruct(int _a, short _b, string _c) { a = _a; b = _b; c = _c; }
            public int a;
            public short b;
            public string c;
        }

        public static NetSerializer.Serializer s_globalSerializer = null;
        public static NetSerializer.Serializer GetGlobalSerializer() { if (s_globalSerializer == null) Initialize(); return s_globalSerializer; }

        public static void Initialize()
        {
            var types = NetSerializable.GetTypesWithAttribute(System.Reflection.Assembly.GetExecutingAssembly()).ToList();
            if (System.Reflection.Assembly.GetCallingAssembly() != System.Reflection.Assembly.GetExecutingAssembly())
                types.AddRange(NetSerializable.GetTypesWithAttribute(System.Reflection.Assembly.GetCallingAssembly()).ToList());

            var serializerSettings = new Settings();
            //serializerSettings.CustomTypeSerializers = new ITypeSerializer[] { new SortedDictionarySerializer() };
            serializerSettings.SupportIDeserializationCallback = true;
            serializerSettings.SupportSerializationCallbacks = true;
            s_globalSerializer = new NetSerializer.Serializer(types.ToArray(), serializerSettings);

            Test();
        }

        static public void Test()
        {
            TestStruct obj = new TestStruct();
            obj.a = 10;
            obj.b = 20;
            obj.c = "hey";
            MemoryStream ms = new MemoryStream();
            s_globalSerializer.Serialize(ms, obj);
            ms.Flush();

            MemoryStream msRead = new MemoryStream(ms.ToArray());
            var obj2 = s_globalSerializer.Deserialize(msRead);
        }
    }
}

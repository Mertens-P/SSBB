using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Helpers
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> @this,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue value;
            return (key != null && @this.TryGetValue(key, out value)) ? value : defaultValue;
        }
    }

    public class MathHelpers
    {
        static public float Lerp(float x, float y, float f)
        {
            return x + (y - x) * f;
        }

        public static OpenTK.Vector2 SytemVecToOpenTkVec(System.Numerics.Vector2 vec) { return new OpenTK.Vector2(vec.X, vec.Y); }
        public static System.Numerics.Vector2 OpenTkVecToSystemVec(OpenTK.Vector2 vec) { return new System.Numerics.Vector2(vec.X, vec.Y); }
    }

    public class JsonHelpers
    {
        public static T JsonLoad<T>(System.IO.TextReader stream, bool includeNonPublic = false)
        {
            Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
            Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            if (includeNonPublic)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
#pragma warning restore CS0618 // Type or member is obsolete
                ser.ConstructorHandling = Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor;
            }
            ser.ContractResolver = dcr;
            ser.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            ser.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //ser.Error += On_JsonLoadError;
            return (T)ser.Deserialize(stream, typeof(T));
        }

        public static T JsonLoad<T>(string data, bool includeNonPublic = false)
        {
            using (System.IO.StringReader sreader = new System.IO.StringReader(data))
            {
                return JsonLoad<T>(sreader, includeNonPublic);
            }
        }

        public static string JsonSave(object data, bool includeNonPublic = false, Newtonsoft.Json.TypeNameHandling typeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto)
        {
            using (System.IO.StringWriter swriter = new System.IO.StringWriter())
            {
                Newtonsoft.Json.JsonSerializer ser = new Newtonsoft.Json.JsonSerializer();
                Newtonsoft.Json.Serialization.DefaultContractResolver dcr = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                if (includeNonPublic)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    dcr.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;
#pragma warning restore CS0618 // Type or member is obsolete
                }
                ser.ContractResolver = dcr;
                ser.TypeNameHandling = typeNameHandling;
                ser.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                ser.Serialize(swriter, data);
                return swriter.ToString();
            }
        }

        public static T CopyObject<T>(object toCopy, bool includeNonPublic = false)
        {
            var saveString = JsonSave(toCopy, includeNonPublic);
            return JsonLoad<T>(saveString, includeNonPublic);
        }
    }
}

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
    }
}

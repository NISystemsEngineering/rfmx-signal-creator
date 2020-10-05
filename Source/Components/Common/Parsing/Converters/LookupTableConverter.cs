using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.SignalCreator
{
    public abstract class LookupTableConverter<TKey,TValue> : ValueConverter<TValue>
    {
        protected abstract Dictionary<TKey, TValue> LookupTable { get; }

        protected override TValue Convert(object value)
        {
            TKey key;
            try
            {
                key = (TKey)System.Convert.ChangeType(value, typeof(TKey));
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException)
            {
                throw new ArgumentException($"Lookup value \"{value}\" is not able to be converted to key type {typeof(TKey)}.", nameof(value), ex);
            }
            if (LookupTable.TryGetValue(key, out TValue matchedValue))
            {
                return matchedValue;
            }
            else
            {
                throw new KeyNotFoundException($"No key matching \"{key}\" in lookup table {LookupTable}.");
            }
        }
    }
}

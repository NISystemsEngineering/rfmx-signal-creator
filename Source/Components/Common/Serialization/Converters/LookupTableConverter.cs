using System;
using System.Collections.Generic;

namespace NationalInstruments.Utilities.SignalCreator.Serialization.Converters
{
    /// <summary>
    /// A converter type that converts uses a lookup table in the form of a dictionary to convert a value of 
    /// <typeparamref name="TKey"/> to <typeparamref name="TValue"/>. <para></para> 
    /// 
    /// Since the lookup techinque is the same for use cases, this allows for child classes simply to define the lookup table and not have 
    /// to implement any more functionality.
    /// </summary>
    /// <typeparam name="TKey">Specifies the raw value type of the data source to be used as the key.</typeparam>
    /// <typeparam name="TValue">Specifies the value type that the raw value should be converted to using the lookup table.</typeparam>
    public abstract class LookupTableConverter<TKey,TValue> : ValueConverter<TValue>
    {
        /// <summary>
        /// This abstract property allows child classes to define the lookup table 
        /// </summary>
        protected abstract Dictionary<TKey, TValue> LookupTable { get; }

        /// <summary>
        /// Returns the matching <typeparamref name="TValue"/> from <see cref="LookupTable"/> using <paramref name="value"/> as the key.
        /// </summary>
        /// <param name="value">Specifies the key value to use in the lookup.</param>
        /// <returns></returns>
        protected override TValue Convert(object value)
        {
            TKey key;
            try
            {
                // Make sure that the data read from the data source actually matches the expected key type
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

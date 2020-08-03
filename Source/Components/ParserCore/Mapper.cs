using System;
using Serilog;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Maps settings to the appropriate RFmx properties.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type to be configured</typeparam>
    public abstract class Mapper<T> where T : ISignalConfiguration
    {
        public T Signal { get; }

        public Mapper(T signal)
        {
            Signal = signal;
        }

        /// <summary>
        /// Represents a mapping operation to translate from an <see cref="RFmxPropertyGroup{T}"/> object to the appropriate RFmx
        /// settings as defined by <see cref="RFmxPropertyMap{T}"/>. 
        /// </summary>
        public void MapSection(RFmxPropertyGroup group)
        {
            var properties = group.FetchMappedFields();

            // If the section has any specific configuration that needs to be applied to the RFmx session that is not a simple
            // mapping, this will be invoked here.
            group.ConfigureRFmxSignal(Signal);

            // Discard the attribute; we only need the keys and associated values
            foreach ((_, object property) in properties)
            {
                // Invoke the correct instance of the Apply Configuration overloaded method based on the type of the generic
                // parameter. Skip all keys that do not have a value (meaning they were not found or version was not a match).
                try
                {
                    switch (property)
                    {
                        case RFmxPropertyMap<bool> boolKey:
                            if (boolKey.HasValue)
                            {
                                ApplyConfiguration(group, boolKey);
                            }
                            break;
                        case RFmxPropertyMap<double> doubleKey:
                            if (doubleKey.HasValue)
                            {
                                ApplyConfiguration(group, doubleKey);
                            }
                            break;
                        case RFmxPropertyMap<int> intKey:
                            if (intKey.HasValue)
                            {
                                ApplyConfiguration(group, intKey);
                            }
                            break;
                        case RFmxPropertyMap<string> stringKey:
                            if (stringKey.HasValue)
                            {
                                ApplyConfiguration(group, stringKey);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (RFmxException rfEx)
                {
                    Log.Error(rfEx, "RFmx threw an exception applying property using key {Key}", property);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unknown exception applying property using key {Key}", property);
                }
            }
        }
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RFmxPropertyGroup section, RFmxPropertyMap<bool> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RFmxPropertyGroup section, RFmxPropertyMap<double> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RFmxPropertyGroup section, RFmxPropertyMap<int> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RFmxPropertyGroup section, RFmxPropertyMap<string> key);
    }
}

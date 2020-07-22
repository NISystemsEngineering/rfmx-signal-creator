using NationalInstruments.RFmx.InstrMX;
using System;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{

    /// <summary>
    /// Maps settings to the appropriate RFmx properties.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type to be configured</typeparam>
    public abstract class RfmxMapper<T> where T : ISignalConfiguration
    {
        /// <summary>
        /// Represents a mapping operation to translate from an <see cref="RfwsSection{T}"/> object to the appropriate RFmx
        /// settings as defined by <see cref="RfwsKey{T}"/>. 
        /// </summary>
        public void MapSection(RfwsSection<T> section)
        {
            var keys = RfwsParserUtilities.FetchSectionKeys(section);

            // Discard the attribute; we only need the keys and associated values
            foreach ((_, object key) in keys)
            {
                // Invoke the correct instance of the Apply Configuration overloaded method based on the type of the generic
                // parameter. Skip all keys that do not have a value (meaning they were not found or version was not a match).
                try
                {
                    switch (key)
                    {
                        case RfwsKey<bool> boolKey:
                            if (boolKey.HasValue)
                            {
                                ApplyConfiguration(section, boolKey);
                            }
                            break;
                        case RfwsKey<double> doubleKey:
                            if (doubleKey.HasValue)
                            {
                                ApplyConfiguration(section, doubleKey);
                            }
                            break;
                        case RfwsKey<int> intKey:
                            if (intKey.HasValue)
                            {
                                ApplyConfiguration(section, intKey);
                            }
                            break;
                        case RfwsKey<string> stringKey:
                            if (stringKey.HasValue)
                            {
                                ApplyConfiguration(section, stringKey);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (RFmxException rfEx)
                {
                    Log.Error(rfEx, "Error applying property using key {Key}", key);
                }
            }
        }
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<bool> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<double> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<int> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<string> key);
    }
}

using System;
using Serilog;
using Serilog.Context;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Maps settings to the appropriate RFmx properties.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type to be configured</typeparam>
    public abstract class MapperCore<T> where T : ISignalConfiguration
    {
        public T Signal { get; }

        public MapperCore(T signal)
        {
            Signal = signal;
        }

        #region Core Map Functions
        /// <summary>
        /// Represents a mapping operation to translate from an <see cref="RFmxPropertyGroup{T}"/> object to the appropriate RFmx
        /// settings as defined by <see cref="RFmxPropertyMap{T}"/>. 
        /// </summary>
        public virtual void Map(ParsingGroup group)
        {
            using (LogContext.PushProperty("Group", group.GetType().Name))
            {
                try
                {
                    group.CustomConfigure(Signal);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown performing custom configuration for {Group}", group.GetType().Name);
                    return;
                }

                // Discard the attribute; we only need the keys and associated values
                foreach ((FieldInfo field, object property) in group.MappedFields)
                {
                    using (LogContext.PushProperty("Field", field.Name))
                    {
                        try
                        {
                            // Invoke the correct instance of the Apply Configuration overloaded method based on the type of the generic
                            // parameter. Skip all keys that do not have a value (meaning they were not found or version was not a match).
                            ApplyConfigurationCore(group, property);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Exception occured applying property {Property}", property);
                        }
                    }
                }
            }
        }

        private void ApplyConfigurationCore(ParsingGroup group, object property)
        {
            switch (property)
            {
                case RFmxPropertyMap<bool> boolKey:
                    if (boolKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, boolKey);
                    }
                    break;
                case RFmxPropertyMap<double> doubleKey:
                    if (doubleKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, doubleKey);
                    }
                    break;
                case RFmxPropertyMap<int> intKey:
                    if (intKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, intKey);
                    }
                    break;
                case RFmxPropertyMap<string> stringKey:
                    if (stringKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, stringKey);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, RFmxPropertyMap<bool> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, RFmxPropertyMap<double> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, RFmxPropertyMap<int> key);
        /// <summary>
        /// Applies a setting defined by <paramref name="key"/> to the RFmx signal and selector string 
        /// defined in <paramref name="section"/>.
        /// </summary>
        /// <param name="section">Specifies the section, containing the RFmx signal and selector string, to apply the setting to.</param>
        /// <param name="key">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, RFmxPropertyMap<string> key);
        #endregion

    }
}

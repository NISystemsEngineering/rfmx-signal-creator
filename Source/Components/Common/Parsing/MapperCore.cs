using System;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.WaveformParsing
{
    /// <summary>
    /// Represents the core class used to apply parsed values for a property group to an RFmx session. Concrete
    /// implementations will define how the properties are applied to a specific RFmx signal type.
    /// </summary>
    /// <typeparam name="T">Specifies the RFmx signal type to be configured</typeparam>
    public abstract class MapperCore<T> where T : ISignalConfiguration
    {
        /// <summary>Specifies the RFmx signal that is configured by this class.</summary>
        public T Signal { get; }

        protected MapperCore(T signal)
        {
            Signal = signal;
        }

        #region Core Map Functions
        /// <summary>
        /// Reads the <see cref="PropertyMap{T}"/> objects from <paramref name="propertyGroup"/> and applies the values of each property to <see cref="Signal"/>.
        /// </summary>
        public virtual void Map(PropertyGroup propertyGroup)
        {
            using (LogContext.PushProperty("Group", propertyGroup.GetType().Name))
            {
                try
                {
                    // If the property group has a custom configuration applied, invoke it prior to the automatic property setting
                    propertyGroup.CustomConfigure(Signal);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception thrown performing custom configuration for {Group}", propertyGroup.GetType().Name);
                    return;
                }

                // Discard the attribute; we only need the keys and associated values
                foreach ((FieldInfo field, object property) in propertyGroup.MappedFields)
                {
                    using (LogContext.PushProperty("Field", field.Name))
                    {
                        try
                        {
                            // Invoke the correct instance of the Apply Configuration overloaded method based on the type of the generic
                            // parameter. Skip all keys that do not have a value (meaning they were not found or version was not a match).
                            ApplyConfigurationCore(propertyGroup, property);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Exception occured applying property {Property}", property);
                        }
                    }
                }
            }
        }

        private void ApplyConfigurationCore(PropertyGroup group, object property)
        {
            switch (property)
            {
                case PropertyMap<bool> boolKey:
                    if (boolKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, boolKey);
                    }
                    break;
                case PropertyMap<double> doubleKey:
                    if (doubleKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, doubleKey);
                    }
                    break;
                case PropertyMap<int> intKey:
                    if (intKey.HasValue)
                    {
                        ApplyConfiguration(group.SelectorString, intKey);
                    }
                    break;
                case PropertyMap<string> stringKey:
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
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, PropertyMap<bool> property);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, PropertyMap<double> property);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, PropertyMap<int> property);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(string selectorString, PropertyMap<string> property);
        #endregion

    }
}

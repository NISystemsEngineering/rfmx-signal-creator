using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;
using Serilog;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    /// <summary>
    /// Provides the base functionality for serializing an object to an RFmx signal configuration object.
    /// </summary>
    /// <typeparam name="T">Specifies the type of RFmx signal that the child class can serialize to.</typeparam>
    public abstract class RFmxSerializer<T> where T : ISignalConfiguration
    {
        public void Serialize(T signal, string selectorString, object obj)
        {
            SerializeValue(signal, selectorString, obj, null);
        }
        private void SerializeValue(T signal, string selectorString, object obj, RFmxSerializableAttribute attr)
        {
            switch (obj)
            {
                case bool boolValue:
                    ApplyConfiguration(signal, selectorString, boolValue, (RFmxSerializablePropertyAttribute)attr);
                    break;
                case double doubleValue:
                    ApplyConfiguration(signal, selectorString, doubleValue, (RFmxSerializablePropertyAttribute)attr);
                    break;
                case int intValue:
                    ApplyConfiguration(signal, selectorString, intValue, (RFmxSerializablePropertyAttribute)attr);
                    break;
                case Enum enumValue:
                    ApplyConfiguration(signal, selectorString, enumValue, (RFmxSerializablePropertyAttribute)attr);
                    break;
                case string stringValue:
                    ApplyConfiguration(signal, selectorString, stringValue, (RFmxSerializablePropertyAttribute)attr);
                    break;
                case IList list:
                    int i = 0;
                    foreach (object o in list)
                    {
                        string elementSelectorString;
                        if (attr is RFmxSerializableSectionAttribute sectionAttrList)
                        {
                            elementSelectorString = BuildSelectorString(selectorString, sectionAttrList.SelectorString, i);
                        }
                        else elementSelectorString = selectorString;
                        SerializeValue(signal, elementSelectorString, o, null);
                        i++;
                    }
                    break;
                case null:
                    break;
                default:
                    if (attr is RFmxSerializableSectionAttribute sectionAttr)
                    {
                        if (!string.IsNullOrEmpty(sectionAttr.SelectorString))
                        {
                            selectorString = BuildSelectorString(selectorString, sectionAttr.SelectorString, 0);
                        }
                    }
                    SerializeClass(signal, selectorString, obj);
                    break;
            }
        }
        private void SerializeClass(T signal, string selectorString, object obj)
        {
            Type t = obj.GetType();

            var members = t.GetPropertiesAndFields(MemberAccessibility.Readable)
                           .Where(m => m.IsDefined<RFmxSerializableAttribute>());

            foreach (MemberInfo member in members)
            {
                object value = member.GetValue(obj);
                RFmxSerializableAttribute attr = member.GetCustomAttribute<RFmxSerializableAttribute>();
                try
                {
                    SerializeValue(signal, selectorString, value, attr);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error applying member {MemberName} for {SelectorString}", member.Name, selectorString);
                }
            }
        }

        private static string BuildSelectorString(string selectorString, string segment, int index)
        {
            string indexString = index == -1 ? "::all" : index.ToString();

            string combinedSegment = segment + indexString;
            if (string.IsNullOrEmpty(selectorString))
            {
                return combinedSegment;
            }
            else
            {
                return string.Join("/", selectorString, combinedSegment);
            }
        }

        #region Abstract Methods
        /// <summary>
        /// Applies <paramref name="value"/> to the RFmx signal defined by <paramref name="signal"/> and <paramref name="selectorString"/> using the 
        /// RFmx property ID specified in <paramref name="attribute"/>.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal to apply the property to.</param>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="value">Specifies the value to set the property to.</param>
        /// <param name="attribute">Specifies the RFmx property ID to set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, bool value, RFmxSerializablePropertyAttribute attribute);
        /// <summary>
        /// Applies <paramref name="value"/> to the RFmx signal defined by <paramref name="signal"/> and <paramref name="selectorString"/> using the 
        /// RFmx property ID specified in <paramref name="attribute"/>.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal to apply the property to.</param>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="value">Specifies the value to set the property to.</param>
        /// <param name="attribute">Specifies the RFmx property ID to set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, double value, RFmxSerializablePropertyAttribute attribute);
        /// <summary>
        /// Applies <paramref name="value"/> to the RFmx signal defined by <paramref name="signal"/> and <paramref name="selectorString"/> using the 
        /// RFmx property ID specified in <paramref name="attribute"/>.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal to apply the property to.</param>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="value">Specifies the value to set the property to.</param>
        /// <param name="attribute">Specifies the RFmx property ID to set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, int value, RFmxSerializablePropertyAttribute attribute);
        /// <summary>
        /// Applies <paramref name="value"/> to the RFmx signal defined by <paramref name="signal"/> and <paramref name="selectorString"/> using the 
        /// RFmx property ID specified in <paramref name="attribute"/>.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal to apply the property to.</param>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="value">Specifies the value to set the property to.</param>
        /// <param name="attribute">Specifies the RFmx property ID to set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, Enum value, RFmxSerializablePropertyAttribute attribute);
        /// <summary>
        /// Applies <paramref name="value"/> to the RFmx signal defined by <paramref name="signal"/> and <paramref name="selectorString"/> using the 
        /// RFmx property ID specified in <paramref name="attribute"/>.
        /// </summary>
        /// <param name="signal">Specifies the RFmx signal to apply the property to.</param>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="value">Specifies the value to set the property to.</param>
        /// <param name="attribute">Specifies the RFmx property ID to set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, string value, RFmxSerializablePropertyAttribute attribute);
        #endregion
    }

    
}

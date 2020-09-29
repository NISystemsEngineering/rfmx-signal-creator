using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;

namespace NationalInstruments.Utilities.SignalCreator.Seralization
{
    public abstract class RFmxSerializer<T> where T : ISignalConfiguration
    {
        public void Serialize(T signal, string selectorString, object obj)
        {
            SerializeValue(signal, selectorString, obj, null);
        }
        private void SerializeValue(T signal, string selectorString, object obj, RFmxMappableAttribute attr)
        {
            switch (obj)
            {
                case bool boolValue:
                    ApplyConfiguration(signal, selectorString, boolValue, (RFmxMappablePropertyAttribute)attr);
                    break;
                case double doubleValue:
                    ApplyConfiguration(signal, selectorString, doubleValue, (RFmxMappablePropertyAttribute)attr);
                    break;
                case int intValue:
                    ApplyConfiguration(signal, selectorString, intValue, (RFmxMappablePropertyAttribute)attr);
                    break;
                case Enum enumValue:
                    int convertedValue = Convert.ToInt32(enumValue);
                    ApplyConfiguration(signal, selectorString, convertedValue, (RFmxMappablePropertyAttribute)attr);
                    break;
                case string stringValue:
                    ApplyConfiguration(signal, selectorString, stringValue, (RFmxMappablePropertyAttribute)attr);
                    break;
                case IList list:
                    int i = 0;
                    foreach (object o in list)
                    {
                        string elementSelectorString;
                        if (attr is RFmxMappableSectionAttribute sectionAttrList)
                        {
                            elementSelectorString = BuildSelectorString(selectorString, sectionAttrList.SelectorString, i);
                        }
                        else elementSelectorString = selectorString;
                        SerializeValue(signal, elementSelectorString, o, null);
                        i++;
                    }
                    break;
                default:
                    if (attr is RFmxMappableSectionAttribute sectionAttr)
                    {
                        selectorString = BuildSelectorString(selectorString, sectionAttr.SelectorString, 0);
                    }
                    SerializeClass(signal, selectorString, obj);
                    break;
            }
        }
        private void SerializeClass(T signal, string selectorString, object obj)
        {
            Type t = obj.GetType();

            bool hasAttribute = t.IsDefined<RFmxMappableSectionAttribute>();

            /*string segmentSelectorString = hasAttribute ?
                t.GetCustomAttribute<RFmxMappableSectionAttribute>().SelectorString : string.Empty;*/

            var members = t.GetPropertiesAndFields(MemberAccessibility.Readable)
                           .Where(m => m.IsDefined<RFmxMappableAttribute>());
            foreach (MemberInfo member in members)
            {
                object value = member.GetValue(obj);
                RFmxMappableAttribute attr = member.GetCustomAttribute<RFmxMappableAttribute>();
                SerializeValue(signal, selectorString, value, attr);
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
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, bool value, RFmxMappablePropertyAttribute attribute);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, double value, RFmxMappablePropertyAttribute attribute);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, int value, RFmxMappablePropertyAttribute attribute);
        /// <summary>
        /// Applies a setting defined by <paramref name="property"/> to the RFmx signal and selector string .
        /// </summary>
        /// <param name="selectorString">Specifies the selector string to apply the proprety to.</param>
        /// <param name="property">Specifies the RFmx parameter ID and value to be set.</param>
        protected abstract void ApplyConfiguration(T signal, string selectorString, string value, RFmxMappablePropertyAttribute attribute);
        #endregion
    }

    
}

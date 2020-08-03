using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public abstract class Parser
    {
        public virtual void ParseGroup(RFmxPropertyGroup group)
        {
            #region Parse Keys
            var fields = group.FetchMappedFields();

            foreach (var field in fields)
            {
                if (ValidateProperty(field))
                {
                    try
                    {
                        object value = FetchValue(field);
                        //Log.Verbose("Parsed key {key} with value {value}", attr.Key, value);
                        try
                        {
                            switch (field.Value)
                            {
                                case RFmxPropertyMap<bool> boolKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (boolKey.CustomMap == null)
                                        boolKey.Value = ParseValue<bool>(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        boolKey.Value = boolKey.CustomMap(value);
                                    break;
                                case RFmxPropertyMap<double> doubleKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (doubleKey.CustomMap == null)
                                        doubleKey.Value = ParseValue<double>(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        doubleKey.Value = doubleKey.CustomMap(value);
                                    break;
                                case RFmxPropertyMap<int> intKey:
                                    // If delegate is not set, then just directly parse the value
                                    if (intKey.CustomMap == null)
                                        intKey.Value = ParseValue<int>(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        intKey.Value = intKey.CustomMap(value);
                                    break;
                                case RFmxPropertyMap<string> stringKey:
                                    // If delegate is not set, then just directly pass the value
                                    if (stringKey.CustomMap == null)
                                        stringKey.Value = ParseValue<string>(value);
                                    // Otherwise, invoke the delgate to manually map the value
                                    else
                                        stringKey.Value = stringKey.CustomMap(value);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        catch (Exception ex)
                        {
                            //Log.Error(ex, "Error parsing key {Key}.", attr.Key);
                        }
                    }
                    catch (KeyNotFoundException k)
                    {
                        //Log.Warning("Expected to find key {KeyName} but it was not found", attr.Key);
                    }
                }
                else
                {
                    //Log.Debug("{KeyName} supporting version(s) {Versions} did not match section version {SectionVersion}",
                        //attr.Key, attr.Versions, rfwsSection.Version);
                }

            }
            #endregion
        }
        protected virtual bool ValidateProperty(FieldValuePair field) => true;
        protected abstract object FetchValue(FieldValuePair field);
        protected virtual T ParseValue<T>(object value) => (T)value;
    }
}

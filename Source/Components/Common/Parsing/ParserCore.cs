using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Serilog;
using Serilog.Context;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public abstract class ParserCore
    {
        #region Parse Functions
        public virtual void Parse(ParsingGroup group)
        {
            using (LogContext.PushProperty("Group", group.GetType().Name))
            {
                foreach (var field in group.MappedFields)
                {
                    try
                    {
                        if (ValidateProperty(group, field))
                        {
                            try
                            {
                                object value = FetchValue(group, field);
                                Log.Verbose("Read property {Property} with value {Value}", field.Field.Name, value);
                                try
                                {
                                    ParseAndApplyValue(field, value);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex, "Error parsing property {Property}; attempted to parse {Value} but operation failed",
                                        field.Field.Name, value);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to fetch value for {Property}", field.Field.Name);
                            }
                        }
                        else
                        {
                            LogFailedPropertyValidation(group, field);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error validating property {Property}", field.Field.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        private void ParseAndApplyValue(FieldValuePair field, object value)
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
                    throw new NotImplementedException($"Key type {field.Value.GetType()} not supported");
            }
        }

        protected virtual bool ValidateProperty(ParsingGroup group, FieldValuePair field) => true;
        protected abstract object FetchValue(ParsingGroup group, FieldValuePair field);
        protected virtual T ParseValue<T>(object value) => (T)value;
        #endregion


        #region Optional Log Functions
        protected virtual void LogFailedPropertyValidation(ParsingGroup group, FieldValuePair field)
        {
            Log.Debug("{KeyName} skipped because it did not pass the validation check.", field.Field.Name);
        }
        #endregion
    }
}

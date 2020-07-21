using NationalInstruments.RFmx.InstrMX;
using System;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public abstract class RfmxMapper<T> where T : ISignalConfiguration
    {
        public void MapSection(RfwsSection<T> section)
        {
            var keys = RfwsParserUtilities.FetchSectionKeys(section);

            foreach ((_, object key) in keys)
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
        }

        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<bool> key);
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<double> key);
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<int> key);
        protected abstract void ApplyConfiguration(RfwsSection<T> section, RfwsKey<string> key);
    }
}

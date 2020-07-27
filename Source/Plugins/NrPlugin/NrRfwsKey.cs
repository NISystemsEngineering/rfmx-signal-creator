namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    public enum RfmxNrSelectorStringType
    {
        Default,
        Subblock,
        None
    }
    public class NrRfwsKey<T> : RfwsKey<T>
    {
        public RfmxNrSelectorStringType SelectorStringType;
    }
}

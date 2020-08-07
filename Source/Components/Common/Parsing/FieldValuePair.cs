using System.Reflection;

namespace NationalInstruments.Utilities.WaveformParsing
{
    public readonly struct FieldValuePair
    {
        public FieldInfo Field { get; }
        public object Value { get; }

        public FieldValuePair(FieldInfo field, object fieldOwner)
        {
            Field = field;
            Value = field.GetValue(fieldOwner);
        }

        public void Deconstruct(out FieldInfo field, out object value)
        {
            field = Field;
            value = Value;
        }
    }
}

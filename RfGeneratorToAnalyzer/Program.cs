using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using NationalInstruments.RFmx.InstrMX;
using System.IO;

namespace NationalInstruments.Utilities.WaveformParsing.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\mwhitten\Desktop\20.0 File.rfws";

            var waveform = WaveformConfigFile.Load(path);
            Plugins.NrRfwsParser nr = new Plugins.NrRfwsParser();

            Console.WriteLine(nr.CanParse(waveform));

            RFmxInstrMX instr = new RFmxInstrMX("", "AnalysisOnly=1");
            nr.Parse(waveform, instr);

            string savedPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + "_RFmx_Config.tdms");

            instr.SaveAllConfigurations(savedPath);

            /*XElement root = XElement.Load(path);

            var carriers = root.Descendants("section").Where(e => (string)e.Attribute("name") == "CarrierSetManager");
            foreach (var e in carriers)
            {
                Console.WriteLine(e);
            }*/
            Console.ReadKey();
            instr.Dispose();
        }
    }
    /*
    class Configuration<T>
    {
        public string XmlTag;
        public int PropertyId;
        public T Value;
        public Action ParsingObject;
    }
    public class Subblock
    {
        [MyAttribute]
        public static Configuration<int> numCarriers;
    }
    public class ComponentCarrier
    {
        public Configuration<double> Bandwidth = new Configuration<double>
        {
            XmlTag = "x"
        };
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class XmlConfigurationAttribute : Attribute
    {
        //Class Members
    }

    public class CarrierSet
    {
        public ComponentCarrier[] carriers;

        [MyAttribute]
        public Configuration<int> numcarriersets;

        [MyAttribute]
        public Configuration<int> ReallySlow;

        public void Configure()
        {
            Type t = typeof(ComponentCarrier);
            t.GetCustomAttributes<XmlConfigurationAttribute>().Select( e => e.)

            Configuration<int> result = ReadNode(Subblock.numCarriers);
            ApplyConfiugration(result);
            carrires = new ComponentCarrier[result.Value];


        }

    }
*/
}

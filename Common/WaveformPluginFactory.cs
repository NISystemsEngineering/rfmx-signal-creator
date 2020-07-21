using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace NationalInstruments.Utilities.WaveformParsing
{
    // Adapted from https://www.c-sharpcorner.com/article/introduction-to-building-a-plug-in-architecture-using-C-Sharp/
    public static class WaveformPluginFactory
    {
        private static List<IWaveformFilePlugin> plugins;
        public static List<IWaveformFilePlugin> LoadedPlugins
        {
            get
            {
                if (plugins == null)
                {
                    LoadPlugins();
                }
                return plugins;
            }
        }

        private static List<Assembly> LoadPlugInAssemblies()
        {
            DirectoryInfo dInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Plugins"));
            FileInfo[] files = dInfo.GetFiles("*.dll");
            List<Assembly> plugInAssemblyList = new List<Assembly>();

            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    plugInAssemblyList.Add(Assembly.LoadFrom(file.FullName));
                }
            }

            return plugInAssemblyList;

        }

        private static List<IWaveformFilePlugin> GetPlugins(List<Assembly> assemblies)
        {
            List<Type> availableTypes = new List<Type>();

            //var availableTypes = from assembly in assemblies
            //                     select assembly.GetTypes()


            foreach (Assembly currentAssembly in assemblies)
            {
                try
                {
                    availableTypes.AddRange(currentAssembly.GetTypes());
                }
                catch (ReflectionTypeLoadException typeException)
                {
                    Console.WriteLine(typeException.Message);
                }
            }
                

            var result = from type in availableTypes
                   where type.GetInterface(nameof(IWaveformFilePlugin)) != null
                   //where type.IsDefined(typeof(WaveformFilePlugInAttribute))
                   let plugin = (IWaveformFilePlugin) Activator.CreateInstance(type)
                   select plugin;

            return result.ToList();
            /*
            // get a list of objects that implement the ICalculator interface AND 
            // have the CalculationPlugInAttribute


            List < Type > calculatorList = availableTypes.FindAll(delegate (Type t)
               {
                   List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
                   object[] arr = t.GetCustomAttributes(typeof(CalculationPlugInAttribute), true);
                   return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(ICalculator));
               });

            // convert the list of Objects to an instantiated list of ICalculators
            return calculatorList.ConvertAll<ICalculator>(delegate (Type t) { return Activator.CreateInstance(t) as ICalculator; });
            */
        }

        public static void LoadPlugins()
        {
            List<Assembly> assemblies = LoadPlugInAssemblies();
            plugins = GetPlugins(assemblies);

            if (plugins.Count() <= 0) throw new DllNotFoundException("No matching waveform plugins were found.");
        }
    }
}

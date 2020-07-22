using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    // Adapted from https://www.c-sharpcorner.com/article/introduction-to-building-a-plug-in-architecture-using-C-Sharp/
    public static class WaveformPluginFactory
    {
        public static List<IWaveformFilePlugin> LoadedPlugins { get; private set; }

        private static List<Assembly> LoadPlugInAssemblies()
        {
            string pluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
            Log.Verbose($"Loading plugins from path {pluginPath}");

            DirectoryInfo dInfo = new DirectoryInfo(pluginPath);
            FileInfo[] files = dInfo.GetFiles("*.dll");
            List<Assembly> plugInAssemblyList = new List<Assembly>();

            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    plugInAssemblyList.Add(Assembly.LoadFrom(file.FullName));
                    Log.Verbose("Loaded file {file}", file.FullName);
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
                    Log.Error("Error loading plugin {PluginName} with exceptions {LoaderExceptions}", currentAssembly.FullName,
                        typeException.LoaderExceptions);
                }
            }

            // Filter the loaded assemblies by those implementing the plugin interface
            var filteredTypes = from type in availableTypes
                                where type.GetInterface(nameof(IWaveformFilePlugin)) != null
                                //where type.IsDefined(typeof(WaveformFilePlugInAttribute))
                                select type;

            List<IWaveformFilePlugin> loadedPlugins = new List<IWaveformFilePlugin>();

            foreach (Type t in filteredTypes)
            {
                try
                {
                    IWaveformFilePlugin plugin = (IWaveformFilePlugin)Activator.CreateInstance(t);
                    loadedPlugins.Add(plugin);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating plugin {PluginName}", t.Name);
                }

            }

            return loadedPlugins;
        }

        public static void LoadPlugins()
        {
            List<Assembly> assemblies = LoadPlugInAssemblies();
            LoadedPlugins = GetPlugins(assemblies);

            if (LoadedPlugins.Count <= 0) throw new DllNotFoundException("No matching waveform plugins were found.");
            else
            {
                Log.Debug("Loaded plugins {plugins}", LoadedPlugins);
            }
        }
    }
}

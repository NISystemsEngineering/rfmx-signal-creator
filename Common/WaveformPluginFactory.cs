using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    // Adapted from https://www.c-sharpcorner.com/article/introduction-to-building-a-plug-in-architecture-using-C-Sharp/
    /// <summary>
    /// Factory for dynamic loading of parser plugins at runtime 
    /// </summary>
    public static class WaveformPluginFactory
    {
        public const string PluginDirectory = "Plugins";
        public static string FullPluginDirectoryPath => Path.Combine(Environment.CurrentDirectory, PluginDirectory);

        static List<IWaveformFilePlugin> _loadedPlugins;

        /// <summary>
        /// Returns the cached plugins loaded from the plugins directory. If no plugins have been loaded, <see cref="LoadPlugins"/>,
        /// will be invoked before returning any loaded plugins.
        /// </summary>
        public static List<IWaveformFilePlugin> LoadedPlugins
        {
            get
            {
                if (_loadedPlugins == null)
                {
                    LoadPlugins();
                }
                return _loadedPlugins;
            }
        }
        /// <summary>
        /// Loads all assemblies from the plugin directory.
        /// </summary>
        private static List<Assembly> LoadPlugInAssemblies()
        {
            Log.Verbose("Loading plugins from path {PluginPath}", FullPluginDirectoryPath);

            List<Assembly> plugInAssemblyList = new List<Assembly>();
            string[] files = Directory.GetFiles(FullPluginDirectoryPath, "*.dll");

            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.LoadFrom(file);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex, "Error loading assembly {File}", file);
                        break;
                    }
                    plugInAssemblyList.Add(assembly);
                    Log.Debug("Loaded assembly {file}", file);
                }
            }
            else
            {
                throw new DllNotFoundException("No assemblies are present in the plugin directory");
            }

            return plugInAssemblyList;
        }

        private static List<IWaveformFilePlugin> GetPlugins(List<Assembly> assemblies)
        {
            List<Type> availableTypes = new List<Type>();

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

            // Filter the loaded assemblies by those implementing the plugin interface and attribute
            var filteredTypes = from type in availableTypes
                                where type.GetInterface(nameof(IWaveformFilePlugin)) != null
                                where type.IsDefined(typeof(WaveformFilePlugInAttribute))
                                select type;

            List<IWaveformFilePlugin> loadedPlugins = new List<IWaveformFilePlugin>();

            foreach (Type t in filteredTypes)
            {
                try
                {
                    // Create a new instance of the plugin and add to the list
                    IWaveformFilePlugin plugin = (IWaveformFilePlugin)Activator.CreateInstance(t);
                    loadedPlugins.Add(plugin);

                    WaveformFilePlugInAttribute attr = t.GetCustomAttribute<WaveformFilePlugInAttribute>();
                    Log.Debug("Loaded plugin {PluginName}: \"{Description}\"", t.Name, attr.Description);
                    Log.Debug("{PluginName} supports RFmx version(s): {Versions}", t.Name, attr.RFmxVersions);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating plugin {PluginName}", t.Name);
                }

            }

            return loadedPlugins;
        }

        /// <summary>
        /// Loads all plugins implementing <see cref="IWaveformFilePlugin"/> and <see cref="WaveformFilePlugInAttribute"/> 
        /// from the plugin directory (<see cref="PluginDirectory"/>) and caches them in <see cref="LoadedPlugins"/>.
        /// </summary>
        /// <exception cref="DllNotFoundException">Thrown when no DLLs are found in the plugin directory.</exception>
        /// <exception cref="MissingMemberException">Throdwn when no plugins implementing the expected interface and attribute are found.</exception>
        public static void LoadPlugins()
        {
            Log.Debug("Loading plugins");
            List<Assembly> assemblies = LoadPlugInAssemblies();
            _loadedPlugins = GetPlugins(assemblies);

            if (LoadedPlugins.Count <= 0)
                throw new MissingMemberException("No matching waveform plugins were found.");
        }
    }
}

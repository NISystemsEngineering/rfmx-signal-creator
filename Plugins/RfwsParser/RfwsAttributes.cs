using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RfwsSectionAttribute : Attribute
    {
        public string sectionName;
        public string version;
        public bool regExMatch = false;

        public RfwsSectionAttribute(string sectionName) => this.sectionName = sectionName;

        public void Deconstruct(out string sectionName, out string version, out bool regexMatch)
        {
            sectionName = this.sectionName;
            version = this.version;
            regexMatch = this.regExMatch;
        }
    }

    public enum RfswVersionMode
    {
        SupportedVersionsAndLater,
        SpecificVersions,
        AllVersions
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class RfwsPropertyAttribute : Attribute
    {
        public string Key { get; } 
        public float[] Versions { get; }
        public RfswVersionMode VersionMode { get; } = RfswVersionMode.SpecificVersions;

        public RfwsPropertyAttribute(string keyName)
        {
            Key = keyName;
            // No version is specfied, so ensure that all versions are supported
            VersionMode = RfswVersionMode.AllVersions;
        }
        public RfwsPropertyAttribute(string keyName, float version, RfswVersionMode versionMode = RfswVersionMode.SupportedVersionsAndLater)
        {
            Key = keyName;
            Versions = new float[1] { version };
            VersionMode = versionMode;
        }
        public RfwsPropertyAttribute(string keyName, RfswVersionMode versionMode, params float[] versions)
        {
            Key = keyName;
            Versions = versions;
            VersionMode = versionMode;
        }

        public void Deconstruct(out string key, out float[] versions, out RfswVersionMode versionMode)
        {
            key = this.Key;
            versions = this.Versions;
            versionMode = this.VersionMode;
        }
    }
}

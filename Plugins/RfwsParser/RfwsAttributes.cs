using System;

namespace NationalInstruments.Utilities.WaveformParsing.Plugins
{
    /// <summary>
    /// Maps a class to a specific section and version contained within an RFWS file. 
    /// Only valid on <see cref="RfwsSection{T}"/> classes and derived classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RfwsSectionAttribute : Attribute
    {
        /// <summary>
        /// Specifies the value of the "name" attribute of the section element.
        /// </summary>
        public string sectionName;
        /// <summary>
        /// Specifies valid versions in the "version" attribute of the section element.
        /// </summary>
        public string version;
        /// <summary>
        /// Indicates whether <see cref="sectionName"/> is a regular expression and to be matched as such.
        /// </summary>
        public bool regExMatch = false;

        public RfwsSectionAttribute(string sectionName) => this.sectionName = sectionName;

        public void Deconstruct(out string sectionName, out string version, out bool regexMatch)
        {
            sectionName = this.sectionName;
            version = this.version;
            regexMatch = this.regExMatch;
        }
    }
    /// <summary>
    /// Specifies how version information should be handled when matching a section or key in the RFWS file.
    /// </summary>
    public enum RfswVersionMode
    {
        /// <summary>Indicates that versions equal to or greater than specified may be considered valid.</summary>
        SupportedVersionsAndLater,
        /// <summary>Indicates that only specific versions specified can be considered valid.</summary>
        SpecificVersions,
        /// <summary>Indicates that the version should be ignored for the property.</summary>
        AllVersions
    }

    /// <summary>
    /// Maps a class to a specific key and version contained within an RFWS file.
    /// Only valid on <see cref="RfwsKey{T}"/> classes and derived classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class RfwsPropertyAttribute : Attribute
    {
        /// <summary>
        /// Specifies the value of the "name" attribute of the key element.
        /// </summary>
        public string Key { get; } 
        /// <summary>
        /// Specifies one or more valid versions for this key as defined by the parent section. This list will be 
        /// interpreted based upon the value of <see cref="VersionMode"/>.
        /// </summary>
        public float[] Versions { get; }
        /// <summary>
        /// Specifies how the version numbers specified in <see cref="Versions"/> should be interpreted when attempting to match
        /// a class with an RFWS key.
        /// </summary>
        public RfswVersionMode VersionMode { get; } = RfswVersionMode.SpecificVersions;

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        public RfwsPropertyAttribute(string keyName)
        {
            Key = keyName;
            // No version is specfied, so ensure that all versions are supported
            VersionMode = RfswVersionMode.AllVersions;
        }

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        /// <param name="version">Specifies a single version that this attribute supports.</param>
        /// <param name="versionMode">Optional; with a single version set, it is assumed that this version and later should be supported.<para></para>;
        /// Specifies how the version numbers specified in <see cref="Versions"/> should be interpreted when attempting to match
        /// a class with an RFWS key.</param>
        public RfwsPropertyAttribute(string keyName, float version, RfswVersionMode versionMode = RfswVersionMode.SupportedVersionsAndLater)
        {
            Key = keyName;
            Versions = new float[1] { version };
            VersionMode = versionMode;
        }
        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        /// <param name="versionMode">Specifies how the version numbers specified in <paramref name="versions"/> should 
        /// be interpreted when attempting to match a class with an RFWS key.</param>
        /// <param name="versions">Specifies one or more valid versions for this key as defined by the parent section.</param>
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

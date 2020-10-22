using System;
using System.Linq;

namespace NationalInstruments.Utilities.SignalCreator.Serialization
{
    using Serialization;

    /// <summary>
    /// Indicates that a field or property within a class is representative of an XML section within the RFWS file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RfwsDeserializableSectionAttribute : DeserializableAttribute
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

        public RfwsDeserializableSectionAttribute(string sectionName) => this.sectionName = sectionName;

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
    public enum RfwsVersionMode
    {
        /// <summary>Indicates that versions equal to or greater than specified may be considered valid.</summary>
        SupportedVersionsAndLater,
        /// <summary>Indicates that only specific versions specified can be considered valid.</summary>
        SpecificVersions,
        /// <summary>Indicates that the version should be ignored for the property.</summary>
        AllVersions
    }

    /// <summary>
    /// Indicates that a field or property within a class is representative of an XML key within the RFWS file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class RfwsDeserializableKeyAttribute : DeserializableAttribute
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
        public RfwsVersionMode VersionMode { get; } = RfwsVersionMode.SpecificVersions;

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        public RfwsDeserializableKeyAttribute(string keyName)
        {
            Key = keyName;
            // No version is specfied, so ensure that all versions are supported
            VersionMode = RfwsVersionMode.AllVersions;
        }

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        /// <param name="minimumVersion">Specifies a single version that this attribute supports.</param>
        /// <param name="versionMode">Optional; with a single version set, it is assumed that this version and later should be supported.<para></para>;
        /// Specifies how the version numbers specified in <see cref="Versions"/> should be interpreted when attempting to match
        /// a class with an RFWS key.</param>
        public RfwsDeserializableKeyAttribute(string keyName, float minimumVersion, RfwsVersionMode versionMode = RfwsVersionMode.SupportedVersionsAndLater)
        {
            Key = keyName;
            Versions = new float[1] { minimumVersion };
            VersionMode = versionMode;
        }

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        /// <param name="versionMode">Specifies how the version numbers specified in <paramref name="versions"/> should 
        /// be interpreted when attempting to match a class with an RFWS key.</param>
        /// <param name="versions">Specifies one or more valid versions for this key as defined by the parent section.</param>
        public RfwsDeserializableKeyAttribute(string keyName, RfwsVersionMode versionMode, params float[] versions)
        {
            Key = keyName;
            Versions = versions;
            VersionMode = versionMode;
        }

        /// <summary>
        /// Determines whether <paramref name="versionToCheck"/> is compatible with the combination of <see cref="Versions"/> and
        /// the <see cref="VersionMode"/> setting.
        /// </summary>
        /// <param name="versionToCheck"></param>
        /// <returns></returns>
        public bool IsSupported(float versionToCheck)
        {
            switch (VersionMode)
            {
                case RfwsVersionMode.AllVersions:
                    return true;
                case RfwsVersionMode.SpecificVersions:
                    return Versions.Contains(versionToCheck);
                case RfwsVersionMode.SupportedVersionsAndLater:
                    return (from supportedVersion in Versions
                            select versionToCheck >= supportedVersion)
                            .Aggregate((current, next) => current |= next);
                default:
                    return false;
            }
        }
    }
}

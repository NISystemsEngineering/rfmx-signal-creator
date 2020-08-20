using System;
using System.Linq;

namespace NationalInstruments.Utilities.SignalCreator
{
    /// <summary>
    /// Maps a <see cref="RfwsSection"/> class to a specific section and version contained within an RFWS file. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class RfwsSectionAttribute : Attribute
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
    /// Maps a <see cref="RfwsSectionList{T}"/> class to a specific section and version contained within an RFWS file. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class RfwsSectionListAttribute : RfwsSectionAttribute
    {
        /// <summary>
        /// Specifies the hierarchy of this section list. When the <see cref="RfwsSectionList{T}"/> is parsed, if this is true
        /// then the object will be initialized with the same section as its parent. Otherwise, the specific section will be searched for
        /// and used as the parent section.
        /// <para></para>
        /// This is necessary because some section lists in the RFWS file are contained with a higher-level subsection (i.e. CarrierManager).
        /// Howevever, some of the section lists begin without any subsection at all.
        /// </summary>
        public bool SameSectionAsParent { get; } = true;

        public RfwsSectionListAttribute(string sectionName)
            : base(sectionName) 
        {
            SameSectionAsParent = false;
        }
        public RfwsSectionListAttribute()
            : base("")
        {
            SameSectionAsParent = true;
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
    /// Maps a class to a specific key and version contained within an RFWS file.
    /// Only valid on <see cref="PropertyMap{T}"/> classes and derived classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
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
        public RfwsVersionMode VersionMode { get; } = RfwsVersionMode.SpecificVersions;

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        public RfwsPropertyAttribute(string keyName)
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
        public RfwsPropertyAttribute(string keyName, float minimumVersion, RfwsVersionMode versionMode = RfwsVersionMode.SupportedVersionsAndLater)
        {
            Key = keyName;
            Versions = new float[1] { minimumVersion };
            VersionMode = versionMode;
        }

        /// <param name="keyName">Specifies the value of the "name" attribute of the key element.</param>
        /// <param name="versionMode">Specifies how the version numbers specified in <paramref name="versions"/> should 
        /// be interpreted when attempting to match a class with an RFWS key.</param>
        /// <param name="versions">Specifies one or more valid versions for this key as defined by the parent section.</param>
        public RfwsPropertyAttribute(string keyName, RfwsVersionMode versionMode, params float[] versions)
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

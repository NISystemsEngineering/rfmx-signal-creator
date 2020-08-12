using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Serilog;

namespace NationalInstruments.Utilities.WaveformParsing
{
    // -------------------------------------------------
    // Example Section for this Class
    // -------------------------------------------------
    // <section name="CarrierManager" version="3">
    //      <key name="count">2</key>
    //      <section name="0" version="1">
    //              <section name="Carrier" version="3">
    //                  ...
    //              </section>
    //      </section>
    //      <section name = "1" version="1">
    //              <section name="Carrier" version="3">
    //                  ...
    // -------------------------------------------------


    /// <summary>
    /// Represents a section of an RFWS file that contains an ordered list of subsections of the same type.
    /// <para></para>
    /// This class serves as a simple wrapper around a list while also inheriting from <see cref="RfwsSection"/> so that
    /// it can be parsed alongside the other sections.
    /// </summary>
    /// <typeparam name="T">Specifies the type of each subsection element</typeparam>
    public class RfwsSectionList<T>: RfwsSection, IReadOnlyList<T>
        where T : RfwsSection
    {
        private List<T> _containedSections;

        public RfwsSectionList(XElement childSection, RfwsSection parentGroup)
            : base(childSection, parentGroup)
        {
            _containedSections = new List<T>();

            // Find all of the items contained within this list section and intialize their values in the list
            foreach (var section in RfwsParserUtilities.FindSections(SectionRoot, typeof(T)))
            {
                T newInstance = (T)Activator.CreateInstance(typeof(T), section, this);
                _containedSections.Add(newInstance);
            }
            if (_containedSections.Count == 0)
            {
                // Not a warning because some sections only exist in a certain configuration
                Log.Debug("No section(s) were found for the section list of type {SectionType}", typeof(T).Name);
            }
        }

        // Join any subsections from a derived instance of this class with the ordered list of subsections it contains
        public sealed override IEnumerable<RfwsSection> SubSections
            => base.SubSections.Union(_containedSections); 

        #region IReadOnlyList Impelementations
        public T this[int index] => ((IReadOnlyList<T>)_containedSections)[index];

        public int Count => ((IReadOnlyList<T>)_containedSections).Count;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IReadOnlyList<T>)_containedSections).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<T>)_containedSections).GetEnumerator();
        }
        #endregion
    }
}

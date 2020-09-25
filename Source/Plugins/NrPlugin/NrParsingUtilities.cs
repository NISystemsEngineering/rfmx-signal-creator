using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin
{
    public static class NrParsingUtilities
    {
        public static IReadOnlyList<(int offset, int numRbs)> ParseRbAllocationString(string rbAllocation)
        {
            if (Regex.IsMatch(rbAllocation, @"[^\d,-:last]"))
            {
                throw new ArgumentException("One or more invalid characters are specified", nameof(rbAllocation));
            }

            // Split the string based on the commas
            string[] elements = rbAllocation.Split(',');
            // Inside this array, there will be two types of strings:
            // "0:10", "30:last", or "1-30" or single elements "0", "5", etc.
            // Separate each element into a lookup with the match success as the criteria
            var split = (from element in elements
                         let match = Regex.Match(element, @"(\d+)[:-](\d+|last)")
                         select (element, match))
                         .ToLookup(val => val.match.Success);


            var result = new List<(int offset, int numRbs)>();

            // Iterate through all elements that are of the list format "start:stop"
            foreach ((_, Match m) in split[true])
            {
                int start = int.Parse(m.Groups[1].Value);
                string stopRaw = m.Groups[2].Value;

                int clusterLen;
                if (string.Equals(stopRaw, "last", StringComparison.CurrentCultureIgnoreCase))
                {
                    clusterLen = -1;
                }
                else
                {
                    clusterLen = int.Parse(stopRaw) - start + 1;
                }

                result.Add((start, clusterLen));
            }

            // Now process the single element values
            List<int> singleRbs = (from value in split[false]
                                   select int.Parse(value.element)).ToList();
            singleRbs.Sort();

            if (singleRbs.Count > 0)
            {
                // This segment groups consecutive RBs into offset/number groups. The list has already been sorted.
                var currentSequence = (IEnumerable<int>)singleRbs;
                do
                {
                    // Get the starting value at index currentIndex
                    int start = currentSequence.First();
                    // Build a sequence that contains all consecutive elements. This is accomplished by iterating 
                    // through each element, and comparing its value (f) to the current iteration (i) plus the start value.
                    // Since the list is sorted, consecutive numbers will be equal to the iteration plus the start value. As soon 
                    // as this predicate is no longer true the sequence stops, and this consecutive sequence is counted.
                    int clusterLen = currentSequence.TakeWhile((f, i) => i + start == f).Count();

                    result.Add((start, clusterLen));

                    // Move the sequence to next non-consecutive element to continue grouping
                    currentSequence = currentSequence.Skip(clusterLen);
                } while (currentSequence.Any()); // Continue iterating until the current sequence is empty
            }

            // Order the combined results of both types by the starting offset number
            result.Sort((x, y) => x.offset.CompareTo(y.offset));

            return result.AsReadOnly();
        }
    }
}

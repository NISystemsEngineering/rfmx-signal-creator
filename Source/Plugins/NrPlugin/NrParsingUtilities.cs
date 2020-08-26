using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.SignalCreator.Plugins
{
    public static class NrParsingUtilities
    {
        public static IReadOnlyList<(int offset, int numRbs)> ParseRbAllocationString(string rbAllocation)
        {
            string[] elements = rbAllocation.Split(',');
            var split = (from element in elements
                         let match = Regex.Match(element, @"(\d+):(\d+|last)")
                         select (element, match))
                         .ToLookup(val => val.match.Success);


            var result = new List<(int offset, int numRbs)>();

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

            List<int> singleRbs = (from value in split[false]
                                   select int.Parse(value.element)).ToList();
            singleRbs.Sort();

            if (singleRbs.Count > 0)
            {
                int currentIndex = 0;
                do
                {
                    var currentSequence = singleRbs.Skip(currentIndex);
                    int start = currentSequence.First();
                    int clusterLen = currentSequence.TakeWhile((f, i) => i + start == f).Count();

                    result.Add((start, clusterLen));

                    currentIndex += clusterLen;
                } while (currentIndex < singleRbs.Count);
            }

            result.Sort((x, y) => x.offset.CompareTo(y.offset));

            return result.AsReadOnly();
        }
    }
}

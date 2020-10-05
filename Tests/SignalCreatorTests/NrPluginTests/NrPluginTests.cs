using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NationalInstruments.Utilities.SignalCreator.Plugins;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections;
using NationalInstruments.Utilities.SignalCreator.Plugins.NrPlugin;

namespace NrPluginTests
{
    [TestClass]
    public class NrPluginTests
    {
        [TestMethod]
        public void ParseRbAllocationStringTests()
        {
            List<(int,int)> MkList(params (int,int)[] items)
            {
                return new List<(int, int)>(items);
            }

            var testCases = new Dictionary<string, List<(int, int)>>
            {
                ["0:last"] = MkList((0,-1)),
                ["10:30"] = MkList((10,21)),
                ["0,1,2,3,4,5"] = MkList((0,6)),
                ["50:last,27:30,0,1,2,5,9"] = MkList((0,3),(5,1),(9,1),(27,4),(50,-1)),
                ["1,4-30,39-50,3"] = MkList((1,1),(3,1),(4,27),(39,12))
            };
            
            foreach (var pair in testCases)
            {
                var result = NrParsingUtilities.ParseRbAllocationString(pair.Key);
                result.Should().ContainInOrder(pair.Value);
            }
        }
        [TestMethod]
        public void InvalidRbAllocationStringTest()
        {
            FluentActions.Invoking(() => NrParsingUtilities.ParseRbAllocationString("0:1,2-5,3,7,p")).
                            Should().Throw<ArgumentException>("invalid allocation string");


        }
    }
}

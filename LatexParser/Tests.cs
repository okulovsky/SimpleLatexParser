using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    [TestFixture]
    class Tests
    {
        public void Run(string latex, params IBlock[] entry)
        {
            var expected = new Sequence(SequenceType.Free, entry);
            var given = new Level1Parser().Parse(latex);
            Assert.IsTrue(expected.Equals(given));
        }
        
       
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    [TestFixture]
    public class CommandAssemblingTests
    {
        public void Run(string latex, params IBlock[] entry)
        {
            var expected = new Sequence(SequenceType.Free, entry);
            var given = new Layer2Parser().Parse(latex);
            Assert.IsTrue(expected.Equals(given));
        }
        [Test]
        public void OneArgumentCommand()
        {
            Run("\\cmd{abc}", Latex.Command("cmd", Latex.Curly(Latex.Text("abc"))));
        }


        [Test]
        public void TwoArgumentCommand()
        {
            Run("\\cmd{abc}{def}", Latex.Command("cmd", Latex.Curly(Latex.Text("abc")), Latex.Curly(Latex.Text("def"))));
        }
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    [TestFixture]
    class Level1Tests
    {
        public void Run(string latex, params IBlock[] entry)
        {
            var expected = new Sequence(SequenceType.Free, entry);
            var given = new Level1Parser().Parse(latex);
            Assert.IsTrue(expected.Equals(given));
        }
        
        [Test]
       public void JustText()
        {
            Run("abcd", Latex.Text("abcd"));
        }

        [Test]
        public void JustComment()
        {
            Run("%abcd", Latex.Comment("abcd"));
        }

        [Test]       
        public void TestEscapes()
        {
            Run(@"\\\%\{\}\[\]\<\>",
                Latex.Escape("\\"),
                Latex.Escape("%"),
                Latex.Escape("{"),
                Latex.Escape("}"),
                Latex.Escape("["),
                Latex.Escape("]"),
                Latex.Escape("<"),
                Latex.Escape(">"));
        }
        [Test]
        public void TestCommand()
        {
            Run(@"\test test", Latex.Command("test"), Latex.Text(" test"));
        }

        [Test]
        public void TestBrackets()
        {
            Run("{ab}[ab]<ab>", Latex.Curly(Latex.Text("ab")), Latex.Square(Latex.Text("ab")), Latex.Angular(Latex.Text("ab")));
        }

        [Test]
        public void TestNesting()
        {
            Run("{a{b[c]}}", Latex.Curly(Latex.Text("a"), Latex.Curly(Latex.Text("b"), Latex.Square(Latex.Text("c")))));
        }
        
        [Test]
        public void KnownBugWithAngular()
        {
            Run("a<b", Latex.Text("a<b"));
        }

    }
}

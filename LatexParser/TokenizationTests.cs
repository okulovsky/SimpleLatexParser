using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    [TestFixture]
    class TokenizationTest
    {
        public void Run(string latex, params IBlock[] entry)
        {
            var expected = new Sequence(SequenceType.Free, entry);
            var given = new Layer1Parser().Parse(latex);
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
        public void Escapes()
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
        public void Command()
        {
            Run(@"\test test", Latex.CommandToken("test"), Latex.Text(" test"));
        }

        [Test]
        public void Brackets()
        {
            Run("\\c{ab}[ab]<ab>", Latex.CommandToken("c"), Latex.Curly(Latex.Text("ab")), Latex.Square(Latex.Text("ab")), Latex.Angular(Latex.Text("ab")));
        }

        [Test]
        public void Nesting()
        {
            Run("{a{b{c}}}", Latex.Curly(Latex.Text("a"), Latex.Curly(Latex.Text("b"), Latex.Square(Latex.Text("c")))));
        }
        
        [Test]
        public void AngularWithoutCommand()
        {
            Run("a<b", Latex.Text("a<b"));
        }

        public void CommandsLeftAndRight()
        {
            Run("\\left[\\right]", Latex.CommandToken("left"), Latex.Text("["), Latex.CommandToken("right"), Latex.Text("]"));
        }
    }
}

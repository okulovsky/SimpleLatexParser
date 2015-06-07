using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    public enum SequenceType
    {
        Free,
        Curly,
        Square,
        Angular
    }
    public class Sequence
    {
        public SequenceType Type { get; set; }
        public readonly List<IBlock> Blocks = new List<IBlock>();
        public Sequence(SequenceType type, params IBlock[] blocks)
        {
            Type = type;
            Blocks = blocks.ToList();
        }

        public override bool Equals(object _obj)
        {
            var obj = _obj as Sequence;
            if (obj == null) return false;
            if (Type != obj.Type) return false;
            if (Blocks.Count != obj.Blocks.Count) return false;
            for (int i = 0; i < Blocks.Count; i++)
                if (!Blocks[i].Equals(obj.Blocks[i])) return false;
            return true;
        }
    }

    public interface IBlock
    {
        
    }

    public enum TextBlockType
    {
        Text,
        Comment,
        Screened
    }

    public class TextBlock : IBlock
    {
        public TextBlockType Type { get; set; }
        public string Entry { get; set; }

        public TextBlock(TextBlockType type, string entry)
        {
            Type=type;
            Entry=entry;
        }

        public override bool Equals(object _obj)
        {
            var obj = _obj as TextBlock;
            if (obj == null) return false;
            return Type == obj.Type && Entry == obj.Entry;
        }
    }



    public class CommandBlock : IBlock
    {
        public string Name { get; set; }
        public readonly List<Sequence> Arguments = new List<Sequence>();
        public CommandBlock(string name, params Sequence[] arguments)
        {
            Name = name;
            Arguments = arguments.ToList();
        }

        public override bool Equals(object _obj)
        {
            var obj = _obj as CommandBlock;
            if (obj == null) return false;
            if (Name != obj.Name) return false;
            if (Arguments.Count != obj.Arguments.Count) return false;
            for (int i = 0; i < Arguments.Count; i++)
                if (!Arguments[i].Equals(obj.Arguments[i])) return false;
            return true;
        }
    }

    public static class Latex
    {
        public static IBlock Text(string text) { return new TextBlock(TextBlockType.Text, text); }
        public static IBlock Comment(string comment) { return new TextBlock(TextBlockType.Comment, comment); }
        public static IBlock Screen(string text) { return new TextBlock(TextBlockType.Screened, text); }

        public static IBlock Cmd(string name, params Sequence[] sequence) { return new CommandBlock(name, sequence); }

        public static Sequence Free(params IBlock[] blocks) { return new Sequence(SequenceType.Free, blocks); }
        public static Sequence Curly(params IBlock[] blocks) { return new Sequence(SequenceType.Curly, blocks); }
        public static Sequence Square(params IBlock[] blocks) { return new Sequence(SequenceType.Square, blocks); }
        public static Sequence Angular(params IBlock[] blocks) { return new Sequence(SequenceType.Angular, blocks); }

    }
}

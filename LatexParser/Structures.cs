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
    public class Sequence : IBlock
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

        public override string ToString()
        {
            var open = "";
            var close = "";
            if (Type == SequenceType.Angular) { open = "<"; close = ">"; }
            if (Type == SequenceType.Curly) { open = "{"; close = "}"; }
            if (Type == SequenceType.Square) { open = "["; close = "]"; }
            return open + Blocks.Select(z => z.ToString()).Aggregate((a, b) => a + b) + close;
        }
    }

    public interface IBlock
    {
        
    }

    public enum TextBlockType
    {
        Text,
        Comment,
        Escape,
        Command
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

        public override string ToString()
        {
            var prefix = "";
            if (Type == TextBlockType.Comment) prefix = "%";
            if (Type == TextBlockType.Escape || Type == TextBlockType.Command) prefix = "\\";
            return prefix + Entry;
        }
    }

    public class CommandBlock : IBlock
    {
        public TextBlock CommandToken;
        public List<Sequence> Arguments = new List<Sequence>();
        public override bool Equals(object _obj)
        {
            var obj = _obj as CommandBlock;
            if (obj == null) return false;
            if (!CommandToken.Equals(obj.CommandToken)) return false;
            if (Arguments.Count != obj.Arguments.Count) return false;
            for (int i = 0; i < Arguments.Count; i++)
                if (!Arguments[i].Equals(obj.Arguments[i])) return false;
            return true;
        }
        public override string ToString()
        {
            return CommandToken.ToString() + Arguments.Select(z => z.ToString()).Aggregate((a, b) => a + b);
        }
    }

    public static class Latex
    {
        public static TextBlock Text(string text) { return new TextBlock(TextBlockType.Text, text); }
        public static TextBlock Comment(string comment) { return new TextBlock(TextBlockType.Comment, comment); }
        public static TextBlock Escape(string text) { return new TextBlock(TextBlockType.Escape, text); }

        public static TextBlock CommandToken(string name) { return new TextBlock(TextBlockType.Command, name); }

        public static CommandBlock Command(string name, params Sequence[] arguments )
        {
            return new CommandBlock
            {
                CommandToken = new TextBlock(TextBlockType.Command, name),
                Arguments = arguments.ToList()
            };
        }

        public static Sequence Free(params IBlock[] blocks) { return new Sequence(SequenceType.Free, blocks); }
        public static Sequence Curly(params IBlock[] blocks) { return new Sequence(SequenceType.Curly, blocks); }
        public static Sequence Square(params IBlock[] blocks) { return new Sequence(SequenceType.Square, blocks); }
        public static Sequence Angular(params IBlock[] blocks) { return new Sequence(SequenceType.Angular, blocks); }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    class Layer2Parser
    {
        public void MakeSequence(Sequence sequence)
        {
            foreach (var e in sequence.Blocks.OfType<Sequence>())
                MakeSequence(e);
            for (int i=0;i<sequence.Blocks.Count;i++)
            {
                var textBlock = sequence.Blocks[i] as TextBlock;
                if (textBlock == null) continue;
                if (textBlock.Type != TextBlockType.Command) continue;
                var command = new CommandBlock();
                command.CommandToken = textBlock;
                sequence.Blocks.RemoveAt(i);
                while(i<sequence.Blocks.Count && sequence.Blocks[i] is Sequence)
                {
                    command.Arguments.Add(sequence.Blocks[i] as Sequence);
                    sequence.Blocks.RemoveAt(i);
                }
                sequence.Blocks.Insert(i, command);
            }
        }

        public Sequence Parse(string latex)
        {
            var sequence = new Layer1Parser().Parse(latex);
            MakeSequence(sequence);
            return sequence;
        }
    }
}

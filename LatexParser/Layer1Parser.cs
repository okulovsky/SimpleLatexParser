using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexParser
{
    class Layer1Parser
    {
        public const string EscapedCharacters = "\\[]{}<>%";
        public const string CommandCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string ControlCharacters = "[{<>}]%\\";

        string text;
        int pointer = 0;

        bool End()
        {
            return pointer >= text.Length;
        }

        const char END = '\0';

        char Get()
        {
            if (End())
            {
                pointer++;
                return END;
            }
            return text[pointer++];
        }
        void Back() { pointer--; }
        char Peek()
        {
            if (End()) return END;
            return text[pointer];
        }

        Stack<Sequence> stack;

        TextBlock LookBack()
        {
            var blocks = stack.Peek().Blocks;
            for (int i = blocks.Count - 1; i >= 0; i--)
            {
                if (blocks[i] is Sequence) continue;
                var text = blocks[i] as TextBlock;
                if (text == null) return null;
                if (text.Type != TextBlockType.Command) return null;
                return text;
            }
            return null;
        }

        void Push(SequenceType type)
        {
            stack.Push(new Sequence(type));
        }

        void Pop(SequenceType expectedType)
        {
            var s = stack.Pop();
            if (s.Type != expectedType) throw new Exception("Wrong brackets nesting");
            stack.Peek().Blocks.Add(s);
        }

        void Store(string text, TextBlockType type)
        {
            stack.Peek().Blocks.Add(new TextBlock(type, text));
        }

        void ParseUntil(TextBlockType type, Func<char, bool> stopCondition)
        {
            var builder = new StringBuilder();
            for (int i = 0; ; i++)
            {
                var s = Get();
                if (stopCondition(s) || s == END)
                {
                    Back();
                    Store(builder.ToString(), type);
                    return;
                }
                builder.Append(s);
            }
        }

        public Sequence Parse(string latex)
        {
            text = latex;
            pointer = 0;
            stack = new Stack<Sequence>();
            stack.Push(new Sequence(SequenceType.Free));

            while (true)
            {
                var c = Get();
                if (c == END) break;

                if (!ControlCharacters.Contains(c))
                {
                    Back();
                    ParseUntil(TextBlockType.Text, z => ControlCharacters.Contains(z));
                    continue;
                }

                if (c == '%')
                {
                    ParseUntil(TextBlockType.Comment, z => z == '\n');
                    continue;
                }


                if (c == '\\')
                {
                    var v = Get();
                    if (EscapedCharacters.Contains(v))
                    {
                        Store(v.ToString(), TextBlockType.Escape);
                        continue;
                    }

                    if (!CommandCharacters.Contains(v))
                        throw new Exception("Wrong symbol " + v + " after \\");
                    Back();
                    ParseUntil(TextBlockType.Command, z => !CommandCharacters.Contains(z));
                    continue;
                }

                if (c == '{') Push(SequenceType.Curly);
                else if (c == '}') Pop(SequenceType.Curly);
                else if (c == '[')
                {
                    var back = LookBack();
                    if (back == null || back.Entry == "left") Store(c.ToString(), TextBlockType.Text);
                    else Push(SequenceType.Square);
                }
                else if (c == ']')
                {
                    if (stack.Peek().Type != SequenceType.Square)
                        Store(c.ToString(), TextBlockType.Text);
                    else Pop(SequenceType.Square);
                }
                else if (c == '<')
                {
                    var back = LookBack();
                    if (back == null || back.Entry == "left") Store(c.ToString(), TextBlockType.Text);
                    else Push(SequenceType.Angular);
                }
                else if (c == '>')
                {
                    if (stack.Peek().Type != SequenceType.Angular)
                        Store(c.ToString(), TextBlockType.Text);
                    else Pop(SequenceType.Angular);
                }
                else throw new Exception("Unexpected symbol " + c);
            }

            if (stack.Count != 1) throw new Exception("Not closed " + stack.Peek().Type);
            var sequence = stack.Pop();
            CompressText(sequence);
            return sequence;
        }

        void CompressText(Sequence sequence)
        {
            foreach (var e in sequence.Blocks.OfType<Sequence>())
                CompressText(e);
            for (int i = 0; i < sequence.Blocks.Count - 1; i++)
            {
                var thisText = sequence.Blocks[i] as TextBlock;
                if (thisText == null) continue;
                if (thisText.Type != TextBlockType.Text) continue;
                var nextText = sequence.Blocks[i + 1] as TextBlock;
                if (nextText == null) continue;
                if (nextText.Type != TextBlockType.Text) continue;
                thisText.Entry += nextText.Entry;
                sequence.Blocks.RemoveAt(i + 1);
                i--;
            }
        }
    }
}
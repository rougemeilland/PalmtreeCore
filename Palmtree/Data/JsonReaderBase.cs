/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Data
{
    internal abstract class JsonReaderBase
        : IJsonReader
    {
        public void SkipSpace()
        {
            while (true)
            {
                switch (Peek())
                {
                    case '\t':
                    case '\n':
                    case '\v':
                    case '\f':
                    case '\r':
                    case ' ':
                    case '\u00a0':
                        ReadChar();
                        break;
                    default:
                        return;
                }
            }
        }

        public abstract bool IsEnd { get; }
        public abstract char Peek();
        public abstract char ReadChar();
        public abstract string ReadStr(int length);
    }
}

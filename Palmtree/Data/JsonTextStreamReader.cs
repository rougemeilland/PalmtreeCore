/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.IO;

namespace Palmtree.Data
{
    internal class JsonTextStreamReader
        : JsonReaderBase
    {
        private TextReader _source;
        private char _peek_buf;
        private bool _is_end;

        public JsonTextStreamReader(TextReader source)
        {
            _source = source;
            _peek_buf = '\0';
            _is_end = false;
        }

        public override bool IsEnd
        {
            get
            {
                return (_is_end);
            }
        }

        public override char Peek()
        {
            if (_peek_buf == '\0')
            {
                var c = _source.Read();
                if (c == -1)
                {
                    _peek_buf = '\0';
                    _is_end = true;
                }
                else
                {
//#if DEBUG
//                    Console.Write((char)c);
//#endif
                    _peek_buf = (char)c;
                }
            }
            return (_peek_buf);
        }

        public override char ReadChar()
        {
            if (_peek_buf == '\0')
            {
                var c = _source.Read();
                if (c == -1)
                {
                    _is_end = true;
                    return ('\0');
                }
                else
                {
//#if DEBUG
//                    Console.Write((char)c);
//#endif
                    return ((char)c);
                }
            }
            else
            {
                var c = _peek_buf;
                _peek_buf = '\0';
                return (c);
            }
        }

        public override string ReadStr(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException();
            var buffer = new char[length];
            var index = 0;
            if (_peek_buf != '\0')
            {
                buffer[0] = _peek_buf;
                ++index;
                _peek_buf = '\0';
            }
            while (index < length)
            {
                var count = _source.Read(buffer, index, length - index);
                if (count == 0)
                {
                    if (index == 0)
                        return (null);
                    else
                        throw new FormatException("予期しないEOFが見つかりました。");
                }
//#if DEBUG
//                Console.Write(new string(buffer, index, count));
//#endif
                index += count;
            }
            return (new string(buffer, 0, length));
        }
    }
}

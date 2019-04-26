/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree.Data
{
    internal class JsonStringReader
        : JsonReaderBase
    {
        private string _source;
        private int _index;

        public JsonStringReader(string source)
        {
            _source = source;
            _index = 0;
        }

        public override bool IsEnd => _index >= _source.Length;

        public override char Peek()
        {
            return (_source[_index]);
        }

        public override char ReadChar()
        {
            return (_source[_index++]);
        }

        public override string ReadStr(int length)
        {
            if (_index + length > _source.Length)
                throw new FormatException("予期しないEOFが見つかりました。");
            var s = _source.Substring(_index, length);
            _index += length;
            return (s);
        }
    }
}

/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Text;

namespace Palmtree.Data
{
    internal class JsonStringWriter
        : IJsonWriter
    {
        private StringBuilder _buffer;

        public JsonStringWriter(StringBuilder buffer)
        {
            _buffer = buffer;
        }

        public void Write(char c)
        {
            _buffer.Append(c);
        }

        public void Write(string s)
        {
            _buffer.Append(s);
        }
    }
}

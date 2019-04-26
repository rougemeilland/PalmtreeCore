/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.IO;

namespace Palmtree.Data
{
    internal class JsonTextStreamWriter
        : IJsonWriter
    {
        private TextWriter _native_writer;

        public JsonTextStreamWriter(TextWriter native_writer)
        {
            _native_writer = native_writer;
        }

        public void Write(char c)
        {
            _native_writer.Write(c);
        }

        public void Write(string s)
        {
            _native_writer.Write(s);
        }
    }
}

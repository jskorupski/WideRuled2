using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WideRuled2
{
    public class IndentingWriter
    {
        System.IO.StringWriter Writer = new System.IO.StringWriter();

        int Depth;
        public int IndentChars = 4;

        public void OutDent()
        {
            Depth--;
            if (Depth < 0) Depth = 0;
        }

        public void Indent()
        {
            Depth++;
        }

        public void Write(string value)
        {
            Writer.Write(value);
        }

        public void Write(string format, params object[] values)
        {
            Writer.Write(format, values);
        }

        public void WriteLine()
        {
            printIndenting();
            Writer.WriteLine();
        }

        public void WriteLine(string format, params object[] values)
        {
            printIndenting();
            Writer.WriteLine(format, values);
        }

        public void WriteLine(string value)
        {
            printIndenting();
            Writer.WriteLine(value);
        }

        void printIndenting()
        {
            for (int x = 0; x < Depth; x++)
            {
                for (int y = 0; y < IndentChars; y++)
                    Writer.Write(' ');
            }
        }

        public new string ToString()
        {
            return Writer.ToString();
        }
    }
}

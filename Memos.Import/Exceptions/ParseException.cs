using System;

namespace Memos.Import.Exceptions
{
    class ParseException : Exception
    {
        public ParseException(string message)
            : base(message)
        {}

        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {}
    }
}

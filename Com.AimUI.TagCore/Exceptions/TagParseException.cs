using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore.Exceptions
{
    public class TagParseException : Exception
    {
        public TagParseException(string message)
            : base(message)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Exceptions
{
    public class MException: Exception
    {
        public MException(string message) : base(message) { }
    }
}

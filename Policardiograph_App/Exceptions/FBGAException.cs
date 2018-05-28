using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Exceptions
{
    public class FBGAException: MException
    {
        public FBGAException(string message) : base(message) { }
    }
}

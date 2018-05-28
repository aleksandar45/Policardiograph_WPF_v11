using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Exceptions
{
    public class MWLSException: MException
    {
         public MWLSException(string message) : base(message) { }
    }
}

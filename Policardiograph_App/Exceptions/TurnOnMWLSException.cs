using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Exceptions
{
    public class TurnOnMWLSException: MWLSException
    {
        public TurnOnMWLSException(string message) : base(message) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Policardiograph_App.Exceptions
{
    public class InitMWLSException: MWLSException
    {
        public InitMWLSException(string message) : base(message) { }
    }
}

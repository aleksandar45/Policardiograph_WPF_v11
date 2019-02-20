using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Dialogs.DialogService;

namespace Policardiograph_App.Dialogs.DialogPersonComment
{
    public class DialogResultPersonComment: DialogResult
    {
        public DialogResultPersonComment(string measurementComment)
        {
            MeasurementComment = measurementComment;
        }
        public string MeasurementComment
        {
            get;
            set;
        }
    }
}

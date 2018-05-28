using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Policardiograph_App.Dialogs.DialogService;

namespace Policardiograph_App.Dialogs.DialogYesNo
{
    public class DialogResultYesNo: DialogResult
    {
        public DialogResultYesNoEnum Result
        {
            get;
            private set;
        }
        public DialogResultYesNo(DialogResultYesNoEnum yesNo) {
            this.Result = yesNo;
        }
    }
}

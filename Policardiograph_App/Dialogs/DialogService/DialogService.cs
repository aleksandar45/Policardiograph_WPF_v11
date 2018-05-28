using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Policardiograph_App.Dialogs.DialogService
{
    public static class  DialogService
    {
        public static DialogResult OpenDialog(DialogViewModelBase vm,Window owner)
        {
            DialogWindow win = new DialogWindow();
            win.DataContext = vm;
            if (owner != null)
                win.Owner = owner; 
            win.ShowDialog();
            DialogResult result = (win.DataContext as DialogViewModelBase).UserDialogResult;
            return result;  
        }
        public static DialogResult OpenDialog(DialogViewModelBase vm)
        {
            DialogWindow win = new DialogWindow();
            win.DataContext = vm;            
            win.ShowDialog();
            DialogResult result = (win.DataContext as DialogViewModelBase).UserDialogResult;
            return result;
        }  
    }
}

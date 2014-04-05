using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace iTSfvLib
{
    public class FolderNameEditor : FileNameEditor
    {
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || provider == null)
            {
                return base.EditValue(context, provider, value);
            }

            CommonOpenFileDialog dlg = new CommonOpenFileDialog("Choose music library folder...")
            {
                IsFolderPicker = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                value = dlg.FileName;
            }

            return value;
        }
    }
}

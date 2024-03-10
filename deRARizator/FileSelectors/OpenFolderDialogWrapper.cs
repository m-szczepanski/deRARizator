using deRARizator.Interfaces;
using System;
using Microsoft.Win32;

namespace deRARizator.FileSelectors
{
    public class OpenFolderDialogWrapper : IFolderSelector
    {
        public string GetFolderPath()
        {
            var openFolderDialog = new OpenFolderDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? result = openFolderDialog.ShowDialog();

            return result == true ? openFolderDialog.FolderName : null;
        }
    }
}
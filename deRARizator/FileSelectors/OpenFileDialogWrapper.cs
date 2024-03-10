using Microsoft.Win32;
using deRARizator.Interfaces;
using System;

namespace deRARizator.FileSelectors
{
    public class OpenFileDialogWrapper : IFileSelector
    {
        public string GetFilePath()
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? result = openFileDialog.ShowDialog();

            return result == true ? openFileDialog.FileName : null;
        }
    }
}
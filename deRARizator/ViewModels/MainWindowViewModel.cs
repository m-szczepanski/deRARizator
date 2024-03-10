using deRARizator.Interfaces;
using System;
using System.Threading.Tasks;

namespace deRARizator.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IFileSelector _fileSelector;
        private readonly IFolderSelector _folderSelector;

        public MainWindowViewModel(IFileSelector fileSelector, IFolderSelector folderSelector)
        {
            _fileSelector = fileSelector ?? throw new ArgumentNullException(nameof(fileSelector));
            _folderSelector = folderSelector ?? throw new ArgumentNullException(nameof(folderSelector));
        }

        public string GetSelectedFilePath()
        {
            return _fileSelector.GetFilePath();
        }

        public string GetSelectedFolderPath()
        {
            return _folderSelector.GetFolderPath();
        }

        public async Task<bool> ExtractRarFilesAsync(string rarFilePath, string extractPath, IProgress<int> progress)
        {
            // Implement extraction logic here using RarExtractorService
            // Update progress using ProgressBarUpdater
            return true; // Return success or failure status
        }
    }
}
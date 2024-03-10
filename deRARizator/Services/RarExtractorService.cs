using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace deRARizator.Services
{
    public class RarExtractorService
    {
        public async Task<bool> ExtractRarFilesAsync(string rarFilePath, string extractPath, IProgress<int> progress)
        {
            try
            {
                using (var archive = RarArchive.Open(rarFilePath))
                {
                    int totalFile = archive.Entries.Count(entry => !entry.IsDirectory);
                    progress.Report(0); // Initialize progress

                    await Task.Run(() =>
                    {
                        int completed = 0;

                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            try
                            {
                                entry.WriteToDirectory(extractPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true,
                                });

                                Interlocked.Increment(ref completed);
                                var percentage = Convert.ToInt16(((double)completed / totalFile) * 100d);

                                progress.Report(percentage);
                            }
                            catch (Exception writeException)
                            {
                                MessageBox.Show($"Error extracting {entry.Key}: {writeException.Message}", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    });

                    MessageBox.Show("Extraction complete.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
            }
            catch (Exception openException)
            {
                MessageBox.Show($"Error opening RAR file: {openException.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}

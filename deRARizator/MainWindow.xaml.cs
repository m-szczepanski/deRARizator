using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace deRARizator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void SelectFileButton_Click(object sender, RoutedEventArgs e) 
    {
        var openFileDialog = new OpenFileDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            FilePath.Content = openFileDialog.FileName;
        }

        progressBar.Value = 0;
    }

    private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
    {
        var openFolderDialog = new OpenFolderDialog()
        {
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = openFolderDialog.ShowDialog();

        if (result == true)
        {
            DestinationPath.Content = openFolderDialog.FolderName;
        }

        progressBar.Value = 0;
    }

    int totalFile = 0;
    int completed = 0;

    private async void ExtractButton_Click(object sender, RoutedEventArgs e)
    {
        string rarFilePath = FilePath.Content.ToString();
        string extractPath = DestinationPath.Content.ToString();

        completed = 0;

        IProgress<int> progress = new Progress<int>(value =>
        {
            progressBar.Value = value;
        });

        await ExtractRarFiles(rarFilePath, extractPath, progress);
    }

    private async Task ExtractRarFiles(string rarFilePath, string extractPath, IProgress<int> progress)
    {
        try
        {
            using (var archive = RarArchive.Open(rarFilePath))
            {
                totalFile = archive.Entries.Where(entry => !entry.IsDirectory).ToList().Count;
                progressBar.Maximum = totalFile;

                await Task.Run(() =>
                {
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
                            var percentage = Convert.ToInt16(((double)completed / totalFile * 1.0) * 100d);

                            // Aktualizuj ProgressBar za pomocą obiektu IProgress na wątku interfejsu użytkownika
                            progress.Report(completed);

                        }
                        catch (Exception writeException)
                        {
                            MessageBox.Show($"Error extracting {entry.Key}: {writeException.Message}", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });

                MessageBox.Show("Extraction complete.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception openException)
        {
            MessageBox.Show($"Error opening RAR file: {openException.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FilePathDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] file = (string[]) e.Data.GetData(DataFormats.FileDrop);

            if (file[0].ToString().EndsWith(".rar"))
                FilePath.Content = file[0];
            else
                MessageBox.Show("Only .rar files are accepted.", "Error - bad file type.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }
        else
        {
            throw new NullReferenceException();
        }

        progressBar.Value = 0;
    }

    private void DestinationPathDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] destination = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Directory.Exists(destination[0]))
            {
                DestinationPath.Content = destination[0];
            }
            else
            {
                DestinationPath.Content = "Chose destination directory";
                MessageBox.Show("Only directories are accepted here.", "Error - not a directory", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            throw new NotSupportedException("Bad format.");
        }

        progressBar.Value = 0;
    }
}
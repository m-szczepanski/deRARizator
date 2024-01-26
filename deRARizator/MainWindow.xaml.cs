using System.IO;
using System.Windows;
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
        var openFileDialog = new OpenFileDialog
        {
            Filter = "(*.rar)|",

            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            FilePath.Content = openFileDialog.FileName;
        }
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
    }

    private void ExtractButton_Click(object sender, RoutedEventArgs e)
    {
        string rarFilePath = FilePath.Content.ToString();
        string extractPath = DestinationPath.Content.ToString();

        ExtractRarFiles(rarFilePath, extractPath);
    }

    private void ExtractRarFiles(string rarFilePath, string extractPath)
    {
        try
        {
            using (var archive = RarArchive.Open(rarFilePath))
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
                    }
                    catch (Exception writeException)
                    {
                        MessageBox.Show($"Error extracting {entry.Key}: {writeException.Message}", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            MessageBox.Show("Extraction complete.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
    
}
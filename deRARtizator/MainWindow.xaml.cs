using System.Windows;
using Microsoft.Win32;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace deRARtizator;

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
            Filter = "All Files (*.*)|*.*|Text Files (*.rar)|*.txt",

            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            TextBox.Text = openFileDialog.FileName;
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
            DestinationTextBox.Text = openFolderDialog.FolderName;
        }
    }

    private void ExtractButton_Click(object sender, RoutedEventArgs e)
    {
        string rarFilePath = TextBox.Text;
        string extractPath = DestinationTextBox.Text;

        ExtractRarFiles(rarFilePath, extractPath);
    }

    private void ExtractRarFiles(string rarFilePath, string extractPath)
    {
        int totalFile = 0;
        try
        {
            using (var archive = RarArchive.Open(rarFilePath))
            {
                totalFile = archive.Entries.Where(entry => !entry.IsDirectory).ToList().Count;
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
}
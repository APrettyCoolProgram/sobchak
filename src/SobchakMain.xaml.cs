/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 1-26-2021-2:52 PM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2020 A Pretty Cool Program All rights reserved
 */

using System.IO;
using System.Windows;
using System.Windows.Media;
using Du;

namespace Sobchak
{
    /// <summary></summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetupLogoAndVersion();
        }

        /// <summary></summary>
        private void SetupLogoAndVersion()
        {
            var sobchakAssembly = DuApplication.GetAssemblyName();

            Title = $"Sobchak v{DuApplication.GetVersionInformational()}";
            imgLogo.Source = DuBitmap.FromUri(sobchakAssembly, "/Resources/Asset/Image/Logo/sobchak-logo-800x150.png");
        }

        /// <summary></summary>
        private void Go()
        {
            var sourcePath = txbxSourcePath.Text;
            FileInfo[] fileNames = DuDirectory.GetFileNames(sourcePath);

            DuDirectory.Create($"{sourcePath}/.sobchak");

            if(rbtnCreate.IsChecked is true)
            {
                CreateHashes(sourcePath, fileNames);
            }

            if(rbtnVerify.IsChecked is true)
            {
                VerifyHashes(sourcePath, fileNames);
            }
        }

        /// <summary></summary>
        /// <param name="sourcePath"></param>
        /// <param name="fileNames"></param>
        private void CreateHashes(string sourcePath, FileInfo[] fileNames)
        {
            var fileCounter = 1;
            var feedbackText = "";

            foreach(FileInfo fileName in fileNames)
            {
                feedbackText += $"Creating hash for file {fileCounter} of {fileNames.Length}: \"{fileName.Name}\"...";
                RefreshFeedbackText(feedbackText);

                DuSha256.WriteHashValueAsContent(fileName.FullName, $"{sourcePath}/.sobchak/{fileName.Name}.sobchak");

                feedbackText += "complete.\n";
                RefreshFeedbackText(feedbackText);

                UpdateProgressBar(fileCounter, fileNames.Length);

                fileCounter++;
            }
        }

        /// <summary></summary>
        /// <param name="fileCoutner"></param>
        /// <param name="numberOfFiles"></param>
        private void UpdateProgressBar(int fileCoutner, int numberOfFiles)
        {
            var percentComplete = ((decimal)fileCoutner /numberOfFiles) * 100;
            lblProgressBar.Width = (int)percentComplete * 7;
            lblProgressBar.Content = $"{(int)percentComplete}%";
        }

        /// <summary></summary>
        /// <param name="sourcePath"></param>
        /// <param name="fileNames"></param>
        private void VerifyHashes(string sourcePath, FileInfo[] fileNames)
        {
            var fileCounter = 1;
            var feedbackText = "";

            var invalidTotal = 0;

            foreach(FileInfo fileName in fileNames)
            {
                if(!File.Exists($"{sourcePath}/.sobchak/{fileName.Name}.sobchak"))
                {
                    feedbackText += $"MISSING HASH: \"{fileName.Name}\" (File {fileCounter} of {fileNames.Length})...creating...";
                    RefreshFeedbackText(feedbackText);

                    DuSha256.WriteHashValueAsContent(fileName.FullName, $"{sourcePath}/.sobchak/{fileName.Name}.sobchak");

                    feedbackText += "complete.\n";
                    RefreshFeedbackText(feedbackText);
                }
                else
                {
                    var sobchakHash = File.ReadAllText($"{sourcePath}/.sobchak/{fileName.Name}.sobchak");

                    var hashesAreEqual = DuSha256.FileMatchesSha256Value($"{sourcePath}/{fileName.Name}", sobchakHash);

                    if(hashesAreEqual)
                    {
                        feedbackText += $"VALID HASH: \"{fileName.Name}\" (File {fileCounter} of {fileNames.Length})\n";
                        RefreshFeedbackText(feedbackText);
                    }
                    else
                    {
                        feedbackText += $"INVALID HASH: \"{fileName.Name}\" (File {fileCounter} of {fileNames.Length})\n";
                        RefreshFeedbackText(feedbackText);
                        invalidTotal++;
                    }
                }

                UpdateProgressBar(fileCounter, fileNames.Length);

                fileCounter++;
            }

            if(invalidTotal != 0)
            {
                MessageBoxResult errMsg = MessageBox.Show("There are invalid hashes!", "INVALID HASHES FOUND!", MessageBoxButton.OK);
            }
        }

        /// <summary></summary>
        /// <param name="feedbackText"></param>
        private void RefreshFeedbackText(string feedbackText)
        {
            txbxFeedback.Text = feedbackText;
            DuTextBox.RefreshContent(txbxFeedback);
        }

        /// <summary></summary>
        private void SourcePathChanged()
        {
            if(txbxSourcePath.Text is not "")
            {
                if(Directory.Exists(txbxSourcePath.Text))
                {
                    btnGo.Background = new SolidColorBrush(Color.FromArgb(100, 0, 166, 166));
                    btnGo.Foreground = new SolidColorBrush(Colors.Black);
                    btnGo.IsEnabled = true;
                }
                else
                {
                    btnGo.Foreground = new SolidColorBrush(Color.FromArgb(100, 0, 166, 166));
                    btnGo.IsEnabled = false;
                }
            }
        }

        /// <summary></summary>
        private void ChooseSource()
        {
            txbxSourcePath.Text = DuFolderDialog.GetFolderPath();
        }

        // EVENT HANDLERS
        private void btnChooseSourcePath_Click(object sender, RoutedEventArgs e) => ChooseSource();
        private void btnGo_Click(object sender, RoutedEventArgs e) => Go();
        private void txbxSourcePath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => SourcePathChanged();
    }
}
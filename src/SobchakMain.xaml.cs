/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 1-26-2021-10:03 AM
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
            if(rbtnCreate.IsChecked is true)
            {
                CreateHashes();
            }

            if(rbtnVerify.IsChecked is true)
            {
                VerifyHashes();
            }
        }
        

        private void CreateHashes()
        {
            var sourcePath = txbxSourcePath.Text;

            DuDirectory.Create($"{sourcePath}/.sobchak");

            FileInfo[] files = Du.DuDirectory.GetFileNames(sourcePath);

            var fileNums = 1;

            var feedbackText = "";

            foreach(FileInfo file in files)
            {
                decimal percentComplete = (decimal)fileNums /files.Length;

                lblProgress.Content = $"Computing hash values for file {fileNums} of {files.Length}";
                DuLabel.RefreshContent(lblProgress);
                feedbackText += $"Creating hash for \"{file.Name}\"...";
                txbxFeedback.Text = feedbackText;
                Du.DuTextBox.RefreshContent(txbxFeedback);
                DuSha256.WriteHashValueAsContent(file.FullName, $"{sourcePath}/.sobchak/{file.Name}.sobchak");
                feedbackText += "complete.\n";
                txbxFeedback.Text = feedbackText;
                Du.DuTextBox.RefreshContent(txbxFeedback);
                var p = percentComplete * 100;
                var n = (int)p;
                lblProgressBar.Width = n * 4;
                lblProgressBar.Content = $"{n}%";
                fileNums++;
            }

            lblProgress.Content = "All hash values computed!";
            DuLabel.RefreshContent(lblProgress);
        }

        private void VerifyHashes()
        {

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
/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 1-26-2021-9:49 AM
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
            imgLogo.Source = Du.DuBitmap.FromUri(sobchakAssembly, "/Resources/Asset/Image/Logo/sobchak-logo-800x150.png");
        }

        /// <summary></summary>
        private void Go()
        {
            DuDirectory.Create($"{txbxSourcePath.Text}/.sobchak");

            FileInfo[] files = Du.DuDirectory.GetFileNames(txbxSourcePath.Text);

            foreach(FileInfo file in files)
            {
                DuSha256.WriteHashValueAsContent(file.FullName, $"{txbxSourcePath.Text}/.sobchak/{file.Name}.sobchak");

                //var sha256Value = Du.DuSha.GetSha256Value(file.FullName);
            }
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
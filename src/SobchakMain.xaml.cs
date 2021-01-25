/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 1-25-2021-11:57 AM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2020 A Pretty Cool Program All rights reserved
 */
using System.Windows;
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
            var sobchakAssembly = DuApplication.GetApplicationAssemblyName();

            Title = $"Sobchak v{DuApplication.GetApplicationVersion()}";
            imgLogo.Source = Du.DuBitmap.FromUri(sobchakAssembly, "/Resources/Asset/Image/Logo/sobchak-logo-800x150.png");
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
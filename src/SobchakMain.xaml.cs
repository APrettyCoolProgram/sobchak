/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 8-26-2021-12:44 PM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2021 A Pretty Cool Program All rights reserved
 */
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Sobchak
{
    /// <summary></summary>
    public partial class MainWindow : Window
    {

        private static readonly Action EmptyDelegate = delegate { };

        public MainWindow()
        {
            InitializeComponent();

            SetupSobchak();
        }

        /// <summary></summary>
        private void SetupSobchak()
        {
            Title = $"Sobchak v{Assembly.GetEntryAssembly().GetName().Version}";

            lblCurrentDirectory.Content = Directory.GetCurrentDirectory();

            lblLogFileMessage.Content = "Log files located in ./sobchak/";
        }

        /// <summary></summary>
        private void VerifyShas()
        {
            string currentDirectory = lblCurrentDirectory.Content.ToString();

            var directory = new DirectoryInfo(currentDirectory);

            FileInfo[] fileNames = directory.GetFiles();


            if (!Directory.Exists($"{currentDirectory}/.sobchak"))
            {
                Directory.CreateDirectory($"{currentDirectory}/.sobchak");
            }

            VerifyHashes(currentDirectory, fileNames);

            //if(rbtnCreate.IsChecked is true)
            //{
            //    CreateHashes(sourcePath, fileNames);
            //}

            //if(rbtnVerify.IsChecked is true)
            //{
            //    VerifyHashes(sourcePath, fileNames);
            //}
        }


        /// <summary></summary>
        /// <param name="currentDirectory"></param>
        /// <param name="fileNames"></param>
        private void VerifyHashes(string currentDirectory, FileInfo[] fileNames)
        {
            var fileCounter = 1;
            var feedbackText = "";

            var invalidTotal = 0;

            foreach (FileInfo fileName in fileNames)
            {
                if (!File.Exists($"{currentDirectory}/.sobchak/{fileName.Name}.sobchak"))
                {
                    feedbackText += $"MISSING HASH: \"{fileName.Name}\" (File {fileCounter} of {fileNames.Length})...creating...";
                    RefreshFeedbackText(feedbackText);

                    WriteHashValueAsContent(fileName.FullName, $"{currentDirectory}/.sobchak/{fileName.Name}.sobchak");

                    feedbackText += "complete.\n";
                    RefreshFeedbackText(feedbackText);
                }
                else
                {
                    var sobchakHash = File.ReadAllText($"{currentDirectory}/.sobchak/{fileName.Name}.sobchak");

                    var hashesAreEqual = FileMatchesSha256Value($"{currentDirectory}/{fileName.Name}", sobchakHash);

                    if (hashesAreEqual)
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

            if (invalidTotal != 0)
            {
                MessageBoxResult errMsg = MessageBox.Show("There are invalid hashes!", "INVALID HASHES FOUND!", MessageBoxButton.OK);
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

                WriteHashValueAsContent(fileName.FullName, $"{sourcePath}/.sobchak/{fileName.Name}.sobchak");

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
        /// <param name="feedbackText"></param>
        private void RefreshFeedbackText(string feedbackText)
        {
            txbxFeedback.Text = feedbackText;
            RefreshContent(txbxFeedback);
        }

        ///// <summary></summary>
        //private void SourcePathChanged()
        //{
        //    if(txbxSourcePath.Text is not "")
        //    {
        //        if(Directory.Exists(txbxSourcePath.Text))
        //        {
        //            btnGo.Background = new SolidColorBrush(Color.FromArgb(100, 0, 166, 166));
        //            btnGo.Foreground = new SolidColorBrush(Colors.Black);
        //            btnGo.IsEnabled = true;
        //        }
        //        else
        //        {
        //            btnGo.Foreground = new SolidColorBrush(Color.FromArgb(100, 0, 166, 166));
        //            btnGo.IsEnabled = false;
        //        }
        //    }
        //}

        /// <summary></summary>
        //private void ChooseSource()
        //{
        //    txbxSourcePath.Text = GetFolderPath();
        //}


        /// <summary>Get the SHA256 value of a file as a byte[].</summary>
        /// <param name="filePath">The file to get the SHA256 value of.</param>
        /// <returns>A SHA256 value as a byte[].</returns>
        private static byte[] GetHashAsBytes(string filePath)
        {
            var workingHashValue = SHA256.Create();
            byte[] hashAsBytes;

            using (FileStream stream = File.OpenRead(filePath))
            {
                hashAsBytes = workingHashValue.ComputeHash(stream);
            }

            return hashAsBytes;
        }

        /// <summary>Get the SHA256 value of a file as a string.</summary>
        /// <param name="filePath">The file to get the SHA256 value of.</param>
        /// <returns>A SHA256 value as a string.</returns>
        public static string GetHashAsString(string filePath)
        {
            var hashAsBytes  = GetHashAsBytes(filePath);
            var hashAsString = ConvertHashToString(hashAsBytes);

            return hashAsString;
        }

        /// <summary>Convert a SHA256 hash as a byte[] to a string</summary>
        /// <param name="hashAsBytes">The byte[] that holds the SHA256 hash.</param>
        /// <returns>A SHA256 hash as a string.</returns>
        public static string ConvertHashToString(byte[] hashAsBytes)
        {
            var hashAsString = "";

            for (var currentBit = 0; currentBit < hashAsBytes.Length; currentBit++)
            {
                hashAsString += $"{hashAsBytes[currentBit]:X2}";

                if ((currentBit % 4) == 3)
                {
                    hashAsString += " ";
                }
            }

            return hashAsString;
        }

        /// <summary></summary>
        /// <param name="filePath1"></param>
        /// <param name="filePath2"></param>
        /// <returns></returns>
        public static bool BothFilesMatchSha256(string filePath1, string filePath2)
        {
            return GetHashAsString(filePath1) == GetHashAsString(filePath2);
        }

        /// <summary></summary>
        /// <param name="filePath"></param>
        /// <param name="sha256Value"></param>
        /// <returns></returns>
        public static bool FileMatchesSha256Value(string filePath, string sha256Value)
        {
            return GetHashAsString(filePath) == sha256Value;
        }

        /// <summary></summary>
        /// <param name="fileToCalculate"></param>
        /// <param name="pathToSave"></param>
        public static void WriteHashValueAsContent(string fileToCalculate, string pathToSave)
        {
            File.WriteAllText(pathToSave, GetHashAsString(fileToCalculate));
        }



        public static void RefreshContent(TextBox textBoxToRefresh)
        {
            _ = textBoxToRefresh.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }











        // EVENT HANDLERS
        private void btnVerify_Click(object sender, RoutedEventArgs e) => VerifyShas();
    }
}
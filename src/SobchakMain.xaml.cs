/* PROJECT: Sobchak (https://github.com/aprettycoolprogram/Sobchak)
 *    FILE: Sobchak.SobchakMain.xaml.cs
 * UPDATED: 9-5-2021-2:45 PM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2021 A Pretty Cool Program All rights reserved
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        /// <summary>Setup Sobchak.</summary>
        private void SetupSobchak()
        {
            string sobchakVersion = $"Sobchak v{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";

            Title                       = $"Sobchak v{sobchakVersion}";
            lblCurrentDirectory.Content = Directory.GetCurrentDirectory();
            lblLogFileMessage.Content   = "Log files located in ./sobchak/";
            //lblProgressBar.Content      = "Click \"Verify\" to start";
        }

        /// <summary>When the user clicks the Verify button.</summary>
        private void VerifyShas()
        {
            string currentDirectory = lblCurrentDirectory.Content.ToString();

            DirectoryInfo directory        = new DirectoryInfo(currentDirectory);
            FileInfo[] fileNames = directory.GetFiles();

            List<string> fNames = new List<string>();

            List<string> ignoredFiles = new List<string>()
            {
                "Sobchak.exe",
                "autorun.inf"
            };

            foreach (FileInfo fileName in fileNames)
            {
                if (!ignoredFiles.Contains(fileName.Name))
                {
                    fNames.Add(fileName.Name);
                }
            }

            if (!Directory.Exists($"{currentDirectory}/.sobchak"))
            {
                Directory.CreateDirectory($"{currentDirectory}/.sobchak");
            }

            string dateStamp        = DateTime.Now.ToString("MM/dd/yyyy-HH:mm");
            string logTextSeperator = $"============================={Environment.NewLine}Sobchak log: {dateStamp}{Environment.NewLine}============================={Environment.NewLine}";
            File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", logTextSeperator);

            VerifyHashes(currentDirectory, fNames);

            string logTextSeperator2 = $"{Environment.NewLine}";
            File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", logTextSeperator2);
        }

        /// <summary>Verify SHA256 values.</summary>
        /// <param name="currentDirectory">The directory that Sobchak was launched.</param>
        /// <param name="fileNames">A list of filenames in the directory</param>
        private void VerifyHashes(string currentDirectory, List<string> fNames)
        {
            int fileCounter  = 1;
            string feedbackText = "Starting verification...";
            int invalidTotal = 0;

            lblProgressBar.Background = Brushes.Teal;
            lblProgressBar.Foreground = Brushes.White;

            UpdateProgressBar(0, fNames.Count);
            UpdateProgressStatus(feedbackText);

            foreach (string fName in fNames)
            {
                var currentFileNumber = fNames.Count;

                feedbackText = $"VERIFYING HASH: \"{fName}\" (File {fileCounter} of {currentFileNumber})\n";
                UpdateProgressStatus(feedbackText);

                if (!File.Exists($"{currentDirectory}/.sobchak/{fName}.sobchak"))
                {
                    feedbackText = $"MISSING HASH: \"{fName}\" (File {fileCounter} of {currentFileNumber})...creating...";
                    File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", feedbackText);
                    UpdateProgressStatus(feedbackText);

                    WriteHashValueAsContent(fName, $"{currentDirectory}/.sobchak/{fName}.sobchak");

                    feedbackText = "complete.\n";
                    File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", feedbackText);
                    UpdateProgressStatus(feedbackText);
                }
                else
                {
                    //feedbackText = $"VERIFYING HASH: \"{fName}\" (File {fileCounter} of {currentFileNumber})\n";
                    //UpdateProgressStatus(feedbackText);

                    string sobchakHash = File.ReadAllText($"{currentDirectory}/.sobchak/{fName}.sobchak");

                    bool hashesAreEqual = FileMatchesSha256Value($"{currentDirectory}/{fName}", sobchakHash);

                    if (hashesAreEqual)
                    {
                        feedbackText = $"VALID HASH: \"{fName}\" (File {fileCounter} of {currentFileNumber})\n";
                        File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", feedbackText);
                        UpdateProgressStatus(feedbackText);
                    }
                    else
                    {
                        feedbackText = $"INVALID HASH: \"{fName}\" (File {fileCounter} of {currentFileNumber})\n";
                        File.AppendAllText($"{currentDirectory}/.sobchak/sobchak.log", feedbackText);
                        UpdateProgressStatus(feedbackText);

                        invalidTotal++;

                        lblProgressBar.Background = Brushes.Salmon;
                        lblProgressBar.Foreground = Brushes.Black;
                    }
                }

                UpdateProgressBar(fileCounter, fNames.Count);
                UpdateProgressStatus(feedbackText);

                fileCounter++;
            }

            if (invalidTotal != 0)
            {
                feedbackText = $"There are {invalidTotal} invalid hashes! Please see ./sobchak/sobchak.log for details.";
                UpdateProgressStatus(feedbackText);
            }
            else
            {
                feedbackText = $"Complete, no issues found!.";
                UpdateProgressStatus(feedbackText);
            }
        }

        /// <summary></summary>
        /// <param name="fileCounter"></param>
        /// <param name="numberOfFiles"></param>
        private void UpdateProgressBar(int fileCounter, int numberOfFiles)
        {
            int prog                = 765 / numberOfFiles;
            decimal percentComplete = ((decimal)fileCounter/numberOfFiles) * 100;

            lblProgressBar.Width   = fileCounter * prog;
            lblProgressBar.Content = $"{(int)percentComplete}%";

            RefreshContent(lblProgressBar);
        }

        private void UpdateProgressStatus(string feedbackText)
        {

            lblProgressStatus.Content = feedbackText;

            RefreshContent(lblProgressStatus);
        }

        /// <summary>Get the SHA256 value of a file as a byte[].</summary>
        /// <param name="filePath">The file to get the SHA256 value of.</param>
        /// <returns>A SHA256 value as a byte[].</returns>
        private static byte[] GetHashAsBytes(string filePath)
        {
            SHA256 workingHashValue = SHA256.Create();
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
            byte[] hashAsBytes  = GetHashAsBytes(filePath);
            string hashAsString = ConvertHashToString(hashAsBytes);

            return hashAsString;
        }

        /// <summary>Convert a SHA256 hash as a byte[] to a string</summary>
        /// <param name="hashAsBytes">The byte[] that holds the SHA256 hash.</param>
        /// <returns>A SHA256 hash as a string.</returns>
        public static string ConvertHashToString(byte[] hashAsBytes)
        {
            string hashAsString = "";

            for (int currentBit = 0; currentBit < hashAsBytes.Length; currentBit++)
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

        public static void RefreshContent(Label labelToRefresh)
        {
            _ = labelToRefresh.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        // EVENT HANDLERS
        private void btnVerify_Click(object sender, RoutedEventArgs e) => VerifyShas();
    }
}
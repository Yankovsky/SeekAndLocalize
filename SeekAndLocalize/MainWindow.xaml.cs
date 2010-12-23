using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using SeekAndLocalize.Core;

namespace SeekAndLocalize.Presentation
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Public Properties

        private ObservableCollection<StringsSearcherSupportedFileInfo> files;
        public ObservableCollection<StringsSearcherSupportedFileInfo> Files
        {
            get { return files; }
            set { files = value; RaisePropertyChanged("Files"); }
        }

        private StringsSearcherSupportedFileInfo selectedFile;
        public StringsSearcherSupportedFileInfo SelectedFile
        {
            get { return selectedFile; }
            set { selectedFile = value; RaisePropertyChanged("SelectedFile"); }
        }

        private string fileContent;
        public string FileContent
        {
            get { return fileContent; }
            set { fileContent = value; RaisePropertyChanged("FileContent"); }
        }

        private ObservableCollection<StringInFile> stringsInFile;
        public ObservableCollection<StringInFile> StringsInFile
        {
            get { return stringsInFile; }
            set { stringsInFile = value; RaisePropertyChanged("StringsInFile"); }
        }

        private string fileExtension;
        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; RaisePropertyChanged("FileExtension"); }
        }

        private string currentResXOutPath;
        public string CurrentResXOutPath
        {
            get { return currentResXOutPath; }
            set { currentResXOutPath = value; RaisePropertyChanged("CurrentResXOutPath"); }
        }

        private string csKeyTemplate = "Resource.{key}";
        public string CsKeyTemplate
        {
            get { return csKeyTemplate; }
            set { csKeyTemplate = value; RaisePropertyChanged("CsKeyTemplate"); }
        }

        private string xamlKeyTemplate = "{Binding LocalizationResources.{key}}";
        public string XamlKeyTemplate
        {
            get { return xamlKeyTemplate; }
            set { xamlKeyTemplate = value; RaisePropertyChanged("XamlKeyTemplate"); }
        }
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        #region Events

        private void OpenDirectoryClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select Project Folder";
            dlg.ShowNewFolderButton = false;
            dlg.SelectedPath = @"C:\Users\Andrey\Documents\Visual Studio 2010\Projects\SeekAndLocalize";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var allFiles = new List<string>(Directory.GetFiles(dlg.SelectedPath, "*", SearchOption.AllDirectories));
                var filesWithSupportedExtension = FileSifter.SelectFilesWithSupportedExtension(allFiles);
                var list = FileSifter.SelectFilesWithSupportedExtension(allFiles).OrderBy(fi => fi.Extension);
                Files = new ObservableCollection<StringsSearcherSupportedFileInfo>(list);
            }
        }

        private List<string> GetAllDirectoryFiles(string dirPath)
        {
            return new List<string>(Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories));
        }

        private void CreateNewResXFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = "Resource file (*.resx)|*.resx";
            dlg.Title = "Create ResX file";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                CurrentResXOutPath = dlg.FileName;
                ResXManager.CreateResXFile(CurrentResXOutPath);
            }
        }

        private void SaveToExistingResXFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = "Resource file (*.resx)|*.resx";
            dlg.Title = "Open ResX file";
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                CurrentResXOutPath = dlg.FileName;
            }
        }

        private void SiftOutTheFiles(object sender, RoutedEventArgs e)
        {
            var filesToRemove = new List<StringsSearcherSupportedFileInfo>();
            foreach (var file in Files)
            {
                file.LoadFileContent();
                if (file.StringsInFile.Count == 0)
                    filesToRemove.Add(file);
            }
            foreach (var file in filesToRemove)
                Files.Remove(file);
        }

        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            Files.Remove(SelectedFile);
        }

        private void AddStringToResources(object sender, RoutedEventArgs e)
        {
            var button = (sender as System.Windows.Controls.Button);
            var stringKey = ((button.Parent as FrameworkElement).FindName("StringKey") as System.Windows.Controls.TextBox).Text;
            if (String.IsNullOrWhiteSpace(currentResXOutPath))
                System.Windows.MessageBox.Show("Please select ResX out path");
            else if (String.IsNullOrWhiteSpace(stringKey))
                System.Windows.MessageBox.Show("Please enter string key");
            else if (!Regex.IsMatch(XamlKeyTemplate, @"{key}"))
                System.Windows.MessageBox.Show("Your xaml key template doesn't contain keyword {key}");
            else if (!Regex.IsMatch(CsKeyTemplate, @"{key}"))
                System.Windows.MessageBox.Show("Your cs key template doesn't contain keyword {key}");
            else
            {
                var listBoxItem = button.GetParent<ListBoxItem>();
                var stringInFile = (listBoxItem.Content as StringInFile);
                ResXManager.AddResource(currentResXOutPath, stringKey, stringInFile.Content);
                string stringKeyInTemplate;
                if (SelectedFile.Extension == StringsSearcherSupportedFileExtension.Cs)
                    stringKeyInTemplate = CsKeyTemplate.Replace("{key}", stringKey);
                else
                    stringKeyInTemplate = XamlKeyTemplate.Replace("{key}", stringKey);
                SelectedFile.Replace(stringInFile, stringKeyInTemplate);
                SelectedFile.LoadFileContent(true);
                FileContent = SelectedFile.Content;
                StringsInFile = new ObservableCollection<StringInFile>(SelectedFile.StringsInFile);
                FileExtension = SelectedFile.Extension.ToString();
            }
        }

        private void FilesListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedFile != null)
            {
                SelectedFile.LoadFileContent();
                FileContent = SelectedFile.Content;
                StringsInFile = new ObservableCollection<StringInFile>(SelectedFile.StringsInFile);
                FileExtension = SelectedFile.Extension.ToString();
            }
        }

        private void FileStringsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileStringsList.SelectedItem != null)
            {
                var stringInFile = fileStringsList.SelectedItem as StringInFile;
                if (stringInFile != null)
                {
                    FileContentTextBox.Focus();
                    FileContentTextBox.SelectionStart = stringInFile.Index;
                    FileContentTextBox.SelectionLength = stringInFile.Length;
                }
                fileStringsList.SelectedItem = null;
            }
        }

        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
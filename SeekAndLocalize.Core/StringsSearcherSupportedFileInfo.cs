using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SeekAndLocalize.Core
{
    public class StringsSearcherSupportedFileInfo
    {
        public string FilePath { get; private set; }
        public string Content { get; private set; }
        public StringsSearcherSupportedFileExtension Extension { get; private set; }
        public IList<StringInFile> StringsInFile { get; private set; }
        public bool Loaded { get; private set; }

        public StringsSearcherSupportedFileInfo(string filePath, string content, StringsSearcherSupportedFileExtension extension)
        {
            FilePath = filePath;
            Content = content;
            Extension = extension;
        }

        public void Replace(StringInFile stringInFile, string str)
        {
            var newContent = Content.Remove(stringInFile.Index, stringInFile.Length);
            newContent = newContent.Insert(stringInFile.Index, str);
            var writer = new StreamWriter(FilePath);
            writer.Write(newContent);
            writer.Close();
        }

        public void LoadFileContent(bool forcedLoading = false)
        {
            if (!Loaded || forcedLoading)
            {
                var reader = new StreamReader(FilePath);
                Content = reader.ReadToEnd();
                reader.Close();
                FindAllStringsInFile();
                Loaded = true;
            }
        }

        #region Private methods

        private void FindAllStringsInFile()
        {
            StringsInFile = new List<StringInFile>();
            if (Extension == StringsSearcherSupportedFileExtension.Cs)
                FindAllHardCodedStringsInCsFile();
            else if (Extension == StringsSearcherSupportedFileExtension.Xaml)
                FindAllStringsInXamlFile();
        }

        private void FindAllHardCodedStringsInCsFile()
        {
            var hardCodedStringRegex = new Regex(@"(@?"".+"")");
            MatchCollection matches = hardCodedStringRegex.Matches(Content);
            foreach (Match match in matches)
            {
                var a = match.Groups[1];
                if (!String.IsNullOrWhiteSpace(a.Value))
                    StringsInFile.Add(new StringInFile(a.Value, a.Index, a.Length));
            }
        }

        private void FindAllStringsInXamlFile()
        {
            var textPropertiesRegex = new Regex(@"(?<=Text=|Header=|Title=|Content=)""(?!{Binding|{TemplateBinding)(.+?)""");
            var stringContentRegex = new Regex(@"(?<!Visibility|Boolean)>\s*([^<>]+?)\s*</");
            MatchCollection textPropertiesMatches = textPropertiesRegex.Matches(Content);
            foreach (Match match in textPropertiesMatches)
            {
                var a = match.Groups[1];
                if (!String.IsNullOrWhiteSpace(match.Groups[1].Value))
                    StringsInFile.Add(new StringInFile(a.Value, a.Index, a.Length));
            }
            MatchCollection stringContentMatches = stringContentRegex.Matches(Content);
            foreach (Match match in stringContentMatches)
            {
                var a = match.Groups[1];
                if (!String.IsNullOrWhiteSpace(match.Groups[1].Value))
                    StringsInFile.Add(new StringInFile(a.Value, a.Index, a.Length));
            }
        }

        #endregion

    }
}
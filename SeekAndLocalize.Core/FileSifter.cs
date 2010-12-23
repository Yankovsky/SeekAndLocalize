using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SeekAndLocalize.Core
{
    public class FileSifter
    {
        public static List<StringsSearcherSupportedFileInfo> SelectFilesWithSupportedExtension(List<string> filesPaths, Dictionary<string, object> options = null)
        {
            var FilesWithSupportedExtension = new List<StringsSearcherSupportedFileInfo>();
            foreach (var filePath in filesPaths)
            {
                if (xamlFileRegex.IsMatch(filePath))
                    FilesWithSupportedExtension.Add(new StringsSearcherSupportedFileInfo(filePath, null, StringsSearcherSupportedFileExtension.Xaml));
                else if (csFileRegex.IsMatch(filePath))
                    FilesWithSupportedExtension.Add(new StringsSearcherSupportedFileInfo(filePath, null, StringsSearcherSupportedFileExtension.Cs));
            }
            return FilesWithSupportedExtension;
        }

        private static Regex xamlFileRegex = new Regex(@".+(\.xaml)$");
        private static Regex csFileRegex = new Regex(@".+(?<!(\.g(\.i)?)|([dD]esigner)|AssemblyInfo)\.(cs)$");

    }
}

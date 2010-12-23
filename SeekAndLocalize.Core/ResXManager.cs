using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace SeekAndLocalize.Core
{
    public static class ResXManager
    {
        public static void CreateResXFile(string path)
        {
            using (var writer = new ResXResourceWriter(path))
            {
            }
        }

        public static void AddResource(string path, string key, string value)
        {
            var dict = ReadAllResources(path);
            using (var writer = new ResXResourceWriter(path))
            {
                dict[key] = value;
                foreach (var item in dict)
                    writer.AddResource(item.Key, item.Value);
            }
        }

        private static Dictionary<string, string> ReadAllResources(string path)
        {
            var dict = new Dictionary<string, string>();
            using (var reader = new ResXResourceReader(path))
            {
                foreach (DictionaryEntry item in reader)
                    dict.Add(item.Key.ToString(), item.Value.ToString());
            }
            return dict;
        }

    }
}

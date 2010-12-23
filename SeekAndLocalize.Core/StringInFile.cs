namespace SeekAndLocalize.Core
{
    public class StringInFile
    {
        public StringInFile(string content, int index, int length)
        {
            Content = content;
            Index = index;
            Length = length;
        }

        public string Content { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}

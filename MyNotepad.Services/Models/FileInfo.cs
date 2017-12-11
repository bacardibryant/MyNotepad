using Windows.Storage;

namespace MyNotepad.Services.Models
{
    public class FileInfo
    {
        public StorageFile Ref { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
    }
}

using System;


namespace com.companyname.navigationgraph9net9
{
    internal class FileItem
    {
        // 10/03/2016 Need FullName for deletion
        public string? FullName { get; set; }
        public string? Description { get; set; }
        public DateTime TimeStamp { get; set; }
        public long Length { get; set; }
        // 09/03/2016 Added for file maintenance
        public bool Checked { get; set; }
        // 09/07/2016 Added
        public string? DirectoryName { get; set; }
    }
}
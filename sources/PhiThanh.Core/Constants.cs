namespace PhiThanh.Core
{
    public static class Constants
    {
        public const string Anonymous = "Anonymous";
        public const string System = "System";

        public enum ActiveStatus
        {
            Active = 0,
            Inactive = 1
        }

        public enum PostStatus
        {
            Draft = 0,
            Publish = 1
        }

        public static readonly List<string> ExtensionsAllowUpload =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".mp3",
            ".mp4",
            ".flv",
            ".svg",
            ".xlsx",
            ".xls",
            ".pdf",
            ".doc",
            ".docx",
            ".html",
            ".zip",
            ".rar",
            ".7zip",
            ".css",
            ".js",
            ".txt",
            ".xml",
            ".docm",
            ".xlsm",
            ".ico",
            "csv",
            ".avi",
            ".mpeg",
            ".vlc",
            ".json"
        ];

        public static readonly List<string> ExtensionsVideo =
        [
             ".mp4",
            ".flv",
            ".avi",
            ".mpeg",
            ".vlc"
        ];

        public static readonly List<string> ExtensionsImages =
        [
             ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".svg"
        ];
    }
}

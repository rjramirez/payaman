using Common.DataTransferObjects._Base;

namespace Common.DataTransferObjects.ErrorLog
{
    public class SaveErrorLog : SaveDTOExtension
    {
        public string StackTraceId { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        public string StackTrace { get; set; }

        public string Path { get; set; }

        public string Source { get; set; }

        public string UserIdentity { get; set; }

        public string BuildVersion { get; set; }

    }
}

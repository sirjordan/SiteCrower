using System;

namespace SiteCrower.Core
{
    public class ProcessResult
    {
        public ProcessResultStatus Status { get; set; }

        public string Url { get; set; }

        public TimeSpan Finished { get; set; }
    }

    public enum ProcessResultStatus
    {
        Ok,
        Fail,
        Error
    }
}

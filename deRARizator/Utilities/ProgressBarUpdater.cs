using System;

namespace deRARizator.Utilities
{
    public static class ProgressBarUpdater
    {
        public static void UpdateProgressBar(IProgress<int> progress, int value)
        {
            progress.Report(value);
        }
    }
}
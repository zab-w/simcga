using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib
{
    public enum FinishStatus
    {
        Success,
        FailedNormal,
        FailedWithException,
        Interrupted
    }
    public interface IProgressObserver
    {
        void NewWindow(string header);
        void NewWindow(string header, int max);
        void Report(int current, out bool _continue);
        void Finished(FinishStatus result);
        void OnException(Exception ex);
    }
}

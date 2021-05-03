using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib
{
    public abstract class ProgressableObject
    {
        #region Members
        
        IProgressObserver observer = null;

        #endregion

        public ProgressableObject(IProgressObserver observer)
        {
            this.observer = observer;
        }

        #region Observer invoking

        public void SetObserver(IProgressObserver newObserver)
        {
            this.observer = newObserver;
        }

        protected bool OnResult(int current)
        {
            bool c = true;
            if (this.observer != null)
            {
                this.observer.Report(current, out c);
            }
            return c;
        }

        protected void OnFinished(FinishStatus status)
        {
            if (this.observer != null)
            {
                this.observer.Finished(status);
            }
        }

        protected void OnNewOperation(string operation)
        {
            if (this.observer != null)
            {
                this.observer.NewWindow(operation);
            }
        }

        protected void OnNewOperation(string operation, int max)
        {
            if (this.observer != null)
            {
                this.observer.NewWindow(operation, max);
            }
        }

        protected void OnException(Exception ex)
        {
            if (this.observer != null)
            {
                this.observer.OnException(ex);
            }
        }

        #endregion
    }
}

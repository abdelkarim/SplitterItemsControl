using System.ComponentModel;
using System.Runtime.CompilerServices;
using Lib.Demo.Annotations;

namespace Lib.Demo.Core
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region "Methods & Events"

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
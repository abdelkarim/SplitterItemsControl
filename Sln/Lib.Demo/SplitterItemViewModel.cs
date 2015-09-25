using Lib.Demo.Core;

namespace Lib.Demo
{
    public class SplitterItemViewModel : ObservableObject
    {
        #region "Fields"

        private string _content;
        private ItemLength _length; 

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="SplitterItemViewModel"/> class.
        /// </summary>
        public SplitterItemViewModel()
        {
            Length = new ItemLength(1, ItemLengthUnitType.Star);
        } 

        #endregion

        #region "Properties"

        public ItemLength Length
        {
            get { return _length; }
            set
            {
                if (value.Equals(_length)) return;
                _length = value;
                RaisePropertyChanged();
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                if (value == _content) return;
                _content = value;
                RaisePropertyChanged();
            }
        } 

        #endregion
    }
}
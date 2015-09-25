using System.Collections.Generic;
using System.Windows.Input;
using Lib.Demo.Core;

namespace Lib.Demo
{
    public class MainViewModel
    {
        private ICommand _insertCommand;
        private ICommand _addCommand;

        public XtendedObservableCollection<SplitterItemViewModel> SplitterItemViewModels { get; set; }

        /// <summary>
        /// Initializes instance members of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            SplitterItemViewModels = new XtendedObservableCollection<SplitterItemViewModel>();

            for (int i = 0; i < 3; i++)
            {
                SplitterItemViewModels.Add(new SplitterItemViewModel
                {
                    Content = i.ToString()
                });
            }
        }

        private int _itemsCounter = 3;

        public ICommand InsertCommand
        {
            get
            {
                if (_insertCommand == null)
                {
                    _insertCommand = new RelayCommand(o =>
                    {
                        /*var tempList = new List<SplitterItemViewModel>();

                        for (int i = 3; i < 5; i++)
                        {
                            tempList.Add(new SplitterItemViewModel
                            {
                                Content = i.ToString()
                            });
                        }

                        SplitterItemViewModels.AddRange(tempList);*/

                        SplitterItemViewModels.Insert(1, new SplitterItemViewModel {Content = (_itemsCounter++).ToString()});
                    });
                }

                return _insertCommand;
            }
        }

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(o =>
                    {
                        SplitterItemViewModels.Add(new SplitterItemViewModel { Content = (_itemsCounter++).ToString() });
                    });
                }

                return _addCommand;
            }
        }
    }
}

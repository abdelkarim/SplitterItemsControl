using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Lib.Demo.Annotations;

namespace Lib.Demo.Core
{
    public class XtendedObservableCollection<T> : ObservableCollection<T>
    {
        public virtual void AddRange([NotNull] IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.BlockReentrancy();

            var itemsList = items.ToList();
            if (itemsList.Count == 0)
            {
                return;
            }
            
            var startingIndex = this.Items.Count;
            foreach (var item in itemsList)
            {
                this.Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Items[]"));

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsList, startingIndex));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }
    }
}

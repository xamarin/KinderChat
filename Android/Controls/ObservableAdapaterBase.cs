using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace KinderChat
{
    public class ObservableAdapaterBase<TItem> : BaseAdapter where TItem : INotifyPropertyChanged
    {
        readonly Context context;
        readonly ObservableCollection<TItem> sourceItems;
        readonly Func<int, TItem, int> layoutTemplateSelector;
        readonly List<JavaObjectWrapper<TItem>> wrappedItems;
        readonly Action<int, View, TItem> viewSetter;
        private readonly Action<int, View, TItem, string> viewUpdater;
        private ListView listView;

        /// <summary>
        /// An addapter that handles ObservableCollection as a soruce
        /// </summary>
        /// <param name="context">current context</param>
        /// <param name="sourceItems">source observable collection</param>
        /// <param name="layoutTemplateSelector">layout template selector (based on position and item)</param>
        /// <param name="viewSetter">view builder</param>
        /// <param name="viewUpdater">optional: view updater. If not set - notifyDatasetChanged will be called</param>
        public ObservableAdapaterBase(Context context, ObservableCollection<TItem> sourceItems, Func<int, TItem, int> layoutTemplateSelector, Action<int, View, TItem> viewSetter, Action<int, View, TItem, string> viewUpdater = null)
        {
            this.context = context;
            this.sourceItems = sourceItems;
            this.layoutTemplateSelector = layoutTemplateSelector;
            this.viewSetter = viewSetter;
            this.viewUpdater = viewUpdater;

            wrappedItems = new List<JavaObjectWrapper<TItem>>(sourceItems.Count);
            sourceItems.CollectionChanged += OnSourceChanged; //don't forget to call adapter.Dispose in onDestroy (to unsubscribe)
            foreach (var sourceItem in sourceItems)
            {
                sourceItem.PropertyChanged += ValueOnPropertyChanged;
                wrappedItems.Add(new JavaObjectWrapper<TItem>(sourceItem));
            }
        }

        void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var startingIndex = e.NewStartingIndex;
                    foreach (TItem newItem in e.NewItems)
                    {
                        var newWrapper = new JavaObjectWrapper<TItem>(newItem);
                        newItem.PropertyChanged += ValueOnPropertyChanged;
                        if (startingIndex >= 0)
                        {
                            wrappedItems.Insert(startingIndex, newWrapper);
                            startingIndex++;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (TItem oldItem in e.OldItems)
                    {
                        foreach (var item in wrappedItems.ToList())
                        {
                            if (oldItem.Equals(item.Value))
                            {
                                wrappedItems.Remove(item);
                                item.Value.PropertyChanged -= ValueOnPropertyChanged;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Reset:
                    wrappedItems.Clear();
                    break;
            }
            NotifyDataSetChanged();
        }

        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChanged)
        {
            if (viewUpdater == null)
            {
                NotifyDataSetChanged();
                return;
            }

            int position = -1;
            
            for (int index = 0; index < wrappedItems.Count; index++)
                if (((TItem) sender).Equals(wrappedItems[index].Value))
                    position = index;

            if (position < 0)
                return;

			var view = listView.GetChildAt(position - listView.FirstVisiblePosition);
            if (view != null)//means it's visible
            {
                viewUpdater(position, view, ((TItem) sender), propertyChanged.PropertyName);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            sourceItems.CollectionChanged -= OnSourceChanged;
        }

        public override Object GetItem(int position)
        {
            return wrappedItems[position];
        }

        public override long GetItemId(int position)
        {
            return wrappedItems[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            var obj = wrappedItems[position];

            var layoutResourceId = layoutTemplateSelector(position, obj.Value);
            if (row == null || (int)row.Tag != layoutResourceId)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                row = inflater.Inflate(layoutResourceId, parent, false);
            }
            viewSetter(position, row, obj.Value);

            return row;
        }

        public override int Count
        {
            get { return wrappedItems.Count;}
        }

        public void AssignListView(ListView listView)
        {
            this.listView = listView;
            this.listView.Adapter = this;
        }
    }

    public class JavaObjectWrapper<T> : Object
    {
        public T Value { get; set; }

        public JavaObjectWrapper(T value)
        {
            Value = value;
        }
    }
}
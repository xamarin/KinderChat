using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace KinderChat.iOS
{
	public class CollectionUpdater<T>
	{
		public event EventHandler Reset;
		public event EventHandler<AddRemoveReplaceEventArgs> Add;
		public event EventHandler<MoveEventArgs> Move;
		public event EventHandler<AddRemoveReplaceEventArgs> Remove;
		public event EventHandler<AddRemoveReplaceEventArgs> Replace;

		Queue<ChangeInfo<T>> queue;

		ObservableCollection<T> collection;
		List<T> cache;

		public CollectionUpdater (ObservableCollection<T> collection, List<T> cache)
		{
			this.collection = collection;
			this.cache = cache;
			queue = new Queue<ChangeInfo<T>> ();
		}

		public void EnqueCollectionChange(NotifyCollectionChangedEventArgs e)
		{
			var change = ChangeInfo<T>.Create (e, collection);
			lock (this) {
				queue.Enqueue (change);
			}
		}

		public void SyncCache ()
		{
			// TODO: this is not thread safe !!!
			cache.Clear ();
			cache.AddRange (collection);
		}

		public void PerformCollectionUpdate()
		{
			ChangeInfo<T> change;
			lock (this) {
				change = queue.Dequeue ();
			}

			switch(change.Action) {
				case NotifyCollectionChangedAction.Reset:
					cache.Clear ();
					if (change.Items != null)
						cache.AddRange (change.Items);
					RaiseReset ();
					break;

				case NotifyCollectionChangedAction.Add:
					cache.InsertRange (change.NewIndex, change.NewItem);
					RaiseAdd (CreateIndexes (change.NewIndex, change.NewItem.Length));
					break;

				case NotifyCollectionChangedAction.Move:
					for (int i = 0; i < change.OldItem.Length; i++) {
						cache.RemoveAt (change.OldIndex + i);
						cache.Insert (change.NewIndex, change.NewItem [i]);
						RaiseMove (CreateIndex (change.OldIndex), CreateIndex (change.NewIndex));
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					for (int i = 0; i < change.OldItem.Length; i++)
						cache.RemoveAt (change.OldIndex + i);
					RaiseRemove (CreateIndexes (change.OldIndex, change.OldItem.Length));
					break;

				case NotifyCollectionChangedAction.Replace:
					for (int i = 0; i < change.NewItem.Length; i++)
						cache [change.NewIndex + i] = change.NewItem [i];
					RaiseReplace (CreateIndexes (change.NewIndex, change.NewItem.Length));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		NSIndexPath[] CreateIndexes(int startIndex, int count)
		{
			NSIndexPath[] arr = new NSIndexPath[count];
			for (int i = 0; i < count; i++)
				arr [i] = CreateIndex(startIndex + i);

			return arr;
		}

		NSIndexPath CreateIndex(int row)
		{
			return NSIndexPath.FromRowSection (row, 0);
		}

		void RaiseReset()
		{
			var handler = Reset;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		void RaiseAdd(NSIndexPath[] indexes)
		{
			var handler = Add;
			if (handler != null)
				handler (this, new AddRemoveReplaceEventArgs { IndexPaths = indexes });
		}

		void RaiseMove(NSIndexPath from, NSIndexPath to)
		{
			var handler = Move;
			if (handler != null)
				handler (this, new MoveEventArgs { From = from, To = to });
		}

		void RaiseRemove(NSIndexPath[] indexes)
		{
			var handler = Remove;
			if (handler != null)
				handler (this, new AddRemoveReplaceEventArgs { IndexPaths = indexes });
		}

		void RaiseReplace(NSIndexPath[] indexes)
		{
			var handler = Replace;
			if (handler != null)
				handler (this, new AddRemoveReplaceEventArgs { IndexPaths = indexes });
		}
	}

	public class ChangeInfo<T>
	{
		public NotifyCollectionChangedAction Action { get; set; }

		public int OldIndex { get; set; }
		public T[] OldItem { get; set; }

		public int NewIndex { get; set; }
		public T[] NewItem { get; set; }

		// Collection copy 
		// Use only for Reset Action
		public T[] Items { get; set; }

		public static ChangeInfo<T> Create(NotifyCollectionChangedEventArgs e, ObservableCollection<T> collection)
		{
			var change = new ChangeInfo<T> ();
			change.Action = e.Action;
			change.OldIndex = e.OldStartingIndex;
			change.NewIndex = e.NewStartingIndex;

			// More info
			// http://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html
			switch(e.Action) {
				case NotifyCollectionChangedAction.Reset:
					if (collection.Count > 0)
						change.Items = collection.ToArray ();
					break;

					// Assume that add/remove/move/replace only single element
				case NotifyCollectionChangedAction.Add:
					change.NewItem = Copy (e.NewItems);
					break;

				case NotifyCollectionChangedAction.Remove:
					change.OldItem = Copy (e.OldItems);
					break;

				case NotifyCollectionChangedAction.Move:
					change.NewItem = Copy (e.NewItems);
					change.OldItem = Copy (e.OldItems);
					break;

				case NotifyCollectionChangedAction.Replace:
					change.NewItem = Copy (e.NewItems);
					change.OldItem = Copy (e.OldItems);
					break;

				default:
					throw new NotImplementedException ();
			}

			return change;
		}

		static T[] Copy(IList list)
		{
			return list.Cast<T> ().ToArray ();
		}
	}
}


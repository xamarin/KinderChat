using System;
using KinderChat;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KinderChat.Models;

namespace KinderChat
{
	public class KinderDatabase
	{
		readonly SQLiteAsyncConnection database;
		public static string DatabaseLocation {
			get;
			set;
		}

		public KinderDatabase ()
		{
			database = new SQLiteAsyncConnection (DatabaseLocation);

			// We are not awaiting this task, then we will never get exception (if any)
			InitTables ();
		}

		Task InitTables ()
		{
			return Task.WhenAll (
				database.CreateTableAsync<Message> (),
				database.CreateTableAsync<Friend> (),
				database.CreateTableAsync<AvatarItem> (),
				database.CreateTableAsync<DeviceInfo> (),
				database.CreateTableAsync<KinderTask> ()
			);
		}

		public async Task DropAll()
		{
			await database.DropTableAsync<Message> ();
			await database.DropTableAsync<Friend> ();
			await database.DropTableAsync<AvatarItem>();
			await database.DropTableAsync<DeviceInfo>();
		}

		public Task<Friend> GetFriendAsync (long friendId)
		{
			return database.Table<Friend> ()
				.Where (c => c.FriendId == friendId)
				.FirstOrDefaultAsync ();
		}

		public Task<List<Message>> GetRecentMessageAsync()
		{
			return database.Table<Message> ()
				.OrderByDescending (c => c.Date)
				.ToListAsync ();
		}
			
		public Task<List<T>> GetItemsAsync<T> (Expression<Func<T, bool>> predicate = null) where T : EntityBase, new()
		{
		    if (predicate == null)
		    {
		        return database.Table<T> ().ToListAsync ();
            }
            return database.Table<T>().Where(predicate).ToListAsync();
		}

		public Task<T> GetItemAsync<T> (int id) where T : EntityBase, new()
		{
			return database.Table<T> ().Where (x => x.Id == id).FirstOrDefaultAsync ();
		}

        public Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate) where T : EntityBase, new()
        {
            return database.Table<T>().Where(predicate).FirstOrDefaultAsync();
        }

		public Task<int> SaveItemAsync<T> (T item) where T : EntityBase
		{
			if (item.Id != 0) {
				return database.UpdateAsync (item);
			} else {
				return database.InsertAsync (item);
			}
		}

        public async Task SaveItemsAsync<T>(IEnumerable<T> items) where T : EntityBase
        {
            var withId = items.Where(i => i.Id != 0).ToList();
            var withoutId = items.Where(i => i.Id == 0).ToList();

            if (withId.Count > 0)
            {
                await database.UpdateAllAsync(withId);
            }
            if (withoutId.Count > 0)
            {
                await database.InsertAllAsync(withoutId);
            }
        }

		public Task<int> DeleteItemAsync<T> (T item) where T : EntityBase, new()
		{
			return database.DeleteAsync (item);
		}

        public SQLiteAsyncConnection Database { get { return database; } }
	}
}

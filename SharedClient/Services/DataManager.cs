using System;
using KinderChat;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using KinderChat.Helpers;
using KinderChat.Models;
using KinderChat.Services.Messages;

namespace KinderChat
{
	public interface IDataManager
	{
		Task<List<Message>> GetLatestMessagesAsync ();
		Task<List<Friend>> GetFriendsAsync ();
		Task<int> AddOrSaveFriendAsync(Friend friend);
		Task<Friend> GetFriendAsync(long friendId);
		Task<List<AvatarItem>> GetAvatarsAsync();
		Task<int> AddAvatarAsync (AvatarItem avatar);
        Task<Dictionary<string, string>> GetFriendPublicKeysAsync(int userId);
        Task<List<string>> GetDeviceListAsync(long userId);
	    Task AddFriendsPublicKeysAsync(IEnumerable<DeviceInfo> devices);
		Task<List<Message>> GetUnsentMessagesAsync(long friendId);
		Task DropDatabase();
	    Task<bool> AddMessageAsync(Message msg);

		Task<List<KinderTask>> GetKinderTaskAsync ();
		Task AddOrSaveKinderTaskAsync (KinderTask item);
	}

	public class DataManager : IDataManager, IMessageRepository
	{
        readonly KinderDatabase database;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

		public DataManager()
		{
			database = new KinderDatabase ();
		}

		public Task DropDatabase ()
		{
			return database.DropAll ();
		}

		public Task<List<KinderTask>> GetKinderTaskAsync ()
		{
			return database.GetItemsAsync<KinderTask> ();
		}

		public Task AddOrSaveKinderTaskAsync (KinderTask item)
		{
			return database.SaveItemAsync (item);
		}

		public Task<List<AvatarItem>> GetAvatarsAsync ()
		{
			return database.GetItemsAsync<AvatarItem> ();
		}

		public Task<int> AddAvatarAsync (AvatarItem avatar)
		{
			return database.SaveItemAsync<AvatarItem> (avatar);
		}

	    public async Task<Dictionary<string, string>> GetFriendPublicKeysAsync(int userId)
	    {
	        var items = await database.GetItemsAsync<DeviceInfo>(i => i.UserId == userId);
            var result = new Dictionary<string, string>();
	        foreach (var item in items)
	        {
	            result[item.DeviceId] = item.PublicKey;
	        }
	        return result;
	    }

	    public async Task<List<string>> GetDeviceListAsync(long userId)
        {
            var items = await database.GetItemsAsync<DeviceInfo>(i => i.UserId == userId);
	        return items.Select(i => i.DeviceId).Distinct().ToList();
        }

	    public Task AddFriendsPublicKeysAsync(IEnumerable<DeviceInfo> keys)
	    {
	        return database.SaveItemsAsync(keys);
	    }

	    public Task<List<Message>> GetUnsentMessagesAsync (long friendId)
		{
            return database.GetItemsAsync<Message>(m => m.Status == MessageStatus.Unsent && m.Recipient == friendId);
		}

	    public async Task<List<Message>> GetLatestMessagesAsync ()
	    {
			//selected messages by date
            //TODO: refactor it! it loads ALL messages!
			var items = await database.GetRecentMessageAsync ();
			return await Task.Run(()=>
				{
					return items
							.GroupBy(x=>x.ConversationId)
							.Select(c => c.OrderByDescending(m => m.Date).First())
							.OrderByDescending(m => m.Date)
							.ToList();
				});
		}

        #region IMessageRepository

	    public async Task<bool> AddMessageAsync(Message message)
	    {		    
            //using (await LockAsync())
            {
                Message existingMsg = null;
                if (message.MessageToken != Guid.Empty)
                {
                    //ignore duplicates (by Token) - it may happen
                    existingMsg = await database.GetItemAsync<Message>(msg => msg.MessageToken == message.MessageToken);
                    if (existingMsg != null && message.Id < 1)
                    {
                        message.Id = existingMsg.Id;
                    }
                }
                await database.SaveItemAsync<Message>(message);
                return existingMsg == null;
		    }
	    }

        public Task<List<Message>> GetUnsentMessagesAsync()
        {
            return database.GetItemsAsync<Message>(m => m.Status == MessageStatus.Unsent && m.Recipient > 0);
	    }
        
	    public async Task<bool> UpdateMessageStatusAsync(Guid messageToken, MessageStatus status)
	    {
	        //using (await LockAsync())
	        {
	            /*
                var query = string.Format("UPDATE Message WHERE Status < {0} AND MessageToken = '{1}'", (int)status, messageToken);
	            var result = await database.Database.ExecuteAsync(query);
	            return result > 0;
                */

                var msg = await database.GetItemAsync<Message>(i => i.MessageToken == messageToken);
                if (msg == null)
                {
                    return false;
                }
                if (msg.Status >= status)
                {
                    return false;
                }
                msg.Status = status;
                await database.SaveItemAsync(msg);
                return true;
	        }
	    }

        public Task<List<Message>> GetMessagesAsync(long recepientId)
	    {
	        return database.Database.Table<Message> ()
				.Where (c => c.Recipient == recepientId || c.Sender == recepientId)
				.OrderBy(c => c.Date)
				.ToListAsync ();
	    }

        #endregion

        public Task<List<Friend>> GetFriendsAsync ()
		{
			return database.GetItemsAsync<Friend> ();
		}

		public Task<int> AddOrSaveFriendAsync (Friend friend)
		{
			return database.SaveItemAsync<Friend> (friend);
		}

		public Task<Friend> GetFriendAsync (long friendId)
		{
			return database.GetFriendAsync (friendId);
		}

	    private async Task<IDisposable> LockAsync()
	    {
	        await semaphore.WaitAsync();
	        return new DisposableAction(() => semaphore.Release());
	    }
	}
}

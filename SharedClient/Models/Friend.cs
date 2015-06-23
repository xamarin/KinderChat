using System;
using SQLite;

using KinderChat.ServerClient.Entities;

namespace KinderChat
{
	public class Friend : EntityBase
	{
		
		public Friend ()
		{
		}
		[Indexed]
		public long FriendId { get; set; }

		public string Name { get; set; }

		public string Photo {get;set;}

		public AvatarType AvatarType { get; set; }
	}
}


using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace KinderChat.iOS
{
	public static class ConversationsStore
	{
		static string GetJSONPath (string outputFile)
		{
			var path = string.Empty;
			var FileManager = new Foundation.NSFileManager ();
			var appGroupContainer = FileManager.GetContainerUrl ("group.com.xamarin.kinderchat.security");
			if (appGroupContainer == null) {
				Console.WriteLine ("You must create the app group: \"group.com.xamarin.kinderchat.security\" in order to run the project.");
			} else {
				path = Path.Combine (appGroupContainer.Path, outputFile);
			}
			//Console.WriteLine ("Json file path: {0}", path);
			return path;
		}

		public static void Save (List<Message> messageList)
		{
			var path = GetJSONPath ("messageList.xml");

			if (!string.IsNullOrWhiteSpace (path)) {
				var json = JsonConvert.SerializeObject (messageList);
				File.WriteAllText (path, json);
				//Console.WriteLine ("Json content: {0}", json);
			}
		}

		public static void Save (List<Message> conversation, string outputFile)
		{
			var path = GetJSONPath (outputFile);

			if (!string.IsNullOrWhiteSpace (path)) {
				var json = JsonConvert.SerializeObject (conversation);
				File.WriteAllText (path, json);
				//Console.WriteLine ("Json content: {0}", json);
			}
		}
	}
}
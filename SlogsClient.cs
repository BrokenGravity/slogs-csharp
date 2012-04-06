using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.IO;

namespace slogs_csharp
{
	public class SlogsClient
	{
		private string _serverUrl;

		public SlogsClient(string serverUrl)
		{
			if (string.IsNullOrEmpty(serverUrl)) throw new ArgumentNullException("serverUrl");

			_serverUrl = serverUrl;

			if (!serverUrl.EndsWith("/"))
			{
				_serverUrl += "/";
			}
		}

		public void Debug(object data)
		{
			SendEvent(EventType.Debug, data);
		}

		public void Info(object data)
		{
			SendEvent(EventType.Info, data);
		}

		public void Warn(object data)
		{
			SendEvent(EventType.Warn, data);
		}

		public void Error(object data)
		{
			SendEvent(EventType.Error, data);
		}

		private void SendEvent(EventType eventType, object data)
		{
			var json = JsonConvert.SerializeObject(new
			{
				eventType = eventType.ToString(),
				eventData = data
			});

			var bytes = Encoding.Default.GetBytes(json);

			var fullPath = _serverUrl + "events";

			using (var webClient = new WebClient())
			{
				webClient.Headers.Add("Content-Type", "application/json");

				var response = webClient.UploadData(fullPath, "POST", bytes);
				var parsed = Encoding.UTF8.GetString(response);
			}
		}

		public enum EventType
		{
			Fatal,
			Error,
			Warn,
			Info,
			Debug
		}
	}
}

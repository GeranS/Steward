using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Steward.Context;
using Steward.Context.Models;

namespace Steward.Services
{
	public class ActivityService
	{
		private ArrayList messageCache;

		private readonly StewardContext _context;

		public ActivityService(StewardContext context)
		{
			_context = context;

			var cache = new ArrayList();
			messageCache = ArrayList.Synchronized(cache);
		}

		private void WriteCacheToDb()
		{
			foreach (MessageCache c in messageCache)
			{
				var toInsert = new UserMessageRecord()
				{
					ServerId = (long) c.ServerId,
					UserId = c.UserId.ToString(),
					Time = DateTime.Now,
					Amount = c.Amount
				};

				_context.MessageRecords.Add(toInsert);
			}

			_context.SaveChanges();
		}

		public void AddOneToCache(ulong userId, ulong serverId)
		{
			var cache = (List<MessageCache>) from MessageCache mc 
				in messageCache 
				where mc.UserId == userId 
				&& mc.ServerId == serverId
				select mc;

			if (cache.Count == 0)
			{
				var userCache = new MessageCache()
				{
					UserId = userId,
					ServerId = serverId,
					Amount = 1 
				};
				messageCache.Add(userCache);
			}
			else
			{
				cache.First().Amount++;
			}
		}

		private class MessageCache
		{
			public ulong UserId { get; set; }
			public ulong ServerId { get; set; }
			public int Amount { get; set; }
		}
	}
}

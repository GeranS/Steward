using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Steward.Context.Models
{
	public class UserMessageRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long RecordId { get; set; }
		public long ServerId { get; set; }
		public DateTime Time { get; set; }
		public int Amount { get; set; }

		public string UserId { get; set; }
		public DiscordUser User { get; set; }
	}
}

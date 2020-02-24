using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Steward.Context.Models
{
	public class DiscordUser
	{
		[Key]
		public string DiscordId { get; set; }

		public List<PlayerCharacter> Characters { get; set; }

		public List<UserMessageRecord> MessageRecords { get; set; }

		public List<StaffAction> CreatedStaffActions { get; set; }

		public List<StaffAction> AssignedStaffActions { get; set; }

		public bool CanUseAdminCommands { get; set; }
	}
}
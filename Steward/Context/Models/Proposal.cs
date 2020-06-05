using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Steward.Context.Models
{
    public class Proposal
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ProposalId { get; set; }

		public string ProposerId { get; set; }
		public DiscordUser Proposer { get; set; }

		public string ProposedId { get; set; }
		public DiscordUser Proposed { get; set; }



	}
}

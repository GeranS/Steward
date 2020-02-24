using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Steward.Context.Models
{
    public class Graveyard
    {
        [Key]
	    public string ChannelId { get; set; }
        public string ServerId { get; set; }
    }
}

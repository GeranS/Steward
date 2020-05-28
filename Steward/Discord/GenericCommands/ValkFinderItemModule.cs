using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Steward.Context;
using Steward.Context.Models;
using Steward.Discord.CustomPreconditions;
using Steward.Services;


namespace Steward.Discord.GenericCommands
{
    public class ValkFinderItemModule : ModuleBase<SocketCommandContext>
    {
        private readonly StewardContext _stewardContext;

        public ValkFinderItemModule(StewardContext c)
        {
            _stewardContext = c;
        }


    }
}

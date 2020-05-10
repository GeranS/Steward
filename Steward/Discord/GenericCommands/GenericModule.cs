using Discord.Commands;
using System.Threading.Tasks;
using Steward.Context;

namespace Steward.Discord.GenericCommands
{
    public class GenericModule : ModuleBase<SocketCommandContext>
    {
	    private readonly StewardContext _stewardContext;

	    public GenericModule(StewardContext context)
	    {
		    _stewardContext = context;
	    }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }
    }
}

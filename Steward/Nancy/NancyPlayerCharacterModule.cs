using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Nancy;
using Steward.Context;
using Steward.Context.Models;

namespace Steward.Nancy
{
	public class NancyPlayerCharacterModule : NancyModule
	{
		public NancyPlayerCharacterModule()
		{
			Get("all", _ => GetAll());
		}

		public PlayerCharacter GetAll()
		{
			return new StewardContext().PlayerCharacters.ToList()[0];
		}
	}
}

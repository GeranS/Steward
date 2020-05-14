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
			Get("character", (id) => GetOneById(id));
		}

		public PlayerCharacter GetAll()
		{
			return new StewardContext().PlayerCharacters.ToList()[0];
		}

		public PlayerCharacter GetOneById(string id)
		{
			return new StewardContext().PlayerCharacters.SingleOrDefault(pc => pc.CharacterId == id);
		}
	}
}

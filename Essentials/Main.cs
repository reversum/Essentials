using Essentials.EventHandler;
using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace Essentials
{
	public class Main : Plugin<Config>
	{
		public override string Name => "Essentials";
		public override string Description { get; } = "Essentials.";
		public override Version Version { get; } = new Version(1, 0, 0, 0);
		public override string Author => "YannikAufDie1";
		public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
		public static Main Instance { get; set; }
		public EssentialsEventHandler EssentialsEvents { get; set; }
		private Harmony harmony;
		public override void Enable()
		{
			EssentialsEvents = new EssentialsEventHandler(this.Config);
			CustomHandlersManager.RegisterEventsHandler(EssentialsEvents);
			Instance = this;
			harmony = new Harmony("de.yannik.essentials");
			harmony.PatchAll();
		}

		
		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(EssentialsEvents);
			harmony.UnpatchAll();
		}
	}
}

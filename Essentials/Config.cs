using LabApi.Features.Wrappers;
using MapGeneration;
using PlayerRoles;
using Scp914;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Essentials
{
	public class Config
	{
		[Description("Enable debug logs.")]
		public bool Debug { get; set; } = false;

		// ---------------- SCP Related ----------------
		[Description("Adds SCP-3114 to the normal SCP-Pool.")]
		public bool EnableSCP3114 { get; set; } = true;

		[Description("Sets the chance of being SCP-3114.")]
		public float SCP3114SpawnChance { get; set; } = 0.25f;

		[Description("Sets the player preference of being SCP-3114 (1 to 5).")]
		public int SCP3114PlayerPreference { get; set; } = 3;

		[Description("Sets the min count of SCPs to spawn SCP-3114.")]
		public float SCP3114SCPCount { get; set; } = 0;

		// ---------------- Deadman Sequence ----------------
		[Description("Disables the Deadman Sequence Autoheal.")]
		public bool DisableDeadManSequenceAutoHeal { get; set; } = false;

		[Description("Enables editing Deadman Sequence Autoheal settings.")]
		public bool EditDeadManSequenceSettings { get; set; } = false;

		[Description("Deadman Sequence Autoheal Percentage (adds % to the hp of the players).")]
		public float DeadManSequenceAutoHealPercentage { get; set; } = 0.25f;

		// ---------------- Join Message ----------------
		[Description("Enable the join message.")]
		public bool ShouldShowJoinMessage { get; set; } = false;

		[Description("Set the join message text.")]
		public string JoinMessage { get; set; } = "Welcome! Enjoy your stay!";

		[Description("Set the duration of the join message.")]
		public ushort JoinMessageDuration { get; set; } = 5;

		[Description("Set the join message to be a broadcast, otherwise it will be a hint.")]
		public bool JoinMessageAsBroadcast { get; set; } = true;

		// ---------------- Round End ----------------
		[Description("Enable FriendlyFire at the round end.")]
		public bool EnableFriendlyFireAtRoundEnd { get; set; } = true;

		[Description("Show round stats at the end of the round as a broadcast.")]
		public bool ShowRoundStats { get; set; } = true;

		[Description("Set the round stats message (You can use the following placeholders: {mosthumankills}, {mostscpkills}, {mostdamagetoscps}, {fastestescape}, {scpitemsused})")]
		public string RoundStatsMessage { get; set; } = "<size=26>[<color=#F82A2A>R</color><color=#E62724>o</color><color=#D4241E>u</color><color=#C32119>n</color><color=#B11E13>d</color> <color=#A71C10>S</color><color=#AE1C13>t</color><color=#B51C16>a</color><color=#BC1C19>t</color><color=#C31C1B>s</color>]</size>"
			+ "\n<size=24>Player with the most kills (as human): <color=\"red\">{mosthumankills}</color></size>"
			+ "\n<size=24>Player with the most kills (as SCP): <color=\"red\">{mostscpkills}</color></size>"
			+ "\n<size=24>Player that used the most SCP items: <color=\"green\">{scpitemsused}</color></size>";

		[Description("Set the message if nobody has this stat")]
		public string RoundStatsNoOneMessage { get; set; } = "Nobody (sad)...";

		[Description("Set the message to your own language")]
		public string RoundStatsWith { get; set; } = "with";

		[Description("Set the message to your own language")]
		public string RoundStatsIn { get; set; } = "in";

		// ---------------- Advert Messages ----------------
		[Description("Enable random advert messages.")]
		public bool EnableAdvertMessages { get; set; } = false;

		[Description("Advert messages to be displayed randomly.")]
		public List<string> AdvertMessages { get; set; } = new List<string>()
		{
			"Did you know that SCPs are dangerous?",
			"<color=green>Info: You can upgrade items in SCP-914!</color>"
		};

		[Description("Time in seconds between different advert messages.")]
		public ushort AdvertMessageWaitTime { get; set; } = 300;

		[Description("Set the duration of the advert message.")]
		public ushort AdvertMessageDuration { get; set; } = 5;

		[Description("Show advert messages as a broadcast instead of a hint.")]
		public bool AdvertMessageAsBroadcast { get; set; } = true;

		// ---------------- Escape ----------------
		[Description("Keep Effects when escaped.")]
		public bool KeepEffectsAfterEscape { get; set; } = true;

		[Description("Define Custom Escape Scenarios (while being cuffed).")]
		public Dictionary<RoleTypeId, RoleTypeId> CustomCuffedEscapes { get; set; }
			= new Dictionary<RoleTypeId, RoleTypeId>
			{
				[RoleTypeId.NtfPrivate] = RoleTypeId.ChaosRifleman,
				[RoleTypeId.NtfSergeant] = RoleTypeId.ChaosMarauder,
				[RoleTypeId.NtfSpecialist] = RoleTypeId.ChaosMarauder,
				[RoleTypeId.NtfCaptain] = RoleTypeId.ChaosRepressor,
				[RoleTypeId.ChaosRifleman] = RoleTypeId.NtfPrivate,
				[RoleTypeId.ChaosMarauder] = RoleTypeId.NtfSergeant,
				[RoleTypeId.ChaosRepressor] = RoleTypeId.NtfCaptain,
			};

		// ---------------- Role Setup ----------------
		[Description("Set custom start health to Roles on spawn.")]
		public Dictionary<RoleTypeId, uint> CustomRoleHealth { get; set; }
	= new Dictionary<RoleTypeId, uint>
	{
		[RoleTypeId.Scp3114] = 2500,
	};

		// ---------------- Spawn Items ----------------
		[Description("Define spawnitems for roles. If item is a gun amount will be used as ammo.")]
		public Dictionary<RoleTypeId, Dictionary<ItemType, ushort>> SpawnItems { get; set; }
			= new Dictionary<RoleTypeId, Dictionary<ItemType, ushort>>
			{
				[RoleTypeId.Scientist] = new Dictionary<ItemType, ushort>
				{
					[ItemType.Adrenaline] = 1,
				}
			};

		// ---------------- SCP-914 Custom Recipes ----------------
		[Description("Define custom output for each input item, per knob setting.")]
		public Dictionary<ItemType, Dictionary<Scp914KnobSetting, ItemType>> CustomItemRecipes { get; set; }
			= new Dictionary<ItemType, Dictionary<Scp914KnobSetting, ItemType>>
			{
				[ItemType.Coin] = new Dictionary<Scp914KnobSetting, ItemType>
				{
					[Scp914KnobSetting.Rough] = ItemType.Adrenaline,
					[Scp914KnobSetting.Coarse] = ItemType.Adrenaline,
					[Scp914KnobSetting.OneToOne] = ItemType.Adrenaline,
					[Scp914KnobSetting.Fine] = ItemType.Adrenaline,
					[Scp914KnobSetting.VeryFine] = ItemType.Adrenaline
				}
			};

		// ---------------- SCP-914 Role Mappings ----------------
		[Description("Define custom role output per input role, per knob setting.")]
		public Dictionary<RoleTypeId, Dictionary<Scp914KnobSetting, RoleTypeId>> CustomRoleMappings { get; set; }
			= new Dictionary<RoleTypeId, Dictionary<Scp914KnobSetting, RoleTypeId>>
			{
				[RoleTypeId.ClassD] = new Dictionary<Scp914KnobSetting, RoleTypeId>
				{
					[Scp914KnobSetting.Fine] = RoleTypeId.Scientist,
				}
		};

		// ---------------- SCP-914 Teleport Rooms ----------------
		[Description("Custom teleport rooms per knob setting.")]
		public Dictionary<Scp914KnobSetting, List<RoomName>> TeleportRooms { get; set; }
			= new Dictionary<Scp914KnobSetting, List<RoomName>>
			{
				[Scp914KnobSetting.Rough] = new List<RoomName> { RoomName.HczArmory, RoomName.HczMicroHID },
				[Scp914KnobSetting.Coarse] = new List<RoomName> { RoomName.LczGreenhouse, RoomName.LczGlassroom }
			};

		// ---------------- Custom Info ----------------
		[Description("Shows the Player-Health in the Custom-Info.")]
		public bool ShowHealthInCustomInfo { get; set; } = false;

		// ---------------- Weapon Damage Multipliers ----------------
		[Description("Enables/Disables the weapon damage multiplier.")]
		public bool EnableDamageMultiplier { get; set; } = false;

		[Description("Enables/Disables if a human should also give the SCPs more/less damage.")]
		public bool DamageSCPs { get; set; } = false;

		[Description("Default multipliers used if no weapon-specific multipliers are defined.")]
		public Dictionary<string, float> DefaultMultipliers { get; set; } = new Dictionary<string, float>()
		{
			{ "headshot", 1.0f },
			{ "body", 1.0f },
			{ "limb", 1.0f },
		};

		[Description("Weapon-specific damage multipliers per hitbox.")]
		public Dictionary<string, Dictionary<string, float>> WeaponMultipliers { get; set; } =
			new Dictionary<string, Dictionary<string, float>>()
		{
			{ "GunCOM15", new Dictionary<string, float>()
				{
					{ "headshot", 3.0f },
					{ "body", 1.0f },
					{ "limb", 0.8f },
				}
			},
			{ "GunE11SR", new Dictionary<string, float>()
				{
					{ "headshot", 3.0f },
					{ "body", 1.0f },
					{ "limb", 0.8f },
				}
			}
		};

		// ---------------- Candy Stuff ----------------
		[Description("Set the chance to get a pink candy (1 = 1%). Set 0 to disable.")]
		public ushort PinkCandyChance { get; set; } = 1;

		[Description("Sets the maximum number of candies a player can pick up before the player is loosing his hands.")]
		public ushort MaxCandy { get; set; } = 2;

		[Description("Add a message when a players grab a candy. Leave empty to disable it!")]
		public string CandyMessage { get; set; } = "<color=yellow>You grabbed a {type} candy!</color>";

		[Description("Set the duration of the grab candy message.")]
		public ushort CandyMessageDuration { get; set; } = 2;

		[Description("Show the candy message as a hint instead of a broadcast.")]
		public bool CandyMessageAsHint { get; set; } = true;

		// ---------------- Spawnwave Stuff ----------------
		[Description("Change Surface light when a spawnwave/miniwave spawns.")]
		public bool SpawnwaveColorLights { get; set; } = true;

		[Description("Sets the time how long the lights are changed into the team colors.")]
		public float SpawnwaveColorLightsDuration { get; set; } = 15;

		[Description("MTF Spawnwave Light color.")]
		public Color MTFLightColor { get; set; } = new Color(0.17f, 0.34f, 0.85f, 1);

		[Description("CI Spawnwave Light color.")]
		public Color CILightColor { get; set; } = new Color(0.75f, 0.83f, 0.32f, 1);

		// ---------------- Cleanup ----------------
		[Description("Enable the cleanup item timer.")]
		public bool CleanupItems { get; set; } = true;

		[Description("Set the message that is sent when cleaning up items.")]
		public string CleanupItemsBroadcast { get; set; } = "<size=26>[<color=#FF0000>I</color><color=#F70303>t</color><color=#EF0707>e</color><color=#E70B0B>m</color> <color=#D71212>C</color><color=#CF1616>l</color><color=#C71D14>e</color><color=#BF2413>a</color><color=#B72B12>n</color><color=#AF3210>u</color><color=#A7390F>p</color>]</size>"
			+ "\n<size=24><color=#228B22>Deleted <color=#ADFF2F>{itemcount}</color> Items around the map.</color></size>";

		[Description("Set the message that is sent when alerting that cleaning up items is happening in 60 seconds.")]
		public string CleanupItemsAlertBroadcast { get; set; } = "<size=26>[<color=#FF0000>I</color><color=#F70303>t</color><color=#EF0707>e</color><color=#E70B0B>m</color> <color=#D71212>C</color><color=#CF1616>l</color><color=#C71D14>e</color><color=#BF2413>a</color><color=#B72B12>n</color><color=#AF3210>u</color><color=#A7390F>p</color>]</size>"
	+ "\n<size=24><color=#228B22>Warning! Pickups will be deleted in 60 seconds.</color></size>";

		[Description("Cleanup broadcast duration.")]
		public ushort CleanupItemsBroadcastDuration { get; set; } = 5;

		[Description("Cleanup every items X seconds.")]
		public float CleanupItemsDelay { get; set; } = 300;

		[Description("Select the items that should be deleted. (Keycard, Medical, Radio, Firearm, Grenade, SCPItem, SpecialWeapon, Ammo, Armor)")]
		public List<ItemCategory> CleanupItemTypes { get; set; } = new()
		{
			ItemCategory.Armor,
			ItemCategory.Radio,
		};

		[Description("Select the rooms that should be ignored while deleting items.")]
		public List<RoomName> CleanupIgnoreItemRooms { get; set; } = new()
		{
			RoomName.Lcz914,
		};

		[Description("Enable the cleanup ragdolls timer.")]
		public bool CleanupRagdolls { get; set; } = true;

		[Description("Cleanup every ragdoll X seconds.")]
		public float CleanupRagdollsDelay { get; set; } = 120;
	}
}

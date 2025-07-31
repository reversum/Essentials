
using CustomPlayerEffects;
using Essentials.Models;
using InventorySystem;
using InventorySystem.Items.Firearms;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using MEC;
using PlayerStatsSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Escape;
using Logger = LabApi.Features.Console.Logger;
using Random = System.Random;

namespace Essentials.EventHandler
{
	public class EssentialsEventHandler : CustomEventsHandler
	{
		private Config Config;

		private CoroutineHandle timingHandle;
		private CoroutineHandle cleanupItemHandle;
		private CoroutineHandle cleanupRagdollHandle;

		private Dictionary<Player, Dictionary<RoundStatsType, uint>> RoundStats = new();
		private Dictionary<Player, List<EffectKeeper>> PlayerEffects = new();

		public EssentialsEventHandler(Config _config)
		{
			Config = _config;
		}

		public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
		{
			if (Config.ShouldShowJoinMessage)
			{
				if (Config.JoinMessageAsBroadcast)
				{
					ev.Player.SendBroadcast(Config.JoinMessage, Config.JoinMessageDuration);
				} 
				else
				{
					ev.Player.SendHint(Config.JoinMessage, Config.JoinMessageDuration);
				}
			}
		}

		public override void OnPlayerLeft(PlayerLeftEventArgs ev)
		{
			if (Config.ShowRoundStats)
			{
				if (RoundStats.ContainsKey(ev.Player))
				{
					RoundStats.Remove(ev.Player);
				}
			}

			if (Config.KeepEffectsAfterEscape)
			{
				if (PlayerEffects.ContainsKey(ev.Player))
				{
					PlayerEffects[ev.Player].Clear();
				}
			}
		}

		public override void OnServerRoundStarted()
		{
			if (Config.EnableFriendlyFireAtRoundEnd)
			{
				Server.FriendlyFire = false;
			}

			if (Config.EnableAdvertMessages)
			{
				timingHandle = Timing.RunCoroutine(AdvertLoop());
			}

			if (Config.CleanupRagdolls)
			{
				cleanupRagdollHandle = Timing.RunCoroutine(CleanUpRagdolls());
			}

			if (Config.CleanupItems)
			{
				cleanupItemHandle = Timing.RunCoroutine(CleanUpItems());
			}
		}

		public override void OnServerRoundEnded(RoundEndedEventArgs ev)
		{
			if (Config.EnableFriendlyFireAtRoundEnd)
			{
				Server.FriendlyFire = true;
			}

			if (Config.ShowRoundStats)
			{
				string mostHumanKillsMessage = GetStatMessage(RoundStatsType.Humankill);
				string mostScpKillsMessage = GetStatMessage(RoundStatsType.SCPkill);
				string mostDamageToScpsMessage = GetStatMessage(RoundStatsType.MostDamageDoneToScps);
				string fastestEscapeMessage = GetFastestEscapeMessage();
				string scpItemsUsedMessage = GetStatMessage(RoundStatsType.SCPItemsUsed);

				var roundStatsMessage = Config.RoundStatsMessage
					.Replace("{mosthumankills}", mostHumanKillsMessage)
					.Replace("{mostscpkills}", mostScpKillsMessage)
					.Replace("{mostdamagetoscps}", mostDamageToScpsMessage)
					.Replace("{fastestescape}", fastestEscapeMessage)
					.Replace("{scpitemsused}", scpItemsUsedMessage);
				Server.SendBroadcast(roundStatsMessage, 15, Broadcast.BroadcastFlags.Normal, true);

				RoundStats.Clear();
			}

			if (Config.KeepEffectsAfterEscape)
			{
				PlayerEffects.Clear();
			}

			if (Config.EnableAdvertMessages)
			{
				if (timingHandle != null && timingHandle.IsValid && timingHandle.IsRunning)
				{
					Timing.KillCoroutines(timingHandle);

					timingHandle = default;
				}
			}

			if (Config.CleanupItems)
			{
				if (cleanupItemHandle != null && cleanupItemHandle.IsValid && cleanupItemHandle.IsRunning)
				{
					Timing.KillCoroutines(cleanupItemHandle);
				}
			}

			if (Config.CleanupRagdolls)
			{
				if (cleanupRagdollHandle != null && cleanupRagdollHandle.IsValid && cleanupRagdollHandle.IsRunning)
				{
					Timing.KillCoroutines(cleanupRagdollHandle);
				}
			}
		}

		public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
		{
			if (!Config.ShowRoundStats) return;

			if (ev.UsableItem.Base.Category == ItemCategory.SCPItem)
			{
				AddRoundStatToPlayer(ev.Player, RoundStatsType.SCPItemsUsed);
			}
		}

		public override void OnPlayerDeath(PlayerDeathEventArgs ev)
		{
			if (!Config.ShowRoundStats) return;

			if (ev.Attacker != null)
			{
				if (ev.Attacker.IsSCP)
				{
					AddRoundStatToPlayer(ev.Attacker, RoundStatsType.SCPkill);
				} else
				{
					AddRoundStatToPlayer(ev.Attacker, RoundStatsType.Humankill);
				}
			}
		}

		public override void OnPlayerHurt(PlayerHurtEventArgs ev)
		{
			if (!Config.ShowRoundStats) return;

			if (ev.Attacker != null)
			{
				if (ev.Player.IsSCP)
				{
					var damageBase = (StandardDamageHandler)ev.DamageHandler;
					AddRoundStatToPlayer(ev.Attacker, RoundStatsType.MostDamageDoneToScps, (uint)damageBase.TotalDamageDealt);
				}
			}
		}

		public override void OnPlayerEscaping(PlayerEscapingEventArgs ev)
		{
			if (Config.KeepEffectsAfterEscape)
			{
				if (PlayerEffects.ContainsKey(ev.Player))
				{
					PlayerEffects[ev.Player].Clear();
				} else
				{
					PlayerEffects[ev.Player] = new List<EffectKeeper>();
				}

				foreach (var effect in ev.Player.ActiveEffects)
				{
					PlayerEffects[ev.Player].Add(new EffectKeeper() { EffectBase = effect, Intensity = effect.Intensity, Duration = effect.Duration });
				}
			}

			if (ev.Player.IsDisarmed)
			{
				if (Config.CustomCuffedEscapes.TryGetValue(ev.Player.RoleBase.RoleTypeId, out var overrideRole))
				{
					ev.NewRole = overrideRole;
					ev.EscapeScenario = EscapeScenarioType.Custom;
					ev.IsAllowed = true;
				}
			}
		}

		public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
		{
			if (Config.ShowRoundStats)
			{
				AddRoundStatToPlayer(ev.Player, RoundStatsType.MostDamageDoneToScps, (uint)Round.Duration.TotalSeconds);
			}

			if (Config.KeepEffectsAfterEscape)
			{
				if (PlayerEffects.ContainsKey(ev.Player))
				{
					var effects = PlayerEffects[ev.Player];

					if (effects.Count == 0) return;

					Timing.CallDelayed(1f, () =>
					{
						foreach (var effect in effects)
						{
							ev.Player.EnableEffect(effect.EffectBase, effect.Intensity, effect.Duration);
						}
						PlayerEffects[ev.Player].Clear();
					});
				}
			}
		}

		public override void OnScp914ProcessingPlayer(Scp914ProcessingPlayerEventArgs ev)
		{
			var player = ev.Player;
			var knob = ev.KnobSetting;

			{
				if (Config.TeleportRooms.ContainsKey(knob))
				{
					var newRoomName = Config.TeleportRooms[knob].PullRandomItem();

					var newRoom = Room.Get(newRoomName).FirstOrDefault();

					if (newRoom != null)
					{
						Timing.CallDelayed(0.1f, () =>
						{
							player.Position = newRoom.Position + new Vector3(2, 2, 2);
						});
					}
				}
			}

			{
				if (!Config.CustomRoleMappings.ContainsKey(player.RoleBase.RoleTypeId)) return;

				var roleMapping = Config.CustomRoleMappings[player.RoleBase.RoleTypeId];

				if (roleMapping == null) return;

				if (!roleMapping.ContainsKey(knob)) return;

				var newRole = roleMapping[knob];

				player.SetRole(newRole, PlayerRoles.RoleChangeReason.ItemUsage, PlayerRoles.RoleSpawnFlags.None);
			}
		}

		public override void OnScp914ProcessedPickup(Scp914ProcessedPickupEventArgs ev)
		{
			var pickup = ev.Pickup;
			var knob = ev.KnobSetting;

			if (!Config.CustomItemRecipes.ContainsKey(pickup.Type)) return;

			var itemRecipes = Config.CustomItemRecipes[pickup.Type];

			if (itemRecipes == null) return;

			if (!itemRecipes.ContainsKey(knob)) return;

			var newItem = itemRecipes[knob];

			var newpickup = Pickup.Create(newItem, pickup.Position);
			newpickup.Spawn();
			pickup.Destroy();
		}

		public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev)
		{
			var player = ev.Player;
			var item = ev.Item;
			var knob = ev.KnobSetting;

			if (!Config.CustomItemRecipes.ContainsKey(item.Type)) return;

			var itemRecipes = Config.CustomItemRecipes[item.Type];

			if (itemRecipes == null) return;

			if (!itemRecipes.ContainsKey(knob)) return;

			var newItem = itemRecipes[knob];

			Timing.CallDelayed(0.1f, () =>
			{
				player.RemoveItem(item);
				player.AddItem(newItem, InventorySystem.Items.ItemAddReason.Scp914Upgrade);
			});
		}

		public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
		{
			var player = ev.Player;

			if (Config.ShowHealthInCustomInfo)
			{
				player.CustomInfo = $"({player.Health}/{player.MaxHealth})";
			}

			if (!Config.EnableDamageMultiplier) return;

			if (!ev.IsAllowed || ev.Attacker == null) return;

			if (ev.Attacker.Role.IsScp() && !Config.DamageSCPs) return;

			string weaponName = GetWeaponName(ev);

			if (Config.Debug)
			{
				LabApi.Features.Console.Logger.Debug($"[DEBUG] Weaponname: {weaponName}");
			}

			StandardDamageHandler standardDamageHandler = (StandardDamageHandler)ev.DamageHandler;
			string hitbox = standardDamageHandler.Hitbox.ToString().ToLower();

			Dictionary<string, float> multipliers = Config.DefaultMultipliers;

			if (Config.WeaponMultipliers.ContainsKey(weaponName))
			{
				multipliers = Config.WeaponMultipliers[weaponName];
			}

			float multiplier = multipliers.ContainsKey(hitbox) ? multipliers[hitbox] : 1.0f;

			if (multiplier != 1.0f)
			{
				float oldDamage = standardDamageHandler.Damage;
				standardDamageHandler.Damage *= multiplier;
				if (Config.Debug)
				{
					Logger.Debug($"[DEBUG] {ev.Attacker.Nickname} → {ev.Player.Nickname} | Weapon: {weaponName}, Hitbox: {hitbox}, Multiplier: x{multiplier}, Damage: {oldDamage} → {standardDamageHandler.Damage}");
				}
			}
		}

		public override void OnPlayerInteractingScp330(PlayerInteractingScp330EventArgs ev)
		{
			if (Config.PinkCandyChance > 0)
			{
				if (new Random().Next(1, 100) <= Config.PinkCandyChance)
				{
					ev.CandyType = InventorySystem.Items.Usables.Scp330.CandyKindID.Pink;
				}
			}

			if (Config.MaxCandy == 2) return;

			if (ev.Uses < Config.MaxCandy)
			{
				ev.AllowPunishment = false;
				ev.CandyType = InventorySystem.Items.Usables.Scp330.CandyKindID.None;
			}
		}
		
		public override void OnPlayerInteractedScp330(PlayerInteractedScp330EventArgs ev)
		{
			if (!string.IsNullOrEmpty(Config.CandyMessage) && !ev.AllowPunishment)
			{
				string rainbowText = "<color=#FF0000>R</color>" + "<color=#FF7F00>a</color>" + "<color=#FFFF00>i</color>" + "<color=#00FF00>n</color>" + "<color=#0000FF>b</color>" + "<color=#4B0082>o</color>" + "<color=#8B00FF>w</color>";
				if (Config.CandyMessageAsHint)
				{
					if (ev.CandyType == InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow)
					{
						ev.Player.SendHint(Config.CandyMessage.Replace("{type}", rainbowText), Config.CandyMessageDuration);
					}
					else
					{
						ev.Player.SendHint(Config.CandyMessage.Replace("{type}", $"<color=\"{ev.CandyType.ToString()}\">{ev.CandyType.ToString()}</color>").Replace("<color=\"Rainbow\">Rainbow</color>", ""), Config.CandyMessageDuration);
					}
				}
				else
				{
					if (ev.CandyType == InventorySystem.Items.Usables.Scp330.CandyKindID.Rainbow)
					{
						ev.Player.SendBroadcast(Config.CandyMessage.Replace("{type}", rainbowText), Config.CandyMessageDuration);
					}
					else
					{
						ev.Player.SendBroadcast(Config.CandyMessage.Replace("{type}", $"<color=\"{ev.CandyType.ToString()}\">{ev.CandyType.ToString()}</color>").Replace("<color=\"Rainbow\">Rainbow</color>", ""), Config.CandyMessageDuration);
					}
				}
			}
		}

		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
		{
			var roleType = ev.Role.RoleTypeId;
			var player = ev.Player;

			if (Config.ShowHealthInCustomInfo)
			{
				player.CustomInfo = $"({player.Health}/{player.MaxHealth})";
			}

			if (Config.SpawnItems.ContainsKey(roleType))
			{
				var items = Config.SpawnItems[roleType];

				foreach (var itemDic in items)
				{
					var item = player.AddItem(itemDic.Key);

					if (item.Base is Firearm firearm)
					{
						player.AddAmmo(item.Type, itemDic.Value);
					}
					else
					{
						for (int i = 1; i < itemDic.Value; i++)
						{
							player.AddItem(itemDic.Key);
						}
					}
				}
			}

			if (Config.CustomRoleHealth.ContainsKey(roleType))
			{
				player.MaxHealth = Config.CustomRoleHealth[roleType];
				player.Health = Config.CustomRoleHealth[roleType];
			}
		}

		public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
		{
			if (!Config.SpawnwaveColorLights) return;

			if (ev.Wave.Faction == PlayerRoles.Faction.FoundationStaff)
			{
				Map.SetColorOfLights(Config.MTFLightColor, MapGeneration.FacilityZone.Surface);
			} else if (ev.Wave.Faction == PlayerRoles.Faction.FoundationEnemy)
			{
				Map.SetColorOfLights(Config.CILightColor, MapGeneration.FacilityZone.Surface);
			}

			Timing.CallDelayed(Config.SpawnwaveColorLightsDuration, () =>
			{
				Map.ResetColorOfLights(MapGeneration.FacilityZone.Surface);
			});
		}

		private string GetWeaponName(PlayerHurtingEventArgs ev)
		{
			if (ev.Attacker.CurrentItem is Item firearm)
				return firearm.Type.ToString();
			else if (ev.Attacker.Role.IsScp())
				return ev.Attacker.RoleBase.RoleName;
			else
				return "Unknown";
		}

		private void AddRoundStatToPlayer(Player p, RoundStatsType type, uint amount = 1)
		{
			if (!RoundStats.ContainsKey(p))
			{
				RoundStats[p] = new Dictionary<RoundStatsType, uint>();
			}

			if (RoundStats[p].ContainsKey(type))
			{
				RoundStats[p][type] += amount;
			}
			else
			{
				RoundStats[p].Add(type, amount);
			}
		}

		private Player GetPlayerWithHighestStat(RoundStatsType type)
		{
			Player topPlayer = null;
			uint highestValue = 0;

			foreach (var kvp in RoundStats)
			{
				Player player = kvp.Key;
				var stats = kvp.Value;

				if (stats.ContainsKey(type) && stats[type] > highestValue)
				{
					highestValue = stats[type];
					topPlayer = player;
				}
			}

			return topPlayer;
		}

		private string GetStatMessage(RoundStatsType type)
		{
			var player = GetPlayerWithHighestStat(type);
			if (player == null)
				return Config.RoundStatsNoOneMessage;

			uint amount = RoundStats[player][type];
			return $"{player.DisplayName} {Config.RoundStatsWith} {amount}";
		}
		private string GetFastestEscapeMessage()
		{
			var player = GetPlayerWithHighestStat(RoundStatsType.FastestEscape);
			if (player == null)
				return Config.RoundStatsNoOneMessage;

			uint totalSeconds = RoundStats[player][RoundStatsType.FastestEscape];
			int minutes = (int)(totalSeconds / 60);
			int seconds = (int)(totalSeconds % 60);

			string timeString = $"{minutes}m {seconds}s";
			return $"{player.DisplayName} {Config.RoundStatsIn} {timeString}";
		}

		public IEnumerator<float> AdvertLoop()
		{
			for (; ; )
			{
				var random = new Random();
				if (Config.AdvertMessages.Count > 0 && Player.List.Count > 0)
				{
					string message = Config.AdvertMessages[random.Next(Config.AdvertMessages.Count)];

					foreach (var player in Player.List)
					{
						if (Config.AdvertMessageAsBroadcast)
						{
							player.SendBroadcast(message, Config.AdvertMessageDuration);
						}
						else
						{
							player.SendHint(message, Config.AdvertMessageDuration);
						}
					}
				}
				yield return Timing.WaitForSeconds(Config.AdvertMessageWaitTime);
			}
		}

		public IEnumerator<float> CleanUpItems()
		{
			for (; ; )
			{
				float delay = Config.CleanupItemsDelay;

				if (delay > 60f)
				{
					Server.SendBroadcast(Config.CleanupItemsAlertBroadcast, Config.CleanupItemsBroadcastDuration);
					yield return Timing.WaitForSeconds(delay - 60f);
				}
				else
				{
					yield return Timing.WaitForSeconds(delay);
				}

				int deletedItems = 0;
				foreach (var pickup in Pickup.List)
				{
					if (Config.CleanupIgnoreItemRooms.Contains(pickup.Room.Name)) continue;
					if (!Config.CleanupItemTypes.Contains(pickup.Category)) continue;

					deletedItems++;
					pickup.Destroy();
				}

				Server.SendBroadcast(Config.CleanupItemsBroadcast.Replace("{itemcount}", deletedItems.ToString()), Config.CleanupItemsBroadcastDuration);
			}
		}

		public IEnumerator<float> CleanUpRagdolls()
		{
			for (; ; )
			{
				yield return Timing.WaitForSeconds(Config.CleanupRagdollsDelay);

				foreach (var ragdoll in Ragdoll.List)
				{
					ragdoll.Destroy();
				}
			}
		}
	}
}

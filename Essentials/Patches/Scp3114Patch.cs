using HarmonyLib;
using LabApi.Features.Console;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using System.Collections.Generic;
using UnityEngine;

namespace Essentials.Patches
{
	[HarmonyPatch(typeof(ScpSpawner))]
	public static class Scp3114SpawnerPatch
	{
		private static bool _spawned3114;

		[HarmonyPatch(nameof(ScpSpawner.SpawnScps))]
		[HarmonyPrefix]
		static void OnSpawnScpsStart()
		{
			_spawned3114 = false;

			if (!Main.Instance.Config.EnableSCP3114)
				return;

			foreach (var kvp in ScpSpawnPreferences.Preferences)
			{
				var prefs = kvp.Value;

				if (!prefs.Preferences.ContainsKey(RoleTypeId.Scp3114))
					prefs.Preferences[RoleTypeId.Scp3114] = Main.Instance.Config.SCP3114PlayerPreference;
			}
		}

		[HarmonyPatch(nameof(ScpSpawner.NextScp), MethodType.Getter)]
		[HarmonyPrefix]
		static bool Force3114Once(ref RoleTypeId __result)
		{
			if (!Main.Instance.Config.EnableSCP3114)
				return true;

			if (!_spawned3114)
			{
				if (ScpSpawner.MaxSpawnableScps >= Main.Instance.Config.SCP3114SCPCount)
				{
					if (Random.value <= Main.Instance.Config.SCP3114SpawnChance)
					{
						_spawned3114 = true;
						__result = RoleTypeId.Scp3114;
						return false;
					}
				}
			}
			return true;
		}
	}
}

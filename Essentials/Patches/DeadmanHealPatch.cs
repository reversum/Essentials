using HarmonyLib;
using PlayerRoles;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Essentials.Patches
{
	[HarmonyPatch(typeof(DeadmanSwitch), nameof(DeadmanSwitch.OnActivationHeal))]
	public static class DeadmanHealPatch
	{
		public static bool Prefix()
		{
			var config = Main.Instance.Config;
			if (config.DisableDeadManSequenceAutoHeal)
			{
				return false;
			}

			if (config.EditDeadManSequenceSettings)
			{
				foreach (ReferenceHub allHub in ReferenceHub.AllHubs)
				{
					PlayerRoleBase currentRole = allHub.roleManager.CurrentRole;
					if (currentRole.Team == Team.SCPs && currentRole.RoleTypeId != RoleTypeId.Scp0492 && currentRole is IHealthbarRole)
					{
						HealthStat module = allHub.playerStats.GetModule<HealthStat>();
						float num = module.MaxValue * config.DeadManSequenceAutoHealPercentage;
						if (!(module.CurValue >= num))
						{
							module.CurValue = num;
						}
					}
				}
				return false;
			}
			return true;
		}
	}
}

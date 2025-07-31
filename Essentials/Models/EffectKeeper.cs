using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Essentials.Models
{
	public class EffectKeeper
	{
		public CustomPlayerEffects.StatusEffectBase EffectBase { get; set; } 
		public float Duration { get; set; }
		public byte Intensity { get; set; }
	}
}

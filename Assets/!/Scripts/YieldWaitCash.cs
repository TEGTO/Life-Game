using UnityEngine;

namespace YieldCash
{
	public class YieldWaitCash
	{
		private static YieldWaitCash instance;
		public static YieldWaitCash Instance
		{
			get
			{
				if (instance == null)
					instance = new YieldWaitCash();
				return instance;
			}
		}

		private float lastUpdateInterval;
		private WaitForSeconds waitForSeconds;

		public WaitForSeconds GetWaitForSeconds(float seconds)
		{
			if (lastUpdateInterval != seconds)
			{
				lastUpdateInterval = seconds;
				waitForSeconds = new WaitForSeconds(seconds);
			}
			return waitForSeconds;
		}
	}
}


using System;
using System.Collections.Generic;

namespace TrueSync
{
    /// <summary>
    /// ӵ��Stack<List<SyncedData>>
    /// </summary>
    internal class ResourcePoolListSyncedData : ResourcePool<List<SyncedData>>
	{
		protected override List<SyncedData> NewInstance()
		{
			return new List<SyncedData>();
		}
	}
}

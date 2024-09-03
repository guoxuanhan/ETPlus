using System;
using System.Collections.Generic;

namespace ET.Server
{
    public static class RealmGateAddressHelper
    {
        public static StartSceneConfig GetGate(int zone)
        {
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Gates[zone];

            int n = RandomGenerator.RandomNumber(0, zoneGates.Count);

            return zoneGates[n];
        }

        public static StartSceneConfig GetGate(int zone, string account)
        {
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Gates[zone];

            int modeCount = account.Mode(zoneGates.Count);

            return zoneGates[modeCount];
        }
    }
}
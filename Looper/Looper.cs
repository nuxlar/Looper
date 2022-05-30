using BepInEx;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Looper
{
    [BepInPlugin("com.zorp.Looper", "Looper", "1.1.0")]
    [NetworkCompatibility]

    public class Looper : BaseUnityPlugin
    {
        private string[] portals = new string[5]
        {   
            // Bazaar
            "PortalShop",
            // Aurelionite
            "PortalGoldshores",
            // Celestial
            "PortalMS",
            // Void Fields
            "PortalArena",
            // Void Locus
            "PortalVoid"
        };

        private Dictionary<int, string[]> stages = new Dictionary<int, string[]>
        {   // 1st Distant Roost, Titanic Plains, Siphoned Forest
            {1, new string[3] { "blackbeach", "golemplains", "snowyforest" }  },
            // 2nd Abandoned Aqueduct, Wetlant Aspect, Aphelian Sanctuary
            {2, new string[3] { "goolake", "foggyswamp", "ancientloft" }  },
            // 3rd Rallypoint Delta, Scorched Acres, Sulfur Pools
            {3, new string[3] { "frozenwall", "wispgraveyard", "sulfurpools" }  },
            // 4th Abyssal Depths, Sirens Call, Sundered Grove
            {4, new string[3] { "dampcavesimple", "shipgraveyard", "rootjungle" }  },
            // 5th Sky Meadow
            {5, new string[1] { "skymeadow" }  },
        };
        int[] portalStageSpawns;
        public void Awake()
        {
            ModConfig.InitConfig(Config);
            portalStageSpawns = SanitizeStagePortalConfig();
            EntityStates.Missions.LunarScavengerEncounter.FadeOut.duration *= 3.0f;
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += new On.RoR2.TeleporterInteraction.hook_AttemptToSpawnAllEligiblePortals(TeleporterInteraction_AttemptToSpawnAllEligiblePortals);
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += new On.RoR2.VoidRaidGauntletController.hook_SpawnOutroPortal(VoidRaidGauntletController_SpawnOutroPortal);
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += new On.EntityStates.Missions.BrotherEncounter.EncounterFinished.hook_OnEnter(MithrixEncounterFinished_OnEnter);
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += new On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.hook_OnEnter(TwistedEncounterFadeOut_OnEnter);
        }

        private void MithrixEncounterFinished_OnEnter (On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            if (!NetworkServer.active)
                return;
            Vector3 portalPosition = new Vector3(-87.5f, 492f, 5.2f);
            int sanitizedPortalValue = SanitizePortalValue(ModConfig.mithrixPortal.Value);

            SpawnPortal(portalPosition, portals[sanitizedPortalValue]);
            orig.Invoke(self);
        }

        private void VoidRaidGauntletController_SpawnOutroPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            orig.Invoke(self);
            int sanitizedPortalValue = SanitizePortalValue(ModConfig.voidlingPortal.Value);

            SpawnCard portalCard = ScriptableObject.CreateInstance<SpawnCard>();
            portalCard.prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/" + portals[sanitizedPortalValue]);
            DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(portalCard, new DirectorPlacementRule()
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = self.minOutroPortalDistance,
                maxDistance = self.maxOutroPortalDistance,
                spawnOnTarget = self.currentDonut.returnPoint
            }, RoR2Application.rng));
        }

        private void TwistedEncounterFadeOut_OnEnter(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            if (!NetworkServer.active)
                return;
            Vector3 portalPosition = new Vector3(0f, -10f, 0f);
            int sanitizedPortalValue = SanitizePortalValue(ModConfig.twistedPortal.Value);

            SpawnPortal(portalPosition, portals[sanitizedPortalValue]);
            orig.Invoke(self);
        }

        private async void TeleporterInteraction_AttemptToSpawnAllEligiblePortals(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnAllEligiblePortals orig, TeleporterInteraction self)
        {
            
            if (portalStageSpawns.Length > 0)
            {
                int[] stageNums = new int[5] { 1, 2, 3, 4, 5 };
                await Task.Run(() =>
                {
                    foreach (int stageNum in stageNums)
                    {
                        foreach (string stage in stages[stageNum])
                        {
                            if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName(stage) && portalStageSpawns[stageNum - 1] != 0)
                            {
                                switch (ModConfig.stagePortalType.Value)
                                {
                                    case 0:
                                        self.shouldAttemptToSpawnShopPortal = true;
                                        break;
                                    case 1:
                                        self.shouldAttemptToSpawnGoldshoresPortal = true;
                                        break;
                                    case 2:
                                        self.shouldAttemptToSpawnMSPortal = true;
                                        break;
                                }
                                return;
                            }
                        }
                    }
                });                    
            }
            orig(self);
        }
        private void SpawnPortal(Vector3 position, string portalType)
        {
            GameObject portal = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/" + portalType), position, Quaternion.identity);
            Logger.LogInfo("Spawning " + portalType + " at " + position.ToString());
            NetworkServer.Spawn(portal);
        }

        private int[] SanitizeStagePortalConfig()
        {
            int[] numbers = new int[5] { 0, 0, 0, 0, 0 };
            int value = ModConfig.stagePortalNum.Value;

            if (value > 12345 || value <= 0)
                return new int[0];

            for (; value > 0; value /= 10)
            {
                int digit = value % 10;
                numbers[digit - 1] = digit;
            }

            return numbers;
        }

        private int SanitizePortalValue(int configValue)
        {
            int portalValue = configValue;
            if (portalValue > 4 || portalValue < 0)
                portalValue = 0;
            return portalValue;
        }
    }
}

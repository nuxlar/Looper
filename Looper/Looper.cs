using BepInEx;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Looper
{
    [BepInPlugin("com.zorp.Looper", "Looper", "2.0.0")]

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

        private Dictionary<int, SpawnCard> portalCards = new Dictionary<int, SpawnCard>
        {
            // Bazaar
            {1, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscShopPortal") },
            // Aurelionite
            {2, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscGoldshoresPortal") },
            // Celestial
            {3, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscMSPortal") },
            // Void Locus
            {4, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscVoidPortal") },
            // Voidling
            {5, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscDeepVoidPortal") }

        };

        private Dictionary<int, string[]> stages = new Dictionary<int, string[]>
        {   // 1st Distant Roost, Titanic Plains, Siphoned Forest
            {1, new string[5] { "blackbeach", "blackbeach2", "golemplains", "golemplains2", "snowyforest"} },
            // 2nd Abandoned Aqueduct, Wetlant Aspect, Aphelian Sanctuary
            {2, new string[3] { "goolake", "foggyswamp", "ancientloft" }  },
            // 3rd Rallypoint Delta, Scorched Acres, Sulfur Pools
            {3, new string[3] { "frozenwall", "wispgraveyard", "sulfurpools" }  },
            // 4th Abyssal Depths, Sirens Call, Sundered Grove
            {4, new string[3] { "dampcavesimple", "shipgraveyard", "rootjungle" }  },
            // 5th Sky Meadow
            {5, new string[1] { "skymeadow" }  },
        };
        public void Awake()
        {
            ModConfig.InitConfig(Config);
            EntityStates.Missions.LunarScavengerEncounter.FadeOut.duration *= 3.0f;
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += TeleporterInteraction_AttemptToSpawnAllEligiblePortals;
            On.RoR2.VoidRaidGauntletController.SpawnOutroPortal += VoidRaidGauntletController_SpawnOutroPortal;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += MithrixEncounterFinished_OnEnter;
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += TwistedEncounterFadeOut_OnEnter;
        }

        private void MithrixEncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            Vector3 portalPosition = new Vector3(-87.5f, 492f, 5.2f);
            int sanitizedPortalValue = SanitizePortalValue(ModConfig.mithrixPortal.Value);
            if (ModConfig.mithrixPortal.Value == 5)
                SpawnDeepVoidPortal(portalPosition);
            else
                SpawnPortal(portalPosition, portals[sanitizedPortalValue]);
        }

        private void VoidRaidGauntletController_SpawnOutroPortal(On.RoR2.VoidRaidGauntletController.orig_SpawnOutroPortal orig, VoidRaidGauntletController self)
        {
            orig(self);
            if (ModConfig.voidlingPortal.Value == 5)
                SpawnDeepVoidPortal(self.currentDonut.returnPoint.position);
            else
            {
                GameObject portal = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/" + portals[SanitizePortalValue(ModConfig.voidlingPortal.Value)]), self.currentDonut.returnPoint.position, Quaternion.identity);
                portal.GetComponent<DirectorPlacementRule>().placementMode = DirectorPlacementRule.PlacementMode.Approximate;
                portal.GetComponent<DirectorPlacementRule>().minDistance = self.minOutroPortalDistance;
                portal.GetComponent<DirectorPlacementRule>().maxDistance = self.maxOutroPortalDistance;
                portal.GetComponent<DirectorPlacementRule>().spawnOnTarget = self.currentDonut.returnPoint;
                NetworkServer.Spawn(portal);
            }
        }

        private void TwistedEncounterFadeOut_OnEnter(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            orig(self);
            Vector3 portalPosition = new Vector3(0f, -10f, 0f);
            int sanitizedPortalValue = SanitizePortalValue(ModConfig.twistedPortal.Value);

            if (ModConfig.twistedPortal.Value == 5)
                SpawnDeepVoidPortal(portalPosition);
            else
                SpawnPortal(portalPosition, portals[sanitizedPortalValue]);
        }

        private void TeleporterInteraction_AttemptToSpawnAllEligiblePortals(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnAllEligiblePortals orig, TeleporterInteraction self)
        {
            int[] stageNums = new int[5] { 1, 2, 3, 4, 5 };
            foreach (int stageNum in stageNums)
            {
                foreach (string stage in stages[stageNum])
                {
                    if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName(stage))
                    {
                        switch (stageNum)
                        {
                            case 1:
                                if (ModConfig.stage1Portal.Value != 0 && Run.instance.loopClearCount == 0)
                                {
                                    // prevent duplicate portals from spawning
                                    switch (ModConfig.stage1Portal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage1Portal.Value, self.transform, self.rng);
                                }
                                else if (ModConfig.stage1LoopPortal.Value != 0 && Run.instance.loopClearCount != 0)
                                {
                                    switch (ModConfig.stage1LoopPortal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage1LoopPortal.Value, self.transform, self.rng);
                                }
                                break;
                            case 2:
                                if (ModConfig.stage2Portal.Value != 0 && Run.instance.loopClearCount == 0)
                                {
                                    switch (ModConfig.stage2Portal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage2Portal.Value, self.transform, self.rng);
                                }
                                else if (ModConfig.stage2LoopPortal.Value != 0 && Run.instance.loopClearCount != 0)
                                {
                                    switch (ModConfig.stage2LoopPortal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage2LoopPortal.Value, self.transform, self.rng);
                                }
                                break;
                            case 3:
                                if (ModConfig.stage3Portal.Value != 0 && Run.instance.loopClearCount == 0)
                                {
                                    switch (ModConfig.stage3Portal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage3Portal.Value, self.transform, self.rng);
                                }
                                else if (ModConfig.stage3LoopPortal.Value != 0 && Run.instance.loopClearCount != 0)
                                {
                                    switch (ModConfig.stage3LoopPortal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage3LoopPortal.Value, self.transform, self.rng);
                                }
                                break;
                            case 4:
                                if (ModConfig.stage4Portal.Value != 0 && Run.instance.loopClearCount == 0)
                                {
                                    switch (ModConfig.stage4Portal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage4Portal.Value, self.transform, self.rng);
                                }
                                else if (ModConfig.stage4LoopPortal.Value != 0 && Run.instance.loopClearCount != 0)
                                {
                                    switch (ModConfig.stage4LoopPortal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage4LoopPortal.Value, self.transform, self.rng);
                                }
                                break;
                            case 5:
                                if (ModConfig.stage5Portal.Value != 0 && Run.instance.loopClearCount == 0)
                                {
                                    switch (ModConfig.stage5Portal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage5Portal.Value, self.transform, self.rng);
                                }
                                else if (ModConfig.stage5LoopPortal.Value != 0 && Run.instance.loopClearCount != 0)
                                {
                                    switch (ModConfig.stage5LoopPortal.Value)
                                    {
                                        case 1:
                                            self.shouldAttemptToSpawnShopPortal = false;
                                            break;
                                        case 2:
                                            self.shouldAttemptToSpawnGoldshoresPortal = false;
                                            break;
                                        case 3:
                                            self.shouldAttemptToSpawnMSPortal = false;
                                            break;
                                        case 4:
                                            self.portalSpawners = new PortalSpawner[0];
                                            break;
                                    }
                                    SpawnStagePortal(ModConfig.stage5LoopPortal.Value, self.transform, self.rng);
                                }
                                break;
                        }
                        return;
                    }
                }
            }
            orig(self);
        }

        private void SpawnStagePortal(int configPortalValue, Transform transform, Xoroshiro128Plus rng)
        {
            DirectorCore instance = DirectorCore.instance;
            DirectorPlacementRule placementRule = new();
            placementRule.minDistance = 10f;
            placementRule.maxDistance = 40f;
            placementRule.placementMode = DirectorPlacementRule.PlacementMode.Approximate;
            placementRule.position = transform.position;
            placementRule.spawnOnTarget = transform;
            DirectorSpawnRequest directorSpawnRequest = new(portalCards[configPortalValue], placementRule, rng);
            GameObject gameObject = instance.TrySpawnObject(directorSpawnRequest);
            if ((bool)gameObject)
                NetworkServer.Spawn(gameObject);

        }

        private void SpawnPortal(Vector3 position, string portalType)
        {
            GameObject portal = Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/" + portalType), position, Quaternion.identity);
            Logger.LogInfo("Spawning " + portalType + " at " + position.ToString());
            NetworkServer.Spawn(portal);
        }

        private void SpawnDeepVoidPortal(Vector3 position)
        {
            DirectorPlacementRule placementRule = new DirectorPlacementRule();
            placementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
            GameObject spawnedInstance = portalCards[5].DoSpawn(position, Quaternion.identity, new DirectorSpawnRequest(portalCards[5], placementRule, Run.instance.runRNG)).spawnedInstance;
            NetworkServer.Spawn(spawnedInstance);
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

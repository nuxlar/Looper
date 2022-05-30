using BepInEx.Configuration;

namespace Looper
{
    internal class ModConfig
    {
        public static ConfigEntry<int> mithrixPortal;
        public static ConfigEntry<int> twistedPortal;
        public static ConfigEntry<int> voidlingPortal;
        public static ConfigEntry<int> stage;
        public static ConfigEntry<int> stagePortal;

        public static void InitConfig(ConfigFile config)
        {
            mithrixPortal = config.Bind("Boss Portals", "Mithrix Portal", 0, new ConfigDescription("Sets which portal should spawn after Mithrix fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal: 3, Void Locus: 4)"));
            twistedPortal = config.Bind("Boss Portals", "Twisted Portal", 0, new ConfigDescription("Sets which portal should spawn after Twisted Scav fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal: 3, Void Locus: 4)"));
            voidlingPortal = config.Bind("Boss Portals", "Voidling Portal", 0, new ConfigDescription("Sets which portal should spawn after Voidling fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal: 3, Void Locus: 4)"));
            stage = config.Bind("Stage Portals", "Stage Number", 0, new ConfigDescription("Sets which stage a portal should constantly spawn (pre and post loop) (None: 0, Stages: 1 - 5)"));
            stagePortal = config.Bind("Stage Portals", "Portal Type", 0, new ConfigDescription("Sets which portal should spawn at the end of the selected stage (Bazaar: 0, Gold: 1, Celestial: 2)"));
        }
    }
}

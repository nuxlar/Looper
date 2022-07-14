using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace Looper
{
    internal class ModConfig
    {
        public static ConfigEntry<int> mithrixPortal;
        public static ConfigEntry<int> twistedPortal;
        public static ConfigEntry<int> voidlingPortal;

        public static ConfigEntry<int> stage1Portal;
        public static ConfigEntry<int> stage2Portal;
        public static ConfigEntry<int> stage3Portal;
        public static ConfigEntry<int> stage4Portal;
        public static ConfigEntry<int> stage5Portal;

        public static ConfigEntry<int> stage1LoopPortal;
        public static ConfigEntry<int> stage2LoopPortal;
        public static ConfigEntry<int> stage3LoopPortal;
        public static ConfigEntry<int> stage4LoopPortal;
        public static ConfigEntry<int> stage5LoopPortal;

        public static void InitConfig(ConfigFile config)
        {
            mithrixPortal = config.Bind("Boss Portals", "Mithrix Portal", 0, new ConfigDescription("Sets which portal should spawn after Mithrix fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal (Next Stage): 3, Void Locus (Before Voidling): 4, Deep Void (Voidling): 5)"));
            twistedPortal = config.Bind("Boss Portals", "Twisted Portal", 0, new ConfigDescription("Sets which portal should spawn after Twisted Scav fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal (Next Stage): 3, Void Locus (Before Voidling): 4, Deep Void (Voidling): 5)"));
            voidlingPortal = config.Bind("Boss Portals", "Voidling Portal", 0, new ConfigDescription("Sets which portal should spawn after Voidling fight (Bazaar: 0, Gold: 1, Celestial: 2, Null Portal (Next Stage): 3, Void Locus (Before Voidling): 4, Deep Void (Voidling): 5)"));

            stage1Portal = config.Bind("Stage Portals", "Stage 1 Portal", 0, new ConfigDescription("Sets which portal should spawn at the end of stage 1 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage2Portal = config.Bind("Stage Portals", "Stage 2 Portal", 0, new ConfigDescription("Sets which portal should spawn at the end of stage 2 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage3Portal = config.Bind("Stage Portals", "Stage 3 Portal", 0, new ConfigDescription("Sets which portal should spawn at the end of stage 3 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage4Portal = config.Bind("Stage Portals", "Stage 4 Portal", 0, new ConfigDescription("Sets which portal should spawn at the end of stage 4 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage5Portal = config.Bind("Stage Portals", "Stage 5 Portal", 0, new ConfigDescription("Sets which portal should spawn at the end of stage 5 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));

            stage1LoopPortal = config.Bind("Loop Portals", "Stage 1 Loop Portal", 0, new ConfigDescription("LOOP Sets which portal should spawn at the end of stage 1 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage2LoopPortal = config.Bind("Loop Portals", "Stage 2 Loop Portal", 0, new ConfigDescription("LOOP Sets which portal should spawn at the end of stage 2 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage3LoopPortal = config.Bind("Loop Portals", "Stage 3 Loop Portal", 0, new ConfigDescription("LOOP Sets which portal should spawn at the end of stage 3 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage4LoopPortal = config.Bind("Loop Portals", "Stage 4 Loop Portal", 0, new ConfigDescription("LOOP Sets which portal should spawn at the end of stage 4 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));
            stage5LoopPortal = config.Bind("Loop Portals", "Stage 5 Loop Portal", 0, new ConfigDescription("LOOP Sets which portal should spawn at the end of stage 5 (Vanilla: 0, Bazaar: 1, Gold: 2, Celestial: 3, Void Locus: 4, Deep Void: 5)"));

            // Risk Of Options Setup
            // Boss Portals
            ModSettingsManager.AddOption(new IntSliderOption(mithrixPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(twistedPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(voidlingPortal, new IntSliderConfig() { min = 0, max = 5 }));
            // Stage Portals
            ModSettingsManager.AddOption(new IntSliderOption(stage1Portal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage2Portal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage3Portal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage4Portal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage5Portal, new IntSliderConfig() { min = 0, max = 5 }));
            // Loop Portals
            // Stage Portals
            ModSettingsManager.AddOption(new IntSliderOption(stage1LoopPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage2LoopPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage3LoopPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage4LoopPortal, new IntSliderConfig() { min = 0, max = 5 }));
            ModSettingsManager.AddOption(new IntSliderOption(stage5LoopPortal, new IntSliderConfig() { min = 0, max = 5 }));

            ModSettingsManager.SetModDescription("Adds configurable portals after bosses/stages so you can keep looping or orchestrate runs! ");
        }
    }
}

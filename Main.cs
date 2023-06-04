using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using HarmonyLib.Tools;

namespace DamageCap
{
    // These are the mods required for our Mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our Mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our Mod Is associated with
    [BepInProcess("Rounds.exe")]
    public class Main : BaseUnityPlugin
    {
        private const string ModId = "tk.scyye.rounds.DamageCap";
        private const string ModName = "Damage Cap";
        private const string Version = "1.0.0";

        private ConfigEntry<int> MaxDamageConfig;
        public int MaxDamage;

        public static Main instance { get; private set; }

        public void Log(string str)
        {
            UnityEngine.Debug.Log($"[{ModName}] {str}");
        }

        public void LogError(string str)
        {
            UnityEngine.Debug.LogError($"[{ModName}] {str}");
        }

        void Awake()
        {
            instance = this;
            Debug.Log(ModId);

            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            HarmonyFileLog.Enabled = true;

            this.MaxDamageConfig = base.Config.Bind<int>(ModName, "MAX-DAMAGE", 350, "The max damage that a bullet should be able to do. Due to the calculation being annoying, this won't be exact; the inaccurate shouldn't affect anything.");
            this.MaxDamage =this.MaxDamageConfig.Value;

            Log($"Current Max Damage (change in config): {this.MaxDamage}");

            this.MaxDamage += 20;

            if (this.MaxDamage < 55)
            {
                this.MaxDamage *= 55;
                LogError("SET MAX-DAMAGE TO BE GREATER THAN 55!");
                Log("350-400 is the recommended setting");
            }
        }
    }

    [HarmonyPatch(typeof(Gun))]
    class DamageCapPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Gun.Attack))]
        [HarmonyPriority(Priority.First)]
        static void LimitDamage(Gun __instance)
        {
            if (__instance == null) return;

            // TODO: See if the following line works properly
            //gun.damage = Math.Min(gun.damage, Main.MaxDamage / 55);



            if (__instance.damage > Main.instance.MaxDamage / 55)
            {
                __instance.damage = Main.instance.MaxDamage / 55;
            }
        }
    }
}

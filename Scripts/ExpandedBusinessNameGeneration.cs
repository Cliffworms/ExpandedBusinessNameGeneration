using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;

namespace ExpandedBusinessNameGeneration
{
    public class ExpandedBusinessNameGeneration : MonoBehaviour
    {
        private static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<ExpandedBusinessNameGeneration>();

            mod.IsReady = true;
        }

        private void Start()
        {
			Game.Mods.Helper.InternalStringExtender.LoadCsvStrings(mod, "extendedbusinessnames_strings.csv");
        }
    }
}

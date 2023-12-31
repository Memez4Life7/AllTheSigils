// Using Inscryption
using DiskCardGame;
using InscryptionAPI.Card;
using System;
// Modding Inscryption
using System.Collections.Generic;


namespace AllTheSigils
{
    public partial class Plugin
    {
        public void AddBond()
        {
            AbilityInfo info = AbilityManager.New(
                       OldLilyPluginGuid,
                       "Bond",
                       "Any creatures adjacent to [creature] will gain either +1 attack or +1 health, depending on which stat it is closest to.",
                       typeof(Bond),
                       GetTextureLily("Bond")
                   );
            info.SetPixelAbilityIcon(GetTextureLily("bond", true));
            info.powerLevel = 2;
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            info.canStack = true;
            info.opponentUsable = true;

            Bond.ability = info.ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[info.ability] = new Tuple<string, string>("bond", "");
            }
        }
    }

    public class Bond : AbilityBehaviour
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }
    }
}
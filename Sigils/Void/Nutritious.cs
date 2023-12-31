﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Port from Cyn Sigil a day
        private void AddNutritious()
        {
            // setup ability
            const string rulebookName = "Nutritious";
            const string rulebookDescription = "When [creature] is sacrificed, it adds 1 power and 2 health to the card it was sacrificed for.";
            const string LearnDialogue = "That creature is so full of nutrients, the creature you play comes in stronger!";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Nutritious);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Nutritious_a2);
            int powerlevel = 2;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Nutritious.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Nutritious), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Nutritious.ability] = new Tuple<string, string>("ability_nutritious", "");
            }
        }
    }

    public class void_Nutritious : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToSacrifice()
        {
            return true;
        }

        public override IEnumerator OnSacrifice()
        {
            yield return base.PreSuccessfulTriggerSequence();
            CardModificationInfo mod = new CardModificationInfo(1, 2);
            Singleton<BoardManager>.Instance.CurrentSacrificeDemandingCard.AddTemporaryMod(mod);
            yield return base.LearnAbility(0f);
            yield break;
        }

    }

}
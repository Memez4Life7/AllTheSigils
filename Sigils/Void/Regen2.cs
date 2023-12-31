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
        private void AddRegen2()
        {
            // setup ability
            const string rulebookName = "Regen 2";
            const string rulebookDescription = "At the end of the owner's turn, [creature] will regen 2 health.";
            const string LearnDialogue = "This creature will heal 2 Health at the end of it's owner's turn.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Regen_2);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Regen_2_a2);
            int powerlevel = 2;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Regen2.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Regen2), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Regen2.ability] = new Tuple<string, string>("ability_regen_2", "");
            }
        }
    }

    public class void_Regen2 : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            yield return base.PreSuccessfulTriggerSequence();
            if (base.Card.Status.damageTaken > 0)
            {
                base.Card.HealDamage(2);
            }
            yield return base.LearnAbility(0.25f);
            yield break;
        }
    }
}
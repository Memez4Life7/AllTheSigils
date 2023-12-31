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
        //Original
        private void AddEntomophage()
        {
            // setup ability
            const string rulebookName = "Entomophage";
            const string rulebookDescription = "[creature] will deal 2 additional damage to cards of the insect tribe.";
            const string LearnDialogue = "They are a good source of protine I hear.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Entomophage);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Entomophage.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Entomophage), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Entomophage.ability] = new Tuple<string, string>("void_insectKiller", "");
            }
        }
    }

    public class void_Entomophage : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            return amount > 0 && target != null && !target.Dead;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            if (target.Info.IsOfTribe(Tribe.Insect))
            {
                yield return base.PreSuccessfulTriggerSequence();
                yield return new WaitForSeconds(0.15f);
                target.Anim.LightNegationEffect();
                yield return new WaitForSeconds(0.15f);
                yield return target.TakeDamage(2, base.Card);
                yield return new WaitForSeconds(0.15f);
                yield return base.LearnAbility(0f);
            }

            yield break;
        }

    }
}
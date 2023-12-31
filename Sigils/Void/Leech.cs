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
        private void AddLeech()
        {
            // setup ability
            const string rulebookName = "Leech";
            const string rulebookDescription = "When [creature] deals damage, it will heal 1 Health for each damage dealt to a card.";
            const string LearnDialogue = "Vigor from blood!";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Leech);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Leech_a2);
            int powerlevel = 3;
            bool LeshyUsable = Plugin.configLeech.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Leech.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Leech), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Leech.ability] = new Tuple<string, string>("ability_leech", "");
            }
        }
    }

    public class void_Leech : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            return amount > 0;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            yield return base.PreSuccessfulTriggerSequence();
            if (base.Card.Status.damageTaken > 0)
            {
                base.Card.HealDamage(Mathf.Clamp(amount, 1, base.Card.Status.damageTaken));
            }
            base.Card.OnStatsChanged();
            base.Card.Anim.StrongNegationEffect();
            yield return new WaitForSeconds(0.25f);
            yield return base.LearnAbility(0.25f);
            yield break;
        }
    }

}
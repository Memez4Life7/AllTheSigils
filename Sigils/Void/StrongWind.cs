﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Original
        private void AddStrongWind()
        {
            // setup ability
            const string rulebookName = "Strong Wind";
            const string rulebookDescription = "If [creature] is on the board, negate the airbone sigil of all cards that are played after it.";
            const string LearnDialogue = "The Wind forces a landing.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_StrongWind);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_StrongWind_a2);
            int powerlevel = 1;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_StrongWind.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_StrongWind), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_StrongWind.ability] = new Tuple<string, string>("void_StrongWind", "");
            }
        }
    }

    public class void_StrongWind : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
        {
            return base.Card.OnBoard && otherCard.HasAbility(Ability.Flying);
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
        {
            base.Card.Anim.LightNegationEffect();
            yield return base.PreSuccessfulTriggerSequence();
            yield return new WaitForSeconds(0.25f);

            CardModificationInfo negateMod = new CardModificationInfo();
            negateMod.negateAbilities.Add(Ability.Flying);

            otherCard.Anim.PlayTransformAnimation();
            yield return new WaitForSeconds(0.15f);
            otherCard.AddTemporaryMod(negateMod);

            yield return new WaitForSeconds(0.3f);
            yield return base.LearnAbility(0.25f);
            yield break;
        }
    }
}
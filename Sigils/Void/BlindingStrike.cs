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
        //Request by Sire
        private void AddBlindingStrike()
        {
            // setup ability
            const string rulebookName = "Blinding Strike";
            const string rulebookDescription = "When [creature] damages another creature, that creature gains the Random Strikes Sigil.";
            const string LearnDialogue = "Pocket Sand!";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_BlindingStrike);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 2;
            bool LeshyUsable = Plugin.configToxin.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_BlindingStrike.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_BlindingStrike), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_BlindingStrike.ability] = new Tuple<string, string>("void_BlindingStrike", "");
            }
        }
    }

    public class void_BlindingStrike : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            if (target.Dead)
            {
                return false;
            }
            return base.Card.HasAbility(void_Toxin_Deadly.ability);
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            if (target != null && !target.HasAbility(Ability.MadeOfStone))
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                yield return new WaitForSeconds(0.2f);
                base.Card.Anim.LightNegationEffect();
                yield return base.PreSuccessfulTriggerSequence();
                //make the card mondification info
                CardModificationInfo cardModificationInfo = new CardModificationInfo(void_Blind.ability);
                //Clone the main card info so we don't touch the main card set
                CardInfo targetCardInfo = target.Info.Clone() as CardInfo;
                //Add the modifincations to the cloned info
                targetCardInfo.Mods.Add(cardModificationInfo);
                //Set the target's info to the clone'd info
                target.SetInfo(targetCardInfo);
                target.Anim.PlayTransformAnimation();
                yield return new WaitForSeconds(0.3f);
                yield return base.LearnAbility(0.1f);
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
            }
            yield break;
        }

    }
}
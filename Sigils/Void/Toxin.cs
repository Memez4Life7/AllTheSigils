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
        private void AddToxin()
        {
            // setup ability
            const string rulebookName = "Toxin";
            const string rulebookDescription = "When [creature] damages another creature, that creature looses 1 power and 1 health.";
            const string LearnDialogue = "All things can be worn down, and in different ways.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Toxin);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Toxin_a2);
            int powerlevel = 2;
            bool LeshyUsable = Plugin.configToxin.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Toxin.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Toxin), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Toxin.ability] = new Tuple<string, string>("void_toxin", "");
            }
        }
    }

    public class void_Toxin : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDealDamage(int amount, PlayableCard target)
        {
            if (target.Dead)
            {
                return false;
            }
            return true;
        }

        public override IEnumerator OnDealDamage(int amount, PlayableCard target)
        {
            if (target && !target.HasAbility(Ability.MadeOfStone))
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                yield return new WaitForSeconds(0.1f);
                base.Card.Anim.LightNegationEffect();
                yield return base.PreSuccessfulTriggerSequence();
                CardModificationInfo cardModificationInfo = target.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_Toxin");
                if (cardModificationInfo == null)
                {
                    cardModificationInfo = new CardModificationInfo();
                    cardModificationInfo.singletonId = "void_Toxin";
                    target.AddTemporaryMod(cardModificationInfo);
                }
                cardModificationInfo.attackAdjustment--;
                cardModificationInfo.healthAdjustment--;
                target.OnStatsChanged();
                if (target.Health <= 0)
                {
                    yield return target.Die(false, base.Card, true);
                }
                yield return new WaitForSeconds(0.1f);
                yield return base.LearnAbility(0.1f);
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;

            }
            yield break;
        }

    }
}
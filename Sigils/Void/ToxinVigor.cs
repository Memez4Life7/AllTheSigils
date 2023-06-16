﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System.Collections;
using UnityEngine;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Original
        private void AddToxinVigor()
        {
            // setup ability
            const string rulebookName = "Toxin (Vigor)";
            const string rulebookDescription = "When [creature] damages another creature, that creature looses 1 health.";
            const string LearnDialogue = "Even once combat is over, vigor leaves it's target";
            // const string TextureFile = "Artwork/void_weaken.png";

            AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 1, Plugin.configToxin.Value);
            info.canStack = false;
            info.SetPixelAbilityIcon(SigilUtils.LoadImageAndGetTexture("void_toxin_health_a2"));

            Texture2D tex = SigilUtils.LoadImageAndGetTexture("void_toxin_health");



            AbilityManager.Add(OldVoidPluginGuid, info, typeof(void_ToxinVigor), tex);

            // set ability to behaviour class
            void_ToxinVigor.ability = info.ability;


        }
    }

    public class void_ToxinVigor : AbilityBehaviour
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
            if (target)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                yield return new WaitForSeconds(0.1f);
                base.Card.Anim.LightNegationEffect();
                yield return base.PreSuccessfulTriggerSequence();
                CardModificationInfo cardModificationInfo = target.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_ToxinVigor");
                if (cardModificationInfo == null)
                {
                    cardModificationInfo = new CardModificationInfo();
                    cardModificationInfo.singletonId = "void_ToxinVigor";
                    target.AddTemporaryMod(cardModificationInfo);
                }
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
﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Original
        private void AddParalise()
        {
            // setup ability
            const string rulebookName = "Paralysis";
            const string rulebookDescription = "[creature] will only attack every other turn. Some effects from sigils may bypass this.";
            const string LearnDialogue = "A shocking event.";
            // const string TextureFile = "Artwork/void_pathetic.png";

            AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -1, Plugin.configPrideful.Value);
            info.canStack = false;
            info.SetPixelAbilityIcon(SigilUtils.LoadImageAndGetTexture("void_paralysis_a2"));

            Texture2D tex = SigilUtils.LoadImageAndGetTexture("void_paralysis");



            AbilityManager.Add(OldVoidPluginGuid, info, typeof(void_Paralysis), tex);

            // set ability to behaviour class
            void_Paralysis.ability = info.ability;


        }
    }


    public class void_Paralysis : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            yield return base.PreSuccessfulTriggerSequence();
            CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_CantAttack");
            if (cardModificationInfo != null)
            {
                base.Card.RemoveTemporaryMod(cardModificationInfo);
            }
            yield break;
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card != null && base.Card.OpponentCard != playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            yield return base.PreSuccessfulTriggerSequence();
            CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_CantAttack");
            if (cardModificationInfo == null)
            {
                cardModificationInfo = new CardModificationInfo();
                cardModificationInfo.singletonId = "void_CantAttack";
                base.Card.AddTemporaryMod(cardModificationInfo);
                base.Card.Anim.StrongNegationEffect();
            }
            else
            {
                base.Card.RemoveTemporaryMod(cardModificationInfo);
            }
            yield return base.LearnAbility(0f);
            yield break;
        }
    }

    [HarmonyPatch(typeof(CombatPhaseManager), "SlotAttackSlot", MethodType.Normal)]
    public class CombatPhaseManager_Paralysis_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref CombatPhaseManager __instance, CardSlot attackingSlot, CardSlot opposingSlot, float waitAfter = 0f)
        {
            if (attackingSlot.Card != null && attackingSlot.Card.HasAbility(void_Paralysis.ability))
            {
                CardModificationInfo cardModificationInfo = attackingSlot.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_CantAttack");
                if (cardModificationInfo != null)
                {
                    attackingSlot.Card.Anim.StrongNegationEffect();
                    return false;
                }
            }
            return true;
        }
    }
}
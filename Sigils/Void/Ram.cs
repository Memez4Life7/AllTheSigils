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
        private void AddRam()
        {
            // setup ability
            const string rulebookName = "Ram";
            const string rulebookDescription = "[creature] will try to ram the card infront of it when played, or every upkeep till it succeeds once. It will send the rammed target to the queue if on my side, or back to the hand if on your side. Does not work during combat.";
            const string LearnDialogue = "Moving creatures around? How Rude!";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Ram);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Ram_a2);
            int powerlevel = 3;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Ram.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Ram), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Ram.ability] = new Tuple<string, string>("void_shove", "");
            }
        }
    }

    public class void_Ram : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public bool hasShoved = false;


        public override bool RespondsToResolveOnBoard()
        {
            if (hasShoved || SigilEffectUtils.combatPhase)
            {
                return false;
            }
            return base.Card.OnBoard;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep && this.hasShoved == false;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            if (base.Card.Slot.IsPlayerSlot)
            {
                if (base.Card.slot.opposingSlot.Card != null
                && base.Card.HasAbility(void_Ram.ability))
                {
                    PlayableCard target = base.Card.slot.opposingSlot.Card;

                    if (!target.FaceDown && !target.Info.HasTrait(Trait.Uncuttable) && !target.Info.HasTrait(Trait.Giant))
                    {
                        CardSlot oldSlot = target.slot;
                        Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                        yield return new WaitForSeconds(0.1f);
                        yield return new WaitForSeconds(0.1f);
                        target.UnassignFromSlot();
                        yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target, 0.25f);
                        yield return new WaitForSeconds(0.4f);
                        yield return base.LearnAbility(0.25f);
                        yield return new WaitForSeconds(0.1f);
                        hasShoved = true;
                    }
                }
            }
            else
            {
                if (base.Card.slot.opposingSlot.Card != null)
                {
                    PlayableCard target = base.Card.slot.opposingSlot.Card;
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
                    UnityEngine.Object.Destroy(target.gameObject);
                    hasShoved = true;
                }
            }
            yield break;

        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (base.Card.Slot.IsPlayerSlot && playerUpkeep == true)
            {
                if (base.Card.slot.opposingSlot.Card != null
                && base.Card.HasAbility(void_Ram.ability))
                {
                    PlayableCard target = base.Card.slot.opposingSlot.Card;

                    if (!target.FaceDown && !target.Info.HasTrait(Trait.Uncuttable) && !target.Info.HasTrait(Trait.Giant))
                    {
                        CardSlot oldSlot = target.slot;
                        Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
                        yield return new WaitForSeconds(0.1f);
                        yield return new WaitForSeconds(0.1f);
                        target.UnassignFromSlot();
                        yield return Singleton<TurnManager>.Instance.Opponent.ReturnCardToQueue(target, 0.25f);
                        yield return new WaitForSeconds(0.4f);
                        yield return base.LearnAbility(0.25f);
                        yield return new WaitForSeconds(0.1f);
                        hasShoved = true;
                    }
                }
            }
            else
            {
                if (base.Card.slot.opposingSlot.Card != null && playerUpkeep == false)
                {

                    PlayableCard target = base.Card.slot.opposingSlot.Card;
                    target.Anim.PlayDeathAnimation();
                    yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target.Info, null, 0.25f, null);
                    CardSlot slotBeforeDeath = target.slot;
                    target.UnassignFromSlot();
                    target.StartCoroutine(target.DestroyWhenStackIsClear());
                    slotBeforeDeath = null;
                    hasShoved = true;
                }
            }
            yield break;
        }
    }
}
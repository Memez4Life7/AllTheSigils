﻿using APIPlugin;
using DiskCardGame;
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
        private void AddHighTide()
        {
            // setup ability
            const string rulebookName = "High Tide";
            const string rulebookDescription = "While [creature] is on the board, it will grant the waterborne sigil of cards that are played after it on it's side of the board. Does not affect cards that are Airborne.";
            const string LearnDialogue = "The waters rise.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_HighTide);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_HighTide_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_HighTide.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_HighTide), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_HighTide.ability] = new Tuple<string, string>("void_HighTide", "");
            }
        }
    }

    public class void_HighTide : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToOtherCardResolve(PlayableCard otherCard)
        {
            return base.Card.OnBoard && otherCard.slot != base.Card.slot && !otherCard.HasAbility(Ability.Flying);
        }

        public override IEnumerator OnOtherCardResolve(PlayableCard otherCard)
        {

            /// I hate how I coded this but I couldn't figure out (might be cause I made this at 5am) how to make sure the base card is on the players side
            /// and the card that is resolving on board is also on the player's side. So I just got all slots based on if the base.card.slot is a player slot or not
            /// then ran a for loop to check if the other card is in a slot on that side. 

            base.Card.Anim.LightNegationEffect();
            yield return base.PreSuccessfulTriggerSequence();
            yield return new WaitForSeconds(0.25f);
            List<CardSlot> cardSlots = Singleton<BoardManager>.Instance.GetSlots(base.Card.slot.IsPlayerSlot);
            for (var index = 0; index < cardSlots.Count; index++)
            {
                if (cardSlots[index].Card == otherCard && !otherCard.HasAbility(Ability.Submerge) && !otherCard.HasAbility(Ability.SubmergeSquid))
                {
                    //make the card mondification info
                    CardModificationInfo cardModificationInfo = new CardModificationInfo(Ability.Submerge);
                    //Clone the main card info so we don't touch the main card set
                    CardInfo targetCardInfo = otherCard.Info.Clone() as CardInfo;
                    //Add the modifincations to the cloned info
                    targetCardInfo.Mods.Add(cardModificationInfo);
                    targetCardInfo.Mods.AddRange(otherCard.Info.Mods);
                    //Set the target's info to the clone'd info
                    otherCard.SetInfo(targetCardInfo);
                    otherCard.Anim.PlayTransformAnimation();
                    yield return new WaitForSeconds(0.3f);
                    yield return base.LearnAbility(0.25f);
                    yield break;
                }
            }
            yield break;
        }
    }
}
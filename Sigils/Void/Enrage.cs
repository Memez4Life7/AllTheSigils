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
        //Request by Blind
        private void AddEnrage()
        {
            // setup ability
            const string rulebookName = "Enrage";
            const string rulebookDescription = "[creature] will empower adjacent allies, increasing their strenght by 2. However, if they perish while empowered, they are permamently removed from your deck.";
            const string LearnDialogue = "A boost of strength, but at what cost...";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Enrage);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = -1;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Enrage.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Enrage), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Enrage.ability] = new Tuple<string, string>("void_enrage", "");
            }
        }
    }

    public class void_Enrage : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public CardInfo cardInfo;

        public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            return base.Card.OnBoard && deathSlot.IsPlayerSlot && card != base.Card;
        }

        public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer)
        {
            //Get the Deck info
            DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;

            //Find the card in the deck
            CardInfo ci = currentDeck.Cards.Find((CardInfo x) => x.name == card.Info.name);

            //get the base card
            PlayableCard zapper = base.Card;

            //get cards adjacent to the base card
            List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(zapper.slot);

            //if ci is null, then skip the rest
            if (ci == null)
            {
                yield break;
            }

            //check the lower number slot first 
            if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < zapper.slot.Index)
            {
                if (adjacentSlots[0] == deathSlot)
                {
                    currentDeck.RemoveCard(ci);
                    if (!base.HasLearned)
                    {
                        CustomCoroutine.WaitThenExecute(2f, delegate
                        {
                            Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
                            Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
                        }, false);
                    }
                    yield return base.LearnAbility(0.5f);
                }
                adjacentSlots.RemoveAt(0);
            }
            if (adjacentSlots.Count > 0 && adjacentSlots[0] == deathSlot)
            {
                currentDeck.RemoveCard(ci);
                if (!base.HasLearned)
                {
                    CustomCoroutine.WaitThenExecute(2f, delegate
                    {
                        Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
                        Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
                    }, false);
                }
                yield return base.LearnAbility(0.5f);
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(PlayableCard), "GetPassiveAttackBuffs")]
    public class sigil_passive_buffs
    {
        [HarmonyPostfix]
        public static void Postfix(ref int __result, ref PlayableCard __instance)
        {
            if (__instance.OnBoard)
            {
                foreach (CardSlot slotState in Singleton<BoardManager>.Instance.GetAdjacentSlots(__instance.slot))
                {
                    if (slotState.Card != null && slotState.Card.Info.HasAbility(void_Enrage.ability))
                    {
                        __result += 2;
                    }
                }

                foreach (CardSlot slotState in Singleton<BoardManager>.Instance.AllSlots)
                {
                    if (slotState.Card != null && slotState.Card.HasAbility(void_Schooling.ability) && (__instance.HasAbility(Ability.Submerge) || __instance.HasAbility(Ability.SubmergeSquid)))
                    {
                        __result++;
                    }
                }

            }
        }
    }
}
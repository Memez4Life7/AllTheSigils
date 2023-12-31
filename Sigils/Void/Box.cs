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
        private void AddBox()
        {
            // setup ability
            const string rulebookName = "Box";
            const string rulebookDescription = "[creature] will get removed from your deck on death, and a new creature contained within will be added to it.";
            const string LearnDialogue = "What is contained within?";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Box);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = false;
            bool canStack = false;

            // set ability to behaviour class
            void_Box.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Box), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Box.ability] = new Tuple<string, string>("void_crate", "");
            }
        }
    }

    public class void_Box : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return !wasSacrifice;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return this.BreakCage(true);
            yield break;
        }

        private IEnumerator BreakCage(bool fromBattle)
        {
            string creatureWithinId = "Opossum";
            bool flag = base.Card.Info.iceCubeParams != null && base.Card.Info.iceCubeParams.creatureWithin != null;
            if (flag)
            {
                creatureWithinId = base.Card.Info.iceCubeParams.creatureWithin.name;
            }
            yield return new WaitForSeconds(0.5f);
            if (fromBattle)
            {
                RunState.Run.playerDeck.RemoveCard(base.Card.Info);
                RunState.Run.playerDeck.AddCard(CardLoader.GetCardByName(creatureWithinId));
                yield return new WaitForSeconds(1f);
                yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.GetCardByName(creatureWithinId), base.Card.Slot, 0.15f, true);
            }
            yield return new WaitForSeconds(1f);
            yield break;
        }
    }
}
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
        private void AddDrawBone()
        {
            // setup ability
            const string rulebookName = "Draw Bone";
            const string rulebookDescription = "When [creature] is played, a random card costing bone is created in your hand.";
            const string LearnDialogue = "What will it release on death?";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_DrawBone);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_DrawBone_a2);
            int powerlevel = 3;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_DrawBone.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_DrawBone), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_DrawBone.ability] = new Tuple<string, string>("void_drawbone", "");
            }
        }
    }

    public class void_DrawBone : DrawCreatedCard
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override CardInfo CardToDraw
        {
            get
            {
                var creatureWithinId = GetRandomChoosableCardWithCost(base.GetRandomSeed());

                return CardLoader.GetCardByName(creatureWithinId.name);
            }
        }

        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        public override IEnumerator OnResolveOnBoard()
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return base.CreateDrawnCard();
            yield return base.LearnAbility(0f);
            yield break;
        }

        public static CardInfo GetRandomChoosableCardWithCost(int randomSeed)
        {
            List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.ChoiceNode, CardTemple.Nature).FindAll((CardInfo x) => x.bonesCost > 0);
            bool flag1 = SaveManager.SaveFile.IsPart2;
            if (flag1)
            {
                list.Clear();
                list = CardLoader.GetUnlockedCards(CardMetaCategory.GBCPack, CardTemple.Undead).FindAll((CardInfo x) => x.bonesCost > 0);
            }
            bool flag = list.Count == 0;
            CardInfo result;
            if (flag)
            {
                result = null;
            }
            else
            {
                result = CardLoader.GetCardByName(list[SeededRandom.Range(0, list.Count, randomSeed)].name);
            }
            if (result == null)
            {
                result = CardLoader.GetCardByName("Opposum");
            }
            return result;
        }


    }
}
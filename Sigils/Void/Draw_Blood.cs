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
        private void AddDrawBlood()
        {
            // setup ability
            const string rulebookName = "Draw Blood";
            const string rulebookDescription = "When [creature] is played, a random card costing blood is created in your hand.";
            const string LearnDialogue = "What will it release on death?";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_DrawBlood);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_DrawBlood_a2);
            int powerlevel = 3;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_DrawBlood.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_DrawBlood), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_DrawBlood.ability] = new Tuple<string, string>("void_drawblood", "");
            }
        }
    }

    public class void_DrawBlood : DrawCreatedCard
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
            bool flag = Singleton<ViewManager>.Instance.CurrentView != this.DrawCardView;
            if (flag)
            {
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(this.DrawCardView, false, false);
                yield return new WaitForSeconds(0.2f);
            }
            yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(this.CardToDraw, base.Card.TemporaryMods, 0.25f, null);
            yield return new WaitForSeconds(0.45f);
            yield return base.LearnAbility(0.1f);
            yield break;
        }

        public static CardInfo GetRandomChoosableCardWithCost(int randomSeed)
        {
            List<CardInfo> list = CardLoader.GetUnlockedCards(CardMetaCategory.ChoiceNode, CardTemple.Nature).FindAll((CardInfo x) => x.BloodCost > 0);
            bool flag1 = SaveManager.SaveFile.IsPart2;
            if (flag1)
            {
                list.Clear();
                list = CardLoader.GetUnlockedCards(CardMetaCategory.GBCPack, CardTemple.Nature).FindAll((CardInfo x) => x.BloodCost > 0);
            }
            bool flag = list.Count == 0;
            CardInfo result;
            if (flag)
            {
                result = null;
            }
            else
            {
                result = CardLoader.Clone(list[SeededRandom.Range(0, list.Count, randomSeed)]);
            }
            return result;
        }
    }
}
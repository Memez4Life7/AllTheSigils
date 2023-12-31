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
        private void AddGiant()
        {
            // setup ability
            const string rulebookName = "Giant";
            const string rulebookDescription = "When [creature] is drawn, it will gain one unit blood of cost, as well as one attack and two health.";
            const string LearnDialogue = "What a large creature you have there";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Giant);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Giant_a2);
            int powerlevel = 4;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Giant.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Giant), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Giant.ability] = new Tuple<string, string>("void_Giant", "");
            }
        }
    }

    public class void_Giant : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDrawn()
        {
            return true;
        }

        public override IEnumerator OnDrawn()
        {
            yield return base.PreSuccessfulTriggerSequence();
            (Singleton<PlayerHand>.Instance as PlayerHand3D).MoveCardAboveHand(base.Card);
            yield return base.Card.FlipInHand(new Action(this.AddMod));
            yield return base.LearnAbility(0.5f);
            yield break;
        }

        private void AddMod()
        {
            CardModificationInfo cardModificationInfo = new CardModificationInfo();
            cardModificationInfo.attackAdjustment = 1;
            cardModificationInfo.healthAdjustment = 2;
            cardModificationInfo.nameReplacement = string.Format(Localization.Translate("Giant {0}"), base.Card.Info.DisplayedNameLocalized);
            cardModificationInfo.bloodCostAdjustment = 1;

            CardInfo ClonedCardInfo = base.Card.Info.Clone() as CardInfo;

            //Add the modifincations
            ClonedCardInfo.Mods.Add(cardModificationInfo);

            for (int index = 0; index < base.Card.Info.Mods.Count; index++)
            {
                ClonedCardInfo.Mods.Add(base.Card.Info.Mods[index]);
            }


            //Update the card info
            base.Card.SetInfo(ClonedCardInfo);
        }
    }
}
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
        //Port from Cyn Sigil a day
        private void AddTransient()
        {
            // setup ability
            const string rulebookName = "Transient";
            const string rulebookDescription = "At the end of the owner's turn, [creature] will return to your hand.";
            const string LearnDialogue = "The creature blinks back into the owner's hand at the end of their turn.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Transient);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Transient_a2);
            int powerlevel = -1;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Transient.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Transient), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Transient.ability] = new Tuple<string, string>("ability_transient", "");
            }
        }
    }

    public class void_Transient : DrawCreatedCard
    {

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        public static Ability ability;

        private void Start()
        {
            this.copy = CardLoader.Clone(base.Card.Info);
        }

        public override CardInfo CardToDraw
        {
            get
            {
                return CardLoader.Clone(this.copy);
            }
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return base.CreateDrawnCard();
            base.Card.Anim.PlayDeathAnimation(false);
            base.Card.UnassignFromSlot();
            base.Card.StartCoroutine(base.Card.DestroyWhenStackIsClear());
            base.Card.Slot = null;
            yield break;
        }

        private CardInfo copy;
    }
}
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
        //Request by Sire
        private void AddDiveEnergy()
        {
            // setup ability
            const string rulebookName = "Dive (Energy)";
            const string rulebookDescription = "Pay 2 energy to cause this card to flip face down at the end of the owner's turn.";
            const string LearnDialogue = "Care for a swim?";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Dive_Energy);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;


            var test = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Dive_Energy), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack);
            test.activated = true;
            test.pixelIcon = SigilUtils.LoadSpriteFromResource(Art.void_Dive_Energy_a2);



            // set ability to behaviour class
            void_Lead_Energy.ability = test.ability;

            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Lead_Energy.ability] = new Tuple<string, string>("void_Dive_Energy", "");
            }
        }
    }

    public class void_Dive_Energy : ActivatedAbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        bool timeToDive = false;

        public override int EnergyCost
        {
            get
            {
                return 2;
            }
        }

        public override IEnumerator Activate()
        {
            timeToDive = true;
            base.Card.Anim.StrongNegationEffect();
            yield break;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep && base.Card.FaceDown;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
            yield return new WaitForSeconds(0.15f);
            yield return base.PreSuccessfulTriggerSequence();
            base.Card.SetFaceDown(false, false);
            base.Card.UpdateFaceUpOnBoardEffects();
            this.OnResurface();
            yield return new WaitForSeconds(0.3f);
            this.triggerPriority = int.MinValue;
            yield break;
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card.OpponentCard != playerTurnEnd && !base.Card.FaceDown && timeToDive == true;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
            yield return new WaitForSeconds(0.15f);
            base.Card.SetCardbackSubmerged();
            base.Card.SetFaceDown(true, false);
            yield return new WaitForSeconds(0.3f);
            yield return base.LearnAbility(0f);
            this.triggerPriority = int.MaxValue;
            timeToDive = false;
            yield break;
        }

        protected virtual void OnResurface()
        {
        }


        private int triggerPriority = int.MinValue;
    }
}
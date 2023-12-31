﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;

using Random = UnityEngine.Random;

namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by blind
        private void AddPossessorPowerful()
        {
            // setup ability
            const string rulebookName = "Powerful Possessor";
            const string rulebookDescription = "When [creature] perishes, it will grant a random friendly card that is on the board it's base plus modified power and base plus modified health.";
            const string LearnDialogue = "It passes it's strength onto those who remain";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Possessor_Powerful);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Possessor_Powerful_a2);
            int powerlevel = 3;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Possessor_Powerful.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Possessor_Powerful), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Possessor_Powerful.ability] = new Tuple<string, string>("void_Possessor_Powerful", "");
            }

        }
    }

    public class void_Possessor_Powerful : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        private CardModificationInfo mod;

        private void Start()
        {
            this.mod = new CardModificationInfo();
        }


        public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
        {
            return base.Card.OnBoard;
        }

        public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
        {
            // Get the base card
            PlayableCard card = base.Card;
            int modifiedHealth = 0;
            int modifiedAttack = 0;
            if (card.TemporaryMods.Count > 0)
            {
                var holder = card.TemporaryMods.FindAll((CardModificationInfo x) => x.healthAdjustment != 0);
                foreach (CardModificationInfo mod in holder)
                {
                    modifiedHealth += mod.healthAdjustment;
                }

                holder = card.TemporaryMods.FindAll((CardModificationInfo x) => x.attackAdjustment != 0);
                foreach (CardModificationInfo mod in holder)
                {
                    modifiedAttack += mod.attackAdjustment;
                }
            }
            this.mod.attackAdjustment = card.Info.baseAttack + card.GetPassiveAttackBuffs() + modifiedAttack;
            this.mod.healthAdjustment = card.Info.baseHealth + card.GetPassiveHealthBuffs() + modifiedHealth;



            if (card.slot.IsPlayerSlot)
            {
                // Get all slots
                List<CardSlot> allSlots = Singleton<BoardManager>.Instance.playerSlots;

                // Initalize target list
                List<PlayableCard> targets = new List<PlayableCard>();

                // Go thru all slots to see if there is a card in it, and if there is, add it to the target list
                for (int index = 0; index < allSlots.Count; index++)
                {
                    if (allSlots[index].Card != null && allSlots[index].Card != base.Card)
                    {
                        targets.Add(allSlots[index].Card);
                    }
                }
                if (targets.Count > 0)
                {
                    // pick a random target from the target list
                    PlayableCard target = targets[SeededRandom.Range(0, (targets.Count), base.GetRandomSeed())];
                    base.Card.Anim.LightNegationEffect();
                    yield return base.PreSuccessfulTriggerSequence();
                    target.Anim.StrongNegationEffect();
                    target.temporaryMods.Add(this.mod);
                    target.Anim.PlayTransformAnimation();
                    yield return base.LearnAbility(0.25f);
                }

            }
            else
            {
                // Get all slots
                List<CardSlot> allSlots = Singleton<BoardManager>.Instance.opponentSlots;

                // Initalize target list
                List<PlayableCard> targets = new List<PlayableCard>();

                // Go thru all slots to see if there is a card in it, and if there is, add it to the target list
                for (int index = 0; index < allSlots.Count; index++)
                {
                    if (allSlots[index].Card != null && allSlots[index].Card != base.Card)
                    {
                        targets.Add(allSlots[index].Card);
                    }
                }
                if (targets.Count > 0)
                {
                    // pick a random target from the target list
                    PlayableCard target = targets[Random.Range(0, (targets.Count))];
                    base.Card.Anim.LightNegationEffect();
                    yield return base.PreSuccessfulTriggerSequence();
                    target.Anim.StrongNegationEffect();
                    target.temporaryMods.Add(this.mod);
                    target.Anim.PlayTransformAnimation();
                    yield return base.LearnAbility(0.25f);
                }
            }
            yield break;
        }
    }
}
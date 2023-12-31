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
        private void AddBloodGrowth()
        {
            // setup ability
            const string rulebookName = "Blood Growth";
            const string rulebookDescription = "When [creature] attacks, the amount of blood it is counted as when sacrificed will increase.";
            const string LearnDialogue = "There is power in the blood.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_BloodGrowth);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_BloodGrowth_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;



            // set ability to behaviour class
            void_BloodGrowth.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_BloodGrowth), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_BloodGrowth.ability] = new Tuple<string, string>("void_bloodgrowth", "");
            }

        }
    }

    public class void_BloodGrowth : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public int attacks = 0;

        private CardModificationInfo mod;


        public static readonly Ability CustomAbility1 = InscryptionAPI.Guid.GuidManager.GetEnumValue<Ability>("org.memez4life.inscryption.customsigils", "Noble Sacrifice");
        public static readonly Ability CustomAbility2 = InscryptionAPI.Guid.GuidManager.GetEnumValue<Ability>("org.memez4life.inscryption.customsigils", "Superior Sacrifice");


        private void Start()
        {
            this.mod = new CardModificationInfo();
        }

        public override bool RespondsToSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            return attacker == base.Card;
        }

        public override IEnumerator OnSlotTargetedForAttack(CardSlot slot, PlayableCard attacker)
        {
            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
            yield return new WaitForSeconds(0.05f);
            yield return base.PreSuccessfulTriggerSequence();
            base.Card.Status.hiddenAbilities.Add(this.Ability);

            this.attacks += 1;

            switch (this.attacks)
            {
                case 1:
                    base.Card.RemoveTemporaryMod(this.mod);
                    Ability Ab1 = CustomAbility1;
                    this.mod.abilities.Clear();
                    this.mod.abilities.Add(Ab1);
                    base.Card.AddTemporaryMod(this.mod);
                    break;
                case 2:
                    base.Card.RemoveTemporaryMod(this.mod);
                    Ability Ab2 = Ability.TripleBlood;
                    this.mod.abilities.Clear();
                    this.mod.abilities.Add(Ab2);
                    base.Card.AddTemporaryMod(this.mod);
                    break;
                case 3:
                    base.Card.RemoveTemporaryMod(this.mod);
                    Ability Ab3 = CustomAbility2;
                    this.mod.abilities.Clear();
                    this.mod.abilities.Add(Ab3);
                    base.Card.AddTemporaryMod(this.mod);
                    break;
            }


            yield return new WaitForSeconds(0.05f);
            yield return base.LearnAbility(0f);
            yield break;
        }

    }
}
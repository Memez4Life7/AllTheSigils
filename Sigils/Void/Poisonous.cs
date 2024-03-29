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
        private void AddPoisonous()
        {
            // setup ability
            const string rulebookName = "Poisonous";
            const string rulebookDescription = "When [creature] perishes, the creature that killed it perishes as well.";
            const string LearnDialogue = "Attacking something poisonous, isn't that smart.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Poisonous);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Poisonous_a2);
            int powerlevel = 2;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Poisonous.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Poisonous), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Poisonous.ability] = new Tuple<string, string>("ability_poisonous", "");
            }
        }
    }

    public class void_Poisonous : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return !wasSacrifice && base.Card.OnBoard;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            yield return base.PreSuccessfulTriggerSequence();
            yield return new WaitForSeconds(0.25f);
            if (killer != null && killer.LacksAbility(Ability.MadeOfStone))
            {
                yield return killer.Die(false, base.Card, true);
                if (Singleton<BoardManager>.Instance is BoardManager3D)
                {
                    yield return new WaitForSeconds(0.5f);
                    yield return base.LearnAbility(0.5f);
                }
            }
            yield break;
        }
    }
}
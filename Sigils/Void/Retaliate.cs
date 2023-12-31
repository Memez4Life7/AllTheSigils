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
        //Request by Blind
        private void AddRetaliate()
        {
            // setup ability
            const string rulebookName = "Retaliate";
            const string rulebookDescription = "[creature] will strike those who strike their adjacent allies.";
            const string LearnDialogue = "It will defend it's allies";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Retaliate);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Retaliate_a2);
            int powerlevel = 5;
            bool LeshyUsable = Plugin.configFamiliar.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Retaliate.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Retaliate), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Retaliate.ability] = new Tuple<string, string>("void_Retaliate", "");
            }
        }
    }

    public class void_Retaliate : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;



        public override bool RespondsToOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
        {
            CardSlot toLeft = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, true);
            CardSlot toRight = Singleton<BoardManager>.Instance.GetAdjacent(base.Card.Slot, false);
            return !attacker.Dead && target.slot == toLeft || target.slot == toRight;
        }
        public override IEnumerator OnOtherCardDealtDamage(PlayableCard attacker, int amount, PlayableCard target)
        {
            yield return new WaitForSeconds(0.25f);
            base.Card.Anim.StrongNegationEffect();
            yield return new WaitForSeconds(0.25f);
            CardModificationInfo removeFlyingMod = null;
            bool flag = base.Card.HasAbility(Ability.Flying);
            if (flag)
            {
                removeFlyingMod = new CardModificationInfo();
                removeFlyingMod.negateAbilities.Add(Ability.Flying);
                base.Card.AddTemporaryMod(removeFlyingMod);
            }
            yield return Singleton<TurnManager>.Instance.CombatPhaseManager.SlotAttackSlot(base.Card.Slot, attacker.Slot, 0f);
            bool flag2 = removeFlyingMod != null;
            if (flag2)
            {
                base.Card.RemoveTemporaryMod(removeFlyingMod, true);
            }
            yield return new WaitForSeconds(0.25f);
            yield break;
        }
    }
}
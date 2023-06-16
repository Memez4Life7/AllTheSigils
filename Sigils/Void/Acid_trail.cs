﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System.Collections;
using UnityEngine;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by Blind
        private void AddAcidTrail()
        {
            // setup ability
            const string rulebookName = "Acidic Trail";
            const string rulebookDescription = "At the end of the owner's turn, [creature] will move in the direction inscribed in the sigil, and deal 1 damage to the opposing creature if it is able to move.";
            const string LearnDialogue = "The trail they leave behind, hurts.";
            // const string TextureFile = "Artwork/void_pathetic.png";


            AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, 2, Plugin.configAcidTrail.Value);
            info.canStack = false;
            info.SetPixelAbilityIcon(SigilUtils.LoadImageAndGetTexture("void_Acid_a2"));

            Texture2D tex = SigilUtils.LoadImageAndGetTexture("void_acid");



            AbilityManager.Add(OldVoidPluginGuid, info, typeof(void_AcidTrail), tex);

            // set ability to behaviour class
            void_AcidTrail.ability = info.ability;
        }
    }

    public class void_AcidTrail : Strafe
    {
        public override Ability Ability => ability;

        public static Ability ability;

        //Copied Strafe's code and just added damage

        public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
        {
            if (oldSlot.opposingSlot.Card != null)
            {
                bool impactFrameReached = false;
                base.Card.Anim.PlayAttackAnimation(false, oldSlot.opposingSlot, delegate ()
                {
                    impactFrameReached = true;
                });
                yield return new WaitUntil(() => impactFrameReached);
                yield return oldSlot.opposingSlot.Card.TakeDamage(1, base.Card);
                yield return new WaitForSeconds(0.25f);
            }
            yield break;
        }
    }
}
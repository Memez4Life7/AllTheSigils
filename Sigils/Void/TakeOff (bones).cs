﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System.Collections;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by Sire
        private void AddTakeOffBones()
        {
            // setup ability
            const string rulebookName = "Take-Off (Bones)";
            const string rulebookDescription = "Pay 2 bones to give this card Airborne till the start of owner's next turn.";
            const string LearnDialogue = "Spread your wings and fly.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_TakeOff_Bones);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 0;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;



            var test = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_TakeOff_Bones), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack);

            test.activated = true;
            test.pixelIcon = SigilUtils.LoadSpriteFromResource(Art.void_Dive_Bones_a2);


            // set ability to behaviour class
            void_TakeOff_Bones.ability = test.ability;
        }
    }

    public class void_TakeOff_Bones : ActivatedAbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override int BonesCost
        {
            get
            {
                return 2;
            }
        }

        public override IEnumerator Activate()
        {
            yield return new WaitForSeconds(0.15f);
            yield return base.PreSuccessfulTriggerSequence();
            CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_TakeOff_Flying");
            if (cardModificationInfo == null)
            {
                //make the card mondification info
                cardModificationInfo = new CardModificationInfo(Ability.Flying);
                cardModificationInfo.singletonId = "void_TakeOff_Flying";
                //Clone the main card info so we don't touch the main card set
                CardInfo targetCardInfo = base.Card.Info.Clone() as CardInfo;
                //Add the modifincations to the cloned info
                targetCardInfo.Mods.Add(cardModificationInfo);
                //Set the target's info to the clone'd info
                base.Card.SetInfo(targetCardInfo);
                base.Card.Anim.PlayTransformAnimation();
            }
            yield return new WaitForSeconds(0.3f);
            yield return base.LearnAbility(0f);
            yield break;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            Plugin.Log.LogMessage("Takeoff Upkeep response fired 1");
            return base.Card.OpponentCard != playerUpkeep && base.Card.HasAbility(Ability.Flying);
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            Plugin.Log.LogMessage("Takeoff Upkeep response fired 2");
            yield return new WaitForSeconds(0.15f);
            yield return base.PreSuccessfulTriggerSequence();
            CardModificationInfo cardModificationInfo = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == "void_TakeOff_Flying");
            CardInfo targetCardInfo = base.Card.Info.Clone() as CardInfo;
            //Add the modifincations to the cloned info
            targetCardInfo.Mods.Remove(cardModificationInfo);
            //Set the target's info to the clone'd info
            base.Card.SetInfo(targetCardInfo);
            base.Card.Anim.PlayTransformAnimation();
            yield return new WaitForSeconds(0.3f);
            yield break;
        }
    }
}
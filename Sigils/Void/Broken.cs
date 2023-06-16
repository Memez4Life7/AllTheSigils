﻿using APIPlugin;
using DiskCardGame;
using InscryptionAPI.Card;
using System.Collections;
using UnityEngine;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Port from act 3 to match act 1 better
        private void AddBroken()
        {
            // setup ability
            const string rulebookName = "Broken";
            const string rulebookDescription = "[creature] is permanently removed from your deck if it dies.";
            const string LearnDialogue = "None of us can escape our age";
            // const string TextureFile = "Artwork/void_pathetic.png";

            AbilityInfo info = SigilUtils.CreateInfoWithDefaultSettings(rulebookName, rulebookDescription, LearnDialogue, true, -2);
            info.canStack = false;
            info.SetPixelAbilityIcon(SigilUtils.LoadImageAndGetTexture("void_broken_a2"));

            Texture2D tex = SigilUtils.LoadImageAndGetTexture("void_broken");



            AbilityManager.Add(OldVoidPluginGuid, info, typeof(void_Broken), tex);

            // set ability to behaviour class
            void_Broken.ability = info.ability;


        }
    }

    public class void_Broken : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        public override bool RespondsToDie(bool wasSacrifice, PlayableCard killer)
        {
            return true;
        }

        public override IEnumerator OnDie(bool wasSacrifice, PlayableCard killer)
        {
            DeckInfo currentDeck = SaveManager.SaveFile.CurrentDeck;
            CardInfo card = currentDeck.Cards.Find((CardInfo x) => x.HasAbility(void_Broken.ability) && x.name == base.Card.Info.name);
            currentDeck.RemoveCard(card);
            if (!base.HasLearned)
            {
                CustomCoroutine.WaitThenExecute(2f, delegate
                {
                    Singleton<VideoCameraRig>.Instance.PlayCameraAnim("refocus_medium");
                    Singleton<VideoCameraRig>.Instance.VOPlayer.PlayVoiceOver("God damn it.", "vo_goddamnit");
                }, false);
            }
            yield return base.LearnAbility(0.5f);
            yield break;
        }
        public override bool LearnAbilityConditionsMet()
        {
            return true;
        }
    }
}
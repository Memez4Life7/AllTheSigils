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
        //Request by memes4life
        private void AddThief()
        {
            // setup ability
            const string rulebookName = "Thief";
            const string rulebookDescription = "[creature] will try to steal a random default sigil from an opposing creature when played, or at the start of the owner's turn until it does.";
            const string LearnDialogue = "If only I could steal the moon...";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Thief);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Thief_a2);
            int powerlevel = 3;
            bool LeshyUsable = Plugin.configThief.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Thief.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Thief), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Thief.ability] = new Tuple<string, string>("void_thief", "");
            }
        }
    }

    public class void_Thief : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        private bool hasStolen = false;


        public override bool RespondsToResolveOnBoard()
        {
            return base.Card.OnBoard && this.hasStolen == false;
        }


        public override IEnumerator OnResolveOnBoard()
        {
            PlayableCard card = base.Card.Slot.opposingSlot.Card;
            if (card != null && card.Info.abilities.Count > 0)
            {

                if (!card.FaceDown)
                {
                    Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                    base.Card.Anim.LightNegationEffect();
                    yield return base.PreSuccessfulTriggerSequence();
                    base.Card.Status.hiddenAbilities.Add(this.Ability);
                    yield return new WaitForSeconds(0.1f);
                    CardModificationInfo cardModificationInfo = new CardModificationInfo(this.ChooseAbility());
                    yield return new WaitForSeconds(0.1f);
                    CardModificationInfo cardModificationInfo2 = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
                    yield return new WaitForSeconds(0.1f);
                    if (cardModificationInfo2 == null)
                    {
                        cardModificationInfo2 = base.Card.Info.Mods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
                    }
                    if (cardModificationInfo2 != null)
                    {
                        cardModificationInfo.fromTotem = cardModificationInfo2.fromTotem;
                        cardModificationInfo.fromCardMerge = cardModificationInfo2.fromCardMerge;
                    }
                    yield return new WaitForSeconds(0.1f);
                    base.Card.AddTemporaryMod(cardModificationInfo);
                    yield return new WaitForSeconds(0.1f);
                    yield return base.LearnAbility(0.5f);
                    Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
                    this.hasStolen = true;
                    yield break;
                }
            }
        }



        private Ability ChooseAbility()
        {

            //Get the opponant's card
            CardSlot OpponentCardSlot = base.Card.Slot.opposingSlot;

            //Get the list of abilities from the opponant and make a list to return them to the card
            List<Ability> OpponentCardAbilities = OpponentCardSlot.Card.Info.DefaultAbilities;
            Ability RandomAbilityFrom = OpponentCardAbilities[Random.Range(0, (OpponentCardAbilities.Count - 1))];

            //create new modification info
            CardModificationInfo negateMod = new CardModificationInfo();

            //go through each of the cards default abilities and add them to the modifincation info
            negateMod.negateAbilities.Add(RandomAbilityFrom);

            //Clone the main card info so we don't touch the main card set
            CardInfo OpponentCardInfo = OpponentCardSlot.Card.Info.Clone() as CardInfo;

            //Add the modifincations
            OpponentCardInfo.Mods.Add(negateMod);

            //Update the opponant card info
            OpponentCardSlot.Card.SetInfo(OpponentCardInfo);

            //Return the original abilities we saved
            return RandomAbilityFrom;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep && this.hasStolen == false;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (base.Card.slot.opposingSlot.Card != null
                && base.Card.HasAbility(void_Thief.ability))
            {
                PlayableCard card = base.Card.Slot.opposingSlot.Card;
                
                if (card.Info.HasTrait(Trait.Uncuttable))
                {
                    CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
                        "You can't steal from that Card", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
                    yield break;

                }
                else if (card.Info.HasTrait(Trait.Giant))
                {

                    CustomCoroutine.Instance.StartCoroutine(Singleton<TextDisplayer>.Instance.ShowThenClear(
                        "You can't steal the moon!", 2.5f, 0f, Emotion.Anger, TextDisplayer.LetterAnimation.Jitter, DialogueEvent.Speaker.Single, null));
                    yield break;

                }
                else
                {
                    if (card != null && card.Info.DefaultAbilities.Count > 0)
                    {

                        if (!card.FaceDown)
                        {
                            Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                            base.Card.Anim.LightNegationEffect();
                            yield return base.PreSuccessfulTriggerSequence();
                            base.Card.Status.hiddenAbilities.Add(this.Ability);
                            yield return new WaitForSeconds(0.1f);
                            CardModificationInfo cardModificationInfo = new CardModificationInfo(this.ChooseAbility());
                            yield return new WaitForSeconds(0.1f);
                            CardModificationInfo cardModificationInfo2 = base.Card.TemporaryMods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
                            yield return new WaitForSeconds(0.1f);
                            if (cardModificationInfo2 == null)
                            {
                                cardModificationInfo2 = base.Card.Info.Mods.Find((CardModificationInfo x) => x.HasAbility(this.Ability));
                            }
                            if (cardModificationInfo2 != null)
                            {
                                cardModificationInfo.fromTotem = cardModificationInfo2.fromTotem;
                                cardModificationInfo.fromCardMerge = cardModificationInfo2.fromCardMerge;
                            }
                            yield return new WaitForSeconds(0.1f);
                            base.Card.AddTemporaryMod(cardModificationInfo);
                            yield return new WaitForSeconds(0.1f);
                            yield return base.LearnAbility(0.5f);
                            Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;
                            this.hasStolen = true;
                            yield break;
                        }
                    }
                }
            }
            yield break;
        }
    }
}
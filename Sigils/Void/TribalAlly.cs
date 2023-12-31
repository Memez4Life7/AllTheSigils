﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
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
        //Original
        private void AddTribalAlly()
        {
            // setup ability
            const string rulebookName = "Tribal Ally";
            const string rulebookDescription = "When [creature] is played, A card of the same tribe is created in your hand. No tribe counts as a tribe of tribeless.";
            const string LearnDialogue = "It calls for it's kin.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.no_a2);
            int powerlevel = 2;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_TribalAlly.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_TribalAlly), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_TribalAlly.ability] = new Tuple<string, string>("void_tribeAlly", "");
            }
        }
    }

    [HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
    public class TribalAllyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
        {
            if (ability.ability == void_TribalAlly.ability)
            {
                if (info != null)
                {
                    Texture2D tex1 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_bird);

                    Texture2D tex2 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_canine);

                    Texture2D tex3 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_hooved);

                    Texture2D tex4 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_insect);

                    Texture2D tex5 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_reptile);

                    Texture2D tex6 = SigilUtils.LoadTextureFromResource(Art.void_tribeAlly_none);

                    if (info.IsOfTribe(Tribe.Bird))
                    {
                        __result = tex1;

                    }
                    else if (info.IsOfTribe(Tribe.Canine))
                    {
                        __result = tex2;
                    }
                    else if (info.IsOfTribe(Tribe.Hooved))
                    {
                        __result = tex3;
                    }
                    else if (info.IsOfTribe(Tribe.Insect))
                    {
                        __result = tex4;
                    }
                    else if (info.IsOfTribe(Tribe.Reptile))
                    {
                        __result = tex5;
                    }
                    else
                    {
                        __result = tex6;
                    }
                }
            }
        }
    }

    public class void_TribalAlly : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;



        public override bool RespondsToResolveOnBoard()
        {
            return true;
        }

        protected virtual View DrawCardView
        {
            get
            {
                return View.Default;
            }
        }

        public override IEnumerator OnResolveOnBoard()
        {

            /// Make a list of all the cards in the game
            List<CardInfo> allCards = ScriptableObjectLoader<CardInfo>.AllData;

            /// Make a blank list to add stuff too once we filter it out
            List<CardInfo> targets = new List<CardInfo>();

            /// If the tribe count is > 0, then we know it has at least one tribe. if not, it is tribeless
            if (base.Card.Info.tribes.Count > 0)
            {
                /// Get the first tribe of a card (sorry multi tribe cards)
                Tribe cardTribe = base.Card.Info.tribes[0];

                ///Run a for loop to go thru the list, filtering out all cards that is not nature temple, not in the card pool for act 1, and not of the same tribe
                for (int index = 0; index < allCards.Count; index++)
                {
                    if (allCards[index].IsOfTribe(cardTribe) && allCards[index].metaCategories.Contains(CardMetaCategory.ChoiceNode) && allCards[index].temple == CardTemple.Nature)
                    {
                        ///add those that pass to the list
                        targets.Add(allCards[index]);

                    }
                }
            }
            else
            {
                ///For tribeless, we search for all other tribeless cards. then search out which ones are in the card pool and nature temple
                for (int index = 0; index < allCards.Count; index++)
                {
                    if (allCards[index].tribes.Count == 0 && allCards[index].metaCategories.Contains(CardMetaCategory.ChoiceNode) && allCards[index].temple == CardTemple.Nature)
                    {
                        ///add those that pass to the list
                        targets.Add(allCards[index]);

                    }
                }
            }

            ///pick a random card from that list
            CardInfo target = targets[SeededRandom.Range(0, (targets.Count), base.GetRandomSeed())];

            yield return base.PreSuccessfulTriggerSequence();
            base.Card.Anim.StrongNegationEffect();
            yield return new WaitForSeconds(0.4f);
            if (Singleton<ViewManager>.Instance.CurrentView != this.DrawCardView)
            {
                yield return new WaitForSeconds(0.2f);
                Singleton<ViewManager>.Instance.SwitchToView(this.DrawCardView, false, false);
                yield return new WaitForSeconds(0.2f);
            }
            ///draw the card picked
            yield return Singleton<CardSpawner>.Instance.SpawnCardToHand(target, null, 0.25f, null);
            yield return new WaitForSeconds(0.45f);
            yield return base.LearnAbility(0.5f);
            yield break;
        }

    }
}
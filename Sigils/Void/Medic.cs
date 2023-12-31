﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;

using Random = UnityEngine.Random;

namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by blind
        private void AddMedic()
        {
            // setup ability
            const string rulebookName = "Medic";
            const string rulebookDescription = "At the start of the owner's turn, [creature] will try heal 1 damage to a friendly card for each instance of Medic.";
            const string LearnDialogue = "A good patching";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Medic);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Medic_a2);
            int powerlevel = 3;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = true;

            // set ability to behaviour class
            void_Medic.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Medic), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Medic.ability] = new Tuple<string, string>("void_Medic", "");
            }

        }
    }

    [HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
    public class MedicIcon
    {
        [HarmonyPostfix]
        public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
        {
            if (ability.ability == void_Medic.ability)
            {
                if (info != null && !SaveManager.SaveFile.IsPart2)
                {
                    //Get count of how many instances of the ability the card has
                    int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_Medic.ability).Count, 1);
                    //Switch statement to the right texture
                    switch (count)
                    {
                        case 1:
                            __result = SigilUtils.LoadTextureFromResource(Art.void_Medic_1);
                            break;
                        case 2:
                            __result = SigilUtils.LoadTextureFromResource(Art.void_Medic_2);
                            break;
                        case 3:
                            __result = SigilUtils.LoadTextureFromResource(Art.void_Medic_3);
                            break;
                        case 4:
                            __result = SigilUtils.LoadTextureFromResource(Art.void_Medic_4);
                            break;
                        case 5:
                            __result = SigilUtils.LoadTextureFromResource(Art.void_Medic_5);
                            break;
                    }
                }
            }
        }
    }



    public class void_Medic : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;


        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OnBoard && base.Card.OpponentCard != playerUpkeep;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            // Get the base card
            PlayableCard card = base.Card;
            // Get the Medic count

            int finalCount = SigilUtils.getAbilityCount(card, void_Medic.ability);



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
                    PlayableCard target = targets[Random.Range(0, (targets.Count))];
                    base.Card.Anim.LightNegationEffect();
                    yield return new WaitForSeconds(0.15f);
                    yield return base.PreSuccessfulTriggerSequence();
                    target.Anim.StrongNegationEffect();
                    if (target.Status.damageTaken > 0)
                    {
                        target.HealDamage(finalCount);
                    }
                }
                yield return new WaitForSeconds(0.15f);
                yield return base.LearnAbility(0.25f);
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
                    yield return new WaitForSeconds(0.15f);
                    yield return base.PreSuccessfulTriggerSequence();
                    target.Anim.StrongNegationEffect();
                    if (target.Status.damageTaken > 0)
                    {
                        target.HealDamage(finalCount);
                    }
                }
                yield return new WaitForSeconds(0.15f);
                yield return base.LearnAbility(0.25f);
            }
            yield break;
        }
    }
}
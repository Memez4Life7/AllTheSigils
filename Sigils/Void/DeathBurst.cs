﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;


namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by blind
        private void AddDeathburst()
        {
            // setup ability
            const string rulebookName = "Deathburst";
            const string rulebookDescription = "[creature] will deal 1 damage to each oppsing space to the left, right, and center of it.";
            const string LearnDialogue = "Boom";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Deathburst);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Deathburst_a2);
            int powerlevel = 4;
            bool LeshyUsable = Plugin.configDeathburst.Value;
            bool part1Shops = true;
            bool canStack = true;

            // set ability to behaviour class
            void_Deathburst.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Deathburst), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Deathburst.ability] = new Tuple<string, string>("void_deathburst", "");
            }

        }
    }

    [HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
    public class DearthBurstIcon
    {
        [HarmonyPostfix]
        public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
        {
            if (ability.ability == void_Deathburst.ability)
            {
                if (info != null && !SaveManager.SaveFile.IsPart2)
                {
                    Texture2D tex1 = SigilUtils.LoadTextureFromResource(Art.void_deathburst_1);

                    Texture2D tex2 = SigilUtils.LoadTextureFromResource(Art.void_deathburst_2);

                    Texture2D tex3 = SigilUtils.LoadTextureFromResource(Art.void_deathburst_3);

                    int count = Mathf.Max(info.Abilities.FindAll((Ability x) => x == void_Deathburst.ability).Count, 1);

                    switch (count)
                    {
                        case 1:
                            __result = tex1;
                            break;
                        case 2:
                            __result = tex2;
                            break;
                        case 3:
                            __result = tex3;
                            break;
                        case 4:
                            __result = tex3;
                            break;
                        case 5:
                            __result = tex3;
                            break;
                    }
                }
            }
        }
    }


    public class void_Deathburst : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        private void Awake()
        {
            this.bombPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb");
        }

        public override bool RespondsToPreDeathAnimation(bool wasSacrifice)
        {
            return base.Card.OnBoard;
        }

        public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
        {
            base.Card.Anim.LightNegationEffect();
            yield return base.PreSuccessfulTriggerSequence();
            yield return this.ExplodeFromSlot(base.Card.Slot);
            yield return base.LearnAbility(0.25f);
            yield break;
        }

        protected IEnumerator ExplodeFromSlot(CardSlot slot)
        {
            List<CardSlot> adjacentSlots = Singleton<BoardManager>.Instance.GetAdjacentSlots(slot.opposingSlot);
            if (adjacentSlots.Count > 0 && adjacentSlots[0].Index < slot.Index)
            {
                if (adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
                {
                    yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
                }
                adjacentSlots.RemoveAt(0);
            }
            if (slot.opposingSlot.Card != null && !slot.opposingSlot.Card.Dead)
            {
                yield return this.BombCard(slot.opposingSlot.Card, slot.Card);
            }
            if (adjacentSlots.Count > 0 && adjacentSlots[0].Card != null && !adjacentSlots[0].Card.Dead)
            {
                yield return this.BombCard(adjacentSlots[0].Card, slot.Card);
            }
            yield break;
        }

        private IEnumerator BombCard(PlayableCard target, PlayableCard attacker)
        {


            int count = SigilUtils.getAbilityCount(base.Card, void_Deathburst.ability);
            GameObject bomb = UnityEngine.Object.Instantiate<GameObject>(this.bombPrefab);
            bomb.transform.position = attacker.transform.position + Vector3.up * 0.1f;
            Tween.Position(bomb.transform, target.transform.position + Vector3.up * 0.1f, 0.5f, 0f, Tween.EaseLinear, Tween.LoopType.None, null, null, true);
            yield return new WaitForSeconds(0.5f);
            target.Anim.PlayHitAnimation();
            UnityEngine.Object.Destroy(bomb);
            yield return target.TakeDamage(count, attacker);
            yield break;
        }

        private const string BOMB_PREFAB_PATH = "Prefabs/Cards/SpecificCardModels/DetonatorHoloBomb";

        private GameObject bombPrefab;

    }
}
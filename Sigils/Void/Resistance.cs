﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;



namespace AllTheSigils
{
    public partial class Plugin
    {
        //Request by Blind
        private void AddResistant()
        {
            // setup ability
            const string rulebookName = "Resistant";
            const string rulebookDescription = "[creature] will only ever take 1 damage from most things. Some effects might bypass this.";
            const string LearnDialogue = "A hardy creature that one is.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Resistant);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Resistant_a2);
            int powerlevel = 4;
            bool LeshyUsable = Plugin.configResistant.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Resistant.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Resistant), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Resistant.ability] = new Tuple<string, string>("void_Resistant", "");
            }
        }
    }

    public class void_Resistant : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

    }

    [HarmonyPatch(typeof(PlayableCard), "TakeDamage")]
    public class TakeDamagePatch : PlayableCard
    {
        static void Prefix(ref PlayableCard __instance, ref int damage)
        {
            if (__instance.HasAbility(void_Resistant.ability))
            {
                damage = 1;
            }
            if (__instance.HasAbility(void_ThickShell.ability))
            {
                damage--;
            }
        }
    }
}
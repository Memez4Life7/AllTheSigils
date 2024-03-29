﻿using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using System;
using System.Collections;
using UnityEngine;
using Art = AllTheSigils.Artwork.Resources;


namespace AllTheSigils
{
    public partial class Plugin
    {
        //Original
        private void AddDying()
        {
            // setup ability
            const string rulebookName = "Dying";
            const string rulebookDescription = "[creature] will die after X number of turns.";
            const string LearnDialogue = "Tik Toc";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Dying);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Dying_a2);
            int powerlevel = -2;
            bool LeshyUsable = false;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Dying.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Dying), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Dying.ability] = new Tuple<string, string>("void_dying", "");
            }
        }
    }

    [HarmonyPatch(typeof(AbilityIconInteractable), "LoadIcon")]
    public class DyingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Texture __result, ref CardInfo info, ref AbilityInfo ability)
        {
            if (ability.ability == void_Dying.ability)
            {
                if (info != null)
                {
                    //Get count of how many instances of the ability the card has
                    int num = (info.GetExtendedPropertyAsInt("void_dying_count") != null) ? (int)info.GetExtendedPropertyAsInt("void_dying_count") : 2;

                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        //Switch statement to the right texture
                        switch (num)
                        {
                            case 1:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_1);
                                break;
                            case 2:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_2);
                                break;
                            case 3:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_3);
                                break;
                            case 4:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_4);
                                break;
                        }
                    }
                    else
                    {
                        //Switch statement to the right texture
                        switch (num)
                        {
                            case 1:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_a2_1);
                                break;
                            case 2:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_a2_2);
                                break;
                            case 3:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_a2_3);
                                break;
                            case 4:
                                __result = SigilUtils.LoadTextureFromResource(Art.void_dying_a2_4);
                                break;
                        }
                    }
                }
            }
        }
    }



    public class void_Dying : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        private int numTurnsInPlay;

        public override bool RespondsToUpkeep(bool playerUpkeep)
        {
            return base.Card.OpponentCard != playerUpkeep;
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {

            int num = (base.Card.Info.GetExtendedPropertyAsInt("void_dying_count") != null) ? (int)base.Card.Info.GetExtendedPropertyAsInt("void_dying_count") : 2;
            this.numTurnsInPlay++;
            int num2 = Mathf.Max(1, num - this.numTurnsInPlay);
            switch (num2)
            {
                case 0:
                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_Dying));
                    }
                    else
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_Dying_a2));
                    }
                    break;
                case 1:
                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_1));
                    }
                    else
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_a2_1));
                    }
                    break;
                case 2:
                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_2));
                    }
                    else
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_a2_2));
                    }
                    break;
                case 3:
                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_3));
                    }
                    else
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_a2_3));
                    }
                    break;
                case 4:
                    if (!SaveManager.SaveFile.IsPart2)
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_4));
                    }
                    else
                    {
                        base.Card.RenderInfo.OverrideAbilityIcon(void_Dying.ability, SigilUtils.LoadTextureFromResource(Art.void_dying_a2_4));
                    }
                    break;
            }
            base.Card.RenderCard();


            Plugin.Log.LogMessage(num);
            Plugin.Log.LogMessage(this.numTurnsInPlay);
            Plugin.Log.LogMessage(this.numTurnsInPlay >= num);
            if (this.numTurnsInPlay >= num)
            {
                yield return base.PreSuccessfulTriggerSequence();
                base.Card.Anim.LightNegationEffect();
                yield return base.Card.Die(false, null, true);
                yield return base.LearnAbility(0.5f);

            }
            yield break;
        }
    }
}
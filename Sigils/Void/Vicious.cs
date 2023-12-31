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
        //Original
        private void AddVicious()
        {
            // setup ability
            const string rulebookName = "Vicious";
            const string rulebookDescription = "When [creature] is attacked, it gains 1 power.";
            const string LearnDialogue = "A hit just makes it angry.";
            Texture2D tex_a1 = SigilUtils.LoadTextureFromResource(Art.void_Vicious);
            Texture2D tex_a2 = SigilUtils.LoadTextureFromResource(Art.void_Vicious_a2);
            int powerlevel = 1;
            bool LeshyUsable = Plugin.configVicious.Value;
            bool part1Shops = true;
            bool canStack = false;

            // set ability to behaviour class
            void_Vicious.ability = SigilUtils.CreateAbilityWithDefaultSettingsKCM(rulebookName, rulebookDescription, typeof(void_Vicious), tex_a1, tex_a2, LearnDialogue,
                                                                                    true, powerlevel, LeshyUsable, part1Shops, canStack).ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[void_Vicious.ability] = new Tuple<string, string>("void_vicious", "");
            }
        }
    }

    public class void_Vicious : AbilityBehaviour
    {
        public override Ability Ability => ability;

        public static Ability ability;

        private CardModificationInfo mod;

        private void Start()
        {
            this.mod = new CardModificationInfo();
            this.mod.attackAdjustment = 1;
        }



        public override bool RespondsToTakeDamage(PlayableCard source)
        {
            return base.Card.OnBoard;
        }

        public override IEnumerator OnTakeDamage(PlayableCard source)
        {
            if (source)
            {
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, true);
                yield return new WaitForSeconds(0.1f);
                base.Card.Anim.LightNegationEffect();
                yield return base.PreSuccessfulTriggerSequence();
                base.Card.AddTemporaryMod(this.mod);
                yield return new WaitForSeconds(0.1f);
                yield return base.LearnAbility(0.1f);
                Singleton<ViewManager>.Instance.Controller.LockState = ViewLockState.Unlocked;

            }
            yield break;
        }
    }
}
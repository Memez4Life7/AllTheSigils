// Using Inscryption
using DiskCardGame;
// Modding Inscryption
using InscryptionAPI.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


namespace AllTheSigils
{
    public partial class Plugin
    {
        public void AddLauncher()
        {
            AbilityInfo info = AbilityManager.New(
                               OldLilyPluginGuid,
                               "Launcher",
                               "At the end of its owner's turn, [creature] will create another creature on a random empty space on the owner's side of the table.",
                               typeof(Launcher),
                               GetTextureLily("launcher")
                           );
            info.SetPixelAbilityIcon(GetTextureLily("launcher", true));
            info.powerLevel = 5;
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            info.canStack = true;
            info.opponentUsable = false;

            Launcher.ability = info.ability;
            if (Plugin.GenerateWiki)
            {
                Plugin.SigilWikiInfos[info.ability] = new Tuple<string, string>("launcher", "Will use the ice cube parameter to define what creature it spawns.<br>Default: squirrel");
            }
        }
    }

    public class Launcher : AbilityBehaviour
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
            return base.Card != null && base.Card.OpponentCard != playerTurnEnd && base.Card.OnBoard;
        }

        // Token: 0x06000082 RID: 130 RVA: 0x000040BC File Offset: 0x000022BC
        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            List<CardSlot> slots = playerTurnEnd ? Singleton<BoardManager>.Instance.PlayerSlotsCopy : Singleton<BoardManager>.Instance.OpponentSlotsCopy;
            List<CardSlot> openspots = slots.Where(x => x.Card == null).ToList();
            if (openspots.Count > 0)
            {
                Random random = new Random();

                yield return new WaitForSeconds(0.3f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Board, false, false);
                yield return new WaitForSeconds(0.3f);

                if (base.Card.Info?.iceCubeParams?.creatureWithin != null)
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(base.Card.Info.iceCubeParams.creatureWithin, openspots[random.Next(openspots.Count)], 0.1f, true);
                }
                else
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.AllData.Find(info => info.name == "Squirrel"), openspots[random.Next(openspots.Count)], 0.1f, true);
                }
                yield return new WaitForSeconds(0.3f);
                Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
            }
            yield break;
        }
    }
}
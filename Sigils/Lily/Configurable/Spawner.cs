// Using Inscryption
// Modding Inscryption
using DiskCardGame;
using InscryptionAPI.Card;
using System.Collections;
using System.Collections.Generic;


namespace AllTheSigils
{
    public partial class Plugin
    {
        public void AddSpawner()
        {
            AbilityInfo info = AbilityManager.New(
                               OldLilyPluginGuid,
                               "Spawner",
                               "At the end of the owners turn, a card bearing this sigil will move in the direction inscribed in the sigil and create another creature in its old space.",
                               typeof(Spawner),
                               GetTexture("spawner")
                           );
            info.SetPixelAbilityIcon(GetTexture("spawner", true));
            info.powerLevel = 5;
            info.metaCategories = new List<AbilityMetaCategory> { AbilityMetaCategory.Part1Rulebook, AbilityMetaCategory.Part1Modular };
            info.canStack = true;
            info.opponentUsable = false;

            Spawner.ability = info.ability;
        }
    }

    public class Spawner : Strafe
    {
        public static Ability ability;

        public override Ability Ability
        {
            get
            {
                return ability;
            }
        }

        // Token: 0x06001419 RID: 5145 RVA: 0x000444BC File Offset: 0x000426BC
        public override IEnumerator PostSuccessfulMoveSequence(CardSlot oldSlot)
        {
            if (oldSlot.Card == null)
            {

                if (base.Card.Info.iceCubeParams.creatureWithin != null)
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(base.Card.Info.iceCubeParams.creatureWithin, oldSlot, 0.1f, true);
                }
                else
                {
                    yield return Singleton<BoardManager>.Instance.CreateCardInSlot(CardLoader.AllData.Find(info => info.name == "Squirrel"), oldSlot, 0.1f, true);
                }
            }
            yield break;
        }
    }
}
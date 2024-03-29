﻿using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Guid;
using InscryptionAPI.Saves;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AllTheSigils
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]

    // COMMENTS TO COMMUNICATE YAY :D
    //TO DO...
    //Find out what the best method is to add all future sigils (if there is time all old sigils can also be updated to use this method)


    public partial class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "ATS";

        public const string OldLilyPluginGuid = "org.memez4life.inscryption.customsigils";
        public const string OldVoidPluginGuid = "extraVoid.inscryption.voidSigils";
        public const string OldAnthonyPluginGuid = "AnthonyPython.inscryption.AnthonysSigils";

        public static List<string> OldPluginGuids = new List<string>() { OldLilyPluginGuid, OldVoidPluginGuid, OldAnthonyPluginGuid };

        private const string PluginName = "AllTheSigils";

        private const string PluginVersion = "3.0.1";

        public static string Directory;

        internal static ManualLogSource Log;

        public static Dictionary<Ability, Tuple<string, string>> SigilWikiInfos = new Dictionary<Ability, Tuple<string, string>>();

        public static Dictionary<Ability, string> NewSigilVersions = new Dictionary<Ability, string>();

        public const bool GenerateWiki = false;

        private void Awake()
        {
            base.Logger.LogInfo("Loaded AllTheSigils!");
            Plugin.Log = base.Logger;

            AddConfigs();

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            Directory = base.Info.Location;

            //Lily's sigils
            AddBond();
            AddShort();
            AddTribe_Attack();
            AddTribe_Health();
            AddBi_Blood();
            AddQuadra_Blood();
            AddImbuing();
            AddHydra();
            AddFight_Activated();
            AddPicky();
            AddFearful();
            AddTwoDeathBones();
            AddThreeDeathBones();
            AddFiveDeathBones();
            AddSixDeathBones();
            AddOneSummonBones();
            AddTwoSummonBones();
            AddThreeSummonBones();
            AddHost();
            AddSpawner();
            AddLauncher();
            AddShape_Shifter();
            AddDraw_Side_Deck_OnSummon();
            AddBait();
            AddLinguist();
            AddAll_Seeing();
            AddBlood_Shifter();
            AddAsleep();
            AddSong_Of_Sleep();
            AddWarper();
            AddRushing_March();
            AddWild_Hunger();
            AddRight_Slash();
            AddLeft_Slash();
            AddDouble_Slash();
            AddPuppets_Gift();
            AddInstakill();

            //Void's sigils
            //Add Cards
            Voids_work.Cards.Acid_Puddle.AddCard();
            Voids_work.Cards.Jackalope.AddCard();


            AddAbundance();
            AddAcidTrail();
            AddAntler();
            AddAgile();
            AddAmbush();
            AddBlight();
            AddBlindingStrike();
            AddBloodGrowth();
            AddBloodGuzzler();
            AddBodyguard();
            AddBombardier();
            AddBonePicker();
            AddBoneless();
            AddBoneShard();
            AddBox();
            AddBroken();
            AddBurning();
            AddCaustic();
            addCoinFinder();
            AddConsumer();
            AddCoward();
            AddDeadlyWaters();
            AddDeathburst();
            AddDesperation();
            AddDiseaseAbsorbtion();
            AddDiveBones();
            AddDiveEnergy();
            AddDrawBlood();
            AddDrawBone();
            AddDrawIce();
            AddDrawJack();
            AddDrawStrafe();
            AddDwarf();
            AddDying();
            AddEletric();
            AddEnforcer();
            AddEnrage();
            AddEntomophage();
            AddFamiliar();
            AddFireStarter();
            AddFishHook();
            AddFrightful();
            AddGiant();
            AddGrazing();
            AddGripper();
            AddHaste();
            AddHasteful();
            AddHerd();
            AddHighTide();
            AddInjured();
            AddHourglass();
            AddLeech();
            AddLeadBones();
            AddLeadEnergy();
            AddLifeStatsUp(); //Life Gambler
            AddLowTide();
            AddFisher(); //Lure
            AddManeuver();
            AddMedic();
            AddMidas();
            AddDoubleAttack(); //multstrike
            AddNutritious();
            AddOpportunist();
            AddParalise();
            AddPathetic();
            AddPierce();
            AddPoisonous();
            AddPossessor();
            AddPossessorPowerful(); //Powerful Possessor
            AddMovingPowerUp(); //Power from movement
            AddPredator();
            AddPrideful();
            AddProtector();
            AddRam();
            AddRandomStrafe();
            AddBlind(); //Random Strikes
            AddRecoil();
            AddRegenFull();
            AddRegen1();
            AddRegen2();
            AddRegen3();
            AddRepellant();
            AddResistant();
            AddRetaliate();
            AddSchooling();
            AddScissors();
            //			 AddShadowStep();
            AddSickness();
            AddSluggish();
            AddStampede();
            AddStrongWind();
            AddSubmergedAmbush();
            AddSwallower();
            AddTakeOffBones();
            AddTakeOffEnergy();
            AddThickShell();
            AddThief();
            AddToothBargain();
            AddToothPuller();
            AddToothShard();
            AddToxin();
            AddToxinStrength();
            AddToxinVigor();
            AddToxinDeadly();
            AddToxinSickly();
            AddTrample();
            AddTransient();
            AddTribalAlly();
            AddTribalTutor();
            addTurbulentWaters();
            AddStrafePowerUp(); // Velocity
            AddVicious();
            AddWithering();
            AddZapper();

            AddSticky();

            //Anthony's sigils
            anthonyClawPrefab = ResourceBank.Get<GameObject>("Prefabs/Cards/SpecificCardModels/LatchClaw");
            AddActivactedNanoShield();
            AddActivactedBrittle();
            AddActivactedExplodeOnDeath();
            AddActivactedReach();
            AddDecreasePowerIncreaseHealth();
            AddTransformChickenLooseCannon();
            AddTransformChickenEnemyOnly();
            AddEatChicken();
            AddChicken();
            AddChickenCard();

            //ATS sigils
            AddMount();
            AddParasite();
            AddHermit();
            AddDraw_Energy();
            AddDraw_Mox();
            AddDraw_Cost();
            AddHyped();
            AddResourceful();
            AddHoodini();
            AddInaccurate();
            AddDrunk();
            AddUnderdog();
            AddHoming();
            AddLullaby();
            AddNurse();
            AddMedical_Aid();
            AddSympathetic();
            AddArmoured();
            AddFrightened();
            AddTerrified();
            AddAlternating_Scratch();
            AddChoreography();
			AddChangePortraitWhenMerged();

            if (GenerateWiki)
            {
                WriteSigilPartOfTheWikiToFileInPluginDirectory();
            }
            //AddDevStuff();

            AddTemporarySigils();
        }
        public void Start()
        {
            ReplaceNewSigilsOnCards();
        }

        public void WriteSigilPartOfTheWikiToFileInPluginDirectory()
        {

            String dir = Path.Combine(Path.GetDirectoryName(base.Info.Location), "Wiki/");
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            String TopText = $"## Sigils\n\n<details>\n<summary>List of Sigils (Click to Expand)</summary>\n\n|Icon|Sigil Name|Power Level|Description|Notes|\n|-|:-:|:-:|:-:|:-:|";
            String BottomText = $"\n</details>";

            String SigilInfoText = "";

            string assetFolderLink = $"https://raw.githubusercontent.com/Memez4Life7/AllTheSigils/master/Artwork";
            foreach (AbilityInfo ability in AbilityManager.AllAbilityInfos)
            {
                string guid = GetSigilGuid(ability.ability);

                if (OldPluginGuids.Contains(guid) || guid == "ATS")
                {
                    string folderName = "";
                    switch (guid)
                    {
                        case OldLilyPluginGuid:
                            folderName = "Lily/Act1";
                            break;
                        case OldVoidPluginGuid:
                            folderName = "Void_Merged";
                            break;
                        case OldAnthonyPluginGuid:
                            folderName = "Anthony";
                            break;
                        case PluginGuid:
                            folderName = "ATS/Act1";
                            break;
                    }

                    Tuple<string, string> sigilWikiInfo;
                    string link = "";
                    if (SigilWikiInfos.TryGetValue(ability.ability, out sigilWikiInfo))
                    {
                        link = $"{assetFolderLink}/{folderName}/{sigilWikiInfo.Item1}.png";
                    }
                    else
                    {
                        link = $"{assetFolderLink}/Lily/Act1/placeholder.png";
                    }
                    string name = ability.rulebookName;
                    string powerLevel = ability.powerLevel.ToString();
                    string description = ability.rulebookDescription;
                    SigilInfoText += $"\n|<img align=\"center\" src=\"{link}\">|**{name}**|{powerLevel}|{description}|{sigilWikiInfo.Item2}|";
                }
            }

            String WikiText = $"{TopText}{SigilInfoText}\n{BottomText}";
            File.WriteAllText(Path.Combine(dir, "wiki.txt"), WikiText);

            Plugin.Log.LogWarning($"FINISHED WRITING WIKI TEXT TO:\n{dir}wiki.txt");
        }

        public void AddTemporarySigils()
        {
            foreach (AbilityInfo ability in AbilityManager.AllAbilityInfos)
            {
                string guid = GetSigilGuid(ability.ability);

                if (OldPluginGuids.Contains(guid))
                {
                    AbilityInfo info = AbilityManager.New(
                        PluginGuid,
                        ability.rulebookName,
                        ability.rulebookDescription,
                        typeof(EmptySigil),
                        GetTextureLily("placeholder")
                    );
                    NewSigilVersions.Add(info.ability, guid);
                }
            }
        }

        public void ReplaceNewSigilsOnCards()
        {
            List<CardInfo> AllCards = ScriptableObjectLoader<CardInfo>.AllData;
            for (int i = 0; i < AllCards.Count; i++)
            {
                CardInfo card = AllCards[i];
                for (int j = 0; j < card.Abilities.Count; j++)
                {
                    Ability ability = card.Abilities[j];

                    if (NewSigilVersions.ContainsKey(ability))
                    {
                        AllCards[i].abilities[j] = GetCustomAbility(NewSigilVersions[ability], AbilitiesUtil.GetInfo(ability).rulebookName);
                    }
                }
            }

            NewSigilVersions.Keys.ToList().ForEach(x => AbilityManager.Remove(x));
        }

        public string GetSigilGuid(Ability ability)
        {
            Dictionary<string, Dictionary<string, object>> SaveData = (Dictionary<string, Dictionary<string, object>>)AccessTools.Field(typeof(ModdedSaveData), "SaveData").GetValue(ModdedSaveManager.SaveData);

            foreach (KeyValuePair<string, object> keyValuePair in SaveData["cyantist.inscryption.api"])
            {
                List<string> SubstringList = keyValuePair.Key.Split('_').ToList();
                if (SubstringList[0] == "Ability")
                {
                    int AbilityID;
                    if (!int.TryParse(ability.ToString(), out AbilityID))
                    {
                        continue;
                    }
                    int ValueID = int.Parse((string)keyValuePair.Value);

                    if (ValueID == AbilityID)
                    {
                        return SubstringList[1];
                    }
                }
            }
            return null;
        }

        private void AddDevStuff()
        {
            AddDev_Activated();
            CardInfo Squirrel = CardManager.BaseGameCards.CardByName("Squirrel");
            Squirrel.abilities = new List<Ability> { Draw_Cost.ability };
            //Squirrel.abilities = new List<Ability> { Armoured.ability, Draw_Cost.ability, Draw_Energy.ability, Draw_Mox.ability, Drunk.ability, Frightened.ability, Hermit.ability, Homing.ability };
            Squirrel.baseAttack = 1;

            CardInfo wolf = CardManager.BaseGameCards.CardByName("Wolf");
            wolf.abilities = new List<Ability> { Draw_Mox.ability };
            //wolf.abilities = new List<Ability> { Hoodini.ability, Hyped.ability, Inaccurate.ability, Lullaby.ability, Medical_Aid.ability, Mount.ability, Nurse.ability, Parasite.ability };

            CardInfo mole = CardManager.BaseGameCards.CardByName("Mole");
            mole.abilities = new List<Ability> { Draw_Energy.ability };
            //mole.abilities = new List<Ability> { Resourceful.ability, Sympathetic.ability, Terrified.ability, Underdog.ability };
            mole.baseAttack = 1;

            CardInfo wolf_talking = CardManager.BaseGameCards.CardByName("Wolf_Talking");
            wolf_talking.abilities = new List<Ability> { };

            CardInfo grizzly = CardManager.BaseGameCards.CardByName("Grizzly");
            grizzly.abilities = new List<Ability> { };
            grizzly.baseAttack = 1;

            CardInfo stoat = CardManager.BaseGameCards.CardByName("Stoat");
            stoat.abilities = new List<Ability> { };

            //CardInfo Geck = CardManager.BaseGameCards.CardByName("Geck");
            //Geck.abilities = new List<Ability> { Ability.IceCube };
            //Geck.SetIceCube(CardLoader.GetCardByName("Amalgam"));
        }

        public static Texture2D GetTexture(string path, bool Act2 = false)
        {
            string folder = Act2 ? "Act2" : "Act1";
            byte[] imgBytes = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Directory), "Artwork/", "ATS", $"{folder}/", $"{path}.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        public static Texture2D GetTextureLily(string path, bool Act2 = false)
        {
            string folder = Act2 ? "Act2" : "Act1";
            byte[] imgBytes = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Directory), "Artwork/", "Lily", $"{folder}/", $"{path}.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        public static Texture2D GetTextureAnthony(string path)
        {
            byte[] imgBytes = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(Directory), "Artwork/", "Anthony", $"{path}.png"));
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imgBytes);
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        public static Ability GetCustomAbility(string GUID, string rulebookname)
        {
            return GuidManager.GetEnumValue<Ability>(GUID, rulebookname);
        }
    }
}

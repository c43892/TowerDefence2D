using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using Swift;
using Codice.CM.WorkspaceServer.Tree.GameUI.HeadTree;

namespace TowerDefance.Editor.Res
{
    public class ResManager : MonoBehaviour
    {
        [MenuItem("TowerDefenceTools/Update Resource Groups")]
        public static void CreateAddressableResourceGroups()
        {
            // Load the AddressableAssetSettings
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            if (addressableSettings == null)
            {
                addressableSettings = AddressableAssetSettings.Create(Path.Combine("Assets", "AddressableAssetsData"), "AddressableAssetSettings", true, true);
                AddressableAssetSettingsDefaultObject.Settings = addressableSettings;
            }

            
            AddAnimations();
            AddBattleUnits();
            AddBattles();

            // Save the AddressableAssetSettings
            EditorUtility.SetDirty(addressableSettings);
            AssetDatabase.SaveAssets();
        }

        public static void AddBattleUnits()
        {
            const string RootPath = "Assets/AddressableResources/BattleUnits";
            AddFile(RootPath, "Towers.txt");
            AddFile(RootPath, "Enemies.txt");
        }

        public static void AddBattles()
        {
            const string RootPath = "Assets/AddressableResources/Battles";
            AddFile(RootPath, "Battles.txt");
        }

        public static void AddAnimations()
        {
            const string RootPath = "Assets/AddressableResources/Animations";
            const string ResRootPath = RootPath + "/Res";

            AddSubfoldersAsGroups(ResRootPath, "Avatars");
            AddSubfoldersAsGroups2(ResRootPath, "Enemies");
            AddSubfoldersAsGroups2(ResRootPath, "Towers");
            AddSubfoldersAsGroups2(ResRootPath, "Effects");

            AddFile(RootPath, "UnitAnimations.txt");
            AddFile(RootPath, "AvatarAnimations.txt");
            AddFile(RootPath, "EffectAnimations.txt");
        }

        public static void AddFile(string root, string file)
        {
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            AddressableAssetGroup defaultGroup = addressableSettings.DefaultGroup;
            var guid = AssetDatabase.AssetPathToGUID(root + "/" + file);
            var entry = addressableSettings.CreateOrMoveEntry(guid, defaultGroup);
            var label = Path.GetFileNameWithoutExtension(file);
            entry.address = label;

            addressableSettings.AddLabel(label);
            entry.SetLabel(label, true);
        }

        public static void AddSubfoldersAsGroups2(string root, string targetFolder)
        {
            foreach (var f in AssetDatabase.GetSubFolders(root + "/" + targetFolder))
                AddSubfoldersAsGroups(root, targetFolder + "/" + Path.GetFileName(f));
        }

        public static void AddSubfoldersAsGroups(string root, string targetFolder)
        {
            string folderPath = root + "/" + targetFolder;

            // Load the AddressableAssetSettings
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;

            // Iterate through all subfolders in the folderPath
            string[] subfolders = AssetDatabase.GetSubFolders(folderPath);
            foreach (string subfolder in subfolders)
            {
                AddressableAssetGroup defaultGroup = addressableSettings.DefaultGroup;
                var guid = AssetDatabase.AssetPathToGUID(subfolder);
                var entry = addressableSettings.CreateOrMoveEntry(guid, defaultGroup);
                var label = targetFolder + "/" + Path.GetFileNameWithoutExtension(subfolder);
                entry.address = label;

                addressableSettings.AddLabel(label);
                entry.SetLabel(label, true);
            }
        }
    }
}

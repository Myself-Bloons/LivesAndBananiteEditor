using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.Legends;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppNinjaKiwi.Common;
using Il2CppTMPro;
using LivesAndBananiteEditor;
using MelonLoader;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(LivesAndBananiteEditor.LivesAndBananiteEditor), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace LivesAndBananiteEditor;

public class LivesAndBananiteEditor : BloonsTD6Mod { }

[HarmonyPatch(typeof(FrontierMapScreen), nameof(FrontierMapScreen.Open))]
internal static class FrontierMapScreen_Open
{
    [HarmonyPostfix]
    private static void Postfix(FrontierMapScreen __instance) => FrontierEditorSetup.Setup(__instance);
}

[HarmonyPatch(typeof(FrontierMapScreen), nameof(FrontierMapScreen.ReOpen))]
internal static class FrontierMapScreen_ReOpen
{
    [HarmonyPostfix]
    private static void Postfix(FrontierMapScreen __instance) => FrontierEditorSetup.Setup(__instance);
}

internal static class FrontierEditorSetup
{
    internal static void Setup(FrontierMapScreen screen)
    {
        SetupLives(screen);
        SetupBananite(screen);
    }

    private static void SetupLives(FrontierMapScreen screen)
    {
        var text = screen.livesAmountTxt;
        if (text == null) return;

        var button = text.gameObject.GetComponentOrAdd<Button>();
        button.SetOnClick(() =>
        {
            var saveData = FrontierLegendsManager.instance?.FrontierSaveData;
            if (saveData == null) return;

            PopupScreen.instance.SafelyQueue(popup =>
            {
                popup.ShowSetNamePopup(
                    "Edit Lives",
                    "Enter your desired lives amount:",
                    new System.Action<string>(input =>
                    {
                        if (!int.TryParse(input, out var amount)) return;
                        saveData.lives = amount;
                        screen.RefreshHud();
                    }),
                    saveData.lives.ToString()
                );
                popup.ModifyField(field =>
                {
                    field.characterValidation = TMP_InputField.CharacterValidation.Integer;
                    field.characterLimit = 5;
                });
            });
        });
        text.raycastTarget = true;
    }

    private static void SetupBananite(FrontierMapScreen screen)
    {
        var text = screen.currencyAmountTxt;
        if (text == null) return;

        var button = text.gameObject.GetComponentOrAdd<Button>();
        button.SetOnClick(() =>
        {
            var saveData = FrontierLegendsManager.instance?.FrontierSaveData;
            if (saveData == null) return;

            PopupScreen.instance.SafelyQueue(popup =>
            {
                popup.ShowSetNamePopup(
                    "Edit Bananite",
                    "Enter your desired bananite amount:",
                    new System.Action<string>(input =>
                    {
                        if (!float.TryParse(input, out var amount)) return;
                        saveData.SetCurrency(amount);
                        screen.RefreshHud();
                    }),
                    saveData.Currency.ToString()
                );
                popup.ModifyField(field =>
                {
                    field.characterValidation = TMP_InputField.CharacterValidation.Integer;
                    field.characterLimit = 10;
                });
            });
        });
        text.raycastTarget = true;
    }
}

global using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper;
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
    private static void Postfix(FrontierMapScreen __instance) => Setup(__instance);

    internal static void Setup(FrontierMapScreen screen)
    {
        SetupEditor(screen.livesAmountTxt, "Edit Lives", "Enter your desired lives amount:", 5,
            () => FrontierLegendsManager.FrontierSaveData.lives.ToString(),
            input =>
            {
                if (!int.TryParse(input, out var amount)) return;
                FrontierLegendsManager.FrontierSaveData.lives = amount;
                screen.RefreshHud();
            });

        SetupEditor(screen.currencyAmountTxt, "Edit Bananite", "Enter your desired bananite amount:", 10,
            () => FrontierLegendsManager.FrontierSaveData.Currency.ToString(),
            input =>
            {
                if (!float.TryParse(input, out var amount)) return;
                FrontierLegendsManager.FrontierSaveData.SetCurrency(amount);
                screen.RefreshHud();
            });
    }

    private static void SetupEditor(Il2Cpp.NK_TextMeshProUGUI text, string title, string body, int charLimit,
        System.Func<string> getValue, System.Action<string> setValue)
    {
        if (text == null) return;
        text.raycastTarget = true;
        text.gameObject.GetComponentOrAdd<Button>().SetOnClick(() =>
        {
            PopupScreen.instance.SafelyQueue(popup =>
            {
                popup.ShowSetNamePopup(title, body, new System.Action<string>(setValue), getValue());
                popup.ModifyField(field =>
                {
                    field.characterValidation = TMP_InputField.CharacterValidation.Integer;
                    field.characterLimit = charLimit;
                });
            });
        });
    }
}

[HarmonyPatch(typeof(FrontierMapScreen), nameof(FrontierMapScreen.ReOpen))]
internal static class FrontierMapScreen_ReOpen
{
    [HarmonyPostfix]
    private static void Postfix(FrontierMapScreen __instance) => FrontierMapScreen_Open.Setup(__instance);
}

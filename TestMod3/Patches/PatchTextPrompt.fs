module Patches
    open HarmonyLib
    open OpenNefia.Core.UI.Layer
    open OpenNefia.Core.UI
    open MyTextPrompt
    
    [<HarmonyPatch(typedefof<TextPrompt>, nameof Unchecked.defaultof<TextPrompt>.Query)>]
    type PatchTextPrompt() =
        static member Prefix (__instance : NumberPrompt) (__result : UiResult<string> ref) =
            printfn "Running custom text prompt!"
            let myTextPrompt = MyTextPrompt()
            __result.Value <- myTextPrompt.Query()
            false
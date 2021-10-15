module MyTextPrompt

    open OpenNefia.Core.UI
    
    type MyTextPrompt() =
        inherit BaseUiLayer<string>()

        let _Value = ""

        member this.Value = _Value
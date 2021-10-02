using Love;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;

namespace OpenNefia.Game
{
    public class GameScene : Scene
    {
        private GameWrapper Parent;

        public GameScene(GameWrapper parent)
        {
            this.Parent = parent;
        }

        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            var layer = GameWrapper.Instance.CurrentLayer;
            layer?.OnKeyPressed(key, isRepeat);
        }

        public override void KeyReleased(KeyConstant key, Scancode scancode)
        {
            var layer = GameWrapper.Instance.CurrentLayer;
            layer?.OnKeyReleased(key);
        }

        public override void TextEditing(string text, int start, int end)
        {
        }

        public override void TextInput(string text)
        {
            var layer = GameWrapper.Instance.CurrentLayer;
            layer?.OnTextInput(text);
        }
    }
}
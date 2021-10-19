using Love;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;

namespace OpenNefia.Game
{
    public class GameScene : Scene
    {
        private Engine Parent;

        public GameScene(Engine parent)
        {
            this.Parent = parent;
        }

        public override void WindowResize(int w, int h)
        {
            this.Parent.OnWindowResize(w, h);
        }

        public override void KeyPressed(KeyConstant key, Scancode scancode, bool isRepeat)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveKeyPressed(key, isRepeat);
        }

        public override void KeyReleased(KeyConstant key, Scancode scancode)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveKeyReleased(key);
        }

        public override void TextEditing(string text, int start, int end)
        {
        }

        public override void TextInput(string text)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveTextInput(text);
        }

        public override void MouseMoved(float x, float y, float dx, float dy, bool isTouch)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveMouseMoved(x, y, dx, dy, isTouch);
        }

        public override void MousePressed(float x, float y, int button, bool isTouch)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveMousePressed(x, y, button, isTouch);
        }

        public override void MouseReleased(float x, float y, int button, bool isTouch)
        {
            var layer = Engine.Instance.CurrentLayer;
            layer?.OnLoveMouseReleased(x, y, button, isTouch);
        }
    }
}
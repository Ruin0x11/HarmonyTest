﻿using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class TestLayer : BaseUiLayer<int>
    {
        private bool Finished = false;
        private UiWindowBacking WindowBacking;

        public TestLayer()
        {
            this.WindowBacking = new UiWindowBacking(false);

            this.Keys.BindKey(Keybind.Entries.Escape, (_) =>
            {
                this.Finished = true;
                return null;
            });
        }

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            this.WindowBacking.Relayout(x, y, width, height);
        }

        public override void Update(float dt)
        {
            X = X + 1;
            if (X > 200)
            {
                X = 100;
            }

            this.WindowBacking.Update(dt);
        }

        public override UiResult<int>? GetResult()
        {
            if (this.Finished)
            {
                this.Finished = false;
                return UiResult<int>.Finished(42);
            }

            return null;
        }

        public override void Draw()
        {
            Graphics.SetColor(1f, 1f, 1f);
            Graphics.Rectangle(DrawMode.Fill, 100, 100, 100, 100);
            Graphics.SetColor(1f, 0, 1f);
            Graphics.Rectangle(DrawMode.Fill, 50 + X, 50 + Y, 100, 100);

            Graphics.SetColor(1f, 1f, 1f);
            this.WindowBacking.Draw();
        }
    }
}

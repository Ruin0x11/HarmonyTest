﻿using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System.Collections.Generic;
using static OpenNefia.Core.Rendering.AssetDrawable;

namespace OpenNefia.Core.UI.Element
{
    public class UiTopicWindow : BaseUiElement
    {
        public enum FrameStyle : int
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5
        }

        public enum WindowStyle : int
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6
        }

        protected FrameStyle FrameStyle_;
        protected WindowStyle WindowStyle_;

        protected AssetDrawable AssetTopicWindow;
        protected AssetDrawable AssetWindow;
        protected ColorAsset ColorTopicWindowStyle0;
        protected ColorAsset ColorTopicWindowStyle1;
        protected ColorAsset ColorTopicWindowStyle2;
        protected ColorAsset ColorTopicWindowStyle3;
        protected ColorAsset ColorTopicWindowStyle4;
        protected ColorAsset ColorTopicWindowStyle5;
        protected ColorAsset ColorTopicWindowStyle6;
        protected Love.SpriteBatch TopicWindowBatch;

        private AssetDrawable GetTopicWindowAsset(FrameStyle frameStyle)
        {
            switch (frameStyle)
            {
                case FrameStyle.Zero:
                default:
                    return new AssetDrawable(Asset.Entries.TopicWindow0, this.Width, this.Height);
                case FrameStyle.One:
                    return new AssetDrawable(Asset.Entries.TopicWindow1, this.Width, this.Height);
                case FrameStyle.Two:
                    return new AssetDrawable(Asset.Entries.TopicWindow2, this.Width, this.Height);
                case FrameStyle.Three:
                    return new AssetDrawable(Asset.Entries.TopicWindow3, this.Width, this.Height);
                case FrameStyle.Four:
                    return new AssetDrawable(Asset.Entries.TopicWindow4, this.Width, this.Height);
                case FrameStyle.Five:
                    return new AssetDrawable(Asset.Entries.TopicWindow5, this.Width, this.Height);
            }
        }

        public UiTopicWindow(FrameStyle frameStyle = FrameStyle.One, WindowStyle windowStyle = WindowStyle.One)
        {
            this.FrameStyle_ = frameStyle;
            this.WindowStyle_ = windowStyle;

            this.AssetTopicWindow = this.GetTopicWindowAsset(this.FrameStyle_);
            this.AssetWindow = new AssetDrawable(Asset.Entries.Window);
            this.ColorTopicWindowStyle0 = ColorAsset.Entries.TopicWindowStyle0;
            this.ColorTopicWindowStyle1 = ColorAsset.Entries.TopicWindowStyle1;
            this.ColorTopicWindowStyle2 = ColorAsset.Entries.TopicWindowStyle2;
            this.ColorTopicWindowStyle3 = ColorAsset.Entries.TopicWindowStyle3;
            this.ColorTopicWindowStyle4 = ColorAsset.Entries.TopicWindowStyle4;
            this.ColorTopicWindowStyle5 = ColorAsset.Entries.TopicWindowStyle5;
            this.ColorTopicWindowStyle6 = ColorAsset.Entries.TopicWindowStyle6;

            this.TopicWindowBatch = this.MakeBatch();
        }

        private Love.SpriteBatch MakeBatch()
        {
            var parts = new List<AssetBatchPart>();

            for (int i = 0; i < this.Width / 16 - 1; i++)
            {
                parts.Add(new AssetBatchPart("top_mid", i * 16 + 16, 0));
                parts.Add(new AssetBatchPart("bottom_mid", i * 16 + 16, this.Height - 16));
            }

            var innerX = this.Width / 16 * 16 - 16;
            var innerY = this.Height / 16 * 16 - 16;

            parts.Add(new AssetBatchPart("top_mid2", innerX, 0));
            parts.Add(new AssetBatchPart("bottom_mid2", innerX, this.Height - 16));

            for (int i = 0; i < this.Height / 16 - 1; i++)
            {
                parts.Add(new AssetBatchPart("left_mid", 0, i * 16 + 16));
                parts.Add(new AssetBatchPart("right_mid", this.Width - 16, i * 16 + 16));
            }

            parts.Add(new AssetBatchPart("left_mid2", 0, innerY));
            parts.Add(new AssetBatchPart("right_mid2", this.Width - 16, innerY));

            parts.Add(new AssetBatchPart("top_left", 0, 0));
            parts.Add(new AssetBatchPart("bottom_left", 0, this.Height - 16));
            parts.Add(new AssetBatchPart("top_right", this.Width - 16, 0));
            parts.Add(new AssetBatchPart("bottom_right", this.Width - 16, this.Height - 16));

            this.AssetTopicWindow = this.GetTopicWindowAsset(this.FrameStyle_);
            return this.AssetTopicWindow.MakeBatch(parts);
        }

        public override void Relayout(int x = -1, int y = -1, int width = -1, int height = -1)
        {
            base.Relayout(x, y, width, height);
            this.TopicWindowBatch = this.MakeBatch();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            if (this.WindowStyle_ == WindowStyle.Six)
            {
                GraphicsEx.SetColor(this.ColorTopicWindowStyle6);
                GraphicsEx.DrawSpriteBatch(this.TopicWindowBatch, this.X, this.Y, this.Width - 4, this.Height - 4);
            }
            else
            {
                var rect = true;

                switch (this.WindowStyle_)
                {
                    case WindowStyle.Zero:
                        rect = false;
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle0);
                        break;
                    case WindowStyle.One:
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle1);
                        break;
                    case WindowStyle.Two:
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle2);
                        break;
                    case WindowStyle.Three:
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle3);
                        break;
                    case WindowStyle.Four:
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle4);
                        break;
                    case WindowStyle.Five:
                    default:
                        GraphicsEx.SetColor(this.ColorTopicWindowStyle5);
                        break;
                }

                if (rect)
                {
                    Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
                    GraphicsEx.DrawFilledRect(this.X + 4, this.Y + 4, this.Width - 4, this.Height - 4);
                    Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
                }
            }

            this.AssetWindow.DrawRegion("fill", this.X + 4, this.Y + 4, this.Width - 6, this.Height - 8);

            GraphicsEx.SetColor(Love.Color.White);
            GraphicsEx.DrawSpriteBatch(this.TopicWindowBatch, this.X, this.Y);

            if (this.WindowStyle_ == WindowStyle.Five)
            {
                GraphicsEx.SetColor(this.ColorTopicWindowStyle5);
                GraphicsEx.DrawSpriteBatch(this.TopicWindowBatch, this.X + 2, this.Y + 2, this.Width - 4, this.Height - 5);

                Love.Graphics.SetBlendMode(Love.BlendMode.Subtract);
                GraphicsEx.DrawFilledRect(this.X + 4, this.Y + 4, this.Width - 4, this.Height - 4);
                Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            }
        }
    }
}

using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Map;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class PositionPrompt : BaseUiLayer<PositionPrompt.Result>
    {
        public new class Result
        {
            TilePos Pos;
            bool CanSee;

            public Result(TilePos pos, bool canSee)
            {
                Pos = pos;
                CanSee = canSee;
            }
        }

        TilePos Origin;
        TilePos Target;
        bool CanSee = false;

        ColorDef ColorTargetedTile;
        FontDef FontTargetText;
        IUiText TextTarget;

        public PositionPrompt(TilePos origin, TilePos? target = null)
        {
            this.Origin = origin;
            this.Target = target ?? origin;

            this.ColorTargetedTile = ColorDefOf.PromptTargetedTile;
            this.FontTargetText = FontDefOf.TargetText;
            this.TextTarget = new UiText(this.FontTargetText);

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.Enter] += (_) => this.Finish(new Result(this.Target, this.CanSee));
            this.Keybinds[Keybind.Entries.North] += (_) => this.MoveTargetPos(0, -1);
            this.Keybinds[Keybind.Entries.South] += (_) => this.MoveTargetPos(0, 1);
            this.Keybinds[Keybind.Entries.West] += (_) => this.MoveTargetPos(-1, 0);
            this.Keybinds[Keybind.Entries.East] += (_) => this.MoveTargetPos(1, 0);
            this.Keybinds[Keybind.Entries.Northwest] += (_) => this.MoveTargetPos(-1, -1);
            this.Keybinds[Keybind.Entries.Southwest] += (_) => this.MoveTargetPos(-1, 1);
            this.Keybinds[Keybind.Entries.Northeast] += (_) => this.MoveTargetPos(1, -1);
            this.Keybinds[Keybind.Entries.Southeast] += (_) => this.MoveTargetPos(1, 1);
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.NextPage] += (_) => this.NextTarget();
            this.Keybinds[Keybind.Entries.PreviousPage] += (_) => this.PreviousTarget();
        }

        private void MoveTargetPos(int dx, int dy)
        {
            this.Target.X = Math.Clamp(this.Target.X + dx, 0, this.Target.Map.Width - 1);
            this.Target.Y = Math.Clamp(this.Target.Y + dy, 0, this.Target.Map.Height - 1);
            this.UpdateCamera();
            this.UpdateTargetText();
        }

        private void UpdateCamera()
        {
            Current.Field!.Camera.CenterOn(this.Target);
        }

        private void UpdateTargetText()
        {
            var chara = this.Target.GetMapObjects<Chara>().FirstOrDefault();
            if (chara != null)
            {
                this.TextTarget.Text = $"Chara: {chara}";
            }
            else
            {
                this.TextTarget.Text = "";
            }
        }

        private void NextTarget()
        {

        }

        private void PreviousTarget()
        {

        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            this.TextTarget.SetPosition(100, this.Height - Constants.INF_VERH - 45);
        }

        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = Love.Graphics.GetWidth();
            height = Love.Graphics.GetHeight();
        }

        public override void OnQuery()
        {
            this.UpdateCamera();
            this.UpdateTargetText();
        }

        public override void OnQueryFinish()
        {
            Current.Field!.RefreshScreen();
        }

        public override void Update(float dt)
        {
            this.TextTarget.Update(dt);
        }

        private bool ShouldDrawLine()
        {
            var chara = this.Target.GetMapObjects<Chara>().FirstOrDefault();

            if (chara == null || !chara.CanBeSeenBy(Current.Player!) || !chara.HasLos(Origin))
            {
                return false;
            }

            return true;
        }

        public override void Draw()
        {
            Current.Field!.Camera.TileToVisibleScreen(this.Target, out var sx, out var sy);
            var coords = GraphicsEx.Coords;
            Love.Graphics.SetBlendMode(BlendMode.Add);
            GraphicsEx.SetColor(this.ColorTargetedTile);
            Love.Graphics.Rectangle(DrawMode.Fill, sx, sy, coords.TileWidth, coords.TileHeight);

            if (this.ShouldDrawLine())
            {
                foreach (var (tx, ty) in PosUtils.EnumerateLine(Origin.X, Origin.Y, Target.X, Target.Y))
                {
                    Current.Field!.Camera.TileToVisibleScreen(this.Target, out sx, out sy);
                    Love.Graphics.Rectangle(DrawMode.Fill, sx, sy, coords.TileWidth, coords.TileHeight);
                }
            }

            Love.Graphics.SetBlendMode(BlendMode.Alpha);

            this.TextTarget.Draw();
        }
    }
}

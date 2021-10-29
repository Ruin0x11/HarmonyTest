using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Util
{
    public static class TaskRunner
    {
        public static void Run(Task task)
        {
            new TaskRunnerLayer(task).Query();
        }

        public static T Run<T>(Task<T> task)
        {
            new TaskRunnerLayer(task).Query();
            return task.Result;
        }
    }

    internal class TaskRunnerLayer : BaseUiLayer<UiNoResult>
    {
        private Task ActiveTask;
        private float Dt;

        public TaskRunnerLayer(Task task)
        {
            this.ActiveTask = task;
        }

        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = Love.Graphics.GetWidth();
            height = Love.Graphics.GetHeight();
        }

        public override void Update(float dt)
        {
            Dt += dt;

            if (this.ActiveTask.IsCompleted)
            {
                this.Finish(new UiNoResult());
            }
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(0, 0, 0, 128);
            GraphicsEx.FilledRect(this.X, this.Y, this.Width, this.Height);

            {
                Love.Graphics.Push();
                Love.Graphics.Translate(this.X + this.Width / 2, this.Y + this.Height / 2);
                Love.Graphics.Rotate(Dt);
                Love.Graphics.SetColor(Love.Color.White);
                GraphicsEx.FilledRect(-32, -32, 64, 64);
                Love.Graphics.Pop();
            }
        }
    }
}

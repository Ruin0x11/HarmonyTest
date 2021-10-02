using System.Collections;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public class Coroutine
    {
        internal class CoroutineInstance
        {
            private readonly IEnumerator routine;

            public bool IsFinished = false;
            public bool WasCanceled = false;

            public CoroutineInstance(IEnumerator routine) 
            { 
                this.routine = routine; 
            }

            public bool MoveNext()
            {
                var result = this.routine.MoveNext();

                if (!result)
                {
                    this.IsFinished = true;
                    return false;
                }

                return true;
            }

            public bool Cancel()
            {
                if (this.IsFinished)
                {
                    return false;
                }

                this.IsFinished = true;
                this.WasCanceled = true;
                return true;
            }

            public bool Resume()
            {
                if (!this.IsFinished)
                {
                    this.MoveNext();
                }
                return this.IsFinished;
            }
        }

        internal class Scheduler
        {
            private readonly List<CoroutineInstance> Coroutines;

            public CoroutineInstance Start(IEnumerator routine)
            {
                var inst = new CoroutineInstance(routine);
                if (inst.MoveNext())
                {
                    this.Coroutines.Add(inst);
                }
                return inst;
            }

            public void Step()
            {
                this.Coroutines.RemoveAll(c => c.Resume());
            }
        }

        private static Scheduler Instance = new Scheduler();

        public static void Start(IEnumerator routine) => Instance.Start(routine);
    }
}

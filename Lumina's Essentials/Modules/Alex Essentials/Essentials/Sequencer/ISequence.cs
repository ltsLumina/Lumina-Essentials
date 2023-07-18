using System;
using System.Collections;

namespace Lumina.Essentials.Sequencer
{
    public interface ISequence
    {
        ISequence Execute(Action action);

        ISequence ExecuteCoroutine(IEnumerator routine);

        ISequence WaitThenExecute(float duration, Action action);

        ISequence WaitForSeconds(float duration);

        ISequence ContinueWith(Action action);

        ISequence ContinueWithIf(Action action, bool condition);

        ISequence ContinueWithIf(Action action, Func<bool> condition);

        ISequence OnComplete(string message);

        ISequence OnFail(Action<Exception> action);

        ISequence RepeatExecute(int times, Action action);
    }
}

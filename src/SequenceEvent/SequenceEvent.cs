using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SequenceEvent
{
    public class SequenceEvent
    {
        private readonly List<Func<ValueTask>> tasks = new List<Func<ValueTask>>();

        public void Subscribe(Func<ValueTask> valueTask)
        {
            if (valueTask == null) throw new ArgumentNullException(nameof(valueTask));
            tasks.Add(valueTask);
        }

        public void CancelSubscribe(Func<ValueTask> valueTask)
        {
            tasks.Remove(valueTask);
        }

        public async ValueTask OnNextAsync()
        {
            if (tasks.Count == 1)
            {
                var task = tasks[0];
                await task();
                return;
            }
            
            var count = tasks.Count;
            var buff = ArrayPool<Func<ValueTask>>.Shared.Rent(count);
            tasks.CopyTo(buff);
            for (int i = 0, times = count - 1; i < times; i++)
            {
                var cur = buff[i];
                await cur();
            }

            Array.Clear(buff, 0, count);
            ArrayPool<Func<ValueTask>>.Shared.Return(buff);
        }
    }
}
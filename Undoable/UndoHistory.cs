using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Undoable
{
    public class UndoHistory
    {
        private readonly Stack<IUndoable> _undoStack = new Stack<IUndoable>();
        private readonly Stack<IUndoable> _redoStack = new Stack<IUndoable>();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async Task DoAsync(IUndoable undoable)
        {
            await _semaphore.WaitAsync();
            try
            {
                await undoable.DoAsync();
                _undoStack.Push(undoable);
                _redoStack.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UndoAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_undoStack.Any())
                {
                    return;
                }

                var undoable = _undoStack.Pop();
                await undoable.UndoAsync();
                _redoStack.Push(undoable);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task RedoAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!_redoStack.Any())
                {
                    return;
                }

                var undoable = _redoStack.Pop();
                await undoable.DoAsync();
                _undoStack.Push(undoable);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

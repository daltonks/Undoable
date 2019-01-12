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

        public void Do(IUndoable undoable)
        {
            _semaphore.Wait();
            try
            {
                undoable.Do();
                _undoStack.Push(undoable);
                _redoStack.Clear();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Undo()
        {
            _semaphore.Wait();
            try
            {
                if (!_undoStack.Any())
                {
                    return;
                }

                var undoable = _undoStack.Pop();
                undoable.Undo();
                _redoStack.Push(undoable);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Redo()
        {
            _semaphore.Wait();
            try
            {
                if (!_redoStack.Any())
                {
                    return;
                }

                var undoable = _redoStack.Pop();
                undoable.Do();
                _undoStack.Push(undoable);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

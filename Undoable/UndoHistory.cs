using System.Collections.Generic;
using System.Linq;

namespace Undoable
{
    public class UndoHistory
    {
        private readonly Stack<IUndoable> _undoStack = new Stack<IUndoable>();
        private readonly Stack<IUndoable> _redoStack = new Stack<IUndoable>();

        public void AlreadyDone(IUndoable undoable)
        {
            lock (this)
            {
                _undoStack.Push(undoable);
                _redoStack.Clear();
            }
        }

        public void Do(IUndoable undoable)
        {
            lock (this)
            {
                undoable.Do();
                _undoStack.Push(undoable);
                _redoStack.Clear();
            }
        }

        public void Undo()
        {
            lock (this)
            {
                if (!_undoStack.Any())
                {
                    return;
                }

                var undoable = _undoStack.Pop();
                undoable.Undo();
                _redoStack.Push(undoable);
            }
        }

        public void Redo()
        {
            lock (this)
            {
                if (!_redoStack.Any())
                {
                    return;
                }

                var undoable = _redoStack.Pop();
                undoable.Do();
                _undoStack.Push(undoable);
            }
        }
    }
}

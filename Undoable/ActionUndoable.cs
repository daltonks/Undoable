using System;

namespace Undoable
{
    public class ActionUndoable : IUndoable
    {
        private readonly Action _doAction;
        private readonly Action _undoAction;

        public ActionUndoable(Action doAction, Action undoAction)
        {
            _doAction = doAction;
            _undoAction = undoAction;
        }

        public void Do()
        {
            _doAction.Invoke();
        }

        public void Undo()
        {
            _undoAction.Invoke();
        }
    }
}

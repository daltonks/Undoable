namespace Undoable
{
    public class InverseUndoable : IUndoable
    {
        private readonly IUndoable _undoable;

        public InverseUndoable(IUndoable undoable)
        {
            _undoable = undoable;
        }

        public void Do()
        {
            _undoable.Undo();
        }

        public void Undo()
        {
            _undoable.Do();
        }
    }
}

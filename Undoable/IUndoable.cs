namespace Undoable
{
    public interface IUndoable
    {
        void Do();
        void Undo();
    }
}

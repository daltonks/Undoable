namespace Undoable
{
    public static class UndoableExtensions
    {
        public static IUndoable Inverse(this IUndoable undoable)
        {
            return new InverseUndoable(undoable);
        }
    }
}

using System.Threading.Tasks;

namespace Undoable
{
    public interface IUndoable
    {
        Task DoAsync();
        Task UndoAsync();
    }
}

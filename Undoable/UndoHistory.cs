using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Undoable
{
    public class UndoHistory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Stack<IUndoable> _undoStack = new Stack<IUndoable>();
        private readonly Stack<IUndoable> _redoStack = new Stack<IUndoable>();

        private int _undoCount;
        public int UndoCount
        {
            get => _undoCount;
            set => SetProperty(ref _undoCount, value);
        }

        private int _redoCount;
        public int RedoCount
        {
            get => _redoCount;
            set => SetProperty(ref _redoCount, value);
        }

        public void Do(IUndoable undoable)
        {
            lock (this)
            {
                undoable.Do();
                AlreadyDone(undoable);
            }
        }

        public void AlreadyDone(IUndoable undoable)
        {
            lock (this)
            {
                _undoStack.Push(undoable);
                _redoStack.Clear();

                UpdateCounts();
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

                UpdateCounts();
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

                UpdateCounts();
            }
        }

        private void UpdateCounts()
        {
            UndoCount = _undoStack.Count;
            RedoCount = _redoStack.Count;
        }

        private void SetProperty<T>(ref T obj, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(obj, value))
            {
                return;
            }

            obj = value;
            RaisePropertyChanged(propertyName);
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

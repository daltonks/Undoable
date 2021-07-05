using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Undoable
{
    public class UndoHistory : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly object _lock = new object();
        private readonly LinkedList<IUndoable> _undoList = new LinkedList<IUndoable>();
        private readonly LinkedList<IUndoable> _redoList = new LinkedList<IUndoable>();

        public List<IUndoable> UndoItems
        {
            get
            {
                lock (_lock)
                {
                    return new List<IUndoable>(_undoList);
                }
            }
        }

        public List<IUndoable> RedoItems
        {
            get
            {
                lock (_lock)
                {
                    return new List<IUndoable>(_redoList);
                }
            }
        }

        public int MaxUndoCount { get; set; } = int.MaxValue;
        public int MaxRedoCount { get; set; } = int.MaxValue;

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
            lock (_lock)
            {
                undoable.Do();
                AlreadyDone(undoable);
            }
        }

        public void AlreadyDone(IUndoable undoable)
        {
            lock (_lock)
            {
                _undoList.AddLast(undoable);
                _redoList.Clear();

                while (_undoList.Count > MaxUndoCount)
                {
                    _undoList.RemoveFirst();
                }

                UpdateCounts();
            }
        }

        public void Undo()
        {
            lock (_lock)
            {
                if (!_undoList.Any())
                {
                    return;
                }

                var undoable = _undoList.Last.Value;
                _undoList.RemoveLast();
                undoable.Undo();
                _redoList.AddLast(undoable);

                while (_redoList.Count > MaxRedoCount)
                {
                    _redoList.RemoveFirst();
                }

                UpdateCounts();
            }
        }

        public void Redo()
        {
            lock (_lock)
            {
                if (!_redoList.Any())
                {
                    return;
                }

                var undoable = _redoList.Last.Value;
                _redoList.RemoveLast();
                undoable.Do();
                _undoList.AddLast(undoable);

                while (_undoList.Count > MaxUndoCount)
                {
                    _undoList.RemoveFirst();
                }

                UpdateCounts();
            }
        }

        private void UpdateCounts()
        {
            UndoCount = _undoList.Count;
            RedoCount = _redoList.Count;
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

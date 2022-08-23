using System;
using System.Linq;
using System.Collections.Generic;

namespace RyzeEditor.GameWorld
{
	[Serializable]
	public class Selection
	{
		readonly List<IEntity> _entities = new List<IEntity>();

		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

		public void AddEntity(IEntity entity)
		{
            if (_entities.Contains(entity))
            {
                return;
            }

			_entities.Add(entity);

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs());
        }

		public void RemoveEntity(Guid id)
		{
			var entity = _entities.FirstOrDefault(x => x.Id == id);

			if (entity == null)
            {
                return;
            }

			_entities.Remove(entity);

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs());
        }

		public List<IEntity> Get()
		{
			return _entities;
		}

		public void Clear()
		{
			_entities.Clear();

            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs());
        }
	}

	public class SelectionChangedEventArgs : EventArgs
	{
	}
}

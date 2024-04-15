using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using RyzeEditor.UndoRedo;

namespace RyzeEditor.GameWorld
{
	[Serializable]
	public class WorldMap
	{
		private readonly List<EntityBase> _entities = new List<EntityBase>();

		[field: NonSerialized]
		private static UndoRedoManager _undoRedoManager;

        [field: NonSerialized]
        public event EventHandler<EntityEventArgs> EntityAdded;

        [field: NonSerialized]
        public event EventHandler<EntityEventArgs> EntityModified;

        [field: NonSerialized]
        public event EventHandler<EntityEventArgs> EntityDeleted;

        [field: NonSerialized]
        public string _id;

        public void SetModified()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }

        public string Id
        {
            private set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public Camera Camera { get; set; }

		public IEnumerable<EntityBase> Entities
		{
			get { return _entities; }
		}

        public WorldMap(Camera camera)
		{
			_undoRedoManager = new UndoRedoManager(this);

            _id = Guid.NewGuid().ToString();

            Camera = camera;
        }

        public void Update()
        {
            _undoRedoManager.RegisterEntity(Camera);
        }

        public void AddEntity(EntityBase entity)
		{
			if (_entities.Any(e => e == entity) || Camera.Id == entity.Id)
			{
				return;
			}

			_entities.Add(entity);

			_undoRedoManager.RegisterEntity(entity);

            Id = Guid.NewGuid().ToString();

            OnEntityAdded(new EntityEventArgs(entity.Id));
		}

		public void RemoveEntity(EntityBase entity)
		{
			var e = _entities.FirstOrDefault(x => x == entity);

			if (e != null)
			{
				_entities.Remove(e);
			}

			_undoRedoManager.UnRegisterEntity(entity);

            Id = Guid.NewGuid().ToString();

            OnEntityDeleted(new EntityEventArgs(entity.Id));
		}

		public EntityBase FindEntity(Guid id)
		{
            EntityBase entity = (Camera.Id == id ? Camera : null) ?? _entities.FirstOrDefault(x => x.Id == id);

            return entity;
		}

		public List<EntityBase> GetEntities()
		{
			return _entities;
		}

        protected virtual void OnEntityAdded(EntityEventArgs e)
		{
            EntityAdded?.Invoke(this, e);
        }

		protected virtual void OnEntityModified(EntityEventArgs e)
		{
            EntityModified?.Invoke(this, e);
		}

		protected virtual void OnEntityDeleted(EntityEventArgs e)
		{
			EntityDeleted?.Invoke(this, e);
		}

		[OnSerialized]
		internal void OnWorldMapSerialized(StreamingContext context)
		{
			_undoRedoManager = new UndoRedoManager(this);

			foreach (var entity in _entities)
			{
				_undoRedoManager.RegisterEntity(entity);
			}

			_undoRedoManager.Update();
		}

		[OnDeserialized]
		internal void OnWorldMapDeserialized(StreamingContext context)
		{
			_undoRedoManager = new UndoRedoManager(this);

			foreach (var entity in _entities)
			{
				_undoRedoManager.RegisterEntity(entity);
			}
		}

		public static void UndoChanges()
		{
            _undoRedoManager?.Undo();			
		}

		public static void RedoChanges()
		{
            _undoRedoManager?.Redo();
		}

		public static void CommitChanges()
		{
            _undoRedoManager?.CommitChanges();
		}
	}

	public class EntityEventArgs
	{
		public Guid EntityId { get; set; }

		public EntityEventArgs(Guid entityId)
		{
			EntityId = entityId;
		}
	}
}
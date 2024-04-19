using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RyzeEditor.GameWorld
{
	[Serializable]
	public abstract class EntityBase : IEntity, INotifyPropertyChanged
	{
        [InspectorVisible(false)]
        public Guid Id { get; set; }

        [InspectorVisible(false)]
        public Guid ParentId { get; set; }

		public bool IsHidden { get; set; }

        [InspectorVisible(false)]
        public bool BlockNotifications { get; set; }

        public override bool Equals(object entity)
		{
			return entity is EntityBase && this == (EntityBase)entity;
		}

		public static bool operator == (EntityBase base1, EntityBase base2)
		{
			if ((object)base1 == null && (object)base2 == null)
			{
				return true;
			}

			if ((object)base1 == null || (object)base2 == null)
			{
				return false;
			}

			return base1.Id.Equals(base2.Id);
		}

		public static bool operator != (EntityBase base1, EntityBase base2)
		{
			return !(base1 == base2);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
            if (!BlockNotifications)
            {
                PropertyChanged?.Invoke(this, new EntityPropertyChangedEventArgs(propertyName, Id));
            }
        }
	}

	public class EntityPropertyChangedEventArgs : PropertyChangedEventArgs
	{
        public EntityPropertyChangedEventArgs(string propertyName, Guid entityId)
			: base(propertyName)
		{
			EntityId = entityId;
		}

        public Guid EntityId { get; }
    }
}
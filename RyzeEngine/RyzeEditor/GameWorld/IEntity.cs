using System;

namespace RyzeEditor.GameWorld
{
	public interface IEntity
	{
		Guid Id { get; set; }

		Guid ParentId { get; set; }

		bool IsHidden { get; set; }
	}
}
using System;
using System.Collections.Generic;
using RyzeEditor.GameWorld;

namespace RyzeEditor.UndoRedo
{
	public interface ICommand
	{
        EntityBase Entity { get; set; }

        void Execute();

        void Undo();
    }

	public class EditEntityCommand : ICommand
	{
        public EntityBase Entity { get; set; }

        public string PropertyName { get; set; }

		public Object NewValue { get; set; }

        public Object OldValue { get; set; }

        public void Undo()
		{
			if (Entity == null)
			{
				return;
			}

			var prop = Entity.GetType().GetProperty(PropertyName);

			if (prop == null)
			{
				return;
			}

			prop.SetValue(Entity, OldValue);
        }

        public void Execute()
        {
            if (Entity == null)
            {
                return;
            }

            var prop = Entity.GetType().GetProperty(PropertyName);

            if (prop == null)
            {
                return;
            }

            prop.SetValue(Entity, NewValue);
        }
    }

	public class CommandGroup
	{
		public List<ICommand> Commands = new List<ICommand>();

		public void AddCommand(ICommand command)
		{
			Commands.Add(command);
		}

		public void Undo()
		{
			foreach (var command in Commands)
			{
                command.Undo();
			}
		}

        public void Redo()
        {
            foreach (var command in Commands)
            {
                command.Undo();
            }
        }

        public void DeleteCommand(Guid entityId)
        {
            var cmdList = new List<ICommand>();

            foreach (var command in Commands)
            {
                if (command.Entity != null && command.Entity.Id == entityId)
                {
                    cmdList.Add(command);
                }
            }

            foreach (var command in cmdList)
            {
                Commands.Remove(command);
            }
        }
    }
}
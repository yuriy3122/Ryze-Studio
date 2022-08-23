using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RyzeEditor.GameWorld;

namespace RyzeEditor.UndoRedo
{
	public class UndoRedoManager
	{
		private readonly WorldMap _worldMap;

		private readonly Stack<CommandGroup> _undoStack = new Stack<CommandGroup>();
		private readonly Stack<CommandGroup> _redoStack = new Stack<CommandGroup>();

        private readonly Dictionary<Guid, Dictionary<string, ChangeSet>> _entityChangedValues = 
            new Dictionary<Guid, Dictionary<string, ChangeSet>>();

        public UndoRedoManager(WorldMap map)
		{
			_worldMap = map;
		}

		public void Update()
		{
			_undoStack.Clear();
			_redoStack.Clear();
			_entityChangedValues.Clear();

			foreach (var entity in _worldMap.Entities)
			{
				RegisterEntity(entity);
			}
		}

		public void RegisterEntity(EntityBase e)
		{
			var dic = new Dictionary<String, ChangeSet>();
			var properties = e.GetType().GetProperties();

			foreach (var property in properties.Where(property => property.GetSetMethod() != null && property.GetGetMethod() != null))
			{
                var val = property.GetValue(e);

                dic.Add(property.Name, new ChangeSet(val, val));
			}

            _entityChangedValues.Add(e.Id, dic);

			e.PropertyChanged += EntityOnPropertyChanged;
		}

		public void UnRegisterEntity(EntityBase e)
		{
            _entityChangedValues.Remove(e.Id);

            foreach (var cmdGroup in _undoStack)
            {
                cmdGroup.DeleteCommand(e.Id);
            }

            foreach (var cmdGroup in _redoStack)
            {
                cmdGroup.DeleteCommand(e.Id);
            }

            e.PropertyChanged -= EntityOnPropertyChanged;
		}

		public void Undo()
		{
            if (_undoStack.Count == 0)
            {
                return;
            }

            var cmdGroup = _undoStack.Pop();

            foreach(var cmd in cmdGroup.Commands.OfType<EditEntityCommand>())
            {
                cmd.Entity.PropertyChanged -= EntityOnPropertyChanged;
				
				cmd.Undo();
				
				cmd.Entity.PropertyChanged += EntityOnPropertyChanged;

                _entityChangedValues[cmd.Entity.Id][cmd.PropertyName].Value = cmd.OldValue;
                _entityChangedValues[cmd.Entity.Id][cmd.PropertyName].OldValue = cmd.OldValue;
            }

            _redoStack.Push(cmdGroup);
		}

		public void Redo()
		{
            if (_redoStack.Count == 0)
            {
                return;
            }

            var cmdGroup = _redoStack.Pop();

            foreach (var cmd in cmdGroup.Commands.OfType<EditEntityCommand>())
            {
                cmd.Entity.PropertyChanged -= EntityOnPropertyChanged;

                cmd.Execute();

                cmd.Entity.PropertyChanged += EntityOnPropertyChanged;

                _entityChangedValues[cmd.Entity.Id][cmd.PropertyName].Value = cmd.NewValue;
                _entityChangedValues[cmd.Entity.Id][cmd.PropertyName].OldValue = cmd.NewValue;
            }

            _undoStack.Push(cmdGroup);
        }

		private void EntityOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			var eventArg = propertyChangedEventArgs as EntityPropertyChangedEventArgs;

			if (eventArg == null || _entityChangedValues.ContainsKey(eventArg.EntityId) == false)
			{
				return;
			}

			var property = sender.GetType().GetProperty(eventArg.PropertyName);

            _entityChangedValues[eventArg.EntityId][eventArg.PropertyName].Value = property.GetValue(sender);
		}

		public void CommitChanges()
		{
            var cmdGroup = new CommandGroup();

            foreach (var entityChange in _entityChangedValues)
			{
				var entity = _worldMap.Entities.SingleOrDefault(x => x.Id == entityChange.Key);

                var dic = entityChange.Value;

                var keys = dic.Keys.ToList();

                foreach(var key in keys)
                {
                    var change = dic[key];

                    if (change.Value == change.OldValue)
                    {
                        continue;
                    }

                    var command = new EditEntityCommand();
                    command.Entity = entity;
                    command.PropertyName = key;
                    command.OldValue = change.OldValue;
                    command.NewValue = change.Value;

                    cmdGroup.AddCommand(command);

                    dic[key].Value = command.NewValue;
                    dic[key].OldValue = command.NewValue;
                }
            }

            _undoStack.Push(cmdGroup);
        }
	}

    public class ChangeSet
    {
        public ChangeSet(object val, object oldVal)
        {
            Value = val;
            OldValue = oldVal;
        }

        public object Value { get; set; }
        public object OldValue { get; set; }
    }
}
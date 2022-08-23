namespace RyzeEditor.ResourceManagment
{
	public static class StorageFactory
	{
		public static IResourceStorage GetStorage(string storageName)
		{
			IResourceStorage storage = null;
			switch (storageName)
			{
				case "FileStorage":
					storage = new FileStorage();
					break;
			}

			return storage;
		}
	}
}

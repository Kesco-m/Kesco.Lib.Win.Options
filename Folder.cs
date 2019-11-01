using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	/// <summary>
	/// –абота с разделами реестра Windows
	/// </summary>
	public class Folder
	{
		private FolderCollection folders;
		private OptionCollection options;

		private string path;
		private string name;

		/// <summary>
		/// Cоздание раздела в реестре по указанному пути
		/// </summary>
		/// <param name="path">ѕуть в реестре</param>
		/// <param name="name">название раздела реестра</param>
		public Folder(string path, string name)
		{
			this.path = path;
			this.name = name;

			folders = new FolderCollection(FullPath);
			options = new OptionCollection(FullPath);
		}

		/// <summary>
		/// Cоздание раздела в реестре у родителького Folder
		/// </summary>
		/// <param name="parent">родителький Folder</param>
		/// <param name="name">название раздела реестра</param>
		public Folder(Folder parent, string name)
		{
			path = parent.FullPath;
			this.name = name;

			folders = new FolderCollection(FullPath);
			options = new OptionCollection(FullPath);
		}

		/// <summary>
		/// «агрузка нового параметра из реестра
		/// ¬ случае отсутстви€ создаетс€ новый
		/// </summary>
		/// <typeparam name="T">тип параметра. Int или String</typeparam>
		/// <param name="name">Ќазвание параметра</param>
		/// <param name="def">«начеение, в случае отсутви€ параметра в реестре</param>
		/// <returns></returns>
		public IOption LoadOption<T>(string name, T def)
		{
			return LoadOption(name, def, null);
		}

		/// <summary>
		/// «агрузка нового параметра из реестра
		/// ¬ случае отсутстви€ создаетс€ новый
		/// </summary>
		/// <typeparam name="T">тип параметра. Int или String</typeparam>
		/// <param name="name">Ќазвание параметра</param>
		/// <param name="def">«начеение, в случае отсутви€ параметра в реестре</param>
		/// <param name="action">событие изменени€ параметра в программе</param>
		/// <returns></returns>
		public IOption LoadOption<T>(string name, T def, OptionEventHandler action)
		{
			return options.Add(name, def, action);
		}

		public string LoadStringOption(string name, string def)
		{
			return LoadStringOption(name, def, null);
		}

		public string LoadStringOption(string name, string def, OptionEventHandler action)
		{
			return options.Add(name, def, action).Value.ToString();
		}

		public int LoadIntOption(string name, int def)
		{
			return (int)options.Add(name, def, null).Value;
		}

		/// <summary>
		/// ѕолучение значени€ сохраненого в программе
		/// </summary>
		/// <param name="name">Ќазвание параметра</param>
		/// <returns></returns>
		public IOption Option(string name)
		{
			return options.GetByName(name);
		}

		/// <summary>
		/// ѕолучение значени€. ¬ случее отсутстви€ параметра, он создаетс€.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public IOption OptionForced<T>(string name)
		{
			return options.GetByNameForced<T>(name);
		}

		/// <summary>
		/// «агрузка подраздела
		/// </summary>
		/// <param name="name">Ќазвание подраздела</param>
		/// <returns></returns>
		public Folder SubFolder(string name)
		{
			return folders.GetByName(name);
		}

		/// <summary>
		/// «агрузка всех подразделов и параметров.
		/// </summary>
		public void Load()
		{
			folders.Load();
			options.Load();
		}

		/// <summary>
		/// —охранение всех подразделов и параметров.
		/// </summary>
		public void Save()
		{
			folders.Save();
			options.Save();
		}

		public FolderCollection Folders
		{
			get { return folders; }
		}

		public OptionCollection Options
		{
			get { return options; }
		}

		/// <summary>
		/// ѕуть до раздела в реестре
		/// </summary>
		public string Path
		{
			get { return path; }
		}

		/// <summary>
		/// Ќазвание раздела
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// ѕолный путь раздела в реестре
		/// </summary>
		public string FullPath
		{
			get { return path + @"\" + name; }
		}

		public void Clear()
		{
			RegistryKey key = Win.Options.OptionCollection.FolderKey(FullPath);
			string[] keys = key.GetSubKeyNames();
			for(int i = 0; i < keys.Length; i++)
			{
				IOption opt = options.GetByName(keys[i]);
				if(opt != null)
					options.Remove(opt);
				key.DeleteSubKeyTree(keys[i]);
			}
		}

		/// <summary>
		/// ѕроверка на наличие параметра в реестре в текущем разделе
		/// </summary>
		/// <param name="name">»м€ провер€емого параметра</param>
		public bool CheckExists(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(path + "\\" + name, false);
			return key != null;
		}

		/// <summary>
		/// ѕроверка на наличие параметра в реестре в текущем разделе
		/// </summary>
		/// <param name="name">»м€ провер€емого параметра</param>
		public bool CheckExistsOption(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(FullPath, false);
			if(key == null)
				return false;
			return key.GetValue(name) != null;
		}

		/// <summary>
		/// ”даление данных о разделе из реестра и кеша
		/// </summary>
		/// <param name="name">»м€ удал€емого раздела</param>
		public void Delete(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(path + "\\" + this.name, true);
			Folder opt = folders.GetByName(name);
			if(opt != null)
				folders.Delete(name);
			if(key.OpenSubKey(name, false) != null)
				key.DeleteSubKey(name);
		}

		/// <summary>
		/// ”даление данных о параметре из реестра и кеша
		/// </summary>
		/// <param name="name">»м€ удал€емого параметра</param>
		public void DeleteOption(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(FullPath, true);
			IOption opt = options.GetByName(name);
			if(opt != null)
				options.Remove(opt);
			if(key != null)
			key.DeleteValue(name, false);
		}

		public FolderCollection GetSavedFolders()
		{
			FolderCollection fc = new FolderCollection(this.FullPath);
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(this.FullPath, true);
			string[] subk = key.GetSubKeyNames();
			if(subk != null)
				for(int i = 0; i < subk.Length; i++)
					fc.GetByNameForced(subk[i]);
			return fc;
		}
	}
}
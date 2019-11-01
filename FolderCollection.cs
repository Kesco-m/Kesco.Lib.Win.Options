using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	/// <summary>
	/// Коллекция папок опций
	/// </summary>
	public class FolderCollection : IEnumerable
	{
		private List<Folder> folders;
		private string path; // Путь для хранения в реестре

		public FolderCollection(string path)
		{
			folders = new List<Folder>();
			this.path = path;
		}

		public FolderEnumerator GetEnumerator() // non-IEnumerable version
		{
			return new FolderEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator() // IEnumerable version
		{
			return new FolderEnumerator(this);
		}

		// Внутренний класс, реализующий интерфейс IEnumerator

		public class FolderEnumerator : IEnumerator
		{
			private int position = -1;
			private FolderCollection fc;

			public FolderEnumerator(FolderCollection fc)
			{
				this.fc = fc;
			}

			public bool MoveNext()
			{
				if(position >= fc.folders.Count - 1)
					return false;

				position++;
				return true;
			}

			public void Reset()
			{
				position = -1;
			}

			public Folder Current				// non-IEnumerator version. type-safe
			{
				get { return fc.folders[position]; }
			}

			object IEnumerator.Current			// IEnumerator version. returns object
			{
				get { return fc.folders[position]; }
			}
		}

		public Folder Add(string name)
		{
			try
			{
				Folder f = new Folder(path, name);
				folders.Add(f);
				return f;
			}
			catch
			{
			}

			return null;
		}

		public Folder GetByName(string name)
		{
			foreach(Folder folder in folders)
				if(folder.Name == name)
					return folder;

			return null;
		}

		public Folder GetByNameForced(string name)
		{
			foreach(Folder folder in folders)
				if(folder.Name == name)
					return folder;

			return Add(name);
		}

		//public IOption LoadOption(string folder, string name, string def)
		//{
		//    return LoadOption(folder, name, def, null);
		//}

		//public IOption LoadOption(string folder, string name, string def, OptionEventHandler action)
		//{
		//    if(GetByName(folder) == null)
		//        Add(folder);

		//    Folder f = GetByName(folder);
		//    return f.LoadOption<string>(name, def, action);
		//}

		public IOption IOption(string folder, string name)
		{
			if(GetByName(folder) == null)
				return null;

			Folder f = GetByName(folder);
			if(f.Option(name) == null)
				return null;

			return f.Option(name);
		}

		public void Load()
		{
			foreach(Folder folder in folders)
				folder.Load();
		}

		public void Save()
		{
			foreach(Folder folder in folders)
				folder.Save();
		}

		public int Count
		{
			get { return folders.Count; }
		}

		public bool CheckExists(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(path + "\\" + name, false);
			return key != null;
		}

		public void Delete(string name)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey key = curUser.OpenSubKey(path, true);
			Folder opt = GetByName(name);
			if(opt != null)
				folders.Remove(opt);
			if(key.OpenSubKey(name, false) != null)
				key.DeleteSubKeyTree(name);
		}
	}
}
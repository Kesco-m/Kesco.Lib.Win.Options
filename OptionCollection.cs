using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	/// <summary>
	///   Коллекция опций
	/// </summary>
	public class OptionCollection : IEnumerable
	{
		private List<IOption> options;
		private string path;			// Путь для хранения в реестре

		public OptionCollection(string path)
		{
			options = new List<IOption>();
			this.path = path;
		}

		public OptionEnumerator GetEnumerator()			// non-IEnumerable version
		{
			return new OptionEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()			// IEnumerable version
		{
			return new OptionEnumerator(this);
		}

		// Внутренний класс, реализующий интерфейс IEnumerator
		public class OptionEnumerator : IEnumerator
		{
			private int position = -1;
			private OptionCollection oc;

			public OptionEnumerator(OptionCollection oc)
			{
				this.oc = oc;
			}

			public bool MoveNext()
			{
				if(position >= oc.options.Count - 1)
					return false;

				position++;
				return true;
			}

			public void Reset()
			{
				position = -1;
			}

			public IOption Current				// non-IEnumerator version. type-safe
			{
				get { return oc.options[position]; }
			}

			object IEnumerator.Current // IEnumerator version. returns object
			{
				get { return oc.options[position]; }
			}
		}

		public IOption Add(string name, object def, OptionEventHandler action)
		{
			IOption opt = GetByName(name);

			if(opt == null)
			{
				try
				{
					RegistryKey folderKey = FolderKey(path);
					object regValue = folderKey.GetValue(name);

					if(regValue != null)
					{
						RegistryValueKind kind = folderKey.GetValueKind(name);
						switch(kind)
						{
							case RegistryValueKind.DWord:
								opt = new IntOption(path, name, (int)def);
								opt.ValueChanged += action;
								opt.Value = regValue;
								break;
							case RegistryValueKind.String:
							default:
								if(def is int)
									opt = new IntOption(path, name, (int)def);
								else
									opt = new Option(path, name, (string)def);
								opt.ValueChanged += action;
								opt.Value = regValue;
								break;
						}
					}
					else
					{
						if(def is int)
							opt = new IntOption(path, name, (int)def);
						else
							opt = new Option(path, name, (string)def);
						opt.ValueChanged += action;
						opt.Value = def;
						opt.Save();
					}
					options.Add(opt);
				}
				catch
				{
				}
			}

			return opt;
		}

		public void Remove(IOption opt)
		{
			if(opt != null)
				options.Remove(opt);
		}

		public void LoadForced()
		{
			RegistryKey folderKey = OptionCollection.FolderKey(path);
			string[] regNames = folderKey.GetValueNames();
			IOption option;

			foreach(string regName in regNames)
			{
				RegistryValueKind kind = folderKey.GetValueKind(regName);
				object regValue = folderKey.GetValue(regName);
				switch(kind)
				{
					case RegistryValueKind.DWord:
						option = new IntOption(path, regName, 0);
						break;
					case RegistryValueKind.String:
					default:
						option = new Option(path, regName, null);
						break;
				}
				option.Value = regValue;
				options.Add(option);
			}
		}

		public void Load()
		{
			foreach(IOption option in options)
				option.Load();
		}

		public void Save()
		{
			foreach(IOption option in options)
				option.Save();
		}

		public IOption GetByName(string name)
		{
			foreach(IOption opt in options)
				if(opt.Name == name)
					return opt;

			return null;
		}

		/// <summary>
		/// получение опции с определеным типом
		/// </summary>
		/// <typeparam name="T">тип хранимой переменной</typeparam>
		/// <param name="name">название параметра в реестре</param>
		/// <returns>иницализированный класс для данного типа</returns>
		public IOption GetByNameForced<T>(string name)
		{
			IOption opt = GetByName(name);
			object va = default(T);
			if( va == null)
				va = string.Empty;
			return opt ?? Add(name, va, null);
		}

		public int Count
		{
			get { return options.Count; }
		}

		internal static RegistryKey FolderKey(string path)
		{
			RegistryKey curUser = Registry.CurrentUser;
			RegistryKey folderKey = curUser.OpenSubKey(path, true);

			if(folderKey == null)
			{
				curUser.CreateSubKey(path);
				folderKey = curUser.OpenSubKey(path, true);
			}

			return folderKey;
		}

		public void Clear()
		{
			options.Clear();
		}
	}
}
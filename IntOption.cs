using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	public class IntOption : IOption
	{
		private string name; // Имя опции
		private string path; // Папка в реестре
		private int val; // Значение - строка
		private int def; // Значение по умолчанию

		/// <summary>
		/// интовый параметр из реестра
		/// </summary>
		/// <param name="path">путь до параментра</param>
		/// <param name="name">название параметра</param>
		/// <param name="def">значение по умолчанию</param>
		public IntOption(string path, string name, int def)
		{
			this.DefaultKind = Microsoft.Win32.RegistryValueKind.DWord;
			this.path = path;
			this.name = name;
			this.def = def;
		}


		public Microsoft.Win32.RegistryValueKind Kind { get; internal set; }

		public Microsoft.Win32.RegistryValueKind DefaultKind { get; private set; }

		/// <summary>
		/// Сохранение в реестре
		/// </summary>
		public void Save() // 
		{
			RegistryKey folderKey = OptionCollection.FolderKey(path);

			folderKey.SetValue(name, val, DefaultKind);
		}

		/// <summary>
		/// Загрузка значения параметра из реестра
		/// </summary>
		public void Load()
		{
			RegistryKey folderKey = OptionCollection.FolderKey(path);
			object regValue = folderKey.GetValue(name);

			if(regValue != null)
				if(regValue is int)
					val = (int)regValue;
				else
				{
					int res = def;
					if(int.TryParse(regValue.ToString(), out res))
						val = res;
					else
					{
						val = def;
						Save();
					}
				}
			else
			{
				val = def;
				Save();
			}

			if(ValueChanged != null)
				ValueChanged(this, new OptionEventArgs(val));
		}

		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public object Value
		{
			get { return val; }
			set
			{
				if(value is int)
					val = (int)value;
				else
				{
					int res = def;
					if(int.TryParse(value.ToString(), out res))
						val = res;
					else
					{
						val = def;
						Save();
					}
					//if(Log.Logger.IsConfigured)
					//    Log.Logger.WriteEx(new System.Exception("Не верный тип параметра " + Path + "\\" + name));
				}
				if(ValueChanged != null)
					ValueChanged(this, new OptionEventArgs(val));
			}
		}
		
		public event OptionEventHandler ValueChanged;


		public T GetValue<T>()
		{
			if(typeof(T) == typeof(int))
				return (T)(object)val;
			else
			{
				if(Log.Logger.IsConfigured)
					Log.Logger.WriteEx(new System.Exception("Не верный тип параметра" + Path + "\\" + name));
				return default(T);
			}
		}
	}
}

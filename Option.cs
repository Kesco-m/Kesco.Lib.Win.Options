using System.Diagnostics;
using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	/// <summary>
	///   Переменная в настройках приложения
	/// </summary>
	[DebuggerDisplay("Name = {name} Path = {path} Val = {val}")]
	public class Option : IOption
	{
		private string name; // Имя опции
		private string path; // Папка в реестре
		private string val; // Значение - строка
		private string def; // Значение по умолчанию

		public RegistryValueKind Kind { get; internal set; }
		public RegistryValueKind DefaultKind { get; private set; }

		public event OptionEventHandler ValueChanged;

		/// <summary>
		/// Cтроковый параметр из реестра
		/// </summary>
		/// <param name="path">путь до параментра</param>
		/// <param name="name">название параметра</param>
		/// <param name="def">значение по умолчанию</param>
		/// <param name="value">текущее значение</param>
		/// <param name="action">событие о изменении значения параметра</param>
		public Option(string path, string name, string def)
		{
			DefaultKind = RegistryValueKind.String;
			this.name = name;
			this.path = path;
			this.def = def;
		}

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
			string regValue = folderKey.GetValue(name) as string;

			if(regValue != null)
				val = regValue;
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
				val = value as string;
				if(ValueChanged != null)
					ValueChanged(this, new OptionEventArgs(val));
			}
		}


		public T GetValue<T>()
		{
			return (T)(object)val;
		}
	}
}
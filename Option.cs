using System.Diagnostics;
using Microsoft.Win32;

namespace Kesco.Lib.Win.Options
{
	/// <summary>
	///   ���������� � ���������� ����������
	/// </summary>
	[DebuggerDisplay("Name = {name} Path = {path} Val = {val}")]
	public class Option : IOption
	{
		private string name; // ��� �����
		private string path; // ����� � �������
		private string val; // �������� - ������
		private string def; // �������� �� ���������

		public RegistryValueKind Kind { get; internal set; }
		public RegistryValueKind DefaultKind { get; private set; }

		public event OptionEventHandler ValueChanged;

		/// <summary>
		/// C�������� �������� �� �������
		/// </summary>
		/// <param name="path">���� �� ����������</param>
		/// <param name="name">�������� ���������</param>
		/// <param name="def">�������� �� ���������</param>
		/// <param name="value">������� ��������</param>
		/// <param name="action">������� � ��������� �������� ���������</param>
		public Option(string path, string name, string def)
		{
			DefaultKind = RegistryValueKind.String;
			this.name = name;
			this.path = path;
			this.def = def;
		}

		/// <summary>
		/// ���������� � �������
		/// </summary>
		public void Save() // 
		{
			RegistryKey folderKey = OptionCollection.FolderKey(path);

			folderKey.SetValue(name, val, DefaultKind);
		}

		/// <summary>
		/// �������� �������� ��������� �� �������
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
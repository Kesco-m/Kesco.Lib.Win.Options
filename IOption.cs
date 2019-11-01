using System;
namespace Kesco.Lib.Win.Options
{
	public interface IOption
	{

		string Name { get; set; }
		string Path { get; set; }
		object Value { get; set; }

		Microsoft.Win32.RegistryValueKind Kind { get; }
		Microsoft.Win32.RegistryValueKind DefaultKind {get;}

		T GetValue<T>();
		void Load();
		void Save();

		event OptionEventHandler ValueChanged;
	}

	/// <summary>
	///  Делегат для обработки события изменения параметра
	/// </summary>
	public delegate void OptionEventHandler(object source, OptionEventArgs e);

	/// <summary>
	/// данные о cобытие изменения параметра
	/// </summary>
	public class OptionEventArgs : EventArgs
	{
		private object val;

		/// <summary>
		/// данные о cобытие изменения параметра
		/// </summary>
		/// <param name="val">новое значение параметра</param>
		public OptionEventArgs(object val)
		{
			this.val = val;
		}

		public object Value
		{
			get { return val; }
		}
	}
}

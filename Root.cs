namespace Kesco.Lib.Win.Options
{
    /// <summary>
    /// ��������� �� �������� ������� �������, � ������� �������� ����� ����������.
    /// </summary>
    public class Root : Folder
    {
        public Root() : base(@"Software", "Kesco")
        {
        }

        public Root(string name) : base(@"Software\Kesco", name)
        {
        }
    }
}
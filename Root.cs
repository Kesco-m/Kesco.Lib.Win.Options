namespace Kesco.Lib.Win.Options
{
    /// <summary>
    /// ”казывает на корневой каталог реестра, в котором хран€тс€ опции приложени€.
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
namespace Zapp.Desktop.Helpers.Listeners
{
    public static class ArgumentParser
    {
        public static T GetArgument<T>(this object[] args, int index) where T : class
        {
            if (args == null || index >= args.Length)
            {
                return null;
            }
            return args[index] is T arg ? arg : null;
        }

        public static int? GetIntegerArgument(this object[] args, int index)
        {
            if (args == null || index >= args.Length)
            {
                return null;
            }
            if (args[index] is long arg)
            {
                return (int)arg;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class CommandParser
    {
        public static bool ParseCommand(string line, string cmd, int expectedArgs, ref List<string> args)
        {
            args.Clear();

            string shortCmdName = new string(cmd.Where(Char.IsUpper).ToArray());

            if (!line.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase) &&
                !line.StartsWith(shortCmdName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (expectedArgs == 0)
                return true;

            var strings = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (expectedArgs < 0)
            {
                args.AddRange(strings.Skip(1));
                return true;
            }
            for (int i = 1; i < expectedArgs; i++)
            {
                args.Add(strings[i]);
            }
            args.Add(string.Join(" ", strings.Skip(expectedArgs)));
            return true;
        }
    }
}

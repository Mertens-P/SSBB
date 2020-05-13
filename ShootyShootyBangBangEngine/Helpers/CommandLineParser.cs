using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootyShootyBangBangEngine.Helpers
{
    public class CommandLineParser
    {
        Dictionary<string, List<string>> m_parsedCommandLineArguments = new Dictionary<string, List<string>>();

        public void Parse(string[] args)
        {
            string currentKey = "";
            foreach(var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    currentKey = arg;
                    if (!m_parsedCommandLineArguments.ContainsKey(arg))
                        m_parsedCommandLineArguments.Add(arg, new List<string>());
                }
                else if (!string.IsNullOrEmpty(currentKey))
                    m_parsedCommandLineArguments[currentKey].Add(arg);
            }
        }

        public string GetValueForArgument(string argument)
        {
            if (m_parsedCommandLineArguments.TryGetValue(argument, out var values) && values.Count > 0)
                return values.First();
            return null;
        }

        public IEnumerable<string> GetValuesForArgument(string argument)
        {
            if (m_parsedCommandLineArguments.TryGetValue(argument, out var values))
            {
                foreach (var val in values)
                    yield return val;
            }
        }

        public bool IsArgumentSet(string argument)
        {
            return m_parsedCommandLineArguments.ContainsKey(argument);
        }
    }
}

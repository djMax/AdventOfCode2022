using System;
using System.Reflection;

namespace AdventOfCode2022.Util
{
	public sealed class File
	{
		public static string Read(string resourceName)
		{
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
	}
}


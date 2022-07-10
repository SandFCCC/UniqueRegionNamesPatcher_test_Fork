using System.Collections.Generic;
using System.Collections;
using System;

namespace UniqueRegionNamesPatcher.Extensions
{
    public static class ExceptionExtensions
    {
        /*
         * These methods are from my other project: https://github.com/radj307/volume-control
         * Both are licensed under GPLv3 and owned by me (radj307)
         */

        internal static List<DictionaryEntry>? UnpackExceptionData(IDictionary data)
        {
            if (data != null && data.Count > 0)
            {
                List<DictionaryEntry> l = new();
                foreach (DictionaryEntry kvp in data)
                    l.Add(kvp);
                return l;
            }
            return null;
        }
        /// <summary>
        /// Creates a formatted string representation of <paramref name="ex"/>.<br/>
        /// This method uses recursion to include all inner exceptions as well.
        /// </summary>
        /// <param name="ex">An <see cref="Exception"/> object.</param>
        /// <param name="linePrefix">An optional <see langword="string"/> to prepend to each line.</param>
        /// <param name="lineSuffix">An optional <see langword="string"/> to append to each line.</param>
        /// <param name="tabString">The string to use as a single tab. This is used when indenting subsequent lines.</param>
        /// <returns>A <see langword="string"/> representation of <paramref name="ex"/>.</returns>
        public static string FormatExceptionMessage(this Exception ex, string? linePrefix = null, string lineSuffix = "\n", string tabString = "  ")
        {
            string m = string.Empty;
            string tabPrefix = linePrefix + tabString;

            m += $"{{{lineSuffix}";

            // Message
            m += $"{tabPrefix}'Message': '{ex.Message}'{lineSuffix}";
            m += $"{tabPrefix}'HResult': '{ex.HResult}'{lineSuffix}";

            // Source
            if (ex.Source != null)
                m += $"{tabPrefix}'Source': '{ex.Source}'{lineSuffix}";

            // TargetSite
            if (ex.TargetSite != null)
            {
                m += $"{tabPrefix}'TargetSite': {{{lineSuffix}";
                m += $"{tabPrefix}{tabString}'Name': '{ex.TargetSite.Name}'{lineSuffix}";
                if (ex.TargetSite.DeclaringType != null)
                    m += $"{tabPrefix}{tabString}'DeclaringType': '{ex.TargetSite.DeclaringType.FullName}'{lineSuffix}";
                m += $"{tabPrefix}{tabString}'Attributes': '{ex.TargetSite.Attributes:G}'{lineSuffix}";
                m += $"{tabPrefix}{tabString}'CallingConvention': '{ex.TargetSite.CallingConvention:G}'{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // Data
            if (UnpackExceptionData(ex.Data) is List<DictionaryEntry> data)
            { // exception has data entries, include them
                m += $"{tabPrefix}'Data':{{{lineSuffix}";
                foreach ((object key, object? val) in data)
                    m += $"{tabPrefix}{tabString}'{key}': '{val}'{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // Stack Trace
            if (ex.StackTrace != null)
            {
                m += $"{tabPrefix}'StackTrace': {{{lineSuffix}";
                int i = 0;
                foreach (string s in ex.StackTrace.Split('\n'))
                    m += $"{tabPrefix}{tabString}[{i++}] {s.Trim()}{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // InnerException
            if (ex.InnerException != null)
                m += $"{tabPrefix}'InnerException': {ex.InnerException.FormatExceptionMessage(tabPrefix, lineSuffix, tabString)}{lineSuffix}";

            m += $"{linePrefix}}}";
            return m;
        }
    }
}

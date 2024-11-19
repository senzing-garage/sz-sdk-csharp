namespace Senzing.Sdk.Tests.Util;

using System.Collections.Generic;
using System.Text;

public static class TextUtilities {
    /// <summary>
    /// Parses the CSV line.
    /// </summary>
    ///
    /// <param name="line">
    /// The line to parse.
    /// </param>
    ///
    /// <param name="lineNumber">
    /// The current line number in the CSV.
    /// </param>
    public static IList<string> ParseCSVLine(string line,
                                             int?   lineNumber = null)
    {
        IList<string> fields = new List<string>();
        line = line.Trim();
        int index = 0;
        int length = line.Length;

        bool quoted   = false;
        bool escaping = false;
        StringBuilder field = new StringBuilder();
        for (index = 0; index < length; index++) {
            char c = line[index];
            if (escaping) {
                field.Append(c);
                escaping = false;
            } else if (c == '\\') {
                escaping = true;
                continue;
            } else if (!quoted && field.Length == 0 && c == '"') {
                quoted = true;
                continue;
            } else if (quoted && c == '"') {
                // check if the next character is also a double-quote
                if ((index + 1) < length && line[index+1] == '"') {
                    // this is an escaped double quote using 2 x double quotes
                    index++;
                    field.Append(c);
                    continue;
                }
                
                // advance past the separator or to the end
                for (index = index + 1; index < length; index++) {
                    c = line[index];
                    if (c == ',') break;
                    if (!Char.IsWhiteSpace(c) && (c != ',')) {
                        throw new FormatException(
                            "Badly formatted CSV at line " 
                            + ((lineNumber == null) ? "[unknown]" : lineNumber)
                            + " at position " + index + ": " + line);
                    }
                }

                quoted = false;
                fields.Add(field.ToString().Trim());
                field.Clear();

            } else if (!quoted && c == ',') {
                fields.Add(field.ToString().Trim());
                field.Clear();

            } else if (field.Length == 0 && Char.IsWhiteSpace(c)) {
                // skip leading whitespace
                continue;
            } else {
                field.Append(c);
            }
        }

        // add the last field
        if (field.Length > 0) {
            fields.Add(field.ToString().Trim());
            field.Clear();
        }

        // return the fields
        return fields;
    }
}
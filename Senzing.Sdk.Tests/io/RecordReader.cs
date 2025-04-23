namespace Senzing.Sdk.Tests.IO;

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

using static Senzing.Sdk.Tests.Util.TextUtilities;

/// <summary>
/// Represents the supported format for the records.
/// </summary>
public enum RecordFormat
{
    Json,
    JsonLines,
    CSV
}

file class RecordFormatInfo
{
    public RecordFormat Format { get; }
    public string MediaType { get; }
    public string SimpleName { get; }

    private static readonly Dictionary<RecordFormat, RecordFormatInfo> FormatLookup
        = new Dictionary<RecordFormat, RecordFormatInfo>();

    private static readonly Dictionary<string, RecordFormatInfo> MediaTypeLookup
        = new Dictionary<string, RecordFormatInfo>();

    private RecordFormatInfo(RecordFormat format, string mediaType, string simpleName)
    {
        this.Format = format;
        this.MediaType = mediaType;
        this.SimpleName = simpleName;
        FormatLookup.Add(this.Format, this);
        MediaTypeLookup.Add(this.MediaType.ToUpperInvariant(), this);
    }

    public static readonly RecordFormatInfo JsonInfo
        = new RecordFormatInfo(RecordFormat.Json, "application/json", "JSON");
    public static readonly RecordFormatInfo JsonLinesInfo
        = new RecordFormatInfo(RecordFormat.JsonLines, "application/x-jsonlines", "JSON Lines");
    public static readonly RecordFormatInfo CSVInfo
        = new RecordFormatInfo(RecordFormat.CSV, "text/csv", "CSV");

    public static RecordFormatInfo lookup(RecordFormat format)
    {
        return FormatLookup[format];
    }
    public static RecordFormatInfo lookup(string mediaType)
    {
        return MediaTypeLookup[mediaType];
    }
}

public static class FormatExtensions
{
    /// <summary>
    /// Returns the associated media type.
    ///
    /// @return The associated media type.
    /// </summary>
    public static string GetMediaType(RecordFormat format)
    {
        switch (format)
        {
            case RecordFormat.Json:
                return RecordFormatInfo.JsonInfo.MediaType;
            case RecordFormat.JsonLines:
                return RecordFormatInfo.JsonLinesInfo.MediaType;
            case RecordFormat.CSV:
                return RecordFormatInfo.CSVInfo.MediaType;
            default:
                throw new ArgumentException("Unrecognized format; " + format);
        }
    }

    /// <summary>
    /// Returns the simple name for the format.
    ///
    /// @return The simple name for the format.
    /// </summary>
    public static String GetSimpleName(RecordFormat format)
    {
        switch (format)
        {
            case RecordFormat.Json:
                return RecordFormatInfo.JsonInfo.SimpleName;
            case RecordFormat.JsonLines:
                return RecordFormatInfo.JsonLinesInfo.SimpleName;
            case RecordFormat.CSV:
                return RecordFormatInfo.CSVInfo.SimpleName;
            default:
                throw new ArgumentException("Unrecognized format; " + format);
        }
    }
}

/// <summary>
/// Provides a reader over records that are formatted as JSON,
/// JSON-Lines or CSV.
/// </summary>
public class RecordReader
{
    /// <summary>
    /// Returns the <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> for the
    /// specified media type or <c>null</c> if no format is associated
    /// with the media type.
    /// </summary>
    /// 
    /// <remarks>
    /// This method returns <c>null</c> if <c>null</c> is specified as
    /// the media type.
    /// </remarks>
    ///
    /// <param name="mediaType">
    /// The media type for which the <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/>
    /// is being requested.
    /// </param>
    /// 
    /// <returns>
    /// The associated <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> for the
    /// media type, or <c>null</c> if there is none or if the specified
    /// parameter is <c>null</c>
    /// </returns>
    public static RecordFormat? GetFormatFromMediaType(string mediaType)
    {
        if (mediaType == null) return null;
        RecordFormatInfo info = RecordFormatInfo.lookup(
            mediaType.Trim().ToUpperInvariant());
        if (info == null) return null;
        return info.Format;
    }

    /// <summary>
    /// The format for the records.
    /// </summary>
    private readonly RecordFormat format;

    /// <summary>
    /// The backing character reader.
    /// </summary>
    private readonly StreamReader reader;

    /// <summary>
    /// The mapping for the data sources.
    /// </summary>
    private readonly IDictionary<string, string> dataSourceMap;

    /// <summary>
    /// The source ID to assign to the records.
    /// </summary>
    private readonly string? sourceID;

    /// <summary>
    /// The backing <see cref="RecordProvider"/>.
    /// </summary>
    private readonly RecordProvider recordProvider;

    /// <summary>
    /// A interface for providing records.
    /// </summary>
    private interface RecordProvider
    {
        /// <summary>
        /// Gets the next record as a <see cref="System.Text.Json.Nodes.JsonObject"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The next <see cref="System.Text.Json.Nodes.JsonObject"/> record or
        /// <c>null</c> if no more remaining records.
        /// </summary>
        public JsonObject? GetNextRecord();

        /// <summary>
        /// Gets the line number of an error after calling <see cref="GetNextRecord"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This returns <c>null</c> if there was no error after calling
        /// <see cref="GetNextRecord"/> and will return <c>null</c> if
        /// <see cref="GetNextRecord"/> has never been called.
        /// </remarks>
        /// 
        /// <returns>
        /// The line number associated with the error on the last attempt to
        /// get a record, or <c>null</c> if there was no error.
        /// </returns>
        public long? GetErrorLineNumber();
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified <see cref="RecordFormat"/>,
    /// <see cref="System.IO.StreamReader"/>, data source dictionary, and source ID.
    /// </summary>
    /// 
    /// <remarks>
    /// The format is explicitly specified by the first parameter.
    /// </remarks>
    /// 
    /// <param name="format">
    /// The expected <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> or
    /// <c>null</c> if the format should be inferred.
    /// </param>
    /// 
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSourceMap">
    /// The map of original data source names to replacement data source name.
    /// The mapping from empty-string will be used for any record that has no
    /// data source or whose data source is not in the map.   The mapping from
    /// <c>"*"</c> will be for any data source (including no data source) that
    /// has no key in the map.
    /// </param>
    /// 
    /// <param name="sourceID">The source ID to assign to each record.</param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(RecordFormat? format,
                        StreamReader reader,
                        IDictionary<string, string>? dataSourceMap,
                        string? sourceID)
    {
        ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        // set the format
        this.reader = reader;
        if (this.reader == null)
        {
            throw new ArgumentException("The specified reader cannot be null");
        }

        if (format != null)
        {
            this.format = (RecordFormat)format;
        }
        else
        {
            // read characters until the format is set or we hit EOF
            while (format == null)
            {
                // peek at the next character
                int nextChar = reader.Peek();

                // check for EOF
                if (nextChar < 0)
                {
                    format = RecordFormat.JsonLines;
                    break;
                }

                // if whitespace then skip it
                if (Char.IsWhiteSpace((char)nextChar))
                {
                    // read the character if it is whitespace since we need to 
                    // read the next character on the next iteration of the loop
                    nextChar = reader.Read();
                    continue;
                }

                // switch on the character to determine the format
                switch ((char)nextChar)
                {
                    case '[':
                        format = RecordFormat.Json;
                        break;
                    case '{':
                        format = RecordFormat.JsonLines;
                        break;
                    default:
                        format = RecordFormat.CSV;
                        break;
                }
            }
            this.format = (RecordFormat)format;
        }

        // default to JSON format if EOF detected
        switch (this.format)
        {
            case RecordFormat.Json:
                this.recordProvider
                    = new JsonArrayRecordProvider(this, (StreamReader)this.reader);
                break;
            case RecordFormat.JsonLines:
                this.recordProvider
                    = new JsonLinesRecordProvider(this, (StreamReader)this.reader);
                break;
            case RecordFormat.CSV:
                this.recordProvider
                    = new CsvRecordProvider(this, (StreamReader)this.reader);
                break;
            default:
                throw new ArgumentException(
                    "Unrecognized RecordFormat; " + this.format);
        }

        // initialize the data source map with upper-case keys
        this.dataSourceMap = (dataSourceMap == null)
            ? ImmutableDictionary<string, string>.Empty
            : new Dictionary<string, string>();

        if (dataSourceMap != null)
        {
            foreach (KeyValuePair<string, string> entry in dataSourceMap)
            {
                string key = entry.Key.Trim().ToUpperInvariant();
                string value = entry.Value.Trim().ToUpperInvariant();
                this.dataSourceMap.Add(key, value);
            }

            this.dataSourceMap = this.dataSourceMap.ToFrozenDictionary<string, string>();
        }

        this.sourceID = sourceID;
        if (this.sourceID != null)
        {
            this.sourceID = this.sourceID.Trim();
            if (this.sourceID.Length == 0)
            {
                this.sourceID = null;
            }
        }
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified 
    /// <see cref="System.IO.StreamReader"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// The format of the reader is inferred using the first character read.
    /// </remarks>
    /// 
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text
    /// for the records.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">
    /// If an I/O failure occurs.
    /// </exception>
    public RecordReader(StreamReader reader)
        : this(null, reader, ImmutableDictionary<string, string>.Empty, null)
    {
        // do nothing more
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> and 
    /// <see cref="System.IO.StreamReader"/>.
    /// </summary>
    ///
    /// <param name="format">
    /// The expected <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> or
    /// <c>null</c> if the format should be inferred.
    /// </param>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text
    /// for the records.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">
    /// If an I/O failure occurs.
    /// </exception>
    public RecordReader(RecordFormat? format, StreamReader reader)
        : this(format, reader, ImmutableDictionary<string, string>.Empty, null)
    {
        // do nothing more
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="System.IO.StreamReader"/> and data source code to 
    /// assign to every record.
    /// </summary>
    /// 
    /// <remarks>
    /// The format of the reader is inferred from the first character read.
    /// </remarks>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSource">
    /// The data source to assign to each record.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(StreamReader reader, string? dataSource)
        : this(null, reader, (IDictionary<string, string>?)null, null)
    {
        if (dataSource != null)
        {
            this.dataSourceMap = new Dictionary<string, string>();
            this.dataSourceMap.Add("*", dataSource);
            this.dataSourceMap = this.dataSourceMap.ToFrozenDictionary<string, string>();
        }
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="RecordFormat"/>, <see cref="System.IO.StreamReader"/>,
    /// and data source code to assign to every record.
    /// </summary>
    /// 
    /// <param name="format">
    /// The expected <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> or
    /// <c>null</c> if the format should be inferred.
    /// </param>
    /// 
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSource">
    /// The data source to assign to each record.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(RecordFormat? format, StreamReader reader, string? dataSource)
        : this(format, reader, (IDictionary<string, string>?)null, null)
    {
        if (dataSource != null)
        {
            this.dataSourceMap = new Dictionary<string, string>();
            this.dataSourceMap.Add("*", dataSource);
            this.dataSourceMap = this.dataSourceMap.ToFrozenDictionary<string, string>();
        }
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="System.IO.StreamReader"/>, data source and source ID.
    /// </summary>
    /// 
    /// <remarks>
    /// The format of the reader is inferred from the first character.
    /// </remarks>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSource">
    /// The data source to assign to each record.
    /// </param>
    /// 
    /// <param name="sourceID">The source ID to assign to each record.</param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(StreamReader reader, string? dataSource, string sourceID)
        : this(null, reader, (IDictionary<string, string>?)null, sourceID)
    {
        if (dataSource != null)
        {
            this.dataSourceMap = new Dictionary<string, string>();
            this.dataSourceMap.Add("*", dataSource);
            this.dataSourceMap = this.dataSourceMap.ToFrozenDictionary<string, string>();
        }
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified <see cref="RecordFormat"/>,
    /// <see cref="System.IO.StreamReader"/>, data source and source ID.
    /// </summary>
    ///
    /// <param name="format">
    /// The expected <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> or
    /// <c>null</c> if the format should be inferred.
    /// </param>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSource">
    /// The data source to assign to each record.
    /// </param>
    /// 
    /// <param name="sourceID">The source ID to assign to each record.</param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(RecordFormat? format,
                        StreamReader reader,
                        string? dataSource,
                        string sourceID)
        : this(format, reader, (IDictionary<string, string>?)null, sourceID)
    {
        if (dataSource != null)
        {
            this.dataSourceMap = new Dictionary<string, string>();
            this.dataSourceMap.Add("*", dataSource);
            this.dataSourceMap = this.dataSourceMap.ToFrozenDictionary<string, string>();
        }
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="System.IO.StreamReader"/> and data source code map.
    /// </summary>
    ///
    /// <remarks>
    /// The format of the reader is inferred from the first character.
    /// </remarks>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSourceMap">
    /// The map of original data source names to replacement data source name.
    /// The mapping from empty-string will be used for any record that has no
    /// data source or whose data source is not in the map.   The mapping from
    /// <c>"*"</c> will be for any data source (including no data source) that
    /// has no key in the map.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(StreamReader reader,
                        IDictionary<string, string>? dataSourceMap)
        : this(null, reader, dataSourceMap, null)
    {
        // do nothing
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="RecordFormat"/>, <see cref="System.IO.StreamReader"/>
    /// and data source code map.
    /// </summary>
    ///
    /// <param name="format">
    /// The expected <see cref="Senzing.Sdk.Tests.IO.RecordFormat"/> or
    /// <c>null</c> if the format should be inferred.
    /// </param>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSourceMap">
    /// The map of original data source names to replacement data source name.
    /// The mapping from empty-string will be used for any record that has no
    /// data source or whose data source is not in the map.   The mapping from
    /// <c>"*"</c> will be for any data source (including no data source) that
    /// has no key in the map.
    /// </param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(RecordFormat? format,
                        StreamReader reader,
                        IDictionary<string, string>? dataSourceMap)
        : this(format, reader, dataSourceMap, null)
    {
        // do nothing else
    }

    /// <summary>
    /// Constructs a <c>RecordReader</c> with the specified
    /// <see cref="System.IO.StreamReader"/>, data source code map and source ID.
    /// </summary>
    ///
    /// <remarks>
    /// The format of the reader is inferred using the first character read.
    /// </remarks>
    ///
    /// <param name="reader">
    /// The <see cref="System.IO.StreamReader"/> from which to read the text for
    /// the records.
    /// </param>
    ///
    /// <param name="dataSourceMap">
    /// The map of original data source names to replacement data source name.
    /// The mapping from empty-string will be used for any record that has no
    /// data source or whose data source is not in the map.   The mapping from
    /// <c>"*"</c> will be for any data source (including no data source) that
    /// has no key in the map.
    /// </param>
    ///
    /// <param name="sourceID">The source ID to assign to each record.</param>
    ///
    /// <exception cref="System.IO.IOException">If an I/O failure occurs.</exception>
    public RecordReader(StreamReader reader,
                        IDictionary<string, string>? dataSourceMap,
                        string sourceID)
        : this(null, reader, dataSourceMap, sourceID)
    {
        // do nothing
    }

    /// <summary>
    /// Returns the <see cref="RecordFormat"/> of the records.
    /// </summary>
    ///
    /// <returns>
    /// The <see cref="RecordFormat"/> of the records.
    /// </returns>
    public RecordFormat Format
    {
        get
        {
            return this.format;
        }
    }

    /// <summary>
    /// Reads the next record and returns <c>null</c> if there are no more
    /// records.
    /// </summary>
    ///
    /// <records>
    /// The next record as a <see cref="System.Text.Json.Nodes.JsonObject"/> and
    /// returns <c>null</c> if there are no more records.
    /// </records>
    public JsonObject? ReadRecord()
    {
        return this.recordProvider.GetNextRecord();
    }

    /// <summary>
    /// Gets the line number of an error after calling <see cref="ReadRecord"/>.
    /// </summary>
    ///
    /// <remarks>
    /// This returns <c>null</c> if there was no error after calling
    /// <see cref="ReadRecord"/> and will return <c>null</c> if
    /// <see cref="ReadRecord"/> has never been called.
    /// </remarks>
    ///
    /// <returns>
    /// The line number associated with the error on the last attempt to
    /// get a record, or <c>null</c> if there was no error.
    /// </returns>
    public long? GetErrorLineNumber()
    {
        return this.recordProvider.GetErrorLineNumber();
    }

    /// <summary>
    /// Augments the specified record with <c>"DATA_SOURCE"</c>,
    /// <c>"ENTITY_TYPE"</c> and <c>"SOURCE_ID"</c> as appropriate.
    /// </summary>
    ///
    /// <param name="record">
    /// The <see cref="System.Text.Json.Nodes.JsonObject"/> record to be updated.
    /// </param>
    ///
    /// <returns>
    /// The updated <see cref="System.Text.Json.Nodes.JsonObject"/> record.
    /// </returns>
    private JsonObject AugmentRecord(JsonObject record)
    {
        JsonObject localRecord = record;
        JsonNode? node = localRecord["DATA_SOURCE"];
        string dsrc = (node == null) ? "" : node.GetValue<string>().ToUpperInvariant();

        // get the mapped data source
        string? dataSource = null;
        if (this.dataSourceMap.ContainsKey(dsrc))
        {
            dataSource = this.dataSourceMap[dsrc];
        }
        if (dataSource == null && this.dataSourceMap.ContainsKey("*"))
        {
            dataSource = this.dataSourceMap["*"];
        }
        if (dataSource != null && dataSource.Trim().Length == 0)
        {
            dataSource = null;
        }

        // remap the data source
        if (dataSource != null)
        {
            if (localRecord == record)
            {
                localRecord = record.DeepClone().AsObject();
            }
            localRecord["DATA_SOURCE"] = JsonValue.Create(dataSource);
            dsrc = dataSource;
        }

        // set the source ID
        if (this.sourceID != null)
        {
            if (localRecord == record)
            {
                localRecord = record.DeepClone().AsObject();
            }
            localRecord["SOURCE_ID"] = JsonValue.Create(this.sourceID);
        }

        return localRecord;
    }

    /// <summary>
    /// A <see cref="RecordProvider"/> implementation for records when
    /// reading a JSON array.
    /// </summary>
    private class JsonArrayRecordProvider : RecordProvider
    {
        /// <summary>
        /// The owning <see cref="RecordReader"/>
        /// </summary>
        private readonly RecordReader owner;

        /// <summary>
        /// Iterator over <see cref="System.Text.Json.Nodes.JsonObject"/> records.
        /// </summary>
        private readonly IEnumerator<JsonNode?> recordEnum;

        /// <summary>
        /// Indicates whether or not the JSON properly parses to avoid
        /// </summary>
        private bool errant;

        /// <summary>
        /// The line number for the last error.
        /// </summary>
        private long? errorLineNumber;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonArrayRecordProvider(RecordReader owner, StreamReader reader)
        {
            this.owner = owner;
            try
            {
                string text = reader.ReadToEnd();
                JsonNode? node = JsonNode.Parse(text, null, new JsonDocumentOptions()
                {
                    CommentHandling = JsonCommentHandling.Skip
                });
                if (node == null)
                {
                    throw new JsonException(
                            "Failed to parse text as JSON: " + text);
                }

                JsonArray jsonArr = ((JsonNode)node).AsArray();
                this.recordEnum = jsonArr.GetEnumerator();

            }
            catch (Exception e)
            {
                this.recordEnum = (new List<JsonNode?>()).GetEnumerator();
                if (e is JsonException)
                {
                    JsonException je = (JsonException)e;
                    Console.Error.WriteLine("MESSAGE: " + je.Message);
                    Console.Error.WriteLine("LINE NUMBER: " + je.LineNumber);
                    this.errorLineNumber = je.LineNumber;
                }
                this.errant = true;
            }
        }

        /// <summary>
        /// Gets the next record from the JSON array.
        /// </summary>
        ///
        /// <returns>
        /// The next <see cref="System.Text.Json.Nodes.JsonObject"/> from the array.
        /// </returns>
        public JsonObject? GetNextRecord()
        {
            JsonObject? result = null;
            while (result == null)
            {
                try
                {
                    if (!recordEnum.MoveNext()) break;
                    result = this.recordEnum.Current?.AsObject();
                    if (result != null)
                    {
                        result = owner.AugmentRecord(result);
                    }
                    this.errant = false; // clear the errant flag

                }
                catch (Exception e)
                {
                    if (this.errant) continue;
                    if (e is JsonException)
                    {
                        JsonException je = (JsonException)e;
                        Console.Error.WriteLine("MESSAGE: " + je.Message);
                        Console.Error.WriteLine("LINE NUMBER: " + je.LineNumber);
                        this.errorLineNumber = je.LineNumber;
                    }
                    this.errant = true;
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Overridden to return the errant line number or <c>null</c>.
        /// </summary>
        public long? GetErrorLineNumber()
        {
            return this.errorLineNumber;
        }
    }

    /// <summary>
    /// A <see cref="RecordProvider"/> implementation for records when reading
    /// a files in a "JSON lines" format.
    /// </summary>
    private class JsonLinesRecordProvider : RecordProvider
    {
        /// <summary>
        /// The owning <see cref="RecordReader"/>
        /// </summary>
        private readonly RecordReader owner;

        /// <summary>
        /// The backing <see cref="System.IO.StreamReader"/> for reading the
        /// lines from the file.
        /// </summary>
        private StreamReader? reader;

        /// <summary>
        /// The current line number.
        /// </summary>
        private long lineNumber;

        /// <summary>
        /// The error line number if an error is found.
        /// </summary>
        private long? errorLineNumber;

        /// <summary>
        /// Constructs with the specified parameters.
        /// </summary>
        public JsonLinesRecordProvider(RecordReader owner, StreamReader reader)
        {
            this.owner = owner;
            this.reader = reader;
        }

        /// <summary>
        /// Implemented to get the next line from the file and parse it as
        /// a <see cref="System.Text.Json.Nodes.JsonObject"/> record.
        /// </summary>
        ///
        /// <returns>
        /// The next <see cref="System.Text.Json.Nodes.JsonObject"/> record.
        /// </returns>
        public JsonObject? GetNextRecord()
        {
            JsonObject? record = null;

            while (this.reader != null && record == null)
            {
                // read the next line and check for EOF
                string? line = this.reader.ReadLine();
                if (line == null)
                {
                    this.reader.Close();
                    this.reader = null;
                    continue;
                }
                this.lineNumber++;
                this.errorLineNumber = null; // clear the error line number

                // trim the line of extra whitespace
                line = line.Trim();

                // check for blank lines and skip them
                if (line.Length == 0) continue;

                // check if the line begins with a "#" for a comment lines
                if (line.StartsWith('#')) continue;

                // check if the line does NOT start with "{"
                if (!line.StartsWith('{'))
                {
                    throw new JsonException(
                        "Line does not appear to be JSON record: " + line);
                }

                // parse the line
                try
                {
                    record = JsonNode.Parse(line)?.AsObject();

                }
                catch (JsonException)
                {
                    this.errorLineNumber = this.lineNumber;
                    throw;
                }
            }

            return (record != null) ? owner.AugmentRecord(record) : null;
        }

        /// <summary>
        /// Overridden to return the errant line number or <c>null</c>.
        /// </summary>
        public long? GetErrorLineNumber()
        {
            return this.errorLineNumber;
        }
    }

    /// <summary>
    /// Implements <see cref="RecordProvider"/> for a CSV file.
    /// </summary>
    private class CsvRecordProvider : RecordProvider
    {
        /// <summary>
        /// The owning <see cref="RecordReader"/>
        /// </summary>
        private readonly RecordReader owner;

        /// <summary>
        /// The underlying <see cref="System.IO.StreamReader"/>
        /// </summary>
        private readonly StreamReader reader;

        /// <summary>
        /// The list of header names.
        /// </summary>
        private readonly IList<string> headers;

        /// <summary>
        /// The current line number.
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// The line number for the last error.
        /// </summary>
        private long? errorLineNumber;

        public CsvRecordProvider(RecordReader owner, StreamReader reader)
        {
            this.owner = owner;
            this.reader = reader;
            this.headers = new List<string>();
            this.lineNumber = 0;
            char[] splitChars = [','];
            string? headerLine = null;

            while (headerLine == null)
            {
                string? line = reader.ReadLine();
                if (line == null) break;
                this.lineNumber++;
                line = line.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith('#')) continue;
                headerLine = line;
            }

            if (headerLine == null)
            {
                this.errorLineNumber = this.lineNumber;
                throw new FormatException("Could not find CSV header in file");
            }

            try
            {
                IList<string> tokens = ParseCSVLine(headerLine, this.lineNumber);
                foreach (string token in tokens)
                {
                    this.headers.Add(token.Trim().ToUpperInvariant());
                }
            }
            catch (Exception)
            {
                this.errorLineNumber = this.lineNumber;
                throw;
            }
        }

        public JsonObject? GetNextRecord()
        {
            this.errorLineNumber = null;
            try
            {
                string? nextLine = null;

                while (nextLine == null)
                {
                    string? line = reader.ReadLine();
                    if (line == null)
                    {
                        return null;
                    };
                    this.lineNumber++;
                    line = line.Trim();
                    if (line.Length == 0) continue;
                    if (line.StartsWith('#')) continue;
                    nextLine = line;
                }

                IList<string> fields = ParseCSVLine(nextLine, this.lineNumber);

                IEnumerator<string> headerEnum = this.headers.GetEnumerator();
                IEnumerator<string> fieldEnum = fields.GetEnumerator();

                JsonObject jsonObject = new JsonObject();
                while (headerEnum.MoveNext() && fieldEnum.MoveNext())
                {
                    string header = headerEnum.Current;
                    string field = fieldEnum.Current;

                    JsonValue value = JsonValue.Create(field);
                    jsonObject.Add(header, value);
                }

                jsonObject = owner.AugmentRecord(jsonObject);

                return jsonObject;

            }
            catch (Exception)
            {
                this.errorLineNumber = this.lineNumber;
                throw;
            }
        }

        /// <summary>
        /// Overridden to return the errant line number or <c>null</c>.
        /// </summary>
        public long? GetErrorLineNumber()
        {
            return this.errorLineNumber;
        }
    }
}
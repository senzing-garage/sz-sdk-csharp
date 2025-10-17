using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Senzing.Sdk.Core
{
    /// <summary>
    /// Provides utility functions that are useful when implementing
    /// the Senzing SDK interfaces from the <c>Senzing.Sdk</c> namespace.
    /// This is functionality leveraged by the "core" implementation
    /// that is made visible to aid in other implementations.
    /// </summary>
    /// 
    /// <remarks>
    /// None of the functionality provided here is required to use the 
    /// <see cref="Senzing.Sdk.Core.SzCoreEnvironment"/> class, but may
    /// be useful if creating an alternate implementation of
    /// <see cref="Senzing.Sdk.SzEnvironment"/>.
    /// </remarks>
    public static class SzCoreUtilities
    {
        /// <summary>
        /// The result from the <see cref="ToString()"/> function if the
        /// environment is already destroyed.
        /// </summary>
        internal const string DestroyedMessage = "*** DESTROYED ***";

        /// <summary>
        /// The prefix to use if an <see cref="SzException"/> is thrown
        /// from <see cref="Export()"/> and <see cref="ToString()"/>
        /// was called.
        /// </summary>
        internal const string FailurePrefix = "*** FAILURE: ";
        
        /// <summary>
        /// The <b>unmodifiable</b> collection of default data sources.
        /// </summary>
        private static readonly ReadOnlyCollection<string> DefaultSources
            = new ReadOnlyCollection<string>(
                new List<string>(new[] { "TEST", "SEARCH" }));

        /// <summary>
        /// The <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/>
        /// of integer error code keys to <c>Type</c> values representing the exception class
        /// associated with the respective error code.
        /// </summary>
        /// 
        /// <remarks>
        /// The dictionary does not store entries that map to <see cref="SzException"/>
        /// since that is the default for any error code not otherwise mapped.
        /// </remarks>
        internal static readonly ReadOnlyDictionary<long, Type> ExceptionMap;

        /// <summary>
        /// The class initializer.
        /// </summary>
        static SzCoreUtilities()
        {
            IDictionary<long, Type> map = new Dictionary<long, Type>();
            IDictionary<long, Type> result = new Dictionary<long, Type>();
            SzExceptionMapper.RegisterExceptions(map);
            Type baseType = typeof(SzException);
            foreach (KeyValuePair<long, Type> mapping in map)
            {
                if (!baseType.Equals(mapping.Value))
                {
                    result.Add(mapping.Key, mapping.Value);
                }
            }
            ExceptionMap = new ReadOnlyDictionary<long, Type>(result);
        }

        /// <summary>
        /// Creates the appropriate <see cref="SzException"/> instance for the
        /// specified error code. 
        /// </summary>
        /// 
        /// <remarks>
        /// If there is a failure in creating the <see cref="SzException"/> 
        /// instance, then a generic <see cref="SzException"/> is created with 
        /// the specified parameters and "caused by" exception describing the
        /// failure.  
        /// </remarks>
        /// 
        /// <param name="errorCode">
        /// The error code to use to determine the specific type for the
        /// <see cref="SzException"/> instance.
        /// </param>
        /// 
        /// <param name="message">
        /// The error message to associate with the exception.
        /// </param>
        /// 
        /// <returns></returns>
        public static SzException CreateSzException(long errorCode, string message)
        {
            // get the exception class
            Type exceptionType = ExceptionMap.ContainsKey(errorCode)
                ? ExceptionMap[errorCode] : typeof(SzException);

            try
            {
                return (SzException)Activator.CreateInstance(exceptionType,
                                                             errorCode,
                                                             message);
            }
            catch (Exception e)
            {
                return new SzException(errorCode, message, e);
            }
        }

        /// <summary>
        /// Produce an auto-generated configuration comment for the
        /// configuration manager registry.  This is useful when implementing
        /// the <see cref="Senzing.Sdk.SzConfigManager.RegisterConfig(string)"/>
        /// function.
        /// </summary>
        /// 
        /// <param name="configDefinition">
        /// The <c>string</c> configuration definition.
        /// </param>
        ///
        /// <returns>
        /// The auto-generated comment, which may be empty-string
        /// if an auto-generated comment could not otherwise be inferred.
        /// </returns>
        public static string CreateConfigComment(string configDefinition)
        {
            int index = configDefinition.IndexOf("\"CFG_DSRC\"");
            if (index < 0)
            {
                return "";
            }
            char[] charArray = configDefinition.ToCharArray();

            // advance past any whitespace
            index = EatWhiteSpace(charArray, index + "\"CFG_DSRC\"".Length);

            // check for the colon
            if (index >= charArray.Length || charArray[index++] != ':')
            {
                return "";
            }

            // advance past any whitespace
            index = EatWhiteSpace(charArray, index);

            // check for the open bracket
            if (index >= charArray.Length || charArray[index++] != '[')
            {
                return "";
            }

            // find the end index
            int endIndex = configDefinition.IndexOf("]", index);
            if (endIndex < 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Data Sources: ");
            string prefix = "";
            int dataSourceCount = 0;
            ISet<string> defaultSources = new SortedSet<string>();
            while (index > 0 && index < endIndex)
            {
                index = configDefinition.IndexOf("\"DSRC_CODE\"", index);
                if (index < 0 || index >= endIndex)
                {
                    continue;
                }
                index = EatWhiteSpace(charArray, index + "\"DSRC_CODE\"".Length);

                // check for the colon
                if (index >= endIndex || charArray[index++] != ':')
                {
                    return "";
                }

                index = EatWhiteSpace(charArray, index);

                // check for the quote
                if (index >= endIndex || charArray[index++] != '"')
                {
                    return "";
                }
                int start = index;

                // find the ending quote
                while (index < endIndex && charArray[index] != '"')
                {
                    index++;
                }
                if (index >= endIndex)
                {
                    return "";
                }

                // get the data source code
                string dataSource = configDefinition.Substring(start, (index - start));
                if (DefaultSources.Contains(dataSource))
                {
                    defaultSources.Add(dataSource);
                    continue;
                }
                sb.Append(prefix);
                sb.Append(dataSource);
                dataSourceCount++;
                prefix = ", ";
            }

            // check if only the default data sources
            if (dataSourceCount == 0 && defaultSources.Count == 0)
            {
                sb.Append("[ NONE ]");
            }
            else if (dataSourceCount == 0
                    && defaultSources.Count == DefaultSources.Count)
            {
                sb.Append("[ ONLY DEFAULT ]");

            }
            else if (dataSourceCount == 0)
            {

                sb.Append("[ SOME DEFAULT (");
                prefix = "";
                foreach (String source in defaultSources)
                {
                    sb.Append(prefix);
                    sb.Append(source);
                    prefix = ", ";
                }
                sb.Append(") ]");
            }

            // return the constructed string
            return sb.ToString();
        }

        /// <summary>
        /// Finds the index of the first non-whitespace character after the
        /// specified index from the specified character array.
        /// </summary>
        ///
        /// <param name="charArray">The character array.</param>
        /// <param name="fromIndex">The starting index.</param>
        ///
        /// <returns>
        /// The index of the first non-whitespace character or the length of
        /// the character array if no non-whitespace character is found.
        /// </returns>
        internal static int EatWhiteSpace(char[] charArray, int fromIndex)
        {
            int index = fromIndex;

            // advance past any whitespace
            while (index < charArray.Length && Char.IsWhiteSpace(charArray[index]))
            {
                index++;
            }

            // return the index
            return index;
        }

        public static String ConfigToString(SzConfig config)
        {
            try
            {
                return config.Export();
            }
            catch (SzEnvironmentDestroyedException)
            {
                return DestroyedMessage;
            }
            catch (Exception e)
            {
                return FailurePrefix + e.Message;
            }
            
        }
    }
}

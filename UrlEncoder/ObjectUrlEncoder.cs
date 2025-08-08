using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace UrlEncoder;

public class ObjectUrlEncoder
{
    /// <summary>
    /// Serializes complex native objects, nested objects, collections, and nested collections into a URL-encoded query
    /// string. Useful for GET requests or POST requests requiring x-www-form-urlencoded data. Nested object
    /// keys and array index keys are serialized to bracket-style array access.  For example, this object structure:
    /// {
    ///     name: "Bob",
    ///     addresses: [
    ///         { street: "1234 Elm", city: "New York" },
    ///         { street: "5678 Oak", city: "Los Angeles" }
    ///     ],
    ///     profile: {
    ///         hobby: "bungee jumping"
    ///         preferences: null
    ///     }
    /// }
    ///     
    /// will serialize to: 
    /// name=Bob&addresses[0][street]=1234+Elm&addresses[0][city]=New+York&addresses[1][street]=5678+Oak&addresses[1][city]=Los+Angeles&profile[hobby]=bungee+jumping
    /// </summary>
    /// <param name="obj">Data object to be serialized and x-www-form-urlencoded</param>
    /// <returns>Returns</returns>
    public static string SerializeObjectToUrlEncodedString(object? obj)
    {
        if (obj == null) { return ""; }
        var jsonString = SerializeObjectToJsonString(obj);
        return SerializeJsonToUrlEncodedString(jsonString);
    }

    /// <summary>
    /// Converts an object to a JSON-encoded string.  Uses the StringEnumConverter class to ensure enums get
    /// encoded with their text value and not as an integer.
    /// </summary>
    /// <param name="obj">Data object to be serialized</param>
    /// <returns>String containing JSON-encoded object</returns>
    public static string SerializeObjectToJsonString(object obj)
    {
        return JsonConvert.SerializeObject(obj, new StringEnumConverter());
    }

    /// <summary>
    /// Converts a string containing JSON with objects, nested objects, arrays, and nested arrays into a URL-encoded query
    /// string. Useful for GET requests or POST requests requiring x-www-form-urlencoded data. Nested object
    /// keys and array index keys are serialized to bracket-style array access.  For examples, see the documentation on the
    /// SerializeObjectToUrlEncodedString method.
    /// </summary>
    /// 
    /// <param name="jsonString">String of valid JSON to be converted to x-www-form-urlencoded</param>
    /// <returns>Returns</returns>
    public static string SerializeJsonToUrlEncodedString(string jsonString)
    {
        var jToken = (JToken) JsonConvert.DeserializeObject(jsonString);
        return SerializeJToken(jToken);
    }

    /// <summary>
    /// Used to build up the URL-encoded key to a nested path using bracket notation during the recursion process.
    /// </summary>
    /// <param name="prefix">Nested key so far</param>
    /// <param name="nextKey">Next key to add to the nested levels on the prefix.</param>
    /// <returns>Returns modified prefix with the next key added.</returns>
    private static string AddKeyToPrefix(string prefix, string nextKey)
    {
        var encodedKey = HttpUtility.UrlEncode(nextKey);
        return String.IsNullOrEmpty(prefix) ? encodedKey : prefix + "[" + encodedKey + "]";
    }

    /// <summary>
    /// Main recursive parsing function during URL encoding. Determines the object type currently being
    /// parsed and routes to the correct function for encoding that particular JSON data type.  This will
    /// typically be the root call to start parsing an object, array, or primitive.
    /// </summary>
    /// <param name="jToken">Current object being URL encoded</param>
    /// <param name="prefix">Optional key prefix for bracket-style nested path names used during recursion.</param>
    /// <returns>URL-Encoded string representation of the token</returns>
    private static string SerializeJToken(JToken jToken, string prefix = "")
    {
        switch (jToken.Type)
        {
            case JTokenType.Object:
                return SerializeJTokenObject(jToken, prefix);
            case JTokenType.Array:
                return SerializeJTokenArray(jToken, prefix);
            case JTokenType.Property:
                return SerializeJProperty((JProperty)jToken, prefix);
            case JTokenType.Null:
                return "";
            default:
                return String.IsNullOrEmpty(prefix) ? "" : prefix + "=" + HttpUtility.UrlEncode(jToken.ToString());
        }
    }

    /// <summary>
    /// URL Encode an object property (key/value pair), recursively parsing the value if it has child objects.
    /// </summary>
    /// <param name="jProperty">Property to URL-encode</param>
    /// <param name="prefix">Optional prefix for the key containing the recursive path so far</param>
    /// <returns>Encoded string representation of the token</returns>
    private static string SerializeJProperty(JProperty jProperty, string prefix = "")
    {
        return SerializeJToken(jProperty.Value, AddKeyToPrefix(prefix, jProperty.Name));
    }

    /// <summary>
    /// URL Encode an object by encoding each of its properties and concatening the encoded results.
    /// </summary>
    /// <param name="jToken">Object to URL-encode</param>
    /// <param name="prefix">Optional prefix for the key containing the recursive path so far</param>
    /// <returns>Encoded string representation of the token</returns>
    private static string SerializeJTokenObject(JToken jToken, string prefix = "")
    {
        return MergeEncodedItems(jToken.Children()
            .Select<JToken, string>(jTokenChild => {
                return SerializeJToken(jTokenChild, prefix);
            })
        );
    }

    /// <summary>
    /// URL Encode an array by encoding each of its elements and concatening the encoded results.
    /// </summary>
    /// <param name="jToken">Array to URL-encode</param>
    /// <param name="prefix">Optional prefix for the key containing the recursive path so far</param>
    /// <returns>Encoded string representation of the token</returns>
    private static string SerializeJTokenArray(JToken jToken, string prefix = "")
    {
        int index = 0;
        return MergeEncodedItems(jToken.Children()
            .Select<JToken, string>(jTokenChild => {
                return SerializeJToken(jTokenChild, AddKeyToPrefix(prefix, index++.ToString()));
            })
        );
    }

    /// <summary>
    /// Merge a string list of URL-encoded key/value pairs into a single string, with each
    /// pair separated by an & to follow x-www-form-urlencoded conventions.
    /// </summary>
    /// <param name="encodedItems">String list of URL-encoded key-value data items.</param>
    /// <returns>Single string with the concatenated list of encoded key/value pairs.</returns>
    private static string MergeEncodedItems(IEnumerable<string> encodedItems)
    {
        return String.Join("&", encodedItems.Where(str => !string.IsNullOrEmpty(str))) ?? "";
    }

}

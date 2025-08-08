# .NET Object URL Encoder

Currently, .NET lacks a single function for encoding complex objects to a URL-encoded query string. This library
provides methods for passing in an Object of any type and utilizing JSON encoding attributes to produce
the equivaline URL-encoded string:

For example, the following .NET object (represented here by JSON for readability):
```json
{
    "name": "Bob",
    "addresses": [
        { "street": "1234 Elm", "city": "New York" },
        { "street": "5678 Oak", "city": "Los Angeles" }
    ],
    "profile": {
        "hobby": "bungee jumping",
        "preferences": null
    }
}
```
will be converted to the follow URL-encoded string:
```
name=Bob&addresses[0][street]=1234+Elm&addresses[0][city]=New+York&addresses[1][street]=5678+Oak&addresses[1][city]=Los+Angeles&profile[hobby]=bungee+jumping
```

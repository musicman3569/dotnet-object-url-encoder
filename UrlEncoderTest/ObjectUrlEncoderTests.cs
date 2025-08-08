using UrlEncoder;

namespace UrlEncoderTest;

[TestClass]
public sealed class ObjectUrlEncoderTests
{
    [TestMethod]
    public void TestNestedArrays()
    {
        var jsonString = "{\"my_key\":[[\"foo\"],[\"bar\"]]}";
        var urlString = ObjectUrlEncoder.SerializeJsonToUrlEncodedString(jsonString);
        Assert.IsTrue(urlString == "my_key[0][0]=foo&my_key[1][0]=bar");
    }

    [TestMethod]
    public void TestSingleArray()
    {
        var jsonString = "{\"my key\":[\"fo o\",\"bar\"]}";
        var urlString = ObjectUrlEncoder.SerializeJsonToUrlEncodedString(jsonString);
        Assert.IsTrue(urlString == "my+key[0]=fo+o&my+key[1]=bar");
    }

    [TestMethod]
    public void TestIgnoresNulls()
    {
        var jsonString = "{\"SetKey\":\"Foo\",\"EmptyKey\":null,\"AnotherSetKey\":\"AnotherFoo\",\"AnotherEmptyKey\":null}";
        var urlString = ObjectUrlEncoder.SerializeJsonToUrlEncodedString(jsonString);
        Assert.IsTrue(urlString == "SetKey=Foo&AnotherSetKey=AnotherFoo");
    }

    [TestMethod]
    public void TestArrayOfObjectsWithNestedNull()
    {
        var jsonString = "{\"root\":{\"array\":[{\"objectA\": \"A\"},{\"objectB\":\"B\"},{\"objectC\":null}]},\"sibling\":\"S\"}";
        var urlString = ObjectUrlEncoder.SerializeJsonToUrlEncodedString(jsonString);
        Assert.IsTrue(urlString == "root[array][0][objectA]=A&root[array][1][objectB]=B&sibling=S");
    }

    [TestMethod]
    public void TestSpecialCharsAndMixedArray()
    {
        var jsonString = "{\"name\":\"Bob\",\"addresses\":[{\"street\":\"1234 Elm\",\"city\":\"New York\"},{\"street\":\"5678 Oak\",\"city\":\"Los Angeles\"},79,\"!@#$%^&*()\"],\"profile\":{\"hobby\":\"bungee jumping\",\"preferences\":null}}";
        var urlString = ObjectUrlEncoder.SerializeJsonToUrlEncodedString(jsonString);
        Assert.IsTrue(urlString == "name=Bob&addresses[0][street]=1234+Elm&addresses[0][city]=New+York&addresses[1][street]=5678+Oak&addresses[1][city]=Los+Angeles&addresses[2]=79&addresses[3]=!%40%23%24%25%5e%26*()&profile[hobby]=bungee+jumping");
    }
}
# Json Parsing Benchmark

Parsing JSON benchmark in a scenario where no content from the input JSON is lost.

## Possibilites

The possibilities are listed below. The example output happens when parsing the following json content:

```c#
const string json = @"{""extraProp"": ""hello"", ""extraObj"":{ ""foo"": ""bar"" }, ""rxpk"":[{""tmst"":44239419,""chan"":2,""rfch"":1,""freq"":868.500000,""stat"":1,""modu"":""LORA"",""datr"":""SF7BW125"",""codr"":""4/5"",""lsnr"":6.8,""rssi"":-43,""size"":15,""data"":""gFVVVVUAMwAIMtJdEIT/""}]}";
```

### Typed object with JsonExtensionData attribute

Define an object contract with the known payload properties. Create a `Dictionary<string, object>` property with attribute `JsonExtensionData`. Accessing known properties has compile time checking. Checking unknown properties happens through the dictionary property.

```c#
class TypedJson
{

    [JsonExtensionData]
    public Dictionary<string, object> ExtraData { get; } = new Dictionary<string, object>();

    public List<Rxpk> rxpk { get; set; }
}
```

Runnining result:

```c#
var typedJson = JsonConvert.DeserializeObject<TypedJson>(json);
Console.WriteLine(typedJson.ExtraData["extraProp"]);
Console.WriteLine(typedJson.ExtraData["extraObj"]);
```

**Outputs:**
```text
hello
{
  "foo": "bar"
}
Typed: {"rxpk":[{"tmst":44239419,"chan":2,"rfch":1,"freq":868.5,"stat":1,"modu":"LORA","datr":"SF7BW125","codr":"4/5","lsnr":6.8,"rssi":-43,"size":15,"data":"gFVVVVUAMwAIMtJdEIT/"}],"e
xtraProp":"hello","extraObj":{"foo":"bar"} }
```

**Code Generated in benchmark**

```c#
public static void ParseTypedObject()
{
    for (int index = 0; index < 100000; ++index)
    {
        M0 m0 = JsonConvert.DeserializeObject<TypedJson>("{\"extraProp\": \"hello\", \"extraObj\":{ \"foo\": \"bar\" }, \"rxpk\":[{\"tmst\":44239419,\"chan\":2,\"rfch\":1,\"freq\":868.500000,\"stat\":1,\"modu\":\"LORA\",\"datr\":\"SF7BW125\",\"codr\":\"4/5\",\"lsnr\":6.8,\"rssi\":-43,\"size\":15,\"data\":\"gFVVVVUAMwAIMtJdEIT/\"}]}");
        if (m0 == null)
            throw new Exception();
        if (((TypedJson) m0).ExtraData["extraProp"] == null || ((TypedJson) m0).ExtraData["extraObj"] == null)
            throw new Exception();
    }
}
```

### JObject

Deserialize to [Newtonsoft.Json.Linq.JObject](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JObject.htm). Accessing properties is through a dictionary like property of the instance.

```c#
var objJson = (JObject)JsonConvert.DeserializeObject(json);
Console.WriteLine(objJson["extraProp"]);
Console.WriteLine(objJson["extraObj"]);
Console.WriteLine($"JObject: {JsonConvert.SerializeObject(objJson)}");
```

**Outputs:**
```text
hello
{
  "foo": "bar"
}
JObject: {"extraProp":"hello","extraObj":{"foo":"bar"},"rxpk":[{"tmst":44239419,"chan":2,"rfch":1,"freq":868.5,"stat":1,"modu":"LORA","datr":"SF7BW125","codr":"4/5","lsnr":6.8,"rssi":-
43,"size":15,"data":"gFVVVVUAMwAIMtJdEIT/"}]}
```

**Code Generated in benchmark**

```c#
public static void ParseJObject()
{
    for (int index = 0; index < 100000; ++index)
    {
        JObject jobject = (JObject) JsonConvert.DeserializeObject("{\"extraProp\": \"hello\", \"extraObj\":{ \"foo\": \"bar\" }, \"rxpk\":[{\"tmst\":44239419,\"chan\":2,\"rfch\":1,\"freq\":868.500000,\"stat\":1,\"modu\":\"LORA\",\"datr\":\"SF7BW125\",\"codr\":\"4/5\",\"lsnr\":6.8,\"rssi\":-43,\"size\":15,\"data\":\"gFVVVVUAMwAIMtJdEIT/\"}]}");
        if (jobject == null)
            throw new Exception();
        if (jobject.get_Item("extraProp") == null || jobject.get_Item("extraObj") == null)
            throw new Exception();
    }
}
```

### Dynamic object

Creates the dynamic object where access to the properties happens at runtime using reflect underneath the hoods. 

```c#
dynamic dynamicJson = JsonConvert.DeserializeObject(json);
Console.WriteLine(dynamicJson["extraProp"]);
Console.WriteLine(dynamicJson["extraObj"]);
Console.WriteLine($"Dynamic: {JsonConvert.SerializeObject(typedJson)}");
```

**Outputs:**
```text
hello
{
  "foo": "bar"
}
Dynamic: {"rxpk":[{"tmst":44239419,"chan":2,"rfch":1,"freq":868.5,"stat":1,"modu":"LORA","datr":"SF7BW125","codr":"4/5","lsnr":6.8,"rssi":-43,"size":15,"data":"gFVVVVUAMwAIMtJdEIT/"}],
"extraProp":"hello","extraObj":{"foo":"bar"}}
```

**Code generated in benchmark**

```c#
public static void ParseDynamicObject()
{
    for (int index = 0; index < 100000; ++index)
    {
        object obj1 = JsonConvert.DeserializeObject("{\"extraProp\": \"hello\", \"extraObj\":{ \"foo\": \"bar\" }, \"rxpk\":[{\"tmst\":44239419,\"chan\":2,\"rfch\":1,\"freq\":868.500000,\"stat\":1,\"modu\":\"LORA\",\"datr\":\"SF7BW125\",\"codr\":\"4/5\",\"lsnr\":6.8,\"rssi\":-43,\"size\":15,\"data\":\"gFVVVVUAMwAIMtJdEIT/\"}]}");
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p1 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__0, obj1, (object) null);
        if (target1((CallSite) p1, obj2))
            throw new Exception();
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "extraProp", typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__2.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__2, obj1);
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "extraObj", typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__3.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__3, obj1);
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__4.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__4, obj3, (object) null);
        // ISSUE: reference to a compiler-generated field
        if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__8 == null)
        {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        if (!JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__8.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__8, obj5))
        {
            // ISSUE: reference to a compiler-generated field
            if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__7 == null)
            {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, bool> target2 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__7.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, bool>> p7 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__7;
            // ISSUE: reference to a compiler-generated field
            if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__6 == null)
            {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object, object> target3 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__6.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object, object>> p6 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__6;
            object obj6 = obj5;
            // ISSUE: reference to a compiler-generated field
            if (JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__5 == null)
            {
            // ISSUE: reference to a compiler-generated field
            JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (JsonParsingBenchmarks), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj7 = JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__5.Target((CallSite) JsonParsingBenchmarks.\u003C\u003Eo__4.\u003C\u003Ep__5, obj4, (object) null);
            object obj8 = target3((CallSite) p6, obj6, obj7);
            if (!target2((CallSite) p7, obj8))
            continue;
        }
        throw new Exception();
    }
}
```


## Benchmarking Result

|Method|Mean|Error|StdDev|Gen 0/1k Op|Gen 1/1k Op|Gen 2/1k Op|Allocated Memory/Op|
|-|-|-|-|-|-|-|-|
|ParseTypedObject|831.1 ms|12.65 ms|11.21 ms|112000.0000|||448.61 MB|
|ParseJObject|1,169.8 ms| 23.00 ms|24.61 ms|237000.0000|||949.1 MB|
|ParseDynamicObject|1,176.3 ms|23.41 ms|35.04 ms|239000.0000|||958.25 MB|

using System.Collections;
using Horizon.XmlRpc.Client;
using Horizon.XmlRpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NUnit.Framework;
using RobotCore.Library.Core;
using RobotCore.Standalone.TestsLibrary;
using RobotCore.XmlRpcService;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace RobotCore.Standalone.Tests
{
    public class XmlRpcTests
    {
        #region TestFixture

        private const string TestAssembly = "RobotCore.Standalone.TestsLibrary";
        private static IHost _host;
        private HttpService _service;
        private readonly Dictionary<int, string> _testLibraries = new()
        {
            {1, "TestKeywords"},
            {2, "RunKeyword"},
            {3, "WithDocumentation"},
            {4, "Static"},
            {5, "Internal"}
        };

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // 1) Create your custom NLog configuration for the tests
            var nlogConfig = new LoggingConfiguration();

            // Add the same targets/rules you had in your test class
            var consoleTarget = new ConsoleTarget("consoleLog");
            var traceTarget = new TraceTarget("traceLog");

            nlogConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, consoleTarget);
            nlogConfig.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, traceTarget);

            // 2) Build the Host (similar to Program.Main)
            //    but use the test-specific NLog config
            _host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddNLog(nlogConfig); // Use your in-memory config
                })
                .ConfigureServices(services =>
                {
                    // Register RobotRemoteLibrary
                    services.AddSingleton<RobotRemoteLibrary>();
                    // Register all services
                    services.AddSingleton<HttpService>();
                    services.AddSingleton<IXmlRpcRequestHandler>(sp => sp.GetRequiredService<RobotRemoteLibrary>());
                    services.AddSingleton<IRobotFrameworkRemoteApi>(sp => sp.GetRequiredService<RobotRemoteLibrary>());
                    services.AddSingleton<IKeywordManager, KeywordManager>();
                })
                .Build();
            
            // 3) Add libraries
            var remoteLibrary = _host.Services.GetRequiredService<IKeywordManager>();
            remoteLibrary.AddLibrary(new TestKeywords());
            remoteLibrary.AddLibrary(new RunKeyword());
            remoteLibrary.AddLibrary(new WithDocumentation());
            remoteLibrary.AddLibrary(new Static());
            remoteLibrary.AddLibrary(new Internal());

            // 4) Retrieve your service and start it
            _service = _host.Services.GetRequiredService<HttpService>();
            _service.Start();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // Stop your service, dispose of the Host
            _service?.Stop();
            _host?.Dispose();
        }

        #endregion

        #region get_keyword_names

        [Test]
        public void get_keyword_names()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/" + _testLibraries[1].Replace('.', '/');
            var result = client.get_keyword_names();
            
            // Modern NUnit constraint-style assert:
            Assert.That(result.Length, Is.GreaterThan(0), "Expected a non-empty list of keyword names.");
        }

        [Test]
        public void get_keyword_names_invalid_url()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/UnknownType";
            
            // Assert that an exception is thrown
            Assert.Throws<XmlRpcFaultException>(() => client.get_keyword_names());
        }

        #endregion

        #region get_keyword_arguments

        [Test]
        public void get_keyword_arguments()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.get_keyword_arguments("STRING PARAMETERTYPE");

            Assert.That(result.Length, Is.GreaterThan(0), "Verify that the result is not empty");
            Assert.That(result[0], Is.EqualTo("value"), "Verify that the result contains parameter name");
        }

        #endregion

        #region get_keyword_documentation

        [Test]
        public void get_keyword_documentation()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/WithDocumentation";
            var result = client.get_keyword_documentation("MethodWithComments");

            Assert.That(result, Is.Not.Null.And.Not.Empty, "Expected some documentation text");
            Assert.That(result, Is.EqualTo("This is a method with a comment"));
        }

        #endregion

        #region run_keyword

        [Test]
        public void RunKeyword_NoArgs_VoidReturn_EmptyArgs()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/RunKeyword";
            var result = client.run_keyword("NoInputNoOutput", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["error"], Is.Empty);
            Assert.That(result["return"], Is.Empty);
        }

        [Test]
        public void RunKeyword_ThrowsException()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/RunKeyword";
            var result = client.run_keyword("ThrowsException", new object[0]);

            Assert.That(result["status"], Is.EqualTo("FAIL"));
            Assert.That(result["return"], Is.Empty);
            Assert.That(result, Does.Not.ContainKey("fatal"));
            Assert.That(result, Does.Not.ContainKey("continuable"));
            Assert.That(result["error"], Is.EqualTo("A regular exception"));
            Assert.That(result["traceback"], Is.Not.Empty);
        }

        [Test]
        public void RunKeyword_ThrowsFatalException()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/RunKeyword";
            var result = client.run_keyword("ThrowsFatalException", new object[0]);

            Assert.That(result["status"], Is.EqualTo("FAIL"));
            Assert.That(result["return"], Is.Empty);
            Assert.That(result, Contains.Key("fatal"));
            Assert.That(result["error"], Is.EqualTo("A fatal exception"));
            Assert.That(result["traceback"], Is.Not.Empty);
        }

        [Test]
        public void RunKeyword_ThrowsContinuableException()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/RunKeyword";
            var result = client.run_keyword("ThrowsContinuableException", new object[0]);

            Assert.That(result["status"], Is.EqualTo("FAIL"));
            Assert.That(result["return"], Is.Empty);
            Assert.That(result, Contains.Key("continuable"));
            Assert.That(result["error"], Is.EqualTo("A continuable exception"));
            Assert.That(result["traceback"], Is.Not.Empty);
        }

        [Test]
        public void RunKeyword_TraceOutput()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/RunKeyword";
            var result = client.run_keyword("WritesTraceOutput", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.Empty);
            Assert.That(result["output"], Does.Contain("First line"));
            Assert.That(result["output"], Does.Contain("Second line"));
        }

        [Test]
        public void RunKeyword_TraceOutputMethod()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("String ParameterType", new object[] {"one", "two"});

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.Empty);
        }

        [Test]
        public void RunKeyword_IntReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("Int ReturnType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(Convert.ToInt32(result["return"]), Is.EqualTo(1));
        }

        [Test]
        public void RunKeyword_Int64ReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("Int64 ReturnType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(Convert.ToInt64(result["return"]), Is.EqualTo(1));
        }

        [Test]
        public void RunKeyword_StringReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("String ReturnType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(Convert.ToString(result["return"]), Is.EqualTo("1"));
        }

        [Test]
        public void RunKeyword_DoubleReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("Double ReturnType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(Convert.ToDouble(result["return"]), Is.EqualTo(1));
        }

        [Test]
        public void RunKeyword_BooleanReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("Boolean ReturnType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"), result["error"].ToString());
            Assert.That(Convert.ToBoolean(result["return"]), Is.True);
        }

        [Test]
        public void RunKeyword_StringArrayReturnType()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("StringArray ReturnType", new object[0]);
            var returnVal = (string[]) result["return"];

            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(returnVal.Length, Is.EqualTo(3));
        }

        [Test]
        public void RunKeyword_LessThanRequiredArgs()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("String ParameterType", new object[0]);

            Assert.That(result["status"], Is.EqualTo("FAIL"));
        }

        [Test]
        public void RunKeyword_MoreThanRequiredArgs()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("String ParameterType", new object[] {"1", "2", "3"});

            Assert.That(result["status"], Is.EqualTo("FAIL"));
        }

        [Test]
        public void RunKeyword_StaticMethod()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.run_keyword("PublicStatic Method", new object[0]);

            Assert.That(result["status"], Is.EqualTo("PASS"));
        }

        #endregion

        #region OptionalParameters

        [Test]
        public void RunKeyword_OptionalParameters_noneProvided()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var arg1 = "asdf";
            var arg2 = 1;
            var arg3 = 1.0;
            var arg4 = true;
            var arg5 = "optional";
            var expectedString = $"{nameof(arg1)}={arg1}\n" +
                                 $"{nameof(arg2)}={arg2}\n" +
                                 $"{nameof(arg3)}={arg3}\n" +
                                 $"{nameof(arg4)}={arg4}\n" +
                                 $"{nameof(arg5)}={arg5}\n";

            var result = client.run_keyword("OptionalParameters Mixed", new object[] { arg1, arg2, arg3, arg4 });
            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.EqualTo(expectedString));
        }

        [Test]
        public void RunKeyword_OptionalParameters_someProvided()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var arg1 = "asdf";
            var arg2 = 1;
            var arg3 = 1.0;
            var arg4 = true;
            var arg6 = 2;
            var expectedString = $"{nameof(arg1)}={arg1}\n" +
                                 $"{nameof(arg2)}={arg2}\n" +
                                 $"{nameof(arg3)}={arg3}\n" +
                                 $"{nameof(arg4)}={arg4}\n" +
                                 "arg5=optional\n" +
                                 $"{nameof(arg6)}={arg6}";

            var result = client.run_keyword("OptionalParameters Mixed", new object[] { arg1, arg2, arg3, arg4, arg6 });
            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.EqualTo(expectedString));
        }

        [Test]
        public void RunKeyword_OptionalParameters_allProvided()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var arg1 = "asdf";
            var arg2 = 1;
            var arg3 = 1.0;
            var arg4 = true;
            var arg5 = "providedOptional";
            var arg6 = 2;
            var expectedString = $"{nameof(arg1)}={arg1}\n" +
                                 $"{nameof(arg2)}={arg2}\n" +
                                 $"{nameof(arg3)}={arg3}\n" +
                                 $"{nameof(arg4)}={arg4}\n" +
                                 $"{nameof(arg5)}={arg5}\n" +
                                 $"{nameof(arg6)}={arg6}";

            var result = client.run_keyword("OptionalParameters Mixed", new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.EqualTo(expectedString));
        }

        #endregion

        #region MultipleParameters

        [Test]
        public void RunKeyword_MultipleParameters()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var arg1 = "asdf";
            var arg2 = 1;
            var arg3 = true;
            var arg4 = 2;
            var arg5 = "ölkj";
            var expectedString = $"{nameof(arg1)}={arg1}\n" +
                                 $"{nameof(arg2)}={arg2}\n" +
                                 $"{nameof(arg3)}={arg3}\n" +
                                 $"{nameof(arg4)}={arg4}\n" +
                                 $"{nameof(arg5)}={arg5}\n";

            var result = client.run_keyword("MultipleParameters Mixed", new object[] { arg1, arg2, arg3, arg4, arg5 });
            Assert.That(result["status"], Is.EqualTo("PASS"));
            Assert.That(result["return"], Is.EqualTo(expectedString));
        }

        #endregion

        #region ComplexParameters

        /// <summary>
        /// Test all primitive types inside of arrays, lists and dictionarys
        /// </summary>
        /// <remarks>
        /// These types are considered complex, as they require conversion from Robot Framework to .NET due to XML-RPC limitations.
        ///
        /// The keywords used for testing need to follow this naming scheme:
        /// {typeName}{elementTypeName} ParameterType
        /// For example: "ArrayInt32 ParameterType" or "ListInt32 ParameterType" or "DictionaryInt32 ParameterType"
        /// It says ParameterType, but it should also have the same return value.
        /// </remarks>
        /// <param name="type">Type of an Enumerable with a primitive element</param>
        /// <param name="typeName">Name of the Enumerable (Helps finding correct testcase)</param>
        [Test]
        [TestCase(typeof(int[]), "Array")]
        [TestCase(typeof(double[]), "Array")]
        [TestCase(typeof(bool[]), "Array")]
        [TestCase(typeof(string[]), "Array")]
        [TestCase(typeof(List<int>), "List")]
        [TestCase(typeof(List<double>), "List")]
        [TestCase(typeof(List<bool>), "List")]
        [TestCase(typeof(List<string>), "List")]
        [TestCase(typeof(Dictionary<string, string>), "Dictionary")]
        [TestCase(typeof(Dictionary<string, bool>), "Dictionary")]
        [TestCase(typeof(Dictionary<string, int>), "Dictionary")]
        [TestCase(typeof(Dictionary<string, double>), "Dictionary")]
        public void RunKeyword_ComplexTypes(Type type, string typeName)
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var rand = new Random();
            var args = GetRandom(type, rand);
            TestContext.Out.WriteLine("Original parameter:");
            PrintObject(args);

            // This serialization serves only the ProxyClient, since it does not support complex types
            args = MockSerialization(type, args);

            TestContext.Out.WriteLine("Serialized parameter:");
            PrintObject(args);

            var keywordResult = client.run_keyword($"{typeName}{GetElementType(type)} ParameterType", new[] { args });
            TestContext.Out.WriteLine("Returned and deserialized value:");
            TraverseStruct(keywordResult);

            Assert.That(keywordResult["status"], Is.EqualTo("PASS"));
            // Could do additional checks on keywordResult["return"] if needed
        }

        [Test]
        [TestCase(typeof(List<int>), "List", 5)]
        public void RunKeyword_ComplexTypes_MultipleArguments(Type type, string typeName, object otherArgument)
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var rand = new Random();
            var args = GetRandom(type, rand);
            TestContext.Out.WriteLine("Original parameter:");
            PrintObject(args);

            args = MockSerialization(type, args);
            TestContext.Out.WriteLine("Serialized parameter:");
            PrintObject(args);

            var keywordResult = client.run_keyword($"{typeName}{GetElementType(type)} ParameterType Multiple", new[] { args, otherArgument });
            TestContext.Out.WriteLine("Returned and deserialized value:");
            TraverseStruct(keywordResult);

            Assert.That(keywordResult["status"], Is.EqualTo("PASS"));
        }

        #endregion

        #region get_library_information()

        [Test]
        public void get_library_information_Test1()
        {
            var client = XmlRpcProxyGen.Create<IRemoteClient>();
            client.Url = "http://127.0.0.1:8270/TestKeywords";
            var result = client.get_library_information();
            TraverseStruct(result);
        }

        #endregion

        #region PRIVATE HELPERS

        private void TraverseStruct(XmlRpcStruct xmlRpcStruct, bool first = true, string indent = "")
        {
            var total = xmlRpcStruct.Count;
            var i = 0;

            foreach (var obj in xmlRpcStruct)
            {
                i++;

                if (obj is not DictionaryEntry entry)
                {
                    TestContext.Out.Write($"{indent}Unknown: {obj}\n");
                    continue;
                }

                TestContext.Out.Write($"{indent}{entry.Key}:");

                switch (entry.Value)
                {
                    case DictionaryEntry entryDict:
                        PrintObject(entryDict);
                        break;
                    case object[] entryList:
                        TestContext.Out.Write($"[{string.Join(", ", entryList)}]");
                        break;
                    case XmlRpcStruct entryStruct:
                        TestContext.Out.Write("{\n");
                        TraverseStruct(entryStruct, false, indent + "  ");
                        TestContext.Out.Write($"{indent}}}");
                        break;
                    default:
                        TestContext.Out.Write(entry.Value);
                        break;
                }

                if (!first && i < total)
                {
                    TestContext.Out.Write(",\n");
                }
                else
                {
                    TestContext.Out.Write("\n");
                }
            }
        }

        private void PrintObject(object obj, bool isLastElement = true)
        {
            var objType = obj.GetType();
            switch (true)
            {
                case true when obj is DictionaryEntry entry:
                    TestContext.Out.WriteLine($"{{key={entry.Key}, value={entry.Value}}}");
                    break;
                case true when obj is XmlRpcStruct[] xmlRpcStruct:
                    for(var i = 0; i < xmlRpcStruct.Length; i++)
                    {
                        TestContext.Out.WriteLine($"[{i}]{{key={xmlRpcStruct[i]["Key"]}, value={xmlRpcStruct[i]["Value"]}}}");
                    }
                    break;
                case true when obj is IList nonGenericList:
                    for (var i = 0; i < nonGenericList.Count; i++)
                    {
                        var item = nonGenericList[i];
                        TestContext.Out.Write($"[{i}]=");
                        PrintObject(item, i == nonGenericList.Count - 1);
                    }
                    TestContext.Out.WriteLine("");
                    break;
                case true when objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>):
                    var listCount = (int?)objType.GetProperty("Count")?.GetValue(obj);
                    if (listCount == null)
                    {
                        TestContext.Error.WriteLine($"Count of generic list could not be determined (null)");
                        break;
                    }
                    for (var i = 0; i < listCount; i++)
                    {
                        var item = objType.GetProperty("Item")?.GetValue(obj, new object[] { i });
                        TestContext.Out.Write($"[{i}]=");
                        PrintObject(item, i == listCount - 1);
                    }
                    TestContext.Out.WriteLine("");
                    break;
                case true when objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Dictionary<,>):
                    var enumerator = ((dynamic)obj).GetEnumerator();
                    var index = 0;

                    while (enumerator.MoveNext())
                    {
                        var e = enumerator.Current;
                        TestContext.Out.WriteLine($"Pos{index}={{key={e.Key}, value={e.Value}}}");
                        index++;
                    }
                    break;
                default:
                    TestContext.Out.WriteLine(isLastElement ? $"{obj}" : $"{obj},");
                    break;
            }
        }

        private string GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType()?.Name ?? "Unknown";
            
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                    return type.GetGenericArguments()[0].Name;
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    return type.GetGenericArguments()[1].Name;
            }

            return "Unknown";
        }

        private object GetRandom(Type type, Random rand, int minRange = 1, int maxRange = 20)
        {
            if (type == typeof(int))
            {
                return rand.Next();
            }

            if (type == typeof(double))
            {
                return rand.NextDouble();
            }

            if (type == typeof(bool))
            {
                return rand.Next(0, 2) == 1;
            }

            if (type == typeof(string))
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, rand.Next(minRange, maxRange + 1))
                    .Select(s => s[rand.Next(s.Length)]).ToArray());
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType == null)
                {
                    throw new ArgumentException("Type of array element could not be determined");
                }
                var array = Array.CreateInstance(elementType, rand.Next(minRange, maxRange + 1));
                for (var i = 0; i < array.Length; i++)
                {
                    array.SetValue(GetRandom(elementType, rand), i);
                }
                return array;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = type.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var list = (IList)Activator.CreateInstance(listType);
                    var listSize = rand.Next(minRange, maxRange + 1);

                    for (var i = 0; i < listSize; i++)
                    {
                        list.Add(GetRandom(itemType, rand));
                    }
                    return list;
                }
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var keyType = type.GetGenericArguments()[0];
                    var valueType = type.GetGenericArguments()[1];
                    var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    var dict = (IDictionary)Activator.CreateInstance(dictType);
                    var dictSize = rand.Next(minRange, maxRange + 1);

                    for (var i = 0; i < dictSize; i++)
                    {
                        var key = GetRandom(keyType, rand);
                        var value = GetRandom(valueType, rand);
                        dict.Add(key, value);
                    }
                    return dict;
                }
            }

            throw new NotSupportedException("This type is not supported");
        }

        private Array MockSerialization(Type type, object toSerialize)
        {
            if (type.IsArray)
                return (Array)toSerialize;

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var itemType = type.GetGenericArguments()[0];
                    var listSize = (int?)toSerialize.GetType().GetProperty("Count")?.GetValue(toSerialize);
                    if (listSize == null)
                    {
                        TestContext.Error.WriteLine("Count of generic list could not be determined");
                        return Array.Empty<object>();
                    }

                    var array = Array.CreateInstance(itemType, listSize.Value);
                    ((dynamic)toSerialize).CopyTo(array);
                    return array;
                }
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    var keyType = type.GetGenericArguments()[0];
                    var valueType = type.GetGenericArguments()[1];
                    var dictSize = (int?)toSerialize.GetType().GetProperty("Count")?.GetValue(toSerialize);
                    if (dictSize == null)
                    {
                        TestContext.Error.WriteLine("Count of dictionary could not be determined");
                        return Array.Empty<object>();
                    }

                    var enumerator = ((dynamic)toSerialize).GetEnumerator();
                    var entries = new List<DictionaryEntry>();
                    for (var i = 0; i < dictSize; i++)
                    {
                        if (!enumerator.MoveNext()) break;
                        var element = enumerator.Current;
                        entries.Add(new DictionaryEntry(element.Key, element.Value));
                    }
                    return entries.ToArray();
                }
            }

            throw new NotSupportedException("This type is not supported");
        }

        #endregion
    }
}

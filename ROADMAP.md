# RobotCore Project Roadmap

## Usages
- [ ] Programmatic Usage (i.e. RemoteServer used as library in your application / runtime)
- [ ] Service inside ASP.NET (i.e. RemoteServer added as service to your ASP.NET server)
- [ ] Standalone Server (i.e. You launch the default RemoteServer executable)
- [ ] Dependency (i.e. You add the RemoteServer as NuGet package)
## Server Information
- [ ] Status Page informing about loaded libraries and their keywords
- [ ] Robot Framework Library Documentation Format for each loaded Library
## Type conversion support
- [ ] Robot Framework => RemoteServer _(i.e. serialized parameters from RF)_
  - [ ] General type support for parameters in keyword methods _(all RF-supportable standard types of C# should be usable in keyword methods)_
  - [ ] Python type conversions to parameter type of keyword method
    - [ ] Regular python types
      - [ ] `None`
      - [ ] `string`
      - [ ] `int`
      - [ ] `float`
      - [ ] `bool`
    - [ ] Special types
      - [ ] `list, tuple`
        - [ ] Handle regular python element types _(listed above)_
        - [ ] Handle nested `list, tuple`
        - [ ] Handle nested `dict`
        - [ ] _other special cases to consider to be added here_
      - [ ] `dict`
        - [ ] Handle regular python value types _(listed above)_
        - [ ] Handle text keys (`string`)
        - [ ] Handle numeric keys (`int`, `float`)
        - [ ] Handle `object` keys
        - [ ] _other special cases to consider to be added here_
- [ ] RemoteServer => Robot Framework _(i.e. return values serialized to RF)_
  - [ ] special types
    - [ ] `null`
    - [ ] `void`
    - [ ] `dynamic`
    - [ ] `object`
    - [ ] `DateTime`
    - [ ] `enum`
  - [ ] Symbol types
    - [ ] `char`
    - [ ] `string`
  - [ ] numeric types
    - [ ] `int`
    - [ ] `uint`
    - [ ] `short`
    - [ ] `ushort`
    - [ ] `float`
    - [ ] `double`
    - [ ] `decimal`
    - [ ] `ulong`
    - [ ] `byte`
    - [ ] `sbyte`
  - [ ] collection types
    - [ ] based on `IList` or `ICollection`
        - [ ] `Array`
        - [ ] `ArrayList`
        - [ ] `List<T>`
        - [ ] `Queue`
        - [ ] `ConcurrentQueue<T>`
        - [ ] `LinkedList<T>`
    - [ ] based on `IDictionary`
      - [ ] `Hashtable`
      - [ ] `SortedList`
      - [ ] `SortedList<TKey, TValue>`
      - [ ] `Dictionary<TKey, TValue>`
      - [ ] `ConcurrentDictionary<TKey, TValue>`
## Library types
- [ ] Static API Library
- [ ] Dynamic API Library
## Remote Server Communication Features
- [ ] PASS
- [ ] FAIL
- [ ] CONTINUABLE
- [ ] FATAL
- [ ] Exceptions
  - [ ] Traceback
- [ ] Logging
- [ ] Finding keywords
## Remote Server Library Features
- [ ] Library Attributes
- [ ] Keyword Attributes
- [ ] Support positional arguments
- [ ] Support kwargs
- [ ] Support variable arguments
- [ ] Support method overloading
- [ ] Support library class inheritance
- [ ] Support keyword documentation
- [ ] Support keyword tags
- [ ] Support code documentation
## Remote Server Protocol _(XML-RPC)_
- [ ] Data type serialization
- [ ] Robot Framework Remote-Library-Interface specification
  - [ ] `run_keyword()`
  - [ ] `get_keyword_names`
  - [ ] `get_keyword_arguments`
  - [ ] `get_keyword_types`
  - [ ] `get_keyword_tags`
  - [ ] `get_keyword_documentation`
  - [ ] `get_library_information`

## Remote Server Control

- [ ] Controls from Robot Framework
  - [ ] Start server
  - [ ] Stop server
- [ ] Controls in C#
  - [ ] Configure start / stop functionality _(Allow)_
  - [ ] Configure port _(i.e. usage method in C# allows setting a server port)_
  - [ ] Configure logging
## CI/CD
- [ ] Merging strategy
- [ ] Dependency management
  - [ ] XML-RPC packages
  - [ ] Unit test framework
  - [ ] Robot Framework package for remote testing
  - [ ] Logging Framework
- [ ] NuGet-Packages
  - [ ] Sdk.props
  - [ ] Sdk.targets
- [ ] Standalone server (executable)
  - [ ] Windows
  - [ ] Linux
- [ ] Standalone server (Docker image)
  - [ ] Windows
  - [ ] Linux
- [ ] Testautomation
- [ ] Release Notes
## RobotCore internal Tests (`nunit`)
- [ ] Performance testing
## Robot Framework RobotCore Tests (`atest`)
- [ ] Verify Remote Server features
  - [ ] Start server
  - [ ] Stop server
  - [ ] Argument type conversion
  - [ ] Return value type conversion
  - [ ] Finding keywords
  - [ ] Variable arguments
  - [ ] kwargs
  - [ ] Logging
  - [ ] Traceback
  - [ ] Keyword status _(PASS, FAIL, ...)_
- [ ] Verify Static Library features
- [ ] Verify Dynamic Library features
## Documentation
- [ ] Code structure
- [ ] Architecture
## Usage Guides
- [ ] Wiki hosting
- [ ] Getting started
- [ ] FAQ
- [ ] Example programmatic usage
- [ ] Example ASP.NET service
- [ ] Example Standalone server
- [ ] Example NuGet usage
- [ ] Example Static Library
- [ ] Example Dynamic Library
- [ ] Example Robot Framework Usage
- [ ] Example Library Documentation
## Support & Contribution Guides
- [ ] Issue templates
- [ ] Issue tags
- [ ] Codestyle
- [ ] Feedback loop (GitHub Discussions)
## Maintenance
- [ ] C# Language versions
- [ ] .NET Core version updates
- [ ] Dependency management
- [ ] Refactor cycles
## Development support
- [ ] Sponsoring (recurring)
- [ ] Sponsoring (single-donation)
## Licensing
- [ ] Robot Framework Logo Usage?
- [ ] Appropriate copyright?
## Marketing
- [x] Project Name
- [ ] Project Logo
- [ ] README
- [ ] Repository tags
- [ ] RoboCon 2025 Material
  - [ ] Presentation slides
  - [ ] Hosted Live Demo Server
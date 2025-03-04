# RobotCore Project Roadmap

## Usages
- [x] Programmatic Usage (i.e. RemoteServer used as library in your application / runtime)
- [ ] Service inside ASP.NET (i.e. RemoteServer added as service to your ASP.NET server)
- [x] Standalone Server (i.e. You launch the default RemoteServer executable)
- [ ] Dependency (i.e. You add the RemoteServer as NuGet package)
## Server Information
- [ ] Status Page informing about loaded libraries and their keywords
- [ ] Robot Framework Library Documentation Format for each loaded Library
## Type conversion support
- [ ] Robot Framework => RemoteServer _(i.e. serialized parameters from RF)_
  - [x] General type support for parameters in keyword methods _(all RF-supportable standard types of C# should be usable in keyword methods)_
  - [ ] Python type conversions to parameter type of keyword method
    - [ ] Regular python types
      - [x] `None`
      - [x] `string`
      - [x] `int`
      - [x] `float`
      - [x] `bool`
    - [ ] Special types
      - [x] `list, tuple`
        - [x] Handle regular python element types _(listed above)_
        - [x] Handle nested `list, tuple`
        - [x] Handle nested `dict`
        - [ ] _other special cases to consider to be added here_
      - [x] `dict`
        - [x] Handle regular python value types _(listed above)_
        - [x] Handle text keys (`string`)
        - [ ] Handle numeric keys (`int`, `float`)
        - [ ] Handle `object` keys
        - [ ] _other special cases to consider to be added here_
- [ ] RemoteServer => Robot Framework _(i.e. return values serialized to RF)_
  - [ ] special types
    - [ ] `null`
    - [x] `void`
    - [ ] `dynamic`
    - [ ] `object`
    - [ ] `DateTime`
    - [ ] `enum`
  - [ ] Symbol types
    - [ ] `char`
    - [x] `string`
  - [ ] numeric types
    - [x] `int`
    - [ ] `uint`
    - [ ] `short`
    - [ ] `ushort`
    - [x] `float`
    - [x] `double`
    - [ ] `decimal`
    - [ ] `ulong`
    - [ ] `byte`
    - [ ] `sbyte`
  - [ ] collection types
    - [ ] based on `IList` or `ICollection`
        - [x] `Array`
        - [ ] `ArrayList`
        - [x] `List<T>`
        - [ ] `Queue`
        - [ ] `ConcurrentQueue<T>`
        - [ ] `LinkedList<T>`
    - [ ] based on `IDictionary`
      - [ ] `Hashtable`
      - [ ] `SortedList`
      - [ ] `SortedList<TKey, TValue>`
      - [x] `Dictionary<string, TValue>`
      - [ ] `Dictionary<TKey, TValue>`
      - [ ] `ConcurrentDictionary<TKey, TValue>`
## Library types
- [ ] Static API Library
- [x] Dynamic API Library
## Remote Server Communication Features
- [x] PASS
- [x] FAIL
- [x] CONTINUABLE
- [x] FATAL
- [x] Exceptions
  - [x] Traceback
- [x] Logging
- [x] Finding keywords
## Remote Server Library Features
- [x] Library Attributes
- [x] Keyword Attributes
- [x] Support positional arguments
- [x] Support kwargs
- [ ] Support variable arguments
- [ ] Support method overloading
- [ ] Support library class inheritance
- [x] Support keyword documentation
- [x] Support keyword tags
- [x] Support code documentation
## Remote Server Protocol _(XML-RPC)_
- [x] Data type serialization
- [x] Robot Framework Remote-Library-Interface specification
  - [x] `run_keyword()`
  - [x] `get_keyword_names`
  - [x] `get_keyword_arguments`
  - [x] `get_keyword_types`
  - [x] `get_keyword_tags`
  - [x] `get_keyword_documentation`
  - [x] `get_library_information`

## Remote Server Control

- [ ] Controls from Robot Framework
  - [ ] Start server
  - [ ] Stop server
- [ ] Controls in C#
  - [ ] Configure start / stop functionality _(Allow)_
  - [x] Configure port _(i.e. usage method in C# allows setting a server port)_
  - [x] Configure logging
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
- [x] Verify Dynamic Library features
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
- [x] Sponsoring (recurring)
- [x] Sponsoring (single-donation)
## Licensing
- [ ] Robot Framework Logo Usage?
- [ ] Appropriate copyright?
## Marketing
- [x] Project Name
- [ ] Project Logo
- [ ] README
- [ ] Repository tags
- [x] RoboCon 2025 Material
  - [x] Presentation slides
  - [ ] Hosted Live Demo Server
  - [ ] DevContainer for VSCode
[![build](https://github.com/fluentassertions/fluentassertions.json/actions/workflows/build.yml/badge.svg)](https://github.com/fluentassertions/fluentassertions.json/actions/workflows/build.yml)
[![](https://img.shields.io/github/release/FluentAssertions/FluentAssertions.Json.svg?label=latest%20release)](https://github.com/FluentAssertions/FluentAssertions.Json/releases/latest)
[![](https://img.shields.io/nuget/dt/FluentAssertions.Json.svg?label=nuget%20downloads)](https://www.nuget.org/packages/FluentAssertions.Json)
[![](https://img.shields.io/librariesio/dependents/nuget/FluentAssertions.Json.svg?label=dependent%20libraries)](https://libraries.io/nuget/FluentAssertions.Json)
![](https://img.shields.io/badge/release%20strategy-githubflow-orange.svg)
[![Coverage Status](https://coveralls.io/repos/github/fluentassertions/fluentassertions.json/badge.svg?branch=master)](https://coveralls.io/github/fluentassertions/fluentassertions.json?branch=master)

## *"With Fluent Assertions, the assertions look beautiful, natural and most importantly, extremely readable"* (by [Girish](https://twitter.com/girishracharya))

* See [www.fluentassertions.com](http://www.fluentassertions.com/) for more information about the main library.
* Join the chat at [![Join the chat at https://gitter.im/dennisdoomen/fluentassertions](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dennisdoomen/fluentassertions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Available extension methods

- `BeEquivalentTo()`
- `ContainSingleElement()`
- `ContainSubtree()`
- `HaveCount()`
- `HaveElement()`
- `HaveValue()`
- `MatchRegex()`
- `NotBeEquivalentTo()`
- `NotHaveElement()`
- `NotHaveValue()`
- `NotMatchRegex()`

See "in-code" description for more information.

### Usage

Be sure to include `using FluentAssertions.Json;` otherwise false positives may occur.

```c#
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;

... 
var actual = JToken.Parse(@"{ ""key1"" : ""value"" }");
var expected = JToken.Parse(@"{ ""key2"" : ""value"" }");
actual.Should().BeEquivalentTo(expected);
```

You can also use `IJsonAssertionOptions<>` with `Should().BeEquivalentTo()` assertions, which contains helper methods that you can use to specify the way you want to compare specific data types.

Example:

```c#
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;

... 
var actual = JToken.Parse(@"{ ""value"" : 1.5 }");
var expected = JToken.Parse(@"{ ""value"" : 1.4 }");
actual.Should().BeEquivalentTo(expected, options => options
                .Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, 0.1))
                .WhenTypeIs<double>());
```
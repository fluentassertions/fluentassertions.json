## *"With Fluent Assertions, the assertions look beautiful, natural and most importantly, extremely readable"* (by [Girish](https://twitter.com/girishracharya))

* See [www.fluentassertions.com](http://www.fluentassertions.com/) for more information about the main library.
* The build status is [![Build status](https://ci.appveyor.com/api/projects/status/ub0dfcmad2cf26tf/branch/master?svg=true)](https://ci.appveyor.com/project/dennisdoomen/fluentassertions-json/branch/master)
* Join the chat at [![Join the chat at https://gitter.im/dennisdoomen/fluentassertions](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dennisdoomen/fluentassertions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

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


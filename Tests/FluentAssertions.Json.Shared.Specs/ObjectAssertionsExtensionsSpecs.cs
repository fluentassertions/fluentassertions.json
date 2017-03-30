using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentAssertions.Json
{
    [TestClass]
    public class ObjectAssertionsExtensionsSpecs
    {
        [TestMethod]
        public void SerializableClass_ShouldBeJsonSerializable()
        {
            var obj = new SerializableClass();

            obj.Should().BeJsonSerializable<SerializableClass>();
        }

        [TestMethod]
        public void NotSerializableClass_ShouldNotBeJsonSerializable()
        {
            var notJsonSerializable = new NotJsonSerializable(11);

            Action action = () => notJsonSerializable.Should().BeJsonSerializable<NotJsonSerializable>();

            action.ShouldThrow<Exception>()
                .Where(e=>e.Message.Contains(" to be JSON serializable, but serialization failed with:"));
        }

        public class SerializableClass
        {
            public string Name { get; set; }
        }

        public class NotJsonSerializable
        {
            public NotJsonSerializable(int param)
            {
                
            }
        }
    }
}

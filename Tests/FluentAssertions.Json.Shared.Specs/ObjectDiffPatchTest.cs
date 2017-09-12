#region *   License     *
/*
    SimpleHelpers - ObjectDiffPatch   

    Copyright © 2014 Khalid Salomão

    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the “Software”), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE. 

    License: http://www.opensource.org/licenses/mit-license.php
    Website: https://github.com/khalidsalomao/SimpleHelpers.Net
 */

#endregion

// Note mkoertgen: copied tests from original repo.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace FluentAssertions.Json
{
    public class ObjectDiffPatchTest
    {
        private TestClass GetSimpleTestObject()
        {
            return new TestClass()
            {
                StringProperty = "this is a string",
                IntProperty = 1234,
                DoubleProperty = 56.789
            };
        }

        private void PopulateStringListOnTestClass(TestClass testObject)
        {
            testObject.ListOfStringProperty = new List<string>()
            {
                "this", "is", "a", "list", "of", "strings"
            };
        }

        private void PopulateObjectListOnTestClass(TestClass testObject)
        {
            testObject.ListOfObjectProperty = new List<TestClass>()
            {
                new TestClass()
                {
                    StringProperty = "this is the first object",
                    IntProperty = 1,
                    DoubleProperty = 1.01
                },
                new TestClass()
                {
                    StringProperty = "this is the second object",
                    IntProperty = 2,
                    DoubleProperty = 2.02
                },
                new TestClass()
                {
                    StringProperty = "this is the third object",
                    IntProperty = 3,
                    DoubleProperty = 3.03
                }
            };
        }

        [Fact]
        public void AbleToDiffAndPatchSimpleObject()
        {
            var testObj = GetSimpleTestObject();

            var updatedTestObj = GetSimpleTestObject();
            updatedTestObj.StringProperty = "this is an updated string";
            updatedTestObj.IntProperty = 5678;
            updatedTestObj.DoubleProperty = 123.456;

            var diff = ObjectDiffPatch.GenerateDiff(testObj, updatedTestObj);

            var revertPatch = JsonConvert.SerializeObject(diff.OldValues);

            var revertedObj = ObjectDiffPatch.PatchObject(updatedTestObj, revertPatch);

            testObj.StringProperty.Should().Be(revertedObj.StringProperty);
            testObj.IntProperty.Should().Be(revertedObj.IntProperty);
            testObj.DoubleProperty.Should().Be(revertedObj.DoubleProperty);
        }

        [Fact]
        public void AbleToDeleteStringListItemThenRevertViaPatch()
        {
            var testObj = GetSimpleTestObject();
            PopulateStringListOnTestClass(testObj);

            var updatedTestObj = GetSimpleTestObject();
            PopulateStringListOnTestClass(updatedTestObj);

            updatedTestObj.ListOfStringProperty.Remove("list");

            testObj.ListOfStringProperty.Should().NotBeEquivalentTo(updatedTestObj.ListOfStringProperty);


            var diff = ObjectDiffPatch.GenerateDiff(testObj, updatedTestObj);

            var revertPatch = JsonConvert.SerializeObject(diff.OldValues);

            var revertedObj = ObjectDiffPatch.PatchObject(updatedTestObj, revertPatch);

            testObj.ListOfStringProperty.Should().BeEquivalentTo(revertedObj.ListOfStringProperty);
        }

        [Fact]
        public void AbleToDeleteObjectListItemThenRevertViaPatch()
        {
            var testObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(testObj);

            var updatedTestObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(updatedTestObj);

            updatedTestObj.ListOfObjectProperty.RemoveAt(1);

            testObj.ListOfObjectProperty.Count.Should().NotBe(updatedTestObj.ListOfObjectProperty.Count);

            var diff = ObjectDiffPatch.GenerateDiff(testObj, updatedTestObj);

            var revertPatch = JsonConvert.SerializeObject(diff.OldValues);

            var revertedObj = ObjectDiffPatch.PatchObject(updatedTestObj, revertPatch);

            testObj.ListOfObjectProperty.Count.Should().Be(revertedObj.ListOfObjectProperty.Count);
        }

        [Fact]
        public void AbleToEditObjectInListThenRevertViaPatch()
        {
            var testObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(testObj);

            var updatedTestObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(updatedTestObj);

            updatedTestObj.ListOfObjectProperty[2].IntProperty = 30;
            updatedTestObj.ListOfObjectProperty[2].StringProperty = "this is an update to the last object in the list";
            updatedTestObj.ListOfObjectProperty[2].DoubleProperty = 33.333;

            var diff = ObjectDiffPatch.GenerateDiff(testObj, updatedTestObj);

            var revertPatch = JsonConvert.SerializeObject(diff.OldValues);

            var revertedObj = ObjectDiffPatch.PatchObject(updatedTestObj, revertPatch);

            testObj.ListOfObjectProperty[2].IntProperty.Should().Be(revertedObj.ListOfObjectProperty[2].IntProperty);
            testObj.ListOfObjectProperty[2].StringProperty.Should().Be(revertedObj.ListOfObjectProperty[2].StringProperty);
            testObj.ListOfObjectProperty[2].DoubleProperty.Should().Be(revertedObj.ListOfObjectProperty[2].DoubleProperty);
        }

        [Fact]
        public void AbleToAddObjectListItemThenApplyViaPatch()
        {
            var testObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(testObj);

            var updatedTestObj = GetSimpleTestObject();
            PopulateObjectListOnTestClass(updatedTestObj);

            updatedTestObj.ListOfObjectProperty.Add(new TestClass { StringProperty = "added" });

            var diff = ObjectDiffPatch.GenerateDiff(testObj, updatedTestObj);

            var updatePatch = JsonConvert.SerializeObject(diff.NewValues);

            var objToUpdate = GetSimpleTestObject();
            PopulateObjectListOnTestClass(objToUpdate);

            var updatedObj = ObjectDiffPatch.PatchObject(objToUpdate, updatePatch);

            updatedTestObj.ListOfObjectProperty.Count.Should().Be(updatedObj.ListOfObjectProperty.Count);

            var addedListItem = updatedObj.ListOfObjectProperty.SingleOrDefault(obj => obj != null && obj.StringProperty == "added");

            addedListItem.Should().NotBeNull();

        }
    }

    internal class TestClass
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public double DoubleProperty { get; set; }
        public List<TestClass> ListOfObjectProperty { get; set; }
        public List<string> ListOfStringProperty { get; set; }
        public List<int> ListOfIntProperty { get; set; }
        public List<double> ListOfDoubleProperty { get; set; }
    }
}
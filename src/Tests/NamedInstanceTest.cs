﻿using System;
using System.Collections.Generic;
using Cloud.Core.Testing;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloud.Core.Tests
{
    [IsUnit]
    public class NamedInstanceTest
    {
        /// <summary>Make sure the NamedInstance can be mocked as expected.</summary>
        [Fact]
        public void Test_NamedInstance_UsingMocking()
        {
            var i1 = new TestInstance1("testInstance1");
            var i2 = new TestInstance1("testInstance2");

            // Arrange
            var mocked = new Mock<NamedInstanceFactory<ITestInterface>>(new List<ITestInterface> { i1,  i2 });

            // Act
            var foundInstance = mocked.Object["testInstance1"];

            // Assert
            foundInstance.Should().NotBeNull();
        }

        /// <summary>Make sure the NamedInstance can be populated with mocked instances as expected.</summary>
        [Fact]
        public void Test_NamedInstance_UsingMockedInstances()
        {
            var i1 = new Mock<ITestInterface>();
            var i2 = new Mock<ITestInterface>();
            i1.Setup(m => m.Name).Returns("testInstance1");
            i2.Setup(m => m.Name).Returns("testInstance2");

            // Arrange
            var mocked = new Mock<NamedInstanceFactory<ITestInterface>>(new List<ITestInterface> { i1.Object, i2.Object });

            // Act
            var foundInstance = mocked.Object["testInstance1"];

            // Assert
            foundInstance.Should().NotBeNull();
        }

        /// <summary>Make sure we can resolve multiple instances of a class with unique names.</summary>
        [Fact]
        public void Test_NamedInstance_MultipleInstanceResolved()
        {
            // Arrange
            var i1 = new TestInstance1("testInstance1");
            var i2 = new TestInstance2("testInstance2");
            var i3 = new TestInstance3("testInstance3");

            // Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface> { i1, i2, i3 });
            var lookup1 = factory["testInstance1"];
            var lookup2 = factory["testInstance2"];
            var lookup3 = factory["testInstance3"];

            // Assert
            lookup1.Should().Equals(i1);
            lookup2.Should().Equals(i2);
            lookup3.Should().Equals(i3);
            factory.Clients.Count.Should().Be(3);
        }

        /// <summary>Make entries with duplicate names are auto assigned new names (to avoid error situations).</summary>
        [Fact]
        public void Test_NamedInstance_DuplicateInstanceName()
        {
            // Arrange
            var i1 = new TestInstance1("testInstanceA");
            var i2a = new TestInstance2("testInstanceB");
            var i2b = new TestInstance3("testInstanceB");
            var i2c = new TestInstance3("testInstanceB");

            // Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface> { i1, i2a, i2b, i2c });
            var lookup1 = factory["testInstanceA"];
            var lookup2a = factory["testInstanceB"];
            var lookup2b = factory["testInstanceB1"];
            var lookup2c = factory["testInstanceB2"];

            // Assert
            lookup1.Should().Equals(i1);
            lookup2a.Should().Equals(i2a);
            lookup2b.Should().Equals(i2b);
            lookup2c.Should().Equals(i2c);
            factory.Clients.Count.Should().Be(4);
        }

        /// <summary>Make sure instances requested, that don't exist, result in the expected error.</summary>
        [Fact]
        public void Test_NamedInstance_NonExistingInstance()
        {
            // Arrange/Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface>());

            // Assert
            Assert.Throws<ArgumentException>(() => factory["testInstanceA"]);
        }

        /// <summary>Make sure we can resolve multiple instances of a class with unique names.</summary>
        [Fact]
        public void Test_NamedInstance_EmptyNamesAutoset()
        {
            // Arrange
            var i1 = new TestInstance1("");
            var expectedName1 = nameof(TestInstance1);
            var expectedName2 = expectedName1 + "1";

            // Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface> { i1, i1 });

            // Assert
            Assert.Throws<ArgumentException>(() => factory[""]);
            factory[expectedName1].Should().NotBeNull();
            factory[expectedName2].Should().NotBeNull();
            factory.Clients.Count.Should().Be(2);
        }

        /// <summary>When calling TryGetValue on NamedInstanceFactory with instances that exist, should return true with values</summary>
        [Fact]
        public void Test_NamedInstance_TryGetValue_ValuesExist()
        {
            // Arrange
            var i1 = new TestInstance1("");
            var expectedName1 = nameof(TestInstance1);
            var expectedName2 = expectedName1 + "1";

            // Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface> { i1, i1 });

            // Assert
            Assert.True(factory.TryGetValue(expectedName1, out var value));
            Assert.NotNull(value);
            Assert.True(factory.TryGetValue(expectedName2, out var value2));
            Assert.NotNull(value2);
        }

        /// <summary>When calling TryGetValue on NamedInstanceFactory with instances that do not exist, should return false with null values</summary>
        [Fact]
        public void Test_NamedInstance_TryGetValue_ValuesNotExist()
        {
            // Arrange
            var i1 = new TestInstance1("");

            // Act
            var factory = new NamedInstanceFactory<ITestInterface>(new List<ITestInterface> { i1, i1 });

            // Assert
            Assert.False(factory.TryGetValue("WrongName", out var value));
            Assert.Null(value);
        }
    }

    public interface ITestInterface : INamedInstance
    {
    }

    public class TestInstance1 : ITestInterface
    {
        public TestInstance1(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }

    public class TestInstance2 : ITestInterface
    {
        public TestInstance2(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }

    public class TestInstance3 : ITestInterface
    {
        public TestInstance3(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}

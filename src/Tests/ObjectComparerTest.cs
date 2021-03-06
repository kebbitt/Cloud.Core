using Cloud.Core.Testing;
using Cloud.Core.Comparer;
using FluentAssertions;
using Xunit;

namespace Cloud.Core.Tests
{
    [IsUnit]
    public class ObjectComparerTest
    {
        [Theory]
        [InlineData("A10", "A2", 8)]
        [InlineData("A2", "A10", -8)]
        [InlineData("A1", "A1", 0)]
        [InlineData(null, "A10", -1)]
        [InlineData("5", "A10", -1)]
        public void Test_SemiNumericComparer(string s1, string s2, int expectedResult)
        {
            /// Arrange
            var comparer = new SemiNumericComparer();

            // Act
            var result = comparer.Compare(s1, s2);

            // Assert
            result.Should().Be(expectedResult);
        }

        /// <summary>Ensure object reference equality is equal for ints.</summary>
        [Fact]
        public void Test_ObjectReferenceEqualityComparer_EqualsOnInts()
        {
            // Arrange
            var reference1 = (object)1;
            var reference2 = (object)1;

            // Arrange
            var result = ObjectReferenceEqualityComparer<object>.Default.Equals(reference1, reference2);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>Ensure object reference equality is equal for strings.</summary>
        [Fact]
        public void Test_ObjectReferenceEqualityComparer_EqualsOnStrings()
        {
            // Arrange
            var reference1 = 123.ToString();
            var reference2 = 123.ToString();

            // Act
            var result = ObjectReferenceEqualityComparer<object>.Default.Equals(reference1, reference2);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>Ensure object hashcode is equal for ints.</summary>
        [Fact]
        public void Test_ObjectReferenceEqualityComparer_OnInts()
        {
            // Arrange
            var reference1 = (object)1;
            var reference2 = (object)1;

            // Act
            var result = ObjectReferenceEqualityComparer<object>.Default.GetHashCode(reference1);

            // Assert
            result.Should().NotBe(ObjectReferenceEqualityComparer<object>.Default.GetHashCode(reference2));
        }

        /// <summary>Ensure object hashcode is equal for strings.</summary>
        [Fact]
        public void Test_ObjectReferenceEqualityComparer_OnStrings()
        {
            // Arrange
            var reference1 = 123.ToString();
            var reference2 = 123.ToString();

            // Act
            var result = ObjectReferenceEqualityComparer<object>.Default.GetHashCode(reference1);

            // Assert
            result.Should().NotBe(ObjectReferenceEqualityComparer<object>.Default.GetHashCode(reference2));
        }
    }
}

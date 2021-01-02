using Cherokee.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CherokeeTests.Extensions
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class StringExtensionsTests
    {
        #region IsNumeric
        [TestMethod]
        public void IsNumeric_CalledOnNullString_ReturnsFalse()
        {
            // arrange
            string str = null;

            // act
            bool result = str.IsNumeric();

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNumeric_CalledOnEmptyString_ReturnsFalse()
        {
            // arrange
            string str = string.Empty;

            // act
            bool result = str.IsNumeric();

            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("1234567890")]
        public void IsNumeric_CalledOnStringWithOnlyNumbers_ReturnsTrue(string str)
        {
            // act
            bool result = str.IsNumeric();

            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("a")]
        [DataRow("ᎠᎡᎢᎣᎤᎥᏓᏕᏗᏙᏚᏛ")]
        [DataRow("123abc")]
        public void IsNumeric_CalledOnStringWithAnyNonNumericChars_ReturnsFalse(string str)
        {
            // act
            bool result = str.IsNumeric();

            // assert
            result.Should().BeFalse();
        }
        #endregion IsNumeric

        #region AllIndicesOf
        [TestMethod]
        public void AllIndicesOf_CalledOnNullString_ReturnsEmptyEnumerable()
        {
            // arrange
            string input = null;
            char needle = 'x';

            // act
            var result = input.AllIndicesOf(needle);

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AllIndicesOf_CalledOnStringWithoutChar_ReturnsEmptyEnumerable()
        {
            // arrange
            string input = "abcabcabc";
            char needle = 'd';

            // act
            var result = input.AllIndicesOf(needle);

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void AllIndicesOf_CalledOnStringWithOneInstanceOfChar_ReturnsEnumerableWithCorrectIndex()
        {
            // arrange
            string input = "abcdefghijklmnop";
            char needle = 'd';

            // act
            var result = input.AllIndicesOf(needle);

            // assert
            result.Should().HaveCount(1);
            result.Should().Contain(3);
        }

        [TestMethod]
        public void AllIndicesOf_CalledOnStringWithSeveralInstancesOfChar_ReturnsEnumerableWithCorrectIndices()
        {
            // arrange
            string input = "abcabcabcabc";
            char needle = 'b';

            // act
            var result = input.AllIndicesOf(needle);

            // assert
            result.Should().HaveCount(4);
            result.Should().Contain(1);
            result.Should().Contain(4);
            result.Should().Contain(7);
            result.Should().Contain(10);
        }
        #endregion AllIndicesOf

        #region ReplaceFirst
        [TestMethod]
        public void ReplaceFirst_InvokedOnNullString_ReturnsNull()
        {
            // arrange
            string input = null;

            // act
            var result = input.ReplaceFirst("toReplace", "replaceBy");

            // assert
            result.Should().BeNull();
        }

        [TestMethod]
        public void ReplaceFirst_InvokedOnStringWithoutSubstring_SameStringReturned()
        {
            // arrange
            string input = "this is the input";
            string toReplace = "owl";
            string replaceBy = "dumb";

            // act
            var result = input.ReplaceFirst(toReplace, replaceBy);

            // assert
            result.Should().Be(input);
        }

        [TestMethod]
        public void ReplaceFirst_InvokedOnStringWithSubstring_StringWithSubstringReplacedReturned()
        {
            // arrange
            string input = "this is a test";
            string toReplace = "a tes";
            string replaceBy = string.Empty;

            // act
            var result = input.ReplaceFirst(toReplace, replaceBy);

            // assert
            result.Should().Be("this is t");
        }
        #endregion ReplaceFirst
    }
}

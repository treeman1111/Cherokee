using Cherokee.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CherokeeTests.Extensions
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class StringExtensionsTests
    {
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
    }
}

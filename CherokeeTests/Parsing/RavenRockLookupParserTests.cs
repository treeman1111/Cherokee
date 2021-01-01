using Cherokee.Parsing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CherokeeTests.Parsing
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class RavenRockLookupParserTests
    {
        #region empty and null input
        [TestMethod]
        public void ParseLookupLines_GivenNullStringArray_ThrowsArgumentNullException()
        {
            // arrange
            Action act = () => _ = RavenRockLookupParser.ParseLookupLines(null);

            // act & assert
            act.Should().ThrowExactly<ArgumentNullException>().And.ParamName.Should().Be("lines");
        }

        [TestMethod]
        public void ParseLookupLines_GivenEmptyStringArray_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = Array.Empty<string>();

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }
        #endregion empty and null input

        #region lines with roman chars alone
        [TestMethod]
        public void ParseLookupLines_GivenLineWithSingleRomanChar_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "A" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseLookupLines_GivenMultipleLinesWithSingleRomanChar_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "A", "B", "C", "D" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }
        #endregion lines with roman chars alone

        #region lines with page numbers
        [TestMethod]
        public void ParseLookupLines_GivenLineWithPageNumber_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "123" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseLookupLines_GivenMultipleLinesWithPageNumber_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "123", "234", "987", "876" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }
        #endregion lines with page numbers

        #region junk lines
        [TestMethod]
        public void ParseLookupLines_GivenJunkLine_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "C࿮ခ࿰࿵࿶࿻࿴ ࿳࿶࿿࿲" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ParseLookupLines_GivenMultipleJunkLines_ReturnsEmptyLookupList()
        {
            // arrange
            string[] lines = new[] { "C࿵࿼࿽࿽࿶࿻࿴ ࿶ခ", "C࿼࿺࿲ ࿶࿻" };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().BeEmpty();
        }
        #endregion junk lines

        #region single line lookup
        [TestMethod]
        public void ParseLookupLines_GivenOneSingleLineLookupEntry_ReturnsListOfOneLookup()
        {
            // arrange
            string[] lines = new[]
            {
                "Alone: ᎤᏩᏌ [uwạsa] pg 64)"
            };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().HaveCount(1);

            result[0].English.Should().Be("Alone");
            result[0].Entries.Should().HaveCount(1);
            result[0].Entries[0].Page.Should().Be(64);
            result[0].Entries[0].Romanized.Should().Be("uwạsa");
            result[0].Entries[0].Syllabary.Should().Be("ᎤᏩᏌ");
        }

        [TestMethod]
        public void ParseLookupLines_GivenTwoSingleLineLookupEntries_ReturnsListOfTwoLookups()
        {
            // arrange
            string[] lines = new[]
            {
                "Ball game (stickball): ᎠᏁᏣ [ạnetsa] pg 25), ᎠᏁᏦᏗ [ạnetsodi] (pg 26)",
                "Ball player: ᎠᏁᏦᎥᏍᎩ [ạnetsọɂṿsgi] pg 26)"
            };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().HaveCount(2);

            result[0].English.Should().Be("Ball game (stickball)");
            result[0].Entries.Should().HaveCount(2);
            result[0].Entries[0].Page.Should().Be(25);
            result[0].Entries[0].Romanized.Should().Be("ạnetsa");
            result[0].Entries[0].Syllabary.Should().Be("ᎠᏁᏣ");
            result[0].Entries[1].Page.Should().Be(26);
            result[0].Entries[1].Romanized.Should().Be("ạnetsodi");
            result[0].Entries[1].Syllabary.Should().Be("ᎠᏁᏦᏗ");

            result[1].English.Should().Be("Ball player");
            result[1].Entries.Should().HaveCount(1);
            result[1].Entries[0].Page.Should().Be(26);
            result[1].Entries[0].Romanized.Should().Be("ạnetsọɂṿsgi");
            result[1].Entries[0].Syllabary.Should().Be("ᎠᏁᏦᎥᏍᎩ");
        }
        #endregion single line lookup

        #region multi line lookup
        [TestMethod]
        public void ParseLookupLines_GivenOneMultiLineLookupEntry_ReturnsListOfOneLookup()
        {
            // arrange
            string[] lines = new[]
            {
                "Being carried by it (a liquid): ᎦᎾᏨᏍᏗᎭ [gạntsvsdiha]",
                "pg 74)"
            };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().HaveCount(1);

            result[0].English.Should().Be("Being carried by it (a liquid)");
            result[0].Entries.Should().HaveCount(1);
            result[0].Entries[0].Page.Should().Be(74);
            result[0].Entries[0].Romanized.Should().Be("gạntsvsdiha");
            result[0].Entries[0].Syllabary.Should().Be("ᎦᎾᏨᏍᏗᎭ");
        }

        [TestMethod]
        public void ParseLookupLines_GivenTwoMultiLineLookupEntries_ReturnsListOfTwoLookups()
        {
            // arrange
            string[] lines = new[]
            {
                "Bumping his head: ᎠᏍᏆᎶᎠ [asgwạlọɂa] pg 30),",
                "ᎠᏓᏍᏆᎶᏍᏗᎭ [adasgwạlosdiha] (pg 37)",
                "Brake (on a vehicle): ᏗᎦᏅᏐᏍᏙᏗ [dịgạnṿsọsdohdi]",
                "pg 104)",
            };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Should().HaveCount(2);

            result[0].English.Should().Be("Bumping his head");
            result[0].Entries.Should().HaveCount(2);
            result[0].Entries[0].Page.Should().Be(30);
            result[0].Entries[0].Romanized.Should().Be("asgwạlọɂa");
            result[0].Entries[0].Syllabary.Should().Be("ᎠᏍᏆᎶᎠ");
            result[0].Entries[1].Page.Should().Be(37);
            result[0].Entries[1].Romanized.Should().Be("adasgwạlosdiha");
            result[0].Entries[1].Syllabary.Should().Be("ᎠᏓᏍᏆᎶᏍᏗᎭ");

            result[1].English.Should().Be("Brake (on a vehicle)");
            result[1].Entries.Should().HaveCount(1);
            result[1].Entries[0].Page.Should().Be(104);
            result[1].Entries[0].Romanized.Should().Be("dịgạnṿsọsdohdi");
            result[1].Entries[0].Syllabary.Should().Be("ᏗᎦᏅᏐᏍᏙᏗ");
        }
        #endregion multi line lookup

        #region combined
        [TestMethod]
        public void ParseLookupLines_CombinedDifferentLineTypes_AppropriateParseOccurs()
        {
            // arrange
            string[] lines = new[]
            {
                "Cottus spp: ᏥᏍᏆᎸᎾ [tsịsgwạlvna] pg 107)",
                "117",
                "Coughing: ᎤᏏᏩᏍᎦ [usihwah³sga] pg 60)",
                "Council house: ᏗᎦᎳᏫᎢᏍᏗ [dịgạlawịɂịsdi] pg 104)",
                "Covering it: ᎠᏙᏒᎥᏍᎦ [ado³svɂvsga] pg 42), ᎫᏌᎥᏍᎦ",
                "[gu³sạɂvsga] (pg 89)",
                "Cow: ᏩᎧ [wạka] pg 109)",
                "C࿿࿮࿯ ࿮࿽࿽࿹࿲",
                "Crab apple: ᏒᎦᏛ ᎢᎾᎨᎠᏁᎵ [svgdṿ inạgeanehl] pg 97)",
                "Cracking: ᎠᏛᏍᏆᎶᎠ [ạdṿsgwạloɂa] pg 43)"
            };

            // act
            var result = RavenRockLookupParser.ParseLookupLines(lines);

            // assert
            result.Count.Should().Be(7);

            result[0].English.Should().Be("Cottus spp");
            result[0].Entries.Should().HaveCount(1);
            result[0].Entries[0].Page.Should().Be(107);
            result[0].Entries[0].Romanized.Should().Be("tsịsgwạlvna");
            result[0].Entries[0].Syllabary.Should().Be("ᏥᏍᏆᎸᎾ");

            result[1].English.Should().Be("Coughing");
            result[1].Entries.Should().HaveCount(1);
            result[1].Entries[0].Page.Should().Be(60);
            result[1].Entries[0].Romanized.Should().Be("usihwah³sga");
            result[1].Entries[0].Syllabary.Should().Be("ᎤᏏᏩᏍᎦ");

            result[2].English.Should().Be("Council house");
            result[2].Entries.Should().HaveCount(1);
            result[2].Entries[0].Page.Should().Be(104);
            result[2].Entries[0].Romanized.Should().Be("dịgạlawịɂịsdi");
            result[2].Entries[0].Syllabary.Should().Be("ᏗᎦᎳᏫᎢᏍᏗ");

            result[3].English.Should().Be("Covering it");
            result[3].Entries.Should().HaveCount(2);
            result[3].Entries[0].Page.Should().Be(42);
            result[3].Entries[0].Romanized.Should().Be("ado³svɂvsga");
            result[3].Entries[0].Syllabary.Should().Be("ᎠᏙᏒᎥᏍᎦ");
            result[3].Entries[1].Page.Should().Be(89);
            result[3].Entries[1].Romanized.Should().Be("gu³sạɂvsga");
            result[3].Entries[1].Syllabary.Should().Be("ᎫᏌᎥᏍᎦ");

            result[4].English.Should().Be("Cow");
            result[4].Entries.Should().HaveCount(1);
            result[4].Entries[0].Page.Should().Be(109);
            result[4].Entries[0].Romanized.Should().Be("wạka");
            result[4].Entries[0].Syllabary.Should().Be("ᏩᎧ");

            result[5].English.Should().Be("Crab apple");
            result[5].Entries.Should().HaveCount(1);
            result[5].Entries[0].Page.Should().Be(97);
            result[5].Entries[0].Romanized.Should().Be("svgdṿ inạgeanehl");
            result[5].Entries[0].Syllabary.Should().Be("ᏒᎦᏛ ᎢᎾᎨᎠᏁᎵ");

            result[6].English.Should().Be("Cracking");
            result[6].Entries.Should().HaveCount(1);
            result[6].Entries[0].Page.Should().Be(43);
            result[6].Entries[0].Romanized.Should().Be("ạdṿsgwạloɂa");
            result[6].Entries[0].Syllabary.Should().Be("ᎠᏛᏍᏆᎶᎠ");
        }
        #endregion combined
    }
}

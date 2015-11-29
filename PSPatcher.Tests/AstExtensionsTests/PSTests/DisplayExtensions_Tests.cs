using FluentAssertions;
using NUnit.Framework;
using PSPatcher.Core.Storage;

namespace PSPatcher.Tests.AstExtensionsTests.PSTests
{
    [TestFixture]
    public class DisplayExtensions_Tests
    {
        private IStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new StringStorage();
        }

        [Test]
        public void GetText_should_return_source_code()
        {
            var script = "Set-Variable foo_arg -value 99";
            var ast = storage.Open(script);

            var text = ast.GetText();

            text.Should().Be(script);
        }
    }
}
using System;
using FluentAssertions;
using NUnit.Framework;
using PSPatcher.Core.AstExtensions.PS;
using PSPatcher.Core.Storage;

namespace PSPatcher.Tests.AstExtensionsTests.PSTests
{
    [TestFixture]
    public class InvokeExtensions_Tests
    {
        private StringStorage storage;

        [SetUp]
        public void SetUp()
        {
            storage = new StringStorage();
        }

        [Test]
        public void Invoke_should_get_result()
        {
            var newGuid = Guid.NewGuid().ToString();
            var ast = storage.Open("Write " + newGuid);

            var result = ast.Invoke();

            result.TrimEnd().Should().Be(newGuid);
        }

        [Test]
        public void Invoke_should_get_result_when_script_contain_write_host()
        {
            var newGuid = Guid.NewGuid().ToString();
            var ast = storage.Open("Write-Host " + newGuid);

            var result = ast.Invoke();

            result.TrimEnd().Should().Be(newGuid);
        }
    }
}
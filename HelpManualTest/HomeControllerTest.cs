using System;
using Xunit;
using HelpManual.Controllers;
using HelpManual.Entities;

namespace HelpManual.UnitTests.Controllers
{
    /// <summary>
    /// An Abritary Test
    /// </summary>
    public class HomeControllerTests
    {
        private HelpManualDbContext _context;

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(17)]
        public void PrimeTrue(int value)
        {
            var home = new HomeController(_context);
            var result = home.IsPrime(value);

            Assert.True(result, $"{value} should be prime");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        public void PrimeFalse(int value)
        {
            var home = new HomeController(_context);
            var result = home.IsPrime(value);

            Assert.False(result, $"{value} should not be prime");
        }
    }
}

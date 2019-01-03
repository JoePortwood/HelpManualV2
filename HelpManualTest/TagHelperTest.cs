using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HelpManual.Helpers;

namespace HelpManualTest
{
    public class TagHelperTest
    {
        [Fact]
        public void ReturnList()
        {
            var result = TagHelper.Tag(null, "select", "Drop Test", "Drop", "Yes,No", null, null);
            Assert.Equal("<select id=\"Drop1Test\"><option text=\"Yes\" value=\"Yes\">Yes</option><option text=\"No\" value=\"No\">No</option></select>", result.ToString());
        }
    }
}

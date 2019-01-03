using HelpManual.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text.Encodings.Web;

namespace HelpManual.Helpers
{
    public static class TagHelper
    {
        public static IHtmlContent Tag(this IHtmlHelper helper, string tag, string id, string val, string options, byte[] src)
        {
            return Tag(helper, tag, id, val, options, src, null);
        }

        public static IHtmlContent Tag(this IHtmlHelper helper, string tag, string id, string val, string options, byte[] src, object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder(tag);

            // Create valid id
            builder.GenerateId(id, "1");

            switch (tag)
            {
                case "select":
                    TagBuilder option;
                    string[] optionsList = options.Split(',');

                    //Gets each object in the list for the options and adds them to the dropdown
                    foreach (string optionTxt in optionsList)
                    {
                        option = new TagBuilder("option");
                        option.MergeAttribute("value", optionTxt);
                        option.MergeAttribute("text", optionTxt);

                        option.InnerHtml.Append(optionTxt);

                        builder.InnerHtml.AppendHtml(EncodeHtml(option));
                    }
                    break;
                case "img":
                    string imreBase64Data = Convert.ToBase64String(src);
                    builder.MergeAttribute("src", string.Format("data:image/png;base64,{0}", imreBase64Data));
                    //Set arbitary values for height and width
                    builder.MergeAttribute("height", "150");
                    builder.MergeAttribute("width", "150");
                    builder.TagRenderMode = TagRenderMode.SelfClosing;
                    break;
                case "p":
                case "h1":
                    builder.InnerHtml.AppendHtml(val);

                    // Render tag - don't encode as this needs return the Html
                    return builder;
                default:
                    builder.InnerHtml.Append(val);

                    // Add attributes
                    builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                    break;
            }

            // Render tag
            return EncodeHtml(builder);
        }

        private static HtmlString EncodeHtml(TagBuilder builder)
        {
            string htmlOutput;
            using (var writer = new StringWriter())
            {
                builder.WriteTo(writer, HtmlEncoder.Default);
                htmlOutput = writer.ToString();
            }
            return new HtmlString(htmlOutput);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gedcom.viewer
{
    /// <summary>Class to represent user options.</summary>
    public class UserOptions
    {
        #region Member Variables

        private SimpleFont fontBody;
        private SimpleFont fontHeader;
        private SimpleFont fontSmall;
        private SimpleFont fontHtmlSuperscript;

        #endregion

        #region Class Constructors

        /// <summary>Empty class constructor.</summary>
        public UserOptions()
        {
            fontBody = new SimpleFont("Verdana", 9f);
            fontHeader = new SimpleFont("Verdana", 12f);
            fontSmall=new SimpleFont("Verdana", 8f);
            fontHtmlSuperscript=new SimpleFont("Verdana", 8f);
        }

    #endregion

    #region Html

    /// <summary>Adds the html headers to the specified html content, ready for display.</summary>
    /// <param name="htmlContent">Specifies the body content of the html page.</param>
    /// <returns>Fully specified html ready for display.</returns>
    public string renderHtml(string htmlContent)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.Append(htmlStyle());
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.Append(htmlContent);
            html.AppendLine("</body>");
            html.Append("</html>");

            // Return the Html built.
            return html.ToString();
        }



        /// <summary>Returns the standard Html style block for Html output.</summary>
        /// <returns>The standard Html style block.</returns>
        private string htmlStyle()
        {
            StringBuilder htmlStyles = new StringBuilder();

            htmlStyles.AppendLine("<style><!--");
            htmlStyles.AppendLine("body {background-color: #E0E0E0;}");
            htmlStyles.AppendLine("p {font-family: '" + fontBody.name + "'; font-size: " + fontBody.size.ToString() + "pt; margin-top: 3pt; margin-bottom: 3pt;line-height: " + (fontBody.size + 6).ToString() + "pt;}");
            htmlStyles.AppendLine("td {font-family: '" + fontBody.name + "'; font-size:" + fontBody.size.ToString() + "pt; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine("h1 {font-family: '" + fontHeader.name + "'; font-size:" + fontHeader.size.ToString() + "pt; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine("legend {font-size: 8pt; color: grey;}");
            htmlStyles.AppendLine("fieldset {border: 1pt solid grey;}");
            htmlStyles.AppendLine("pre {border: 1pt solid black; background-color: white;}");
            htmlStyles.AppendLine("a {text-decoration: none;}");
            htmlStyles.AppendLine("a:hover {text-decoration: underline;}");
            htmlStyles.AppendLine("a:link {color: black;}");
            htmlStyles.AppendLine("a:visited {color: black;}");
            htmlStyles.AppendLine(".superscript {font-family: '" + fontHtmlSuperscript.name + "'; font-size:" + fontHtmlSuperscript.size.ToString() + "pt; vertical-align: super;}"); // line-height: " + (fontBody.Size + 6).ToString() + "pt;
            htmlStyles.AppendLine(".small {font-family: '" + fontSmall.name + "'; font-size: " + fontSmall.size.ToString() + "pt; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine(".background {font-family: 'Verdana'; font-size: 8pt; color: silver; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine(".census {font-family: 'Times New Roman'; font-size: 8pt; color: darkcyan; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine(".marriage {font-family: 'Times New Roman'; font-size: 8pt; color: seagreen; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine(".birth {font-family: 'Times New Roman'; font-size: 8pt; color: orangered; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine(".death {font-family: 'Times New Roman'; font-size: 8pt; color: purple; margin-top: 3pt; margin-bottom: 3pt}");
            htmlStyles.AppendLine("--> </style>");

            // Return the style block.
            return htmlStyles.ToString();
        }

        #endregion

    }

    /// <summary>Class to represent a font in the user options.</summary>
    public class SimpleFont
    {
        #region Member Variables

        /// <summary>Name of the font.</summary>
        public string name;

        /// <summary>Size of the font.</summary>
        public float size;

        #endregion

        #region Constructors

        public SimpleFont(string defaultName, float defaultSize)
        {
            name = defaultName;
            size = defaultSize;
        }

        #endregion

        #region Properties

        /// <summary>The size of the font as an integer.</summary>
        /// <returns>The size of the font as an integer.</returns>
        public int fontSize()
        {
            return (int)Math.Round(size);
        }
        #endregion
    }
}

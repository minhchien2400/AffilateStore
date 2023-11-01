#region Assembly HtmlAgilityPack, Version=1.11.42.0, Culture=neutral, PublicKeyToken=bd319b19eaf3b43a
// C:\Users\leon.tran\.nuget\packages\htmlagilitypack\1.11.42\lib\netstandard2.0\HtmlAgilityPack.dll
// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace AffiliateStoreBE.Common.Models
{
    //
    // Summary:
    //     Represents a complete HTML document.
    public class HtmlDocument
    {
        private enum ParseState
        {
            Text,
            WhichTag,
            Tag,
            BetweenAttributes,
            EmptyTag,
            AttributeName,
            AttributeBeforeEquals,
            AttributeAfterEquals,
            AttributeValue,
            Comment,
            QuotedAttributeValue,
            ServerSideCode,
            PcData
        }

        internal static bool _disableBehaviorTagP = true;

        //
        // Summary:
        //     Defines the max level we would go deep into the html document
        private static int _maxDepthLevel = int.MaxValue;

        private int _c;

        private Crc32 _crc32;

        private HtmlAttribute _currentattribute;

        private HtmlNode _currentnode;

        private Encoding _declaredencoding;
        private HtmlNode _documentnode;
        private bool _fullcomment;
        private int _index;

        internal Dictionary<string, HtmlNode> Lastnodes = new Dictionary<string, HtmlNode>();

        private HtmlNode _lastparentnode;
        private int _line;

        private int _lineposition;

        private int _maxlineposition;

        internal Dictionary<string, HtmlNode> Nodesid;

        private ParseState _oldstate;

        private bool _onlyDetectEncoding;

        internal Dictionary<int, HtmlNode> Openednodes;

        private List<HtmlParseError> _parseerrors = new List<HtmlParseError>();

        private string _remainder;

        private int _remainderOffset;

        private ParseState _state;

        private Encoding _streamencoding;

        private bool _useHtmlEncodingForStream;

        //
        // Summary:
        //     The HtmlDocument Text. Careful if you modify it.
        public string Text;

        //
        // Summary:
        //     True to stay backward compatible with previous version of HAP. This option does
        //     not guarantee 100% compatibility.
        public bool BackwardCompatibility = true;

        //
        // Summary:
        //     Adds Debugging attributes to node. Default is false.
        public bool OptionAddDebuggingAttributes;

        //
        // Summary:
        //     Defines if closing for non closed nodes must be done at the end or directly in
        //     the document. Setting this to true can actually change how browsers render the
        //     page. Default is false.
        public bool OptionAutoCloseOnEnd;

        //
        // Summary:
        //     Defines if non closed nodes will be checked at the end of parsing. Default is
        //     true.
        public bool OptionCheckSyntax = true;

        //
        // Summary:
        //     Defines if a checksum must be computed for the document while parsing. Default
        //     is false.
        public bool OptionComputeChecksum;

        //
        // Summary:
        //     Defines if SelectNodes method will return null or empty collection when no node
        //     matched the XPath expression. Setting this to true will return empty collection
        //     and false will return null. Default is false.
        public bool OptionEmptyCollection;

        //
        // Summary:
        //     True to disable, false to enable the server side code.
        public bool DisableServerSideCode;

        //
        // Summary:
        //     Defines the default stream encoding to use. Default is System.Text.Encoding.Default.
        public Encoding OptionDefaultStreamEncoding;

        //
        // Summary:
        //     Force to take the original comment instead of creating it
        public bool OptionXmlForceOriginalComment;

        //
        // Summary:
        //     Defines if source text must be extracted while parsing errors. If the document
        //     has a lot of errors, or cascading errors, parsing performance can be dramatically
        //     affected if set to true. Default is false.
        public bool OptionExtractErrorSourceText;

        //
        // Summary:
        //     Defines the maximum length of source text or parse errors. Default is 100.
        public int OptionExtractErrorSourceTextMaxLength = 100;

        //
        // Summary:
        //     Defines if LI, TR, TH, TD tags must be partially fixed when nesting errors are
        //     detected. Default is false.
        public bool OptionFixNestedTags;

        //
        // Summary:
        //     Defines if output must conform to XML, instead of HTML. Default is false.
        public bool OptionOutputAsXml;

        //
        // Summary:
        //     If used together with HtmlAgilityPack.HtmlDocument.OptionOutputAsXml and enabled,
        //     Xml namespaces in element names are preserved. Default is false.
        public bool OptionPreserveXmlNamespaces;

        //
        // Summary:
        //     Defines if attribute value output must be optimized (not bound with double quotes
        //     if it is possible). Default is false.
        public bool OptionOutputOptimizeAttributeValues;

        //
        // Summary:
        //     Defines the global attribute value quote. When specified, it will always win.
        public AttributeValueQuote? GlobalAttributeValueQuote;

        //
        // Summary:
        //     Defines if name must be output with it's original case. Useful for asp.net tags
        //     and attributes. Default is false.
        public bool OptionOutputOriginalCase;

        //
        // Summary:
        //     Defines if name must be output in uppercase. Default is false.
        public bool OptionOutputUpperCase;

        //
        // Summary:
        //     Defines if declared encoding must be read from the document. Declared encoding
        //     is determined using the meta http-equiv="content-type" content="text/html;charset=XXXXX"
        //     html node. Default is true.
        public bool OptionReadEncoding = true;

        //
        // Summary:
        //     Defines the name of a node that will throw the StopperNodeException when found
        //     as an end node. Default is null.
        public string OptionStopperNodeName;

        //
        // Summary:
        //     Defines if attributes should use original names by default, rather than lower
        //     case. Default is false.
        public bool OptionDefaultUseOriginalName;

        //
        // Summary:
        //     Defines if the 'id' attribute must be specifically used. Default is true.
        public bool OptionUseIdAttribute = true;

        //
        // Summary:
        //     Defines if empty nodes must be written as closed during output. Default is false.
        public bool OptionWriteEmptyNodes;

        //
        // Summary:
        //     The max number of nested child nodes. Added to prevent stackoverflow problem
        //     when a page has tens of thousands of opening html tags with no closing tags
        public int OptionMaxNestedChildNodes;

        internal static readonly string HtmlExceptionRefNotChild = "Reference node must be a child of this node";

        internal static readonly string HtmlExceptionUseIdAttributeFalse = "You need to set UseIdAttribute property to true to enable this feature";

        internal static readonly string HtmlExceptionClassDoesNotExist = "Class name doesn't exist";

        internal static readonly string HtmlExceptionClassExists = "Class name already exists";

        internal static readonly Dictionary<string, string[]> HtmlResetters = new Dictionary<string, string[]>
        {
            {
                "li",
                new string[2] { "ul", "ol" }
            },
            {
                "tr",
                new string[1] { "table" }
            },
            {
                "th",
                new string[2] { "tr", "table" }
            },
            {
                "td",
                new string[2] { "tr", "table" }
            }
        };

        private static List<string> BlockAttributes = new List<string> { "\"", "'" };

        //
        // Summary:
        //     True to disable, false to enable the behavior tag p.
        public static bool DisableBehaviorTagP
        {
            get
            {
                return _disableBehaviorTagP;
            }
            set
            {
                if (value)
                {
                    if (HtmlNode.ElementsFlags.ContainsKey("p"))
                    {
                        HtmlNode.ElementsFlags.Remove("p");
                    }
                }
                else if (!HtmlNode.ElementsFlags.ContainsKey("p"))
                {
                    HtmlNode.ElementsFlags.Add("p", HtmlElementFlag.Empty | HtmlElementFlag.Closed);
                }

                _disableBehaviorTagP = value;
            }
        }

        //
        // Summary:
        //     Default builder to use in the HtmlDocument constructor
        public static Action<HtmlDocument> DefaultBuilder { get; set; }

        //
        // Summary:
        //     Action to execute before the Parse is executed
        public Action<HtmlDocument> ParseExecuting { get; set; }

        //
        // Summary:
        //     Gets the parsed text.
        //
        // Value:
        //     The parsed text.
        public string ParsedText => Text;

        //
        // Summary:
        //     Defines the max level we would go deep into the html document. If this depth
        //     level is exceeded, and exception is thrown.
        public static int MaxDepthLevel
        {
            get
            {
                return _maxDepthLevel;
            }
            set
            {
                _maxDepthLevel = value;
            }
        }

        //
        // Summary:
        //     Gets the document CRC32 checksum if OptionComputeChecksum was set to true before
        //     parsing, 0 otherwise.
        public int CheckSum
        {
            get
            {
                if (_crc32 != null)
                {
                    return (int)_crc32.CheckSum;
                }

                return 0;
            }
        }

        //
        // Summary:
        //     Gets the document's declared encoding. Declared encoding is determined using
        //     the meta http-equiv="content-type" content="text/html;charset=XXXXX" html node
        //     (pre-HTML5) or the meta charset="XXXXX" html node (HTML5).
        public Encoding DeclaredEncoding => _declaredencoding;

        //
        // Summary:
        //     Gets the root node of the document.
        public HtmlNode DocumentNode => _documentnode;

        //
        // Summary:
        //     Gets the document's output encoding.
        public Encoding Encoding => GetOutEncoding();

        //
        // Summary:
        //     Gets a list of parse errors found in the document.
        public IEnumerable<HtmlParseError> ParseErrors => _parseerrors;

        //
        // Summary:
        //     Gets the remaining text. Will always be null if OptionStopperNodeName is null.
        public string Remainder => _remainder;

        //
        // Summary:
        //     Gets the offset of Remainder in the original Html text. If OptionStopperNodeName
        //     is null, this will return the length of the original Html text.
        public int RemainderOffset => _remainderOffset;

        //
        // Summary:
        //     Gets the document's stream encoding.
        public Encoding StreamEncoding => _streamencoding;

        //
        // Summary:
        //     Creates an instance of an HTML document.
        public HtmlDocument()
        {
            if (DefaultBuilder != null)
            {
                DefaultBuilder(this);
            }

            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            OptionDefaultStreamEncoding = Encoding.Default;
        }

        //
        // Summary:
        //     Gets a valid XML name.
        //
        // Parameters:
        //   name:
        //     Any text.
        //
        // Returns:
        //     A string that is a valid XML name.
        public static string GetXmlName(string name)
        {
            return GetXmlName(name, isAttribute: false, preserveXmlNamespaces: false);
        }

        public void UseAttributeOriginalName(string tagName)
        {
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)DocumentNode.SelectNodes("//" + tagName))
            {
                foreach (HtmlAttribute item2 in (IEnumerable<HtmlAttribute>)item.Attributes)
                {
                    item2.UseOriginalName = true;
                }
            }
        }

        public static string GetXmlName(string name, bool isAttribute, bool preserveXmlNamespaces)
        {
            string text = string.Empty;
            bool flag = true;
            for (int i = 0; i < name.Length; i++)
            {
                if ((name[i] >= 'a' && name[i] <= 'z') || (name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= '0' && name[i] <= '9') || ((isAttribute || preserveXmlNamespaces) && name[i] == ':') || name[i] == '_' || name[i] == '-' || name[i] == '.')
                {
                    text += name[i];
                    continue;
                }

                flag = false;
                byte[] bytes = Encoding.UTF8.GetBytes(new char[1] { name[i] });
                for (int j = 0; j < bytes.Length; j++)
                {
                    text += bytes[j].ToString("x2");
                }

                text += "_";
            }

            if (flag)
            {
                return text;
            }

            return "_" + text;
        }

        //
        // Summary:
        //     Applies HTML encoding to a specified string.
        //
        // Parameters:
        //   html:
        //     The input string to encode. May not be null.
        //
        // Returns:
        //     The encoded string.
        public static string HtmlEncode(string html)
        {
            return HtmlEncodeWithCompatibility(html);
        }

        internal static string HtmlEncodeWithCompatibility(string html, bool backwardCompatibility = true)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            return (backwardCompatibility ? new Regex("&(?!(amp;)|(lt;)|(gt;)|(quot;))", RegexOptions.IgnoreCase) : new Regex("&(?!(amp;)|(lt;)|(gt;)|(quot;)|(nbsp;)|(reg;))", RegexOptions.IgnoreCase)).Replace(html, "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }

        //
        // Summary:
        //     Determines if the specified character is considered as a whitespace character.
        //
        // Parameters:
        //   c:
        //     The character to check.
        //
        // Returns:
        //     true if if the specified character is considered as a whitespace character.
        public static bool IsWhiteSpace(int c)
        {
            if (c == 10 || c == 13 || c == 32 || c == 9)
            {
                return true;
            }

            return false;
        }

        //
        // Summary:
        //     Creates an HTML attribute with the specified name.
        //
        // Parameters:
        //   name:
        //     The name of the attribute. May not be null.
        //
        // Returns:
        //     The new HTML attribute.
        public HtmlAttribute CreateAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            HtmlAttribute htmlAttribute = CreateAttribute();
            htmlAttribute.Name = name;
            return htmlAttribute;
        }

        //
        // Summary:
        //     Creates an HTML attribute with the specified name.
        //
        // Parameters:
        //   name:
        //     The name of the attribute. May not be null.
        //
        //   value:
        //     The value of the attribute.
        //
        // Returns:
        //     The new HTML attribute.
        public HtmlAttribute CreateAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            HtmlAttribute htmlAttribute = CreateAttribute(name);
            htmlAttribute.Value = value;
            return htmlAttribute;
        }

        //
        // Summary:
        //     Creates an HTML comment node.
        //
        // Returns:
        //     The new HTML comment node.
        public HtmlCommentNode CreateComment()
        {
            return (HtmlCommentNode)CreateNode(HtmlNodeType.Comment);
        }

        //
        // Summary:
        //     Creates an HTML comment node with the specified comment text.
        //
        // Parameters:
        //   comment:
        //     The comment text. May not be null.
        //
        // Returns:
        //     The new HTML comment node.
        public HtmlCommentNode CreateComment(string comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }

            HtmlCommentNode htmlCommentNode = CreateComment();
            htmlCommentNode.Comment = comment;
            return htmlCommentNode;
        }

        //
        // Summary:
        //     Creates an HTML element node with the specified name.
        //
        // Parameters:
        //   name:
        //     The qualified name of the element. May not be null.
        //
        // Returns:
        //     The new HTML node.
        public HtmlNode CreateElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            HtmlNode htmlNode = CreateNode(HtmlNodeType.Element);
            htmlNode.Name = name;
            return htmlNode;
        }

        //
        // Summary:
        //     Creates an HTML text node.
        //
        // Returns:
        //     The new HTML text node.
        public HtmlTextNode CreateTextNode()
        {
            return (HtmlTextNode)CreateNode(HtmlNodeType.Text);
        }

        //
        // Summary:
        //     Creates an HTML text node with the specified text.
        //
        // Parameters:
        //   text:
        //     The text of the node. May not be null.
        //
        // Returns:
        //     The new HTML text node.
        public HtmlTextNode CreateTextNode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            HtmlTextNode htmlTextNode = CreateTextNode();
            htmlTextNode.Text = text;
            return htmlTextNode;
        }

        //
        // Summary:
        //     Detects the encoding of an HTML stream.
        //
        // Parameters:
        //   stream:
        //     The input stream. May not be null.
        //
        // Returns:
        //     The detected encoding.
        public Encoding DetectEncoding(Stream stream)
        {
            return DetectEncoding(stream, checkHtml: false);
        }

        //
        // Summary:
        //     Detects the encoding of an HTML stream.
        //
        // Parameters:
        //   stream:
        //     The input stream. May not be null.
        //
        //   checkHtml:
        //     The html is checked.
        //
        // Returns:
        //     The detected encoding.
        public Encoding DetectEncoding(Stream stream, bool checkHtml)
        {
            _useHtmlEncodingForStream = checkHtml;
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            return DetectEncoding(new StreamReader(stream));
        }

        //
        // Summary:
        //     Detects the encoding of an HTML text provided on a TextReader.
        //
        // Parameters:
        //   reader:
        //     The TextReader used to feed the HTML. May not be null.
        //
        // Returns:
        //     The detected encoding.
        public Encoding DetectEncoding(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            _onlyDetectEncoding = true;
            if (OptionCheckSyntax)
            {
                Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                Openednodes = null;
            }

            if (OptionUseIdAttribute)
            {
                Nodesid = new Dictionary<string, HtmlNode>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                Nodesid = null;
            }

            StreamReader streamReader = reader as StreamReader;
            if (streamReader != null && !_useHtmlEncodingForStream)
            {
                Text = streamReader.ReadToEnd();
                _streamencoding = streamReader.CurrentEncoding;
                return _streamencoding;
            }

            _streamencoding = null;
            _declaredencoding = null;
            Text = reader.ReadToEnd();
            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            try
            {
                Parse();
            }
            catch (EncodingFoundException ex)
            {
                return ex.Encoding;
            }

            return _streamencoding;
        }

        //
        // Summary:
        //     Detects the encoding of an HTML text.
        //
        // Parameters:
        //   html:
        //     The input html text. May not be null.
        //
        // Returns:
        //     The detected encoding.
        public Encoding DetectEncodingHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            using StringReader reader = new StringReader(html);
            return DetectEncoding(reader);
        }

        //
        // Summary:
        //     Gets the HTML node with the specified 'id' attribute value.
        //
        // Parameters:
        //   id:
        //     The attribute id to match. May not be null.
        //
        // Returns:
        //     The HTML node with the matching id or null if not found.
        public HtmlNode GetElementbyId(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (Nodesid == null)
            {
                throw new Exception(HtmlExceptionUseIdAttributeFalse);
            }

            if (!Nodesid.ContainsKey(id))
            {
                return null;
            }

            return Nodesid[id];
        }

        //
        // Summary:
        //     Loads an HTML document from a stream.
        //
        // Parameters:
        //   stream:
        //     The input stream.
        public void Load(Stream stream)
        {
            Load(new StreamReader(stream, OptionDefaultStreamEncoding));
        }

        //
        // Summary:
        //     Loads an HTML document from a stream.
        //
        // Parameters:
        //   stream:
        //     The input stream.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the stream.
        public void Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, detectEncodingFromByteOrderMarks));
        }

        //
        // Summary:
        //     Loads an HTML document from a stream.
        //
        // Parameters:
        //   stream:
        //     The input stream.
        //
        //   encoding:
        //     The character encoding to use.
        public void Load(Stream stream, Encoding encoding)
        {
            Load(new StreamReader(stream, encoding));
        }

        //
        // Summary:
        //     Loads an HTML document from a stream.
        //
        // Parameters:
        //   stream:
        //     The input stream.
        //
        //   encoding:
        //     The character encoding to use.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the stream.
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks));
        }

        //
        // Summary:
        //     Loads an HTML document from a stream.
        //
        // Parameters:
        //   stream:
        //     The input stream.
        //
        //   encoding:
        //     The character encoding to use.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the stream.
        //
        //   buffersize:
        //     The minimum buffer size.
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

        //
        // Summary:
        //     Loads the HTML document from the specified TextReader.
        //
        // Parameters:
        //   reader:
        //     The TextReader used to feed the HTML data into the document. May not be null.
        public void Load(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            _onlyDetectEncoding = false;
            if (OptionCheckSyntax)
            {
                Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                Openednodes = null;
            }

            if (OptionUseIdAttribute)
            {
                Nodesid = new Dictionary<string, HtmlNode>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                Nodesid = null;
            }

            StreamReader streamReader = reader as StreamReader;
            if (streamReader != null)
            {
                try
                {
                    streamReader.Peek();
                }
                catch (Exception)
                {
                }

                _streamencoding = streamReader.CurrentEncoding;
            }
            else
            {
                _streamencoding = null;
            }

            _declaredencoding = null;
            Text = reader.ReadToEnd();
            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            Parse();
            if (!OptionCheckSyntax || Openednodes == null)
            {
                return;
            }

            foreach (HtmlNode value in Openednodes.Values)
            {
                if (!value._starttag)
                {
                    continue;
                }

                string text;
                if (OptionExtractErrorSourceText)
                {
                    text = value.OuterHtml;
                    if (text.Length > OptionExtractErrorSourceTextMaxLength)
                    {
                        text = text.Substring(0, OptionExtractErrorSourceTextMaxLength);
                    }
                }
                else
                {
                    text = string.Empty;
                }

                AddError(HtmlParseErrorCode.TagNotClosed, value._line, value._lineposition, value._streamposition, text, "End tag </" + value.Name + "> was not found");
            }

            Openednodes.Clear();
        }

        //
        // Summary:
        //     Loads the HTML document from the specified string.
        //
        // Parameters:
        //   html:
        //     String containing the HTML document to load. May not be null.
        public void LoadHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            using StringReader reader = new StringReader(html);
            Load(reader);
        }

        //
        // Summary:
        //     Saves the HTML document to the specified stream.
        //
        // Parameters:
        //   outStream:
        //     The stream to which you want to save.
        

        

        

        //
        // Summary:
        //     Loads an HTML document from a file.
        //
        // Parameters:
        //   path:
        //     The complete file path to be read. May not be null.
        public void Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            using StreamReader reader = new StreamReader(path, OptionDefaultStreamEncoding);
            Load(reader);
        }

        //
        // Summary:
        //     Loads an HTML document from a file.
        //
        // Parameters:
        //   path:
        //     The complete file path to be read. May not be null.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the file.
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            using StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        //
        // Summary:
        //     Loads an HTML document from a file.
        //
        // Parameters:
        //   path:
        //     The complete file path to be read. May not be null.
        //
        //   encoding:
        //     The character encoding to use. May not be null.
        public void Load(string path, Encoding encoding)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            using StreamReader reader = new StreamReader(path, encoding);
            Load(reader);
        }

        //
        // Summary:
        //     Loads an HTML document from a file.
        //
        // Parameters:
        //   path:
        //     The complete file path to be read. May not be null.
        //
        //   encoding:
        //     The character encoding to use. May not be null.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the file.
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            using StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks);
            Load(reader);
        }

        //
        // Summary:
        //     Loads an HTML document from a file.
        //
        // Parameters:
        //   path:
        //     The complete file path to be read. May not be null.
        //
        //   encoding:
        //     The character encoding to use. May not be null.
        //
        //   detectEncodingFromByteOrderMarks:
        //     Indicates whether to look for byte order marks at the beginning of the file.
        //
        //   buffersize:
        //     The minimum buffer size.
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            using StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, buffersize);
            Load(reader);
        }


    }
}

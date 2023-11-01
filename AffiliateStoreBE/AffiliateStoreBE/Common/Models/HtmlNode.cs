using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml.XPath;
using System.Xml;

namespace AffiliateStoreBE.Common.Models
{
    //
    // Summary:
    //     Represents an HTML node.
    [DebuggerDisplay("Name: {OriginalName}")]
    public class HtmlNode : IXPathNavigable
    {
        internal const string DepthLevelExceptionMessage = "The document is too complex to parse";

        internal HtmlAttributeCollection _attributes;

        internal HtmlNodeCollection _childnodes;

        internal HtmlNode _endnode;

        private bool _changed;

        internal string _innerhtml;

        internal int _innerlength;

        internal int _innerstartindex;

        internal int _line;

        internal int _lineposition;

        private string _name;
        internal int _namelength;

        internal int _namestartindex;

        internal HtmlNode _nextnode;

        internal HtmlNodeType _nodetype;

        internal string _outerhtml;

        internal int _outerlength;

        internal int _outerstartindex;

        private string _optimizedName;

        internal HtmlDocument _ownerdocument;

        internal HtmlNode _parentnode;

        internal HtmlNode _prevnode;

        internal HtmlNode _prevwithsamename;

        internal bool _starttag;

        internal int _streamposition;

        internal bool _isImplicitEnd;

        internal bool _isHideInnerText;

        //
        // Summary:
        //     Gets the name of a comment node. It is actually defined as '#comment'.
        public static readonly string HtmlNodeTypeNameComment;

        //
        // Summary:
        //     Gets the name of the document node. It is actually defined as '#document'.
        public static readonly string HtmlNodeTypeNameDocument;

        //
        // Summary:
        //     Gets the name of a text node. It is actually defined as '#text'.
        public static readonly string HtmlNodeTypeNameText;

        //
        // Summary:
        //     Gets a collection of flags that define specific behaviors for specific element
        //     nodes. The table contains a DictionaryEntry list with the lowercase tag name
        //     as the Key, and a combination of HtmlElementFlags as the Value.
        public static Dictionary<string, HtmlElementFlag> ElementsFlags;

        //
        // Summary:
        //     Gets the collection of HTML attributes for this node. May not be null.
        public HtmlAttributeCollection Attributes
        {
            get
            {
                if (!HasAttributes)
                {
                    _attributes = new HtmlAttributeCollection(this);
                }

                return _attributes;
            }
            internal set
            {
                _attributes = value;
            }
        }

        //
        // Summary:
        //     Gets all the children of the node.
        public HtmlNodeCollection ChildNodes
        {
            get
            {
                return _childnodes ?? (_childnodes = new HtmlNodeCollection(this));
            }
            internal set
            {
                _childnodes = value;
            }
        }

        //
        // Summary:
        //     Gets a value indicating if this node has been closed or not.
        public bool Closed => _endnode != null;

        //
        // Summary:
        //     Gets the collection of HTML attributes for the closing tag. May not be null.
        public HtmlAttributeCollection ClosingAttributes
        {
            get
            {
                if (HasClosingAttributes)
                {
                    return _endnode.Attributes;
                }

                return new HtmlAttributeCollection(this);
            }
        }

        //
        // Summary:
        //     Gets the closing tag of the node, null if the node is self-closing.
        public HtmlNode EndNode => _endnode;

        //
        // Summary:
        //     Gets the first child of the node.
        public HtmlNode FirstChild
        {
            get
            {
                if (HasChildNodes)
                {
                    return _childnodes[0];
                }

                return null;
            }
        }

        //
        // Summary:
        //     Gets a value indicating whether the current node has any attributes.
        public bool HasAttributes
        {
            get
            {
                if (_attributes == null)
                {
                    return false;
                }

                if (_attributes.Count <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        //
        // Summary:
        //     Gets a value indicating whether this node has any child nodes.
        public bool HasChildNodes
        {
            get
            {
                if (_childnodes == null)
                {
                    return false;
                }

                if (_childnodes.Count <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        //
        // Summary:
        //     Gets a value indicating whether the current node has any attributes on the closing
        //     tag.
        public bool HasClosingAttributes
        {
            get
            {
                if (_endnode == null || _endnode == this)
                {
                    return false;
                }

                if (_endnode._attributes == null)
                {
                    return false;
                }

                if (_endnode._attributes.Count <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        //
        // Summary:
        //     Gets or sets the value of the 'id' HTML attribute. The document must have been
        //     parsed using the OptionUseIdAttribute set to true.
        public string Id
        {
            get
            {
                if (_ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }

                return GetId();
            }
            set
            {
                if (_ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                SetId(value);
            }
        }

        //
        // Summary:
        //     Gets or Sets the HTML between the start and end tags of the object.
        public virtual string InnerHtml
        {
            get
            {
                if (_changed)
                {
                    UpdateHtml();
                    return _innerhtml;
                }

                if (_innerhtml != null)
                {
                    return _innerhtml;
                }

                if (_innerstartindex < 0 || _innerlength < 0)
                {
                    return string.Empty;
                }

                return _ownerdocument.Text.Substring(_innerstartindex, _innerlength);
            }
            set
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(value);
                RemoveAllChildren();
                AppendChildren(htmlDocument.DocumentNode.ChildNodes);
            }
        }

        //
        // Summary:
        //     Gets the text between the start and end tags of the object.
        public virtual string InnerText
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                int depthLevel = 0;
                string name = Name;
                if (name != null)
                {
                    name = name.ToLowerInvariant();
                    bool isDisplayScriptingText = name == "head" || name == "script" || name == "style";
                    InternalInnerText(stringBuilder, isDisplayScriptingText, depthLevel);
                }
                else
                {
                    InternalInnerText(stringBuilder, isDisplayScriptingText: false, depthLevel);
                }

                return stringBuilder.ToString();
            }
        }

        //
        // Summary:
        //     Gets the last child of the node.
        public HtmlNode LastChild
        {
            get
            {
                if (HasChildNodes)
                {
                    return _childnodes[_childnodes.Count - 1];
                }

                return null;
            }
        }

        //
        // Summary:
        //     Gets the line number of this node in the document.
        public int Line
        {
            get
            {
                return _line;
            }
            internal set
            {
                _line = value;
            }
        }

        //
        // Summary:
        //     Gets the column number of this node in the document.
        public int LinePosition
        {
            get
            {
                return _lineposition;
            }
            internal set
            {
                _lineposition = value;
            }
        }

        //
        // Summary:
        //     Gets the stream position of the area between the opening and closing tag of the
        //     node, relative to the start of the document.
        public int InnerStartIndex => _innerstartindex;

        //
        // Summary:
        //     Gets the stream position of the area of the beginning of the tag, relative to
        //     the start of the document.
        public int OuterStartIndex => _outerstartindex;

        //
        // Summary:
        //     Gets the length of the area between the opening and closing tag of the node.
        public int InnerLength => InnerHtml.Length;

        //
        // Summary:
        //     Gets the length of the entire node, opening and closing tag included.
        public int OuterLength => OuterHtml.Length;

        //
        // Summary:
        //     Gets or sets this node's name.
        public string Name
        {
            get
            {
                if (_optimizedName == null)
                {
                    if (_name == null)
                    {
                        Name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
                    }

                    if (_name == null)
                    {
                        _optimizedName = string.Empty;
                    }
                    else if (OwnerDocument != null)
                    {
                        _optimizedName = (OwnerDocument.OptionDefaultUseOriginalName ? _name : _name.ToLowerInvariant());
                    }
                    else
                    {
                        _optimizedName = _name.ToLowerInvariant();
                    }
                }

                return _optimizedName;
            }
            set
            {
                _name = value;
                _optimizedName = null;
            }
        }

        //
        // Summary:
        //     Gets the HTML node immediately following this element.
        public HtmlNode NextSibling
        {
            get
            {
                return _nextnode;
            }
            internal set
            {
                _nextnode = value;
            }
        }

        //
        // Summary:
        //     Gets the type of this node.
        public HtmlNodeType NodeType
        {
            get
            {
                return _nodetype;
            }
            internal set
            {
                _nodetype = value;
            }
        }

        //
        // Summary:
        //     The original unaltered name of the tag
        public string OriginalName => _name;

        //
        // Summary:
        //     Gets or Sets the object and its content in HTML.
        public virtual string OuterHtml
        {
            get
            {
                if (_changed)
                {
                    UpdateHtml();
                    return _outerhtml;
                }

                if (_outerhtml != null)
                {
                    return _outerhtml;
                }

                if (_outerstartindex < 0 || _outerlength < 0)
                {
                    return string.Empty;
                }

                return _ownerdocument.Text.Substring(_outerstartindex, _outerlength);
            }
        }

        //
        // Summary:
        //     Gets the HtmlAgilityPack.HtmlDocument to which this node belongs.
        public HtmlDocument OwnerDocument
        {
            get
            {
                return _ownerdocument;
            }
            internal set
            {
                _ownerdocument = value;
            }
        }

        //
        // Summary:
        //     Gets the parent of this node (for nodes that can have parents).
        public HtmlNode ParentNode
        {
            get
            {
                return _parentnode;
            }
            internal set
            {
                _parentnode = value;
            }
        }

        //
        // Summary:
        //     Gets the node immediately preceding this node.
        public HtmlNode PreviousSibling
        {
            get
            {
                return _prevnode;
            }
            internal set
            {
                _prevnode = value;
            }
        }

        //
        // Summary:
        //     Gets the stream position of this node in the document, relative to the start
        //     of the document.
        public int StreamPosition => _streamposition;

        //
        // Summary:
        //     Gets a valid XPath string that points to this node
        public string XPath => ((ParentNode == null || ParentNode.NodeType == HtmlNodeType.Document) ? "/" : (ParentNode.XPath + "/")) + GetRelativeXpath();

        //
        // Summary:
        //     The depth of the node relative to the opening root html element. This value is
        //     used to determine if a document has to many nested html nodes which can cause
        //     stack overflows
        public int Depth { get; set; }

        //
        // Summary:
        //     Initialize HtmlNode. Builds a list of all tags that have special allowances
        static HtmlNode()
        {
            HtmlNodeTypeNameComment = "#comment";
            HtmlNodeTypeNameDocument = "#document";
            HtmlNodeTypeNameText = "#text";
            ElementsFlags = new Dictionary<string, HtmlElementFlag>(StringComparer.OrdinalIgnoreCase);
            ElementsFlags.Add("script", HtmlElementFlag.CData);
            ElementsFlags.Add("style", HtmlElementFlag.CData);
            ElementsFlags.Add("noxhtml", HtmlElementFlag.CData);
            ElementsFlags.Add("textarea", HtmlElementFlag.CData);
            ElementsFlags.Add("title", HtmlElementFlag.CData);
            ElementsFlags.Add("base", HtmlElementFlag.Empty);
            ElementsFlags.Add("link", HtmlElementFlag.Empty);
            ElementsFlags.Add("meta", HtmlElementFlag.Empty);
            ElementsFlags.Add("isindex", HtmlElementFlag.Empty);
            ElementsFlags.Add("hr", HtmlElementFlag.Empty);
            ElementsFlags.Add("col", HtmlElementFlag.Empty);
            ElementsFlags.Add("img", HtmlElementFlag.Empty);
            ElementsFlags.Add("param", HtmlElementFlag.Empty);
            ElementsFlags.Add("embed", HtmlElementFlag.Empty);
            ElementsFlags.Add("frame", HtmlElementFlag.Empty);
            ElementsFlags.Add("wbr", HtmlElementFlag.Empty);
            ElementsFlags.Add("bgsound", HtmlElementFlag.Empty);
            ElementsFlags.Add("spacer", HtmlElementFlag.Empty);
            ElementsFlags.Add("keygen", HtmlElementFlag.Empty);
            ElementsFlags.Add("area", HtmlElementFlag.Empty);
            ElementsFlags.Add("input", HtmlElementFlag.Empty);
            ElementsFlags.Add("basefont", HtmlElementFlag.Empty);
            ElementsFlags.Add("source", HtmlElementFlag.Empty);
            ElementsFlags.Add("form", HtmlElementFlag.CanOverlap);
            ElementsFlags.Add("br", HtmlElementFlag.Empty | HtmlElementFlag.Closed);
            if (!HtmlDocument.DisableBehaviorTagP)
            {
                ElementsFlags.Add("p", HtmlElementFlag.Empty | HtmlElementFlag.Closed);
            }
        }

        //
        // Summary:
        //     Initializes HtmlNode, providing type, owner and where it exists in a collection
        //
        // Parameters:
        //   type:
        //
        //   ownerdocument:
        //
        //   index:
        public HtmlNode(HtmlNodeType type, HtmlDocument ownerdocument, int index)
        {
            _nodetype = type;
            _ownerdocument = ownerdocument;
            _outerstartindex = index;
            switch (type)
            {
                case HtmlNodeType.Comment:
                    Name = HtmlNodeTypeNameComment;
                    _endnode = this;
                    break;
                case HtmlNodeType.Document:
                    Name = HtmlNodeTypeNameDocument;
                    _endnode = this;
                    break;
                case HtmlNodeType.Text:
                    Name = HtmlNodeTypeNameText;
                    _endnode = this;
                    break;
            }

            if (_ownerdocument.Openednodes != null && !Closed && -1 != index)
            {
                _ownerdocument.Openednodes.Add(index, this);
            }

            if (-1 == index && type != HtmlNodeType.Comment && type != HtmlNodeType.Text)
            {
                SetChanged();
            }
        }

        internal virtual void InternalInnerText(StringBuilder sb, bool isDisplayScriptingText, int depthLevel)
        {
            depthLevel++;
            if (depthLevel > HtmlDocument.MaxDepthLevel)
            {
                throw new Exception($"Maximum deep level reached: {HtmlDocument.MaxDepthLevel}");
            }

            if (!_ownerdocument.BackwardCompatibility)
            {
                if (HasChildNodes)
                {
                    AppendInnerText(sb, isDisplayScriptingText);
                }
                else
                {
                    sb.Append(GetCurrentNodeText());
                }
            }
            else if (_nodetype == HtmlNodeType.Text)
            {
                sb.Append(((HtmlTextNode)this).Text);
            }
            else
            {
                if (_nodetype == HtmlNodeType.Comment || !HasChildNodes || (_isHideInnerText && !isDisplayScriptingText))
                {
                    return;
                }

                foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
                {
                    item.InternalInnerText(sb, isDisplayScriptingText, depthLevel);
                }
            }
        }

        //
        // Summary:
        //     Gets direct inner text.
        //
        // Returns:
        //     The direct inner text.
        public virtual string GetDirectInnerText()
        {
            if (!_ownerdocument.BackwardCompatibility)
            {
                if (HasChildNodes)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    AppendDirectInnerText(stringBuilder);
                    return stringBuilder.ToString();
                }

                return GetCurrentNodeText();
            }

            if (_nodetype == HtmlNodeType.Text)
            {
                return ((HtmlTextNode)this).Text;
            }

            if (_nodetype == HtmlNodeType.Comment)
            {
                return "";
            }

            if (!HasChildNodes)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
            {
                if (item._nodetype == HtmlNodeType.Text)
                {
                    stringBuilder2.Append(((HtmlTextNode)item).Text);
                }
            }

            return stringBuilder2.ToString();
        }

        internal string GetCurrentNodeText()
        {
            if (_nodetype == HtmlNodeType.Text)
            {
                string text = ((HtmlTextNode)this).Text;
                if (ParentNode.Name != "pre")
                {
                    text = text.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                }

                return text;
            }
            return "";
        }

        internal void AppendDirectInnerText(StringBuilder sb)
        {
            if (_nodetype == HtmlNodeType.Text)
            {
                sb.Append(GetCurrentNodeText());
            }

            if (!HasChildNodes)
            {
                return;
            }

            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
            {
                sb.Append(item.GetCurrentNodeText());
            }
        }

        internal void AppendInnerText(StringBuilder sb, bool isShowHideInnerText)
        {
            if (_nodetype == HtmlNodeType.Text)
            {
                sb.Append(GetCurrentNodeText());
            }

            if (!HasChildNodes || (_isHideInnerText && !isShowHideInnerText))
            {
                return;
            }

            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
            {
                item.AppendInnerText(sb, isShowHideInnerText);
            }
        }

        //
        // Summary:
        //     Determines if an element node can be kept overlapped.
        //
        // Parameters:
        //   name:
        //     The name of the element node to check. May not be null.
        //
        // Returns:
        //     true if the name is the name of an element node that can be kept overlapped,
        //     false otherwise.
        public static bool CanOverlapElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!ElementsFlags.TryGetValue(name, out var value))
            {
                return false;
            }

            return (value & HtmlElementFlag.CanOverlap) != 0;
        }

        //
        // Summary:
        //     Creates an HTML node from a string representing literal HTML.
        //
        // Parameters:
        //   html:
        //     The HTML text.
        //
        // Returns:
        //     The newly created node instance.
        public static HtmlNode CreateNode(string html)
        {
            return CreateNode(html, null);
        }

        //
        // Summary:
        //     Creates an HTML node from a string representing literal HTML.
        //
        // Parameters:
        //   html:
        //     The HTML text.
        //
        //   htmlDocumentBuilder:
        //     The HTML Document builder.
        //
        // Returns:
        //     The newly created node instance.
        public static HtmlNode CreateNode(string html, Action<HtmlDocument> htmlDocumentBuilder)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocumentBuilder?.Invoke(htmlDocument);
            htmlDocument.LoadHtml(html);
            if (!htmlDocument.DocumentNode.IsSingleElementNode())
            {
                throw new Exception("Multiple node elements can't be created.");
            }

            for (HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
            {
                if (htmlNode.NodeType == HtmlNodeType.Element && htmlNode.OuterHtml != "\r\n")
                {
                    return htmlNode;
                }
            }

            return htmlDocument.DocumentNode.FirstChild;
        }

        //
        // Summary:
        //     Determines if an element node is a CDATA element node.
        //
        // Parameters:
        //   name:
        //     The name of the element node to check. May not be null.
        //
        // Returns:
        //     true if the name is the name of a CDATA element node, false otherwise.
        public static bool IsCDataElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!ElementsFlags.TryGetValue(name, out var value))
            {
                return false;
            }

            return (value & HtmlElementFlag.CData) != 0;
        }

        //
        // Summary:
        //     Determines if an element node is closed.
        //
        // Parameters:
        //   name:
        //     The name of the element node to check. May not be null.
        //
        // Returns:
        //     true if the name is the name of a closed element node, false otherwise.
        public static bool IsClosedElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!ElementsFlags.TryGetValue(name, out var value))
            {
                return false;
            }

            return (value & HtmlElementFlag.Closed) != 0;
        }

        //
        // Summary:
        //     Determines if an element node is defined as empty.
        //
        // Parameters:
        //   name:
        //     The name of the element node to check. May not be null.
        //
        // Returns:
        //     true if the name is the name of an empty element node, false otherwise.
        public static bool IsEmptyElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                return true;
            }

            if ('!' == name[0])
            {
                return true;
            }

            if ('?' == name[0])
            {
                return true;
            }

            if (!ElementsFlags.TryGetValue(name, out var value))
            {
                return false;
            }

            return (value & HtmlElementFlag.Empty) != 0;
        }

        //
        // Summary:
        //     Determines if a text corresponds to the closing tag of an node that can be kept
        //     overlapped.
        //
        // Parameters:
        //   text:
        //     The text to check. May not be null.
        //
        // Returns:
        //     true or false.
        public static bool IsOverlappedClosingElement(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length <= 4)
            {
                return false;
            }

            if (text[0] != '<' || text[text.Length - 1] != '>' || text[1] != '/')
            {
                return false;
            }

            return CanOverlapElement(text.Substring(2, text.Length - 3));
        }

        //
        // Summary:
        //     Returns a collection of all ancestor nodes of this element.
        public IEnumerable<HtmlNode> Ancestors()
        {
            HtmlNode node = ParentNode;
            if (node != null)
            {
                yield return node;
                while (node.ParentNode != null)
                {
                    yield return node.ParentNode;
                    node = node.ParentNode;
                }
            }
        }

        //
        // Summary:
        //     Get Ancestors with matching name
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlNode> Ancestors(string name)
        {
            for (HtmlNode i = ParentNode; i != null; i = i.ParentNode)
            {
                if (i.Name == name)
                {
                    yield return i;
                }
            }
        }

        //
        // Summary:
        //     Returns a collection of all ancestor nodes of this element.
        public IEnumerable<HtmlNode> AncestorsAndSelf()
        {
            for (HtmlNode i = this; i != null; i = i.ParentNode)
            {
                yield return i;
            }
        }

        //
        // Summary:
        //     Gets all anscestor nodes and the current node
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlNode> AncestorsAndSelf(string name)
        {
            for (HtmlNode i = this; i != null; i = i.ParentNode)
            {
                if (i.Name == name)
                {
                    yield return i;
                }
            }
        }

        //
        // Summary:
        //     Adds the specified node to the end of the list of children of this node.
        //
        // Parameters:
        //   newChild:
        //     The node to add. May not be null.
        //
        // Returns:
        //     The node added.
        public HtmlNode AppendChild(HtmlNode newChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }

            ChildNodes.Append(newChild);
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            SetChildNodesId(newChild);
            SetChanged();
            return newChild;
        }

        //
        // Summary:
        //     Sets child nodes identifier.
        //
        // Parameters:
        //   chilNode:
        //     The chil node.
        public void SetChildNodesId(HtmlNode chilNode)
        {
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)chilNode.ChildNodes)
            {
                _ownerdocument.SetIdForNode(item, item.GetId());
                SetChildNodesId(item);
            }
        }

        //
        // Summary:
        //     Adds the specified node to the end of the list of children of this node.
        //
        // Parameters:
        //   newChildren:
        //     The node list to add. May not be null.
        public void AppendChildren(HtmlNodeCollection newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }

            foreach (HtmlNode item in (IEnumerable<HtmlNode>)newChildren)
            {
                AppendChild(item);
            }
        }

        //
        // Summary:
        //     Gets all Attributes with name
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlAttribute> ChildAttributes(string name)
        {
            return Attributes.AttributesWithName(name);
        }

        //
        // Summary:
        //     Creates a duplicate of the node
        public HtmlNode Clone()
        {
            return CloneNode(deep: true);
        }

        //
        // Summary:
        //     Creates a duplicate of the node and changes its name at the same time.
        //
        // Parameters:
        //   newName:
        //     The new name of the cloned node. May not be null.
        //
        // Returns:
        //     The cloned node.
        public HtmlNode CloneNode(string newName)
        {
            return CloneNode(newName, deep: true);
        }

        //
        // Summary:
        //     Creates a duplicate of the node and changes its name at the same time.
        //
        // Parameters:
        //   newName:
        //     The new name of the cloned node. May not be null.
        //
        //   deep:
        //     true to recursively clone the subtree under the specified node; false to clone
        //     only the node itself.
        //
        // Returns:
        //     The cloned node.
        public HtmlNode CloneNode(string newName, bool deep)
        {
            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }

            HtmlNode htmlNode = CloneNode(deep);
            htmlNode.Name = newName;
            return htmlNode;
        }

        //
        // Summary:
        //     Creates a duplicate of the node.
        //
        // Parameters:
        //   deep:
        //     true to recursively clone the subtree under the specified node; false to clone
        //     only the node itself.
        //
        // Returns:
        //     The cloned node.
        public HtmlNode CloneNode(bool deep)
        {
            HtmlNode htmlNode = _ownerdocument.CreateNode(_nodetype);
            htmlNode.Name = OriginalName;
            switch (_nodetype)
            {
                case HtmlNodeType.Comment:
                    ((HtmlCommentNode)htmlNode).Comment = ((HtmlCommentNode)this).Comment;
                    return htmlNode;
                case HtmlNodeType.Text:
                    ((HtmlTextNode)htmlNode).Text = ((HtmlTextNode)this).Text;
                    return htmlNode;
                default:
                    if (HasAttributes)
                    {
                        foreach (HtmlAttribute item in (IEnumerable<HtmlAttribute>)_attributes)
                        {
                            HtmlAttribute newAttribute = item.Clone();
                            htmlNode.Attributes.Append(newAttribute);
                        }
                    }

                    if (HasClosingAttributes)
                    {
                        htmlNode._endnode = _endnode.CloneNode(deep: false);
                        foreach (HtmlAttribute item2 in (IEnumerable<HtmlAttribute>)_endnode._attributes)
                        {
                            HtmlAttribute newAttribute2 = item2.Clone();
                            htmlNode._endnode._attributes.Append(newAttribute2);
                        }
                    }

                    if (!deep)
                    {
                        return htmlNode;
                    }

                    if (!HasChildNodes)
                    {
                        return htmlNode;
                    }

                    {
                        foreach (HtmlNode item3 in (IEnumerable<HtmlNode>)_childnodes)
                        {
                            HtmlNode newChild = item3.CloneNode(deep);
                            htmlNode.AppendChild(newChild);
                        }

                        return htmlNode;
                    }
            }
        }

        //
        // Summary:
        //     Creates a duplicate of the node and the subtree under it.
        //
        // Parameters:
        //   node:
        //     The node to duplicate. May not be null.
        public void CopyFrom(HtmlNode node)
        {
            CopyFrom(node, deep: true);
        }

        //
        // Summary:
        //     Creates a duplicate of the node.
        //
        // Parameters:
        //   node:
        //     The node to duplicate. May not be null.
        //
        //   deep:
        //     true to recursively clone the subtree under the specified node, false to clone
        //     only the node itself.
        public void CopyFrom(HtmlNode node, bool deep)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            Attributes.RemoveAll();
            if (node.HasAttributes)
            {
                foreach (HtmlAttribute item in (IEnumerable<HtmlAttribute>)node.Attributes)
                {
                    HtmlAttribute newAttribute = item.Clone();
                    Attributes.Append(newAttribute);
                }
            }

            if (!deep)
            {
                return;
            }

            RemoveAllChildren();
            if (!node.HasChildNodes)
            {
                return;
            }

            foreach (HtmlNode item2 in (IEnumerable<HtmlNode>)node.ChildNodes)
            {
                AppendChild(item2.CloneNode(deep: true));
            }
        }

        //
        // Summary:
        //     Gets all Descendant nodes for this node and each of child nodes
        //
        // Parameters:
        //   level:
        //     The depth level of the node to parse in the html tree
        //
        // Returns:
        //     the current element as an HtmlNode
        [Obsolete("Use Descendants() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodes(int level = 0)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException("The document is too complex to parse");
            }

            foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
            {
                yield return node;
                foreach (HtmlNode item in node.DescendantNodes(level + 1))
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Returns a collection of all descendant nodes of this element, in document order
        [Obsolete("Use DescendantsAndSelf() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodesAndSelf()
        {
            return DescendantsAndSelf();
        }

        //
        // Summary:
        //     Gets all Descendant nodes in enumerated list
        public IEnumerable<HtmlNode> Descendants()
        {
            return Descendants(0);
        }

        //
        // Summary:
        //     Gets all Descendant nodes in enumerated list
        public IEnumerable<HtmlNode> Descendants(int level)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException("The document is too complex to parse");
            }

            foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
            {
                yield return node;
                foreach (HtmlNode item in node.Descendants(level + 1))
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Get all descendant nodes with matching name
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlNode> Descendants(string name)
        {
            foreach (HtmlNode item in Descendants())
            {
                if (string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Returns a collection of all descendant nodes of this element, in document order
        public IEnumerable<HtmlNode> DescendantsAndSelf()
        {
            yield return this;
            foreach (HtmlNode item in Descendants())
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Gets all descendant nodes including this node
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlNode> DescendantsAndSelf(string name)
        {
            yield return this;
            foreach (HtmlNode item in Descendants())
            {
                if (item.Name == name)
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Gets first generation child node matching name
        //
        // Parameters:
        //   name:
        public HtmlNode Element(string name)
        {
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        //
        // Summary:
        //     Gets matching first generation child nodes matching name
        //
        // Parameters:
        //   name:
        public IEnumerable<HtmlNode> Elements(string name)
        {
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
            {
                if (item.Name == name)
                {
                    yield return item;
                }
            }
        }

        //
        // Summary:
        //     Gets data attribute.
        //
        // Parameters:
        //   key:
        //     The key.
        //
        // Returns:
        //     The data attribute.
        public HtmlAttribute GetDataAttribute(string key)
        {
            return Attributes.Hashitems.SingleOrDefault((KeyValuePair<string, HtmlAttribute> x) => x.Key.Equals("data-" + key, StringComparison.OrdinalIgnoreCase))!.Value;
        }

        //
        // Summary:
        //     Gets the data attributes in this collection.
        //
        // Returns:
        //     An enumerator that allows foreach to be used to process the data attributes in
        //     this collection.
        public IEnumerable<HtmlAttribute> GetDataAttributes()
        {
            return (from x in Attributes.Hashitems
                    where x.Key.StartsWith("data-", StringComparison.OrdinalIgnoreCase)
                    select x.Value).ToList();
        }

        //
        // Summary:
        //     Gets the attributes in this collection.
        //
        // Returns:
        //     An enumerator that allows foreach to be used to process the attributes in this
        //     collection.
        public IEnumerable<HtmlAttribute> GetAttributes()
        {
            return Attributes.items;
        }

        //
        // Summary:
        //     Gets the attributes in this collection.
        //
        // Parameters:
        //   attributeNames:
        //     A variable-length parameters list containing attribute names.
        //
        // Returns:
        //     An enumerator that allows foreach to be used to process the attributes in this
        //     collection.
        public IEnumerable<HtmlAttribute> GetAttributes(params string[] attributeNames)
        {
            List<HtmlAttribute> list = new List<HtmlAttribute>();
            foreach (string name in attributeNames)
            {
                list.Add(Attributes[name]);
            }

            return list;
        }

        //
        // Summary:
        //     Helper method to get the value of an attribute of this node. If the attribute
        //     is not found, the default value will be returned.
        //
        // Parameters:
        //   name:
        //     The name of the attribute to get. May not be null.
        //
        //   def:
        //     The default value to return if not found.
        //
        // Returns:
        //     The value of the attribute if found, the default value if not found.
        public string GetAttributeValue(string name, string def)
        {
            return this.GetAttributeValue<string>(name, def);
        }

        //
        // Summary:
        //     Helper method to get the value of an attribute of this node. If the attribute
        //     is not found, the default value will be returned.
        //
        // Parameters:
        //   name:
        //     The name of the attribute to get. May not be null.
        //
        //   def:
        //     The default value to return if not found.
        //
        // Returns:
        //     The value of the attribute if found, the default value if not found.
        public int GetAttributeValue(string name, int def)
        {
            return this.GetAttributeValue<int>(name, def);
        }

        //
        // Summary:
        //     Helper method to get the value of an attribute of this node. If the attribute
        //     is not found, the default value will be returned.
        //
        // Parameters:
        //   name:
        //     The name of the attribute to get. May not be null.
        //
        //   def:
        //     The default value to return if not found.
        //
        // Returns:
        //     The value of the attribute if found, the default value if not found.
        public bool GetAttributeValue(string name, bool def)
        {
            return this.GetAttributeValue<bool>(name, def);
        }

        //
        // Summary:
        //     Helper method to get the value of an attribute of this node. If the attribute
        //     is not found, the default value will be returned.
        //
        // Parameters:
        //   name:
        //     The name of the attribute to get. May not be null.
        //
        //   def:
        //     The default value to return if not found.
        //
        // Returns:
        //     The value of the attribute if found, the default value if not found.
        public T GetAttributeValue<T>(string name, T def)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!HasAttributes)
            {
                return def;
            }

            HtmlAttribute htmlAttribute = Attributes[name];
            if (htmlAttribute == null)
            {
                return def;
            }

            try
            {
                return (T)htmlAttribute.Value.To(typeof(T));
            }
            catch
            {
                return def;
            }
        }

        //
        // Summary:
        //     Inserts the specified node immediately after the specified reference node.
        //
        // Parameters:
        //   newChild:
        //     The node to insert. May not be null.
        //
        //   refChild:
        //     The node that is the reference node. The newNode is placed after the refNode.
        //
        // Returns:
        //     The node being inserted.
        public HtmlNode InsertAfter(HtmlNode newChild, HtmlNode refChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }

            if (refChild == null)
            {
                return PrependChild(newChild);
            }

            if (newChild == refChild)
            {
                return newChild;
            }

            int num = -1;
            if (_childnodes != null)
            {
                num = _childnodes[refChild];
            }

            if (num == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }

            if (_childnodes != null)
            {
                _childnodes.Insert(num + 1, newChild);
            }

            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            SetChildNodesId(newChild);
            SetChanged();
            return newChild;
        }

        //
        // Summary:
        //     Inserts the specified node immediately before the specified reference node.
        //
        // Parameters:
        //   newChild:
        //     The node to insert. May not be null.
        //
        //   refChild:
        //     The node that is the reference node. The newChild is placed before this node.
        //
        // Returns:
        //     The node being inserted.
        public HtmlNode InsertBefore(HtmlNode newChild, HtmlNode refChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }

            if (refChild == null)
            {
                return AppendChild(newChild);
            }

            if (newChild == refChild)
            {
                return newChild;
            }

            int num = -1;
            if (_childnodes != null)
            {
                num = _childnodes[refChild];
            }

            if (num == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }

            if (_childnodes != null)
            {
                _childnodes.Insert(num, newChild);
            }

            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            SetChildNodesId(newChild);
            SetChanged();
            return newChild;
        }

        //
        // Summary:
        //     Adds the specified node to the beginning of the list of children of this node.
        //
        // Parameters:
        //   newChild:
        //     The node to add. May not be null.
        //
        // Returns:
        //     The node added.
        public HtmlNode PrependChild(HtmlNode newChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }

            ChildNodes.Prepend(newChild);
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            SetChildNodesId(newChild);
            SetChanged();
            return newChild;
        }

        //
        // Summary:
        //     Adds the specified node list to the beginning of the list of children of this
        //     node.
        //
        // Parameters:
        //   newChildren:
        //     The node list to add. May not be null.
        public void PrependChildren(HtmlNodeCollection newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }

            for (int num = newChildren.Count - 1; num >= 0; num--)
            {
                PrependChild(newChildren[num]);
            }
        }

        //
        // Summary:
        //     Removes node from parent collection
        public void Remove()
        {
            if (ParentNode != null)
            {
                ParentNode.ChildNodes.Remove(this);
            }
        }

        //
        // Summary:
        //     Removes all the children and/or attributes of the current node.
        public void RemoveAll()
        {
            RemoveAllChildren();
            if (HasAttributes)
            {
                _attributes.Clear();
            }

            if (_endnode != null && _endnode != this && _endnode._attributes != null)
            {
                _endnode._attributes.Clear();
            }

            SetChanged();
        }

        //
        // Summary:
        //     Removes all the children of the current node.
        public void RemoveAllChildren()
        {
            if (!HasChildNodes)
            {
                return;
            }

            if (_ownerdocument.OptionUseIdAttribute)
            {
                foreach (HtmlNode item in (IEnumerable<HtmlNode>)_childnodes)
                {
                    _ownerdocument.SetIdForNode(null, item.GetId());
                    RemoveAllIDforNode(item);
                }
            }

            _childnodes.Clear();
            SetChanged();
        }

        //
        // Summary:
        //     Removes all id for node described by node.
        //
        // Parameters:
        //   node:
        //     The node.
        public void RemoveAllIDforNode(HtmlNode node)
        {
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)node.ChildNodes)
            {
                _ownerdocument.SetIdForNode(null, item.GetId());
                RemoveAllIDforNode(item);
            }
        }

        //
        // Summary:
        //     Move a node already associated and append it to this node instead.
        //
        // Parameters:
        //   child:
        //     The child node to move.
        public void MoveChild(HtmlNode child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("Oops! the 'child' parameter cannot be null.");
            }

            HtmlNode parentNode = child.ParentNode;
            AppendChild(child);
            parentNode?.RemoveChild(child);
        }

        //
        // Summary:
        //     Move a children collection already associated and append it to this node instead.
        //
        // Parameters:
        //   children:
        //     The children collection already associated to move to another node.
        public void MoveChildren(HtmlNodeCollection children)
        {
            if (children == null)
            {
                throw new ArgumentNullException("Oops! the 'children' parameter cannot be null.");
            }

            HtmlNode parentNode = children.ParentNode;
            AppendChildren(children);
            parentNode?.RemoveChildren(children);
        }

        //
        // Summary:
        //     Removes the children collection for this node.
        //
        // Parameters:
        //   oldChildren:
        //     The old children collection to remove.
        public void RemoveChildren(HtmlNodeCollection oldChildren)
        {
            if (oldChildren == null)
            {
                throw new ArgumentNullException("Oops! the 'oldChildren' parameter cannot be null.");
            }

            foreach (HtmlNode item in oldChildren.ToList())
            {
                RemoveChild(item);
            }
        }

        //
        // Summary:
        //     Removes the specified child node.
        //
        // Parameters:
        //   oldChild:
        //     The node being removed. May not be null.
        //
        // Returns:
        //     The node removed.
        public HtmlNode RemoveChild(HtmlNode oldChild)
        {
            if (oldChild == null)
            {
                throw new ArgumentNullException("oldChild");
            }

            int num = -1;
            if (_childnodes != null)
            {
                num = _childnodes[oldChild];
            }

            if (num == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }

            if (_childnodes != null)
            {
                _childnodes.Remove(num);
            }

            _ownerdocument.SetIdForNode(null, oldChild.GetId());
            RemoveAllIDforNode(oldChild);
            SetChanged();
            return oldChild;
        }

        //
        // Summary:
        //     Removes the specified child node.
        //
        // Parameters:
        //   oldChild:
        //     The node being removed. May not be null.
        //
        //   keepGrandChildren:
        //     true to keep grand children of the node, false otherwise.
        //
        // Returns:
        //     The node removed.
        public HtmlNode RemoveChild(HtmlNode oldChild, bool keepGrandChildren)
        {
            if (oldChild == null)
            {
                throw new ArgumentNullException("oldChild");
            }

            if (oldChild._childnodes != null && keepGrandChildren)
            {
                HtmlNode refChild = oldChild.PreviousSibling;
                foreach (HtmlNode item in (IEnumerable<HtmlNode>)oldChild._childnodes)
                {
                    refChild = InsertAfter(item, refChild);
                }
            }

            RemoveChild(oldChild);
            SetChanged();
            return oldChild;
        }

        //
        // Summary:
        //     Replaces the child node oldChild with newChild node.
        //
        // Parameters:
        //   newChild:
        //     The new node to put in the child list.
        //
        //   oldChild:
        //     The node being replaced in the list.
        //
        // Returns:
        //     The node replaced.
        public HtmlNode ReplaceChild(HtmlNode newChild, HtmlNode oldChild)
        {
            if (newChild == null)
            {
                return RemoveChild(oldChild);
            }

            if (oldChild == null)
            {
                return AppendChild(newChild);
            }

            int num = -1;
            if (_childnodes != null)
            {
                num = _childnodes[oldChild];
            }

            if (num == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }

            if (_childnodes != null)
            {
                _childnodes.Replace(num, newChild);
            }

            _ownerdocument.SetIdForNode(null, oldChild.GetId());
            RemoveAllIDforNode(oldChild);
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            SetChildNodesId(newChild);
            SetChanged();
            return newChild;
        }

        //
        // Summary:
        //     Helper method to set the value of an attribute of this node. If the attribute
        //     is not found, it will be created automatically.
        //
        // Parameters:
        //   name:
        //     The name of the attribute to set. May not be null.
        //
        //   value:
        //     The value for the attribute.
        //
        // Returns:
        //     The corresponding attribute instance.
        public HtmlAttribute SetAttributeValue(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            HtmlAttribute htmlAttribute = Attributes[name];
            if (htmlAttribute == null)
            {
                return Attributes.Append(_ownerdocument.CreateAttribute(name, value));
            }

            htmlAttribute.Value = value;
            return htmlAttribute;
        }

        //
        // Summary:
        //     Saves all the children of the node to the specified TextWriter.
        //
        // Parameters:
        //   outText:
        //     The TextWriter to which you want to save.
        //
        //   level:
        //     Identifies the level we are in starting at root with 0
        public void WriteContentTo(TextWriter outText, int level = 0)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException("The document is too complex to parse");
            }

            if (_childnodes == null)
            {
                return;
            }

            foreach (HtmlNode item in (IEnumerable<HtmlNode>)_childnodes)
            {
                item.WriteTo(outText, level + 1);
            }
        }

        //
        // Summary:
        //     Saves all the children of the node to a string.
        //
        // Returns:
        //     The saved string.
        public string WriteContentTo()
        {
            StringWriter stringWriter = new StringWriter();
            WriteContentTo(stringWriter);
            stringWriter.Flush();
            return stringWriter.ToString();
        }

        //
        // Summary:
        //     Saves the current node to the specified TextWriter.
        //
        // Parameters:
        //   outText:
        //     The TextWriter to which you want to save.
        //
        //   level:
        //     identifies the level we are in starting at root with 0
        public virtual void WriteTo(TextWriter outText, int level = 0)
        {
            switch (_nodetype)
            {
                case HtmlNodeType.Comment:
                    {
                        string comment = ((HtmlCommentNode)this).Comment;
                        if (_ownerdocument.OptionOutputAsXml)
                        {
                            HtmlCommentNode htmlCommentNode = (HtmlCommentNode)this;
                            if (!_ownerdocument.BackwardCompatibility && htmlCommentNode.Comment.ToLowerInvariant().StartsWith("<!doctype"))
                            {
                                outText.Write(htmlCommentNode.Comment);
                            }
                            else if (OwnerDocument.OptionXmlForceOriginalComment)
                            {
                                outText.Write(htmlCommentNode.Comment);
                            }
                            else
                            {
                                outText.Write("<!--" + GetXmlComment(htmlCommentNode) + " -->");
                            }
                        }
                        else
                        {
                            outText.Write(comment);
                        }

                        break;
                    }
                case HtmlNodeType.Document:
                    if (_ownerdocument.OptionOutputAsXml)
                    {
                        outText.Write("<?xml version=\"1.0\" encoding=\"" + _ownerdocument.GetOutEncoding().BodyName + "\"?>");
                        if (_ownerdocument.DocumentNode.HasChildNodes)
                        {
                            int num = _ownerdocument.DocumentNode._childnodes.Count;
                            if (num > 0)
                            {
                                if (_ownerdocument.GetXmlDeclaration() != null)
                                {
                                    num--;
                                }

                                if (num > 1)
                                {
                                    if (!_ownerdocument.BackwardCompatibility)
                                    {
                                        WriteContentTo(outText, level);
                                    }
                                    else if (_ownerdocument.OptionOutputUpperCase)
                                    {
                                        outText.Write("<SPAN>");
                                        WriteContentTo(outText, level);
                                        outText.Write("</SPAN>");
                                    }
                                    else
                                    {
                                        outText.Write("<span>");
                                        WriteContentTo(outText, level);
                                        outText.Write("</span>");
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    WriteContentTo(outText, level);
                    break;
                case HtmlNodeType.Text:
                    {
                        string comment = ((HtmlTextNode)this).Text;
                        outText.Write(_ownerdocument.OptionOutputAsXml ? HtmlDocument.HtmlEncodeWithCompatibility(comment, _ownerdocument.BackwardCompatibility) : comment);
                        break;
                    }
                case HtmlNodeType.Element:
                    {
                        string text = (_ownerdocument.OptionOutputUpperCase ? Name.ToUpperInvariant() : Name);
                        if (_ownerdocument.OptionOutputOriginalCase)
                        {
                            text = OriginalName;
                        }

                        if (_ownerdocument.OptionOutputAsXml)
                        {
                            if (text.Length <= 0 || text[0] == '?' || text.Trim().Length == 0)
                            {
                                break;
                            }

                            text = HtmlDocument.GetXmlName(text, isAttribute: false, _ownerdocument.OptionPreserveXmlNamespaces);
                        }

                        outText.Write("<" + text);
                        WriteAttributes(outText, closing: false);
                        if (HasChildNodes)
                        {
                            outText.Write(">");
                            bool flag = false;
                            if (_ownerdocument.OptionOutputAsXml && IsCDataElement(Name))
                            {
                                flag = true;
                                outText.Write("\r\n//<![CDATA[\r\n");
                            }

                            if (flag)
                            {
                                if (HasChildNodes)
                                {
                                    ChildNodes[0].WriteTo(outText, level);
                                }

                                outText.Write("\r\n//]]>//\r\n");
                            }
                            else
                            {
                                WriteContentTo(outText, level);
                            }

                            if (_ownerdocument.OptionOutputAsXml || !_isImplicitEnd)
                            {
                                outText.Write("</" + text);
                                if (!_ownerdocument.OptionOutputAsXml)
                                {
                                    WriteAttributes(outText, closing: true);
                                }

                                outText.Write(">");
                            }
                        }
                        else if (IsEmptyElement(Name))
                        {
                            if (_ownerdocument.OptionWriteEmptyNodes || _ownerdocument.OptionOutputAsXml)
                            {
                                outText.Write(" />");
                                break;
                            }

                            if (Name.Length > 0 && Name[0] == '?')
                            {
                                outText.Write("?");
                            }

                            outText.Write(">");
                        }
                        else if (!_isImplicitEnd)
                        {
                            outText.Write("></" + text + ">");
                        }
                        else
                        {
                            outText.Write(">");
                        }

                        break;
                    }
            }
        }

        //
        // Summary:
        //     Saves the current node to the specified XmlWriter.
        //
        // Parameters:
        //   writer:
        //     The XmlWriter to which you want to save.
        public void WriteTo(XmlWriter writer)
        {
            switch (_nodetype)
            {
                case HtmlNodeType.Comment:
                    writer.WriteComment(GetXmlComment((HtmlCommentNode)this));
                    break;
                case HtmlNodeType.Document:
                    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + _ownerdocument.GetOutEncoding().BodyName + "\"");
                    if (!HasChildNodes)
                    {
                        break;
                    }

                    foreach (HtmlNode item in (IEnumerable<HtmlNode>)ChildNodes)
                    {
                        item.WriteTo(writer);
                    }

                    break;
                case HtmlNodeType.Text:
                    {
                        string text = ((HtmlTextNode)this).Text;
                        writer.WriteString(text);
                        break;
                    }
                case HtmlNodeType.Element:
                    {
                        string localName = (_ownerdocument.OptionOutputUpperCase ? Name.ToUpperInvariant() : Name);
                        if (_ownerdocument.OptionOutputOriginalCase)
                        {
                            localName = OriginalName;
                        }

                        writer.WriteStartElement(localName);
                        WriteAttributes(writer, this);
                        if (HasChildNodes)
                        {
                            foreach (HtmlNode item2 in (IEnumerable<HtmlNode>)ChildNodes)
                            {
                                item2.WriteTo(writer);
                            }
                        }

                        writer.WriteEndElement();
                        break;
                    }
            }
        }

        //
        // Summary:
        //     Saves the current node to a string.
        //
        // Returns:
        //     The saved string.
        public string WriteTo()
        {
            using StringWriter stringWriter = new StringWriter();
            WriteTo(stringWriter);
            stringWriter.Flush();
            return stringWriter.ToString();
        }

        //
        // Summary:
        //     Sets the parent Html node and properly determines the current node's depth using
        //     the parent node's depth.
        public void SetParent(HtmlNode parent)
        {
            if (parent == null)
            {
                return;
            }

            ParentNode = parent;
            if (OwnerDocument.OptionMaxNestedChildNodes > 0)
            {
                Depth = parent.Depth + 1;
                if (Depth > OwnerDocument.OptionMaxNestedChildNodes)
                {
                    throw new Exception($"Document has more than {OwnerDocument.OptionMaxNestedChildNodes} nested tags. This is likely due to the page not closing tags properly.");
                }
            }
        }

        internal void SetChanged()
        {
            _changed = true;
            if (ParentNode != null)
            {
                ParentNode.SetChanged();
            }
        }

        private void UpdateHtml()
        {
            _innerhtml = WriteContentTo();
            _outerhtml = WriteTo();
            _changed = false;
        }

        internal static string GetXmlComment(HtmlCommentNode comment)
        {
            string comment2 = comment.Comment;
            return comment2.Substring(4, comment2.Length - 7).Replace("--", " - -");
        }

        internal static void WriteAttributes(XmlWriter writer, HtmlNode node)
        {
            if (!node.HasAttributes)
            {
                return;
            }

            foreach (HtmlAttribute value in node.Attributes.Hashitems.Values)
            {
                writer.WriteAttributeString(value.XmlName, value.Value);
            }
        }

        internal void UpdateLastNode()
        {
            HtmlNode htmlNode = null;
            if (_prevwithsamename == null || !_prevwithsamename._starttag)
            {
                if (_ownerdocument.Openednodes != null)
                {
                    foreach (KeyValuePair<int, HtmlNode> openednode in _ownerdocument.Openednodes)
                    {
                        if ((openednode.Key < _outerstartindex || openednode.Key > _outerstartindex + _outerlength) && openednode.Value._name == _name)
                        {
                            if (htmlNode == null && openednode.Value._starttag)
                            {
                                htmlNode = openednode.Value;
                            }
                            else if (htmlNode != null && htmlNode.InnerStartIndex < openednode.Key && openednode.Value._starttag)
                            {
                                htmlNode = openednode.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                htmlNode = _prevwithsamename;
            }

            if (htmlNode != null)
            {
                _ownerdocument.Lastnodes[htmlNode.Name] = htmlNode;
            }
        }

        internal void CloseNode(HtmlNode endnode, int level = 0)
        {
            if (level > HtmlDocument.MaxDepthLevel)
            {
                throw new ArgumentException("The document is too complex to parse");
            }

            if (!_ownerdocument.OptionAutoCloseOnEnd && _childnodes != null)
            {
                foreach (HtmlNode item in (IEnumerable<HtmlNode>)_childnodes)
                {
                    if (!item.Closed)
                    {
                        HtmlNode htmlNode = new HtmlNode(NodeType, _ownerdocument, -1);
                        htmlNode._endnode = htmlNode;
                        item.CloseNode(htmlNode, level + 1);
                    }
                }
            }

            if (Closed)
            {
                return;
            }

            _endnode = endnode;
            if (_ownerdocument.Openednodes != null)
            {
                _ownerdocument.Openednodes.Remove(_outerstartindex);
            }

            if (Utilities.GetDictionaryValueOrDefault(_ownerdocument.Lastnodes, Name) == this)
            {
                _ownerdocument.Lastnodes.Remove(Name);
                _ownerdocument.UpdateLastParentNode();
                if (_starttag && !string.IsNullOrEmpty(Name))
                {
                    UpdateLastNode();
                }
            }

            if (endnode != this)
            {
                _innerstartindex = _outerstartindex + _outerlength;
                _innerlength = endnode._outerstartindex - _innerstartindex;
                _outerlength = endnode._outerstartindex + endnode._outerlength - _outerstartindex;
            }
        }

        internal string GetId()
        {
            HtmlAttribute htmlAttribute = Attributes["id"];
            if (htmlAttribute != null)
            {
                return htmlAttribute.Value;
            }
            return string.Empty;
        }

        internal void SetId(string id)
        {
            HtmlAttribute htmlAttribute = Attributes["id"] ?? _ownerdocument.CreateAttribute("id");
            htmlAttribute.Value = id;
            _ownerdocument.SetIdForNode(this, htmlAttribute.Value);
            Attributes["id"] = htmlAttribute;
            SetChanged();
        }

        internal void WriteAttribute(TextWriter outText, HtmlAttribute att)
        {
            if (att.Value == null)
            {
                return;
            }

            AttributeValueQuote attributeValueQuote = OwnerDocument.GlobalAttributeValueQuote ?? att.QuoteType;
            bool flag = attributeValueQuote == AttributeValueQuote.WithoutValue || (attributeValueQuote == AttributeValueQuote.Initial && att._isFromParse && !att._hasEqual && string.IsNullOrEmpty(att.XmlValue));
            if (attributeValueQuote == AttributeValueQuote.Initial && (!att._isFromParse || att._hasEqual || !string.IsNullOrEmpty(att.XmlValue)))
            {
                attributeValueQuote = att.InternalQuoteType;
            }

            string text = attributeValueQuote switch
            {
                AttributeValueQuote.SingleQuote => "'",
                AttributeValueQuote.DoubleQuote => "\"",
                _ => "",
            };
            string text2;
            if (_ownerdocument.OptionOutputAsXml)
            {
                text2 = (_ownerdocument.OptionOutputUpperCase ? att.XmlName.ToUpperInvariant() : att.XmlName);
                if (_ownerdocument.OptionOutputOriginalCase)
                {
                    text2 = att.OriginalName;
                }

                if (!flag)
                {
                    outText.Write(" " + text2 + "=" + text + HtmlDocument.HtmlEncodeWithCompatibility(att.XmlValue, _ownerdocument.BackwardCompatibility) + text);
                }
                else
                {
                    outText.Write(" " + text2);
                }

                return;
            }

            text2 = (_ownerdocument.OptionOutputUpperCase ? att.Name.ToUpperInvariant() : att.Name);
            if (_ownerdocument.OptionOutputOriginalCase)
            {
                text2 = att.OriginalName;
            }

            if (att.Name.Length >= 4 && att.Name[0] == '<' && att.Name[1] == '%' && att.Name[att.Name.Length - 1] == '>' && att.Name[att.Name.Length - 2] == '%')
            {
                outText.Write(" " + text2);
            }
            else if (!flag)
            {
                string text3 = attributeValueQuote switch
                {
                    AttributeValueQuote.SingleQuote => att.Value.Replace("'", "&#39;"),
                    AttributeValueQuote.DoubleQuote => (!att.Value.StartsWith("@")) ? att.Value.Replace("\"", "&quot;") : att.Value,
                    _ => att.Value,
                };
                if (_ownerdocument.OptionOutputOptimizeAttributeValues)
                {
                    if (att.Value.IndexOfAny(new char[4] { '\n', '\r', '\t', ' ' }) < 0)
                    {
                        outText.Write(" " + text2 + "=" + att.Value);
                        return;
                    }

                    outText.Write(" " + text2 + "=" + text + text3 + text);
                }
                else
                {
                    outText.Write(" " + text2 + "=" + text + text3 + text);
                }
            }
            else
            {
                outText.Write(" " + text2);
            }
        }

        internal void WriteAttributes(TextWriter outText, bool closing)
        {
            if (_ownerdocument.OptionOutputAsXml)
            {
                if (_attributes == null)
                {
                    return;
                }

                foreach (HtmlAttribute value in _attributes.Hashitems.Values)
                {
                    WriteAttribute(outText, value);
                }
            }
            else if (!closing)
            {
                if (_attributes != null)
                {
                    foreach (HtmlAttribute item in (IEnumerable<HtmlAttribute>)_attributes)
                    {
                        WriteAttribute(outText, item);
                    }
                }

                if (!_ownerdocument.OptionAddDebuggingAttributes)
                {
                    return;
                }

                WriteAttribute(outText, _ownerdocument.CreateAttribute("_closed", Closed.ToString()));
                WriteAttribute(outText, _ownerdocument.CreateAttribute("_children", ChildNodes.Count.ToString()));
                int num = 0;
                foreach (HtmlNode item2 in (IEnumerable<HtmlNode>)ChildNodes)
                {
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_child_" + num, item2.Name));
                    num++;
                }
            }
            else
            {
                if (_endnode == null || _endnode._attributes == null || _endnode == this)
                {
                    return;
                }

                foreach (HtmlAttribute item3 in (IEnumerable<HtmlAttribute>)_endnode._attributes)
                {
                    WriteAttribute(outText, item3);
                }

                if (_ownerdocument.OptionAddDebuggingAttributes)
                {
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_closed", Closed.ToString()));
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_children", ChildNodes.Count.ToString()));
                }
            }
        }

        private string GetRelativeXpath()
        {
            if (ParentNode == null)
            {
                return Name;
            }

            if (NodeType == HtmlNodeType.Document)
            {
                return string.Empty;
            }

            int num = 1;
            foreach (HtmlNode item in (IEnumerable<HtmlNode>)ParentNode.ChildNodes)
            {
                if (!(item.Name != Name))
                {
                    if (item == this)
                    {
                        break;
                    }

                    num++;
                }
            }

            return Name + "[" + num + "]";
        }

        private bool IsSingleElementNode()
        {
            int num = 0;
            for (HtmlNode htmlNode = FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
            {
                if (htmlNode.NodeType == HtmlNodeType.Element && htmlNode.OuterHtml != "\r\n")
                {
                    num++;
                }
            }

            if (num > 1)
            {
                return false;
            }

            return true;
        }

        //
        // Summary:
        //     Adds one or more classes to this node.
        //
        // Parameters:
        //   name:
        //     The node list to add. May not be null.
        public void AddClass(string name)
        {
            AddClass(name, throwError: false);
        }

        //
        // Summary:
        //     Adds one or more classes to this node.
        //
        // Parameters:
        //   name:
        //     The node list to add. May not be null.
        //
        //   throwError:
        //     true to throw Error if class name exists, false otherwise.
        public void AddClass(string name, bool throwError)
        {
            IEnumerable<HtmlAttribute> enumerable = Attributes.AttributesWithName("class");
            if (!IsEmpty(enumerable))
            {
                foreach (HtmlAttribute item in enumerable)
                {
                    if (item.Value != null && item.Value.Split(new char[1] { ' ' }).ToList().Any((string x) => x.Equals(name)))
                    {
                        if (throwError)
                        {
                            throw new Exception(HtmlDocument.HtmlExceptionClassExists);
                        }
                    }
                    else
                    {
                        SetAttributeValue(item.Name, item.Value + " " + name);
                    }
                }
            }
            else
            {
                HtmlAttribute newAttribute = _ownerdocument.CreateAttribute("class", name);
                Attributes.Append(newAttribute);
            }
        }

        //
        // Summary:
        //     Removes the class attribute from the node.
        public void RemoveClass()
        {
            RemoveClass(throwError: false);
        }

        //
        // Summary:
        //     Removes the class attribute from the node.
        //
        // Parameters:
        //   throwError:
        //     true to throw Error if class name doesn't exist, false otherwise.
        public void RemoveClass(bool throwError)
        {
            IEnumerable<HtmlAttribute> enumerable = Attributes.AttributesWithName("class");
            if (IsEmpty(enumerable) && throwError)
            {
                throw new Exception(HtmlDocument.HtmlExceptionClassDoesNotExist);
            }

            foreach (HtmlAttribute item in enumerable)
            {
                Attributes.Remove(item);
            }
        }

        //
        // Summary:
        //     Removes the specified class from the node.
        //
        // Parameters:
        //   name:
        //     The class being removed. May not be null.
        public void RemoveClass(string name)
        {
            RemoveClass(name, throwError: false);
        }

        //
        // Summary:
        //     Removes the specified class from the node.
        //
        // Parameters:
        //   name:
        //     The class being removed. May not be null.
        //
        //   throwError:
        //     true to throw Error if class name doesn't exist, false otherwise.
        public void RemoveClass(string name, bool throwError)
        {
            IEnumerable<HtmlAttribute> enumerable = Attributes.AttributesWithName("class");
            if (IsEmpty(enumerable) && throwError)
            {
                throw new Exception(HtmlDocument.HtmlExceptionClassDoesNotExist);
            }

            foreach (HtmlAttribute item in enumerable)
            {
                if (item.Value == null)
                {
                    continue;
                }

                if (item.Value.Equals(name))
                {
                    Attributes.Remove(item);
                }
                else if (item.Value != null && item.Value.Split(new char[1] { ' ' }).ToList().Any((string x) => x.Equals(name)))
                {
                    string[] array = item.Value.Split(new char[1] { ' ' });
                    string text = "";
                    string[] array2 = array;
                    foreach (string text2 in array2)
                    {
                        if (!text2.Equals(name))
                        {
                            text = text + text2 + " ";
                        }
                    }

                    text = text.Trim();
                    SetAttributeValue(item.Name, text);
                }
                else if (throwError)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionClassDoesNotExist);
                }

                if (string.IsNullOrEmpty(item.Value))
                {
                    Attributes.Remove(item);
                }
            }
        }

        //
        // Summary:
        //     Replaces the class name oldClass with newClass name.
        //
        // Parameters:
        //   newClass:
        //     The new class name.
        //
        //   oldClass:
        //     The class being replaced.
        public void ReplaceClass(string newClass, string oldClass)
        {
            ReplaceClass(newClass, oldClass, throwError: false);
        }

        //
        // Summary:
        //     Replaces the class name oldClass with newClass name.
        //
        // Parameters:
        //   newClass:
        //     The new class name.
        //
        //   oldClass:
        //     The class being replaced.
        //
        //   throwError:
        //     true to throw Error if class name doesn't exist, false otherwise.
        public void ReplaceClass(string newClass, string oldClass, bool throwError)
        {
            if (string.IsNullOrEmpty(newClass))
            {
                RemoveClass(oldClass);
            }

            if (string.IsNullOrEmpty(oldClass))
            {
                AddClass(newClass);
            }

            IEnumerable<HtmlAttribute> enumerable = Attributes.AttributesWithName("class");
            if (IsEmpty(enumerable) && throwError)
            {
                throw new Exception(HtmlDocument.HtmlExceptionClassDoesNotExist);
            }

            foreach (HtmlAttribute item in enumerable)
            {
                if (item.Value != null)
                {
                    if (item.Value.Equals(oldClass) || item.Value.Contains(oldClass))
                    {
                        string value = item.Value.Replace(oldClass, newClass);
                        SetAttributeValue(item.Name, value);
                    }
                    else if (throwError)
                    {
                        throw new Exception(HtmlDocument.HtmlExceptionClassDoesNotExist);
                    }
                }
            }
        }

        //
        // Summary:
        //     Gets the CSS Class from the node.
        //
        // Returns:
        //     The CSS Class from the node
        public IEnumerable<string> GetClasses()
        {
            IEnumerable<HtmlAttribute> enumerable = Attributes.AttributesWithName("class");
            foreach (HtmlAttribute item in enumerable)
            {
                string[] array = item.Value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    yield return array2[i];
                }
            }
        }

        //
        // Summary:
        //     Check if the node class has the parameter class.
        //
        // Parameters:
        //   class:
        //     The class.
        //
        // Returns:
        //     True if node class has the parameter class, false if not.
        public bool HasClass(string className)
        {
            foreach (string @class in GetClasses())
            {
                string[] array = @class.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == className)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsEmpty(IEnumerable en)
        {
            IEnumerator enumerator = en.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    _ = enumerator.Current;
                    return false;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return true;
        }

        //
        // Summary:
        //     Fill an object and go through it's properties and fill them too.
        //
        // Type parameters:
        //   T:
        //     Type of object to want to fill. It should have atleast one property that defined
        //     XPath.
        //
        // Returns:
        //     Returns an object of type T including Encapsulated data.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Why it's thrown.
        //
        //   T:System.ArgumentNullException:
        //     Why it's thrown.
        //
        //   T:System.MissingMethodException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.MissingXPathException:
        //     Why it's thrown.
        //
        //   T:System.Xml.XPath.XPathException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeNotFoundException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeAttributeNotFoundException:
        //     Why it's thrown.
        //
        //   T:System.FormatException:
        //     Why it's thrown.
        //
        //   T:System.Exception:
        //     Why it's thrown.
        public T GetEncapsulatedData<T>()
        {
            return (T)GetEncapsulatedData(typeof(T));
        }

        //
        // Summary:
        //     Fill an object and go through it's properties and fill them too.
        //
        // Parameters:
        //   htmlDocument:
        //     If htmlDocument includes data , leave this parameter null. Else pass your specific
        //     htmldocument.
        //
        // Type parameters:
        //   T:
        //     Type of object to want to fill. It should have atleast one property that defined
        //     XPath.
        //
        // Returns:
        //     Returns an object of type T including Encapsulated data.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Why it's thrown.
        //
        //   T:System.ArgumentNullException:
        //     Why it's thrown.
        //
        //   T:System.MissingMethodException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.MissingXPathException:
        //     Why it's thrown.
        //
        //   T:System.Xml.XPath.XPathException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeNotFoundException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeAttributeNotFoundException:
        //     Why it's thrown.
        //
        //   T:System.FormatException:
        //     Why it's thrown.
        //
        //   T:System.Exception:
        //     Why it's thrown.
        public T GetEncapsulatedData<T>(HtmlDocument htmlDocument)
        {
            return (T)GetEncapsulatedData(typeof(T), htmlDocument);
        }

        //
        // Summary:
        //     Fill an object and go through it's properties and fill them too.
        //
        // Parameters:
        //   targetType:
        //     Type of object to want to fill. It should have atleast one property that defined
        //     XPath.
        //
        //   htmlDocument:
        //     If htmlDocument includes data , leave this parameter null. Else pass your specific
        //     htmldocument.
        //
        // Returns:
        //     Returns an object of type targetType including Encapsulated data.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     Why it's thrown.
        //
        //   T:System.ArgumentNullException:
        //     Why it's thrown.
        //
        //   T:System.MissingMethodException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.MissingXPathException:
        //     Why it's thrown.
        //
        //   T:System.Xml.XPath.XPathException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeNotFoundException:
        //     Why it's thrown.
        //
        //   T:HtmlAgilityPack.NodeAttributeNotFoundException:
        //     Why it's thrown.
        //
        //   T:System.FormatException:
        //     Why it's thrown.
        //
        //   T:System.Exception:
        //     Why it's thrown.
        public object GetEncapsulatedData(Type targetType, HtmlDocument htmlDocument = null)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("Parameter targetType is null");
            }

            HtmlDocument htmlDocument2 = ((htmlDocument != null) ? htmlDocument : OwnerDocument);
            if (!targetType.IsInstantiable())
            {
                throw new MissingMethodException("Parameterless Constructor excpected for " + targetType.FullName);
            }

            object obj = Activator.CreateInstance(targetType);
            if (targetType.IsDefinedAttribute(typeof(HasXPathAttribute)))
            {
                IEnumerable<PropertyInfo> propertiesDefinedXPath = targetType.GetPropertiesDefinedXPath();
                if (propertiesDefinedXPath.CountOfIEnumerable() == 0)
                {
                    throw new MissingXPathException("Type " + targetType.FullName + " defined HasXPath Attribute but it does not have any property with XPath Attribte.");
                }

                {
                    foreach (PropertyInfo item in propertiesDefinedXPath)
                    {
                        XPathAttribute xPathAttribute = ((IList)item.GetCustomAttributes(typeof(XPathAttribute), inherit: false))[0] as XPathAttribute;
                        if (!item.IsIEnumerable())
                        {
                            HtmlNode htmlNode = null;
                            try
                            {
                                htmlNode = htmlDocument2.DocumentNode.SelectSingleNode(xPathAttribute.XPath);
                            }
                            catch (XPathException ex)
                            {
                                throw new XPathException(ex.Message + " That means you have a syntax error in XPath property of this Property : " + item.PropertyType.FullName + " " + item.Name);
                            }
                            catch (Exception inner)
                            {
                                throw new NodeNotFoundException("Cannot find node with giving XPath to bind to " + item.PropertyType.FullName + " " + item.Name, inner);
                            }

                            if (htmlNode == null)
                            {
                                if (!item.IsDefined(typeof(SkipNodeNotFoundAttribute), inherit: false))
                                {
                                    throw new NodeNotFoundException("Cannot find node with giving XPath to bind to " + item.PropertyType.FullName + " " + item.Name);
                                }

                                continue;
                            }

                            if (item.PropertyType.IsDefinedAttribute(typeof(HasXPathAttribute)))
                            {
                                HtmlDocument htmlDocument3 = new HtmlDocument();
                                htmlDocument3.LoadHtml(htmlNode.InnerHtml);
                                object encapsulatedData = GetEncapsulatedData(item.PropertyType, htmlDocument3);
                                item.SetValue(obj, encapsulatedData, null);
                                continue;
                            }

                            string empty = string.Empty;
                            empty = ((xPathAttribute.AttributeName != null) ? htmlNode.GetAttributeValue(xPathAttribute.AttributeName, null) : Tools.GetNodeValueBasedOnXPathReturnType<string>(htmlNode, xPathAttribute));
                            if (empty == null)
                            {
                                throw new NodeAttributeNotFoundException("Can not find " + xPathAttribute.AttributeName + " Attribute in " + htmlNode.Name + " related to " + item.PropertyType.FullName + " " + item.Name);
                            }

                            object value;
                            try
                            {
                                value = Convert.ChangeType(empty, item.PropertyType);
                            }
                            catch (FormatException)
                            {
                                throw new FormatException("Can not convert Invalid string to " + item.PropertyType.FullName + " " + item.Name);
                            }
                            catch (Exception ex3)
                            {
                                throw new Exception("Unhandled Exception : " + ex3.Message);
                            }

                            item.SetValue(obj, value, null);
                            continue;
                        }

                        IList<Type> list = item.GetGenericTypes() as IList<Type>;
                        if (list == null || list.Count == 0)
                        {
                            throw new ArgumentException(item.Name + " should have one generic argument.");
                        }

                        if (list.Count > 1)
                        {
                            throw new ArgumentException(item.Name + " should have one generic argument.");
                        }

                        if (list.Count != 1)
                        {
                            continue;
                        }

                        HtmlNodeCollection htmlNodeCollection;
                        try
                        {
                            htmlNodeCollection = htmlDocument2.DocumentNode.SelectNodes(xPathAttribute.XPath);
                        }
                        catch (XPathException ex4)
                        {
                            throw new XPathException(ex4.Message + " That means you have a syntax error in XPath property of this Property : " + item.PropertyType.FullName + " " + item.Name);
                        }
                        catch (Exception inner2)
                        {
                            throw new NodeNotFoundException("Cannot find node with giving XPath to bind to " + item.PropertyType.FullName + " " + item.Name, inner2);
                        }

                        if (htmlNodeCollection == null || htmlNodeCollection.Count == 0)
                        {
                            if (!item.IsDefined(typeof(SkipNodeNotFoundAttribute), inherit: false))
                            {
                                throw new NodeNotFoundException("Cannot find node with giving XPath to bind to " + item.PropertyType.FullName + " " + item.Name);
                            }

                            continue;
                        }

                        IList list2 = list[0].CreateIListOfType();
                        if (list[0].IsDefinedAttribute(typeof(HasXPathAttribute)))
                        {
                            foreach (HtmlNode item2 in (IEnumerable<HtmlNode>)htmlNodeCollection)
                            {
                                HtmlDocument htmlDocument4 = new HtmlDocument();
                                htmlDocument4.LoadHtml(item2.InnerHtml);
                                object encapsulatedData2 = GetEncapsulatedData(list[0], htmlDocument4);
                                list2.Add(encapsulatedData2);
                            }
                        }
                        else if (xPathAttribute.AttributeName == null)
                        {
                            try
                            {
                                list2 = Tools.GetNodesValuesBasedOnXPathReturnType(htmlNodeCollection, xPathAttribute, list[0]);
                            }
                            catch (FormatException)
                            {
                                throw new FormatException("Can not convert Invalid string in node collection to " + list[0].FullName + " " + item.Name);
                            }
                            catch (Exception ex6)
                            {
                                throw new Exception("Unhandled Exception : " + ex6.Message);
                            }
                        }
                        else
                        {
                            foreach (HtmlNode item3 in (IEnumerable<HtmlNode>)htmlNodeCollection)
                            {
                                string attributeValue = item3.GetAttributeValue(xPathAttribute.AttributeName, null);
                                if (attributeValue == null)
                                {
                                    throw new NodeAttributeNotFoundException("Can not find " + xPathAttribute.AttributeName + " Attribute in " + item3.Name + " related to " + item.PropertyType.FullName + " " + item.Name);
                                }

                                object value2;
                                try
                                {
                                    value2 = Convert.ChangeType(attributeValue, list[0]);
                                }
                                catch (FormatException)
                                {
                                    throw new FormatException("Can not convert Invalid string to " + list[0].FullName + " " + item.Name);
                                }
                                catch (Exception ex8)
                                {
                                    throw new Exception("Unhandled Exception : " + ex8.Message);
                                }

                                list2.Add(value2);
                            }
                        }

                        if (list2 == null || list2.Count == 0)
                        {
                            throw new Exception("Cannot fill " + item.PropertyType.FullName + " " + item.Name + " because it is null.");
                        }

                        item.SetValue(obj, list2, null);
                    }

                    return obj;
                }
            }

            throw new MissingXPathException("Type T must define HasXPath attribute and include properties with XPath attribute.");
        }

        //
        // Summary:
        //     Creates a new XPathNavigator object for navigating this HTML node.
        //
        // Returns:
        //     An XPathNavigator object. The XPathNavigator is positioned on the node from which
        //     the method was called. It is not positioned on the root of the document.
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, this);
        }

        //
        // Summary:
        //     Creates an XPathNavigator using the root of this document.
        public XPathNavigator CreateRootNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, OwnerDocument.DocumentNode);
        }

        //
        // Summary:
        //     Selects a list of nodes matching the HtmlAgilityPack.HtmlNode.XPath expression.
        //
        // Parameters:
        //   xpath:
        //     The XPath expression.
        //
        // Returns:
        //     An HtmlAgilityPack.HtmlNodeCollection containing a collection of nodes matching
        //     the HtmlAgilityPack.HtmlNode.XPath query, or null if no node matched the XPath
        //     expression.
        public HtmlNodeCollection SelectNodes(string xpath)
        {
            HtmlNodeCollection htmlNodeCollection = new HtmlNodeCollection(null);
            XPathNodeIterator xPathNodeIterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            while (xPathNodeIterator.MoveNext())
            {
                HtmlNodeNavigator htmlNodeNavigator = (HtmlNodeNavigator)xPathNodeIterator.Current;
                htmlNodeCollection.Add(htmlNodeNavigator.CurrentNode, setParent: false);
            }

            if (htmlNodeCollection.Count == 0 && !OwnerDocument.OptionEmptyCollection)
            {
                return null;
            }

            return htmlNodeCollection;
        }

        //
        // Summary:
        //     Selects a list of nodes matching the HtmlAgilityPack.HtmlNode.XPath expression.
        //
        // Parameters:
        //   xpath:
        //     The XPath expression.
        //
        // Returns:
        //     An HtmlAgilityPack.HtmlNodeCollection containing a collection of nodes matching
        //     the HtmlAgilityPack.HtmlNode.XPath query, or null if no node matched the XPath
        //     expression.
        public HtmlNodeCollection SelectNodes(XPathExpression xpath)
        {
            HtmlNodeCollection htmlNodeCollection = new HtmlNodeCollection(null);
            XPathNodeIterator xPathNodeIterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            while (xPathNodeIterator.MoveNext())
            {
                HtmlNodeNavigator htmlNodeNavigator = (HtmlNodeNavigator)xPathNodeIterator.Current;
                htmlNodeCollection.Add(htmlNodeNavigator.CurrentNode, setParent: false);
            }

            if (htmlNodeCollection.Count == 0 && !OwnerDocument.OptionEmptyCollection)
            {
                return null;
            }

            return htmlNodeCollection;
        }

        //
        // Summary:
        //     Selects the first XmlNode that matches the HtmlAgilityPack.HtmlNode.XPath expression.
        //
        // Parameters:
        //   xpath:
        //     The XPath expression. May not be null.
        //
        // Returns:
        //     The first HtmlAgilityPack.HtmlNode that matches the XPath query or a null reference
        //     if no matching node was found.
        public HtmlNode SelectSingleNode(string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }

            XPathNodeIterator xPathNodeIterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            if (!xPathNodeIterator.MoveNext())
            {
                return null;
            }

            return ((HtmlNodeNavigator)xPathNodeIterator.Current).CurrentNode;
        }

        //
        // Summary:
        //     Selects the first XmlNode that matches the HtmlAgilityPack.HtmlNode.XPath expression.
        //
        // Parameters:
        //   xpath:
        //     The XPath expression.
        //
        // Returns:
        //     An HtmlAgilityPack.HtmlNodeCollection containing a collection of nodes matching
        //     the HtmlAgilityPack.HtmlNode.XPath query, or null if no node matched the XPath
        //     expression.
        public HtmlNode SelectSingleNode(XPathExpression xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }

            XPathNodeIterator xPathNodeIterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            if (!xPathNodeIterator.MoveNext())
            {
                return null;
            }

            return ((HtmlNodeNavigator)xPathNodeIterator.Current).CurrentNode;
        }
    }
}



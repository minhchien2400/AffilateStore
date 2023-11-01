namespace AffiliateStoreBE.Common.Models
{
    public class ExcelColumn : Attribute
    {
        public string Name { get; }

        public List<string> Names { get; }

        /// <summary>
        /// Is Hidden Column
        /// </summary>
        public bool Hidden { get; }

        /// <summary>
        /// Is Text Wrapped Column
        /// </summary>
        public bool TextWrapped { get; }

        /// <summary>
        /// Is Editable Column
        /// </summary>
        public bool Editable { get; }

        public bool NeedTrim { get; }

        public bool UseHtml { get; }

        /// <summary>
        /// 设置Export出来的Excel 单元格格式是否为纯文本格式（纯文本格式为49）
        /// </summary>
        public bool Text { get; }

        /// <summary>
        /// 是否设置note, 值为对应字段名，为空或字段值为空则不生效
        /// </summary>
        public string NoteName { get; }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        public ExcelColumn(string name)
            : this(name, false, false, false, true, false)
        { }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="noteName">设置note, 值为对应字段名，为空或字段值为空则不生效</param>
        public ExcelColumn(string name, string noteName)
            : this(name, false, false, false, true, false, false, noteName)
        { }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="hidden">Is Hidden Column</param>
        public ExcelColumn(string name, bool hidden)
            : this(name, hidden, false, false, true, false)
        { }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="hidden">Is Hidden Column</param>
        /// <param name="textWrapped">Is Text Wrapped Column</param>
        public ExcelColumn(string name, bool hidden, bool textWrapped)
           : this(name, hidden, textWrapped, false, true, false)
        { }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="hidden">Is Hidden Column</param>
        /// <param name="textWrapped">Is Text Wrapped Column</param>
        /// <param name="canBeEdit">Is Editable Column</param>
        public ExcelColumn(string name, bool hidden, bool textWrapped, bool canBeEdit)
            : this(name, hidden, textWrapped, canBeEdit, true, false)
        {
        }

        /// <summary>
        /// Column
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="hidden">Is Hidden Column</param>
        /// <param name="textWrapped">Is Text Wrapped Column</param>
        /// <param name="canBeEdit">Is Editable Column</param>
        /// <param name="needTrim">Is auto trim Column</param>
        public ExcelColumn(string name, bool hidden, bool textWrapped, bool canBeEdit, bool needTrim)
            : this(name, hidden, textWrapped, canBeEdit, needTrim, false)
        {
        }

        public ExcelColumn(string name, bool hidden, bool textWrapped, bool canBeEdit, bool needTrim, bool useHtml, bool text = false, string noteName = null)
        {

            Names = new List<string>();
            if (string.IsNullOrEmpty(name))
            {
                Name = string.Empty;
            }
            else
            {
                Names.AddRange(name.Split(new[] { "^" }, StringSplitOptions.RemoveEmptyEntries));
                Name = Names[0];
            }
            Hidden = hidden;
            TextWrapped = textWrapped;
            Editable = canBeEdit;
            NeedTrim = needTrim;
            UseHtml = useHtml;
            Text = text;
            NoteName = noteName;
        }
    }
}

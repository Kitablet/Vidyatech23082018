
namespace FISE_API.Services.EmailService
{
    public sealed class Token
    {
        private readonly string _key;
        private readonly string _value;
        private  bool _neverHtmlEncoded;

        public Token(string key, string value):
            this(key, value, false)
        {
            
        }
        public Token(string key, string value, bool neverHtmlEncoded)
        {
            this._key = key;
            this._value = value;
            this._neverHtmlEncoded = neverHtmlEncoded;
        }

        /// <summary>
        /// Token key
        /// </summary>
        public string Key { get { return _key; } }
        /// <summary>
        /// Token value
        /// </summary>
        public string Value { get { return _value; } }
        /// <summary>
        /// Indicates whether this token should not be HTML encoded
        /// </summary>
        public bool NeverHtmlEncoded { get { return _neverHtmlEncoded; }set {  _neverHtmlEncoded=value; } }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Key, Value);
        }
    }
}

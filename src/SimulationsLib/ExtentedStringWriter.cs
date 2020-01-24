using System.IO;
using System.Text;

namespace VectoInputTester
{
    public sealed class ExtentedStringWriter : StringWriter
    {
        private readonly Encoding _stringWriterEncoding;
        public ExtentedStringWriter(StringBuilder builder, Encoding desiredEncoding)
            : base(builder)
        {
            this._stringWriterEncoding = desiredEncoding;
        }

        public ExtentedStringWriter(Encoding desiredEncoding)
            : base()
        {
            this._stringWriterEncoding = desiredEncoding;
        }

        public override Encoding Encoding
        {
            get
            {
                return this._stringWriterEncoding;
            }
        }
    }
}

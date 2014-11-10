using System.IO;
using System.Web;

namespace MrCMS.EcommerceApp.Tests.Stubs
{
    public class BasicHttpPostedFileBase : HttpPostedFileBase
    {
        private readonly MemoryStream _memoryStream;

        public BasicHttpPostedFileBase()
        {
                _memoryStream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            
        }

        public override int ContentLength
        {
            get { return (int)_memoryStream.Length; }
        }
        public override string ContentType
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }
        public override Stream InputStream
        {
            get
            {
                return _memoryStream;
            }
        }
    }
}
using System.IO;
using System.Web;

namespace MrCMS.EcommerceApp.Tests.Stubs
{
    public class BasicHttpPostedFileBaseCSV : HttpPostedFileBase
    {
        private MemoryStream _memoryStream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});

        public override int ContentLength
        {
            get { return 1; }
        }
        public override string ContentType
        {
            get { return "text/CSV"; }
        }
        public override Stream InputStream
        {
            get { return _memoryStream; }
        }
    }
}
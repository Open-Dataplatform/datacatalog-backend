using System.Net;

namespace DataCatalog.Api.IntegrationTests
{
	public class Response
	{
		public HttpWebResponse HttpWebResponse { get; set; }
		public string Content { get; set; }
		public byte[] BinaryContent { get; set; }
	}
}

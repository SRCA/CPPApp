using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CCPApp.Views;
using Xamarin.Forms;
using CCPApp.iOS.Renderers;
using System.Threading.Tasks;
using CoreGraphics;
using System.IO;

[assembly:ExportRenderer(typeof(PdfPage), typeof(PdfPageRenderer))]
namespace CCPApp.iOS.Renderers
{
	public class PdfPageRenderer : PageRenderer
	{
		nint NumberOfPages = 0;
		nfloat PageLength = 0;
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
			{
				return;
			}
			PdfPage page = (PdfPage)Element;
			string path;
			if (page.useTemp)
			{
				string tempFolder = new FileManage().GetTempFolder();
				path = System.IO.Path.Combine(tempFolder, page.FileName);
			}
			else
			{
				string libraryFolder = new FileManage().GetLibraryFolder();
				path = System.IO.Path.Combine(libraryFolder, page.FileName);
			}

			UIWebView webView = new UIWebView();

			if (path.EndsWith(".pdf"))
			{
				CGPDFDocument doc = CGPDFDocument.FromFile(path);
				NumberOfPages = (nint)doc.Pages;
			}

			//NSUrlRequest request = new NSUrlRequest(new NSUrl(path,false));
			webView.LoadRequest(new NSUrlRequest(new NSUrl(path, false)));
			webView.PaginationMode = UIWebPaginationMode.TopToBottom;
			webView.ScalesPageToFit = true;

			View = webView;
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			PdfPage page = (PdfPage)Element;
			if (page.PageNumber > 1 && NumberOfPages > 0)
			{
				UIWebView webView = (UIWebView)View;
				UIScrollView scroll = webView.ScrollView;
				PageLength = (nfloat)scroll.ContentSize.Height / NumberOfPages;
				nfloat pixelDistance = (page.PageNumber - 1) * PageLength - 3;
				scroll.ScrollRectToVisible((CGRect)new CGRect(0, pixelDistance, scroll.Bounds.Width, scroll.Bounds.Height), false);
			}
		}
	}
}
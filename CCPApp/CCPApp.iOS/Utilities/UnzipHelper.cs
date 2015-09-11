using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CCPApp.Items;
using Xamarin.Forms;
using CCPApp.iOS.Items;
using MiniZip.ZipArchive;
using System.IO;

[assembly: Dependency(typeof(UnzipHelper))]
namespace CCPApp.iOS.Items
{
	public class UnzipHelper : IUnzipHelper
	{
		public string Unzip(string fileName)
		{
			ZipArchive zip = new ZipArchive();
			//string folderName = fileName.Substring(0, fileName.Length - ".zip".Length);
			string folderName = Path.Combine((new FileManage()).GetLibraryFolder(),"Checklist");
			zip.UnzipOpenFile(fileName);
			zip.UnzipFileTo(folderName, true);
			return folderName;
		}
	}
}
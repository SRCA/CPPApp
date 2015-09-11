using CoreGraphics;
using System;
using UIKit;
using Foundation;

namespace CCPApp.iOS.Utilities
{
    public static class StringExtensions
    {
        public static float StringHeight(this string text, UIFont font, float width)
        {
            var nativeString = new NSString(text);

            var rect = (CGRect)nativeString.GetBoundingRect(
                new CoreGraphics.CGSize(width, float.MaxValue),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes() { Font = font },
                null);

            return (float)rect.Height;
        }
		public static nfloat StringHeight(this string text, UIFont font, nfloat width)
		{
			var nativeString = new NSString(text);

			var rect = (CGRect)nativeString.GetBoundingRect(
				new CoreGraphics.CGSize(width, nfloat.MaxValue),
				NSStringDrawingOptions.UsesLineFragmentOrigin,
				new UIStringAttributes() { Font = font },
				null);

			return rect.Height;
		}
    }
}


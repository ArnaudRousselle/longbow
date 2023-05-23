using System.Windows;
using System.Windows.Controls;

namespace LongBow.Common.Behaviors
{
    public class WebBrowserExtender
    {
        public static readonly DependencyProperty HtmlContentProperty = DependencyProperty.RegisterAttached(
            "HtmlContent", typeof (string), typeof (WebBrowserExtender), new PropertyMetadata(default(string), HtmlContentChanged));

        private static void HtmlContentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var webBrowser = dependencyObject as WebBrowser;

            if (webBrowser == null)
                return;
            
            var htmlContent = dependencyPropertyChangedEventArgs.NewValue as string;

            webBrowser.NavigateToString(htmlContent ?? "<html></html>");
        }

        public static void SetHtmlContent(DependencyObject element, string value)
        {
            element.SetValue(HtmlContentProperty, value);
        }

        public static string GetHtmlContent(DependencyObject element)
        {
            return (string) element.GetValue(HtmlContentProperty);
        }
    }
}

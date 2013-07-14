using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FeedReader.Resources;
using System.Threading.Tasks;
using FeedReader.Model;
using System.Xml.Linq;

using FeedReader.Code.Utilities;
using System.IO;
using System.Globalization;


namespace FeedReader {
  public partial class MainPage : PhoneApplicationPage {
    // Konstruktor
    public MainPage() {
      InitializeComponent();
    }


    private async Task<IEnumerable<FeedItem>> UpdateFeed(string url) {

      string result = "";
      var request = WebRequest.CreateHttp(url);
      request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

      var response = await request.GetResponseAsync();

      using (var stream = response.GetResponseStream()) {
        using (var reader = new StreamReader(stream)) {
          result = await reader.ReadToEndAsync();
        }
      }

      var feed = XElement.Parse(result);

      var articles = from item in feed.Descendants("item")
                     select new FeedItem() {
                       Title = item.Element("title").Value,
                       DatePublished = item.Element("pubDate").Value,
                       Description = item.Element("description").Value,
                       ArticleURL = item.Element("guid").Value
                     };

      return articles;
    }

    private async void btnRefresh_Click(object sender, RoutedEventArgs e) {
      var articles = await UpdateFeed("http://dotnetautor.de/GetRssFeed");
      lbFeed.ItemsSource = articles;

    }

  }
}
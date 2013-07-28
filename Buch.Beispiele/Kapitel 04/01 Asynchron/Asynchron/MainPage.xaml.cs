using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Asynchron.Resources;
using System.Xml.Linq;
using FeedReader.Model;
using Asynchron.Code;

namespace Asynchron {
  public partial class MainPage : PhoneApplicationPage {
    // Konstruktor
    public MainPage() {
      InitializeComponent();
    }

   
    private void btnAsync_Click(object sender, RoutedEventArgs e) {
      btnAsync.IsEnabled = false;
      
      var client = new WebClient();

      client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(loadHTMLCallback);
      client.DownloadStringAsync(new Uri("http://dotnetautor.de/GetRssFeed"));

      btnAsync.IsEnabled = true;
    }

    public void loadHTMLCallback(Object sender, DownloadStringCompletedEventArgs e) {
      var textData = (string)e.Result;
      // das Ergebnis an die ListBox übergeben
      SetItemsSOurce(textData);
    }
    
    private async void btnTask_Click(object sender, RoutedEventArgs e) {
      btnTask.IsEnabled = false;

      var client = new WebClient();
      var textData = await client.DownloadString(new Uri("http://dotnetautor.de/GetRssFeed"));

      // das Ergebnis an die ListBox übergeben
      SetItemsSOurce(textData);
    
      btnTask.IsEnabled = true;
    }

    private void SetItemsSOurce(string data) {
      var feed = XElement.Parse(data);

      var articles = from item in feed.Descendants("item")
                     select new FeedItem() {
                       Title = item.Element("title").Value,
                       DatePublished = item.Element("pubDate").Value,
                       Description = item.Element("description").Value,
                       ArticleURL = item.Element("guid").Value
                     };

      lbItems.ItemsSource = articles;
    }

  }
}
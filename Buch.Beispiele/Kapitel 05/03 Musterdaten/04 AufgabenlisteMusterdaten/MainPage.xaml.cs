using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Aufgabenliste.Resources;
using System.Collections.ObjectModel;
using Aufgabenliste.Model;

namespace Aufgabenliste {
  public partial class MainPage : PhoneApplicationPage {

    //private ObservableCollection<TodoItem> _items;

    // Konstruktor
    public MainPage() {
      InitializeComponent();
    }

  

    //protected override void OnNavigatedTo(NavigationEventArgs e) {
    //  _items = new ObservableCollection<TodoItem>(DataManager.Instance.TodoItems);
    //  lbTotoItems.ItemsSource = _items;

    //  base.OnNavigatedTo(e);
    //}

    //protected override void OnNavigatedFrom(NavigationEventArgs e) {
    //  DataManager.Instance.TodoItems = _items.ToList();

    //  base.OnNavigatedFrom(e);
    //}


    //private void btnAdd_Click(object sender, RoutedEventArgs e) {
    //  var item = new TodoItem { Name = tbTodo.Text, IsDone = false };
    //  _items.Add(item);
    //}

    //private void deleteTaskButton_Click(object sender, RoutedEventArgs e) {
    //  // den Sender als Button verwenden
    //  var button = sender as Button;

    //  if (button != null) {

    //    // Der DataContext das Buttons enthält das Element der Liste
    //    // in diesem Beispiel ist das ein TodoItem.
    //    var toDoForDelete = button.DataContext as TodoItem;

    //    // entfernen des Elementes
    //    _items.Remove(toDoForDelete);

    //  }
    //}
  }
}
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aufgabenliste.Model {
 public class DataManager {
   private DataManager() {
     TodoItems = new List<TodoItem>();
   }
   public static DataManager Instance = new DataManager();

   public List<TodoItem> TodoItems { get; set; }

 }
}

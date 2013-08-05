using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aufgabenliste.Model {
  public class DataManager {
    private DataManager() {
      TodoItems = new List<TodoItem>();
    }
    public static DataManager Instance = new DataManager();

    public List<TodoItem> TodoItems { get; set; }

    #region -= Load /Save =-

    public void Load() {
      TodoItems = DeserializeFromFile<List<TodoItem>>("Items.dat") ?? new List<TodoItem>();
    }

    public void Save() {
      SerializeToFile("Items.dat", TodoItems);
    }

    #endregion

    #region -= Helper =-
    public static bool SerializeToFile<T>(string path, T serializationSource) {
      try {
        using (var store = IsolatedStorageFile.GetUserStoreForApplication())
        using (var stream = store.OpenFile(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
#if UseDataContract
          var serializer = new DataContractSerializer(typeof(T));
          serializer.WriteObject(stream, serializationSource);
#else
          var serializer = new XmlSerializer(typeof(T));
          serializer.Serialize(stream, serializationSource);
#endif
        }
        return true;

      } catch {
        return false;
      }
    }

    public static T DeserializeFromFile<T>(string path) {
      try {
        T returnable;
        using (var store = IsolatedStorageFile.GetUserStoreForApplication())
        using (var stream = store.OpenFile(path, FileMode.Open, FileAccess.Read)) {
#if UseDataContract
          var serializer = new DataContractSerializer(typeof(T));
          returnable = (T)serializer.ReadObject(stream);
#else
          var serializer = new XmlSerializer(typeof(T));
          returnable = (T)serializer.Deserialize(stream);
#endif
        }
        return returnable;
      } catch {
        // für dan Fall das etwas beim Einlesen fehlschlägt
        return default(T);
      }
    }

    #endregion

  }
}

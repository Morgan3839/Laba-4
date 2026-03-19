/***************************
 * Author: Ksenia Dorozhko *
 ***************************/

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

[Serializable]
class TextFileClass {
  public string FilePath;
  public string Content;

  public void SaveText() {
    FileStream fileStream;
    fileStream = new FileStream(FilePath, FileMode.OpenOrCreate);
    
    StreamWriter streamWriter;
    streamWriter = new StreamWriter(fileStream);
    
    streamWriter.WriteLine(Content);
    streamWriter.Close();
  }

  public void LoadText() {
    FileStream fileStream;
    fileStream = new FileStream(FilePath, FileMode.OpenOrCreate);
    
    StreamReader streamReader;
    streamReader = new StreamReader(fileStream);
    
    Content = streamReader.ReadLine();
    streamReader.Close();
  }

  public void SerializeBinary(string binaryPath) {
    FileStream fileStream;
    fileStream = new FileStream(binaryPath, FileMode.OpenOrCreate);
    
    BinaryFormatter binaryFormatter;
    binaryFormatter = new BinaryFormatter();
    
    binaryFormatter.Serialize(fileStream, this);
    fileStream.Close();
  }

  public void DeserializeBinary(string binaryPath) {
    FileStream fileStream;
    fileStream = new FileStream(binaryPath, FileMode.OpenOrCreate);
    
    BinaryFormatter binaryFormatter;
    binaryFormatter = new BinaryFormatter();
    
    TextFileClass tempObject;
    tempObject = (TextFileClass)binaryFormatter.Deserialize(fileStream);
    
    Content = tempObject.Content;
    fileStream.Close();
  }

  public void SerializeXML(string xmlPath) {
    FileStream fileStream;
    fileStream = new FileStream(xmlPath, FileMode.OpenOrCreate);
    
    XmlSerializer xmlSerializer;
    xmlSerializer = new XmlSerializer(this.GetType());
    
    xmlSerializer.Serialize(fileStream, this);
    fileStream.Close();
  }
}

class TextSearcher {
  public bool ContainsKeyword(string textContent, string keyword) {
        if (textContent == null) {
            return false;
        }
    return textContent.Contains(keyword);
  }
}

class TextMemento {
  public string SavedContent;
}

interface ITextOriginator {
  object GetMemento();
  void SetMemento(object memento);
}

class TextEditor : ITextOriginator {
  public string CurrentText;

  public object GetMemento() {
    TextMemento memento;
    memento = new TextMemento();
    memento.SavedContent = CurrentText;
    return memento;
  }

  public void SetMemento(object memento) {
    if (memento is TextMemento) {
      TextMemento temp;
      temp = (TextMemento)memento;
      CurrentText = temp.SavedContent;
    }
  }
}

class CaretakerClass {
  private object _savedState;

  public void Save(ITextOriginator originator) {
    _savedState = originator.GetMemento();
  }

  public void Restore(ITextOriginator originator) {
    originator.SetMemento(_savedState);
  }
}

class FileApplication {
  public void Run() {
    TextFileClass textFile;
    textFile = new TextFileClass();

    TextSearcher searcher;
    searcher = new TextSearcher();

    TextEditor editor;
    editor = new TextEditor();

    CaretakerClass caretaker;
    caretaker = new CaretakerClass();

    Console.Write("Enter file path: ");
    textFile.FilePath = Console.ReadLine();

    Console.Write("Enter text: ");
    editor.CurrentText = Console.ReadLine();

    caretaker.Save(editor);

    textFile.Content = editor.CurrentText;
    textFile.SaveText();

    Console.Write("Enter keyword: ");
    string keyword;
    keyword = Console.ReadLine();

    textFile.LoadText();

    bool result;
    result = searcher.ContainsKeyword(textFile.Content, keyword);

    Console.WriteLine("Contains keyword: " + result);

    Console.Write("Change text: ");
    editor.CurrentText = Console.ReadLine();

    caretaker.Restore(editor);

    Console.WriteLine("Restored text: " + editor.CurrentText);

    textFile.SerializeBinary("data.bin");
    textFile.SerializeXML("data.xml");
  }
}

class Program {
  static void Main() {
    FileApplication application;
    application = new FileApplication();
    application.Run();
  }
}
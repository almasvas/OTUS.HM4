using Newtonsoft.Json;
using OTUS.HM4;
using System.Diagnostics;
using System.Reflection;
using System.Text;

// Подготовка данных
// Экземпляр класса
var instance = F.Get();

// INI строка
var iniString = LoadIniFile("testData.ini");

// Количество итераций
int numberOfIterations = 10000;

// Прогрев библиотеки NewtonSoft, т.к. первое обращение занмает больше времени чем последующие
JsonConvert.SerializeObject(instance);

// Замер времени рефлексии
// Единичная ериализация
var stopwatch = Stopwatch.StartNew();
var serializedString = SerializeProperties(instance);
stopwatch.Stop();
long singleSerializationTime = stopwatch.ElapsedMilliseconds;

// Множественная сериализация
stopwatch.Restart();
for (int i = 0; i < numberOfIterations; i++)
{
    _ = SerializeProperties(instance);
}
stopwatch.Stop();
long multiSerializationTime = stopwatch.ElapsedMilliseconds;

// Единичная десериализация
stopwatch.Restart();
F deserializedInstance = DeserializeFromIni<F>(iniString);
stopwatch.Stop();
long singleDeserializationTime = stopwatch.ElapsedMilliseconds;

// Множественная десериализация
stopwatch.Restart();
for (int i = 0; i < numberOfIterations; i++)
{
    _ = DeserializeFromIni<F>(iniString);
}
stopwatch.Stop();
long multiDeserializationTime = stopwatch.ElapsedMilliseconds;

// Замер времени стандартных механизмов (JSON)
// Единичная сериализация
stopwatch.Restart();
var jsonString = JsonConvert.SerializeObject(instance);
stopwatch.Stop();
long singleJsonSerializationTime = stopwatch.ElapsedMilliseconds;

//Множественная сериализация
stopwatch.Restart();
for (int i = 0; i < numberOfIterations; i++)
{
    _ = JsonConvert.SerializeObject(instance);
}
stopwatch.Stop();
long multiJsonSerializationTime = stopwatch.ElapsedMilliseconds;

// Единичная десериализация
stopwatch.Restart();
F deserializeJson = JsonConvert.DeserializeObject<F>(jsonString);
stopwatch.Stop();
long singleJsonDeSerializationTime = stopwatch.ElapsedMilliseconds;

// Множественная десериализация
stopwatch.Restart();
for (int r = 0; r < numberOfIterations; r++)
{
    _ = JsonConvert.DeserializeObject<F>(jsonString);
}
stopwatch.Stop();
long multiJsonDeSerializationTime = stopwatch.ElapsedMilliseconds;

// Вывод на экран
stopwatch.Restart();
Console.WriteLine("Serialized String: " + serializedString);
Console.WriteLine($"Serialization Time 1/{numberOfIterations}: {singleSerializationTime}/{multiSerializationTime} ms");

Console.WriteLine("Deserializing String: " + serializedString);
Console.WriteLine($"Serialization Time 1/{numberOfIterations}: {singleDeserializationTime}/{multiDeserializationTime} ms");

Console.WriteLine("JSON Serialized String: " + serializedString);
Console.WriteLine($"JSON Serialization Time 1/{numberOfIterations}: {singleJsonSerializationTime}/{multiJsonSerializationTime} ms");

Console.WriteLine("JSON Deserialized String: " + serializedString);
Console.WriteLine($"JSON Deserialization Time 1/{numberOfIterations}: {singleJsonDeSerializationTime}/{multiJsonDeSerializationTime} ms");
stopwatch.Stop();
long outputToConsoleTime = stopwatch.ElapsedMilliseconds;

Console.WriteLine($"Output Text To Console Time:{outputToConsoleTime} ms");


// Сериализация объекта
string SerializeProperties(object obj)
{
    var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
    return string.Join(", ", properties.Select(p => $"{p.Name}={p.GetValue(obj)}"));
}

// Десериализация ini строки
T DeserializeFromIni<T>(string csvData) where T : new()
{
    var instance = new T();
    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    foreach (var property in properties)
    {
        var keyValuePairs = csvData.Split("\r\n").Where(x => !string.IsNullOrEmpty(x)).Select(pair => pair.Split('=')).ToDictionary(pair => pair[0], pair => pair[1]);
        if (keyValuePairs.TryGetValue(property.Name, out var value))
        {
            property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
        }
    }

    return instance;
}

// Загрузка из файла
static string LoadIniFile(string filePath)
{
    StringBuilder sb = new();

    using (StreamReader reader = new(filePath))
    {
        string line = reader.ReadLine();
        while (line != null)
        {
            sb.AppendLine(line);
            line = reader.ReadLine();
        }
    }

    return sb.ToString();
}
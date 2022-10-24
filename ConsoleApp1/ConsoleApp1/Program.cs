using System;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Collections.Generic;
using System.Xml;
using System.IO.Compression;

namespace HelloApp
{
    class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Company { get; set; }
    }
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    class Program
    {

        public static void Compress(string sourceFile, string compressedFile)
        {
            // поток для чтения исходного файла
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                // поток для записи сжатого файла
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                        Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                            sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
                    }
                }
            }
        }

        public static void Decompress(string compressedFile, string targetFile)
        {
            // поток для чтения из сжатого файла
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                // поток для записи восстановленного файла
                using (FileStream targetStream = File.Create(targetFile))
                {
                    // поток разархивации
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                        Console.WriteLine("Восстановлен файл: {0}", targetFile);
                    }
                }
            }
        }
        static async void json_file()
        {
            // сохранение данных
            using (FileStream fs = new FileStream(@"C:\SomeDir2\user.json", FileMode.OpenOrCreate))
            {
                Person tom = new Person() { Name = "Tom", Age = 35 };
                await JsonSerializer.SerializeAsync<Person>(fs, tom);
                Console.WriteLine("Data has been saved to file");
            }

            // чтение данных
            using (FileStream fs1 = new FileStream(@"C:\SomeDir2\user.json", FileMode.OpenOrCreate))
            {
                Person restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs1);
                Console.WriteLine($"Name: {restoredPerson.Name},  Age: {restoredPerson.Age}");
            }

        }
        



    static void Main(string[] args)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                Console.WriteLine($"Название: {drive.Name}");
                Console.WriteLine($"Тип: {drive.DriveType}");
                if (drive.IsReady)
                {
                    Console.WriteLine($"Объем диска: {drive.TotalSize}");
                    Console.WriteLine($"Свободное пространство: {drive.TotalFreeSpace}");
                    Console.WriteLine($"Метка: {drive.VolumeLabel}");
                }
                Console.WriteLine();
            }

            // создаем каталог для файла
            string path = @"C:\SomeDir2";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            Console.WriteLine("Введите строку для записи в файл:");
            string text = Console.ReadLine();

            // запись в файл
            using (FileStream fstream = new FileStream($@"{path}\note.txt", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                // запись массива байтов в файл
                fstream.Write(array, 0, array.Length);
                Console.WriteLine("Текст записан в файл");
            }

            // чтение из файла
            using (FileStream fstream = File.OpenRead($@"{path}\note.txt"))
            {
                // преобразуем строку в байты
                byte[] array = new byte[fstream.Length];
                // считываем данные
                fstream.Read(array, 0, array.Length);
                // декодируем байты в строку
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine($"Текст из файла: {textFromFile}");
            }
            path = @"C:\SomeDir2\note.txt";
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                Console.WriteLine($"File_deleting");
                File.Delete(path);
            }

            
            json_file();

            
            Console.ReadLine();
            FileInfo fileInf1 = new FileInfo(@"C:\SomeDir2\user.json");
            if (fileInf1.Exists)
            {
                Console.WriteLine($"File_deleting");
                File.Delete(@"C:\SomeDir2\user.json");
            }





            List<User> users = new List<User>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\SomeDir2\users.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            // создаем новый элемент user
            XmlElement userElem = xDoc.CreateElement("user");
            // создаем атрибут name
            XmlAttribute nameAttr = xDoc.CreateAttribute("name");
            // создаем элементы company и age
            XmlElement companyElem = xDoc.CreateElement("company");
            XmlElement ageElem = xDoc.CreateElement("age");
            // создаем текстовые значения для элементов и атрибута
            XmlText nameText = xDoc.CreateTextNode("Mark Zuckerberg");
            XmlText companyText = xDoc.CreateTextNode("Facebook");
            XmlText ageText = xDoc.CreateTextNode("30");

            //добавляем узлы
            nameAttr.AppendChild(nameText);
            companyElem.AppendChild(companyText);
            ageElem.AppendChild(ageText);
            userElem.Attributes.Append(nameAttr);
            userElem.AppendChild(companyElem);
            userElem.AppendChild(ageElem);
            xRoot.AppendChild(userElem);
            xDoc.Save(@"C:\SomeDir2\users.xml");
            foreach (XmlElement xnode in xRoot)
            {
                User user = new User();
                XmlNode attr = xnode.Attributes.GetNamedItem("name");
                if (attr != null)
                    user.Name = attr.Value;

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "company")
                        user.Company = childnode.InnerText;

                    if (childnode.Name == "age")
                        user.Age = Int32.Parse(childnode.InnerText);
                }
                users.Add(user);
            }
            foreach (User u in users)
                Console.WriteLine($"{u.Name} ({u.Company}) - {u.Age}");
            Console.Read();

            /*FileInfo fileInf2 = new FileInfo(@"C:\SomeDir2\users.xml");
            if (fileInf1.Exists)
            {
                Console.WriteLine($"File_deleting");
                File.Delete(@"C:\SomeDir2\users.xml");
            }*/

            string sourceFile = "C://SomeDir2/book.pdf"; // исходный файл
            string compressedFile = "C://SomeDir2/book.gz"; // сжатый файл
            string targetFile = "C://SomeDir2/book_new.pdf"; // восстановленный файл

            // создание сжатого файла
            Compress(sourceFile, compressedFile);
            // чтение из сжатого файла
            Decompress(compressedFile, targetFile);

            Console.ReadLine();
            FileInfo fileInf3 = new FileInfo(@"C://SomeDir2/book_new.pdf");
            if (fileInf1.Exists)
            {
                Console.WriteLine($"File_deleting");
                File.Delete(@"C://SomeDir2/book_new.pdf");
            }
            FileInfo fileInf4 = new FileInfo(@"C://SomeDir2/book.gz");
            if (fileInf1.Exists)
            {
                Console.WriteLine($"File_deleting");
                File.Delete(@"C://SomeDir2/book.gz");
            }
        }
    }
}



/*
Работа с форматом XML
Создать файл формате XML из редактора
Записать в файл новые данные из консоли.
Прочитать файл в консоль.
Удалить файл.
Создание zip архива, добавление туда файла, определение размера архива
Создать архив в форматер zip
Добавить файл, выбранный пользователем, в архив
Разархивировать файл и вывести данные о нем
Удалить файл и архив*/
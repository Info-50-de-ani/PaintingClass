﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PaintingClass.Storage
{
    public static class AppdataIO
    {
        static string folderPath;

        static AppdataIO()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\PaintingClass";
            Directory.CreateDirectory(folderPath);
        }

        public static T Load<T>(string fileName, Type[] extraTypes=null) where T:class,new()
        {
            string filePath = folderPath + @"\" +fileName;
            XmlSerializer serializer = new XmlSerializer(typeof(T),extraTypes);

            if (File.Exists(filePath) == true)
            {
                using FileStream fs = File.OpenRead(filePath);
                if (fs.Length == 0)
                    return new T();
                return (T)serializer.Deserialize(fs);
            }
            else
                return new T();
        }

        public static void Save<T>(string fileName, T obj, Type[] extraTypes=null) 
        {
            string filePath = folderPath + @"\" + fileName;
            XmlSerializer serializer = new XmlSerializer(typeof(T),extraTypes);

            File.Delete(filePath);
            using FileStream fs = File.OpenWrite(filePath);
            serializer.Serialize(fs, obj);
        }
    }
}

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PaintingClass.Storage
{
    [Serializable]
    public class Settings
    {

        void Save()
        {
            AppdataIO.Save<Settings>("GlobalSettings.xml", this);
        }
    }
}

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PaintingClass.Storage
{
    [Serializable]
    public class Settings
    {
		//daca sa genereze un nou ClientId la fiecare start al programului si sa ignore cel din fisier 
		//daca este false atunci clientul se poate reconecta la acelasi room chiar daca da crash
		const bool regenerateClientIdAtStart = true;

		static Settings _instance;
		public static Settings instance
        {
            get
            {
                if (_instance==null)
                {
                    _instance = AppdataIO.Load<Settings>("Settings.xml");
                    if (_instance == null) _instance = new Settings();
                    _instance.deserializationFinished = true;
                }
                return _instance;
            }
        }

        //ne asiguram ca nu salvam cand deserializer-ul seteaza variabilele
        bool deserializationFinished;

        int _clientID;
        public int clientID
        {
            get
            {
                //daca nu am generat un clientID generam unul
                if (_clientID==0)
                {
                    _clientID = (new Random(DateTime.Now.Millisecond)).Next(1,int.MaxValue);
                    Save();
                }

                return _clientID;
            }

            set
            {
                if (deserializationFinished || regenerateClientIdAtStart==false)
                    _clientID = value;
                Save();
            }
        }

        int _profToken;
        public int profToken
		{
            get => _profToken;
			set
			{
                if (deserializationFinished)
                    _profToken = value;
                Save();
            }
        }

        void Save()
        {
            if (deserializationFinished)
                AppdataIO.Save<Settings>("Settings.xml", this);
        }
    }
}

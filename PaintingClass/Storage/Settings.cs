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
        public static void Init()
		{
            instance = AppdataIO.Load<Settings>("Settings.xml");
            if (instance == null)
                instance = new Settings();
            instance.deserializationFinished = true;
        }
		public static Settings instance{ get;set; }

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
                _clientID = value;
                if (deserializationFinished) 
                    Save();
                else if (regenerateClientIdAtStart)
				{
                    _clientID = 0;
				}

            }
        }

        string _elevName;
        public string elevName
		{
            get => _elevName;
            set
			{
                _elevName = value;
                if (deserializationFinished)
                    Save();
			}
        }


        int _profToken;
        public int profToken
		{
            // TODO de terminat ProfToken storing
            get => 0;
			set
			{
                _profToken = value;
                if (deserializationFinished)
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

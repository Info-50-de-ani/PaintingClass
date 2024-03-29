﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
namespace PaintingClass.Login
{
	/// <summary>
	/// Folosit pentru a trimite informatile obtinute din procesul de inregistrare la server
	/// </summary>
	[Serializable]
	class RegisterUserData
	{
		public string name { set; get; }
		public string email { set; get; }
		public string password { set; get; }
	}
	/// <summary>
	/// mesaj trimis la server 
	/// contine email si parola pt login
	/// </summary>
	[Serializable]
	class LoginUserData
	{
		public string email { set; get; }
		public string password { set; get; }
	}
	/// <summary>
	/// Raspuns de la server
	/// serverul contine acelasi enum
	/// </summary>
	enum ServerResponse
	{
		AlreadyRegistered = 0, Fail, Succes, NotRegistered, WaitingForConfirmation
	}

	/// <summary>
	/// Interaction logic for LoginSauRegister.xaml
	/// </summary>
	public partial class LoginSauRegister : Page
	{
		private Frame CurrentFrame;
		public LoginSauRegister(Frame frame)
		{
			InitializeComponent();
			if (!App.DEBUG_MODE)
				Injector.Visibility = Visibility.Hidden;
			CurrentFrame = frame;
			ShowLoginMenu();
			StartWindow.FadeAnimateElement(this, new Duration(new TimeSpan(0, 0, 0, 0, 400)), false);
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			ShowRegisterMenu();
		}

		#region Debugging

		//pt debbuging
		private void Inject_Click(object sender, RoutedEventArgs e)
		{
			UserData ud = new UserData
			{
				name = "profu",
				clientID = PaintingClass.Storage.Settings.instance.clientID,
				profToken = 1234,
				roomId = 0
			};

			(new MainWindow(ud)).Show();
			Window.GetWindow(CurrentFrame).Close();
		}
		
		#endregion

		#region Login

		private void ShowLoginMenu()
		{
			LoginMenuTitle.Visibility = Visibility.Visible;
			LoginMenu.Visibility = Visibility.Visible;
			RegisterMenu.Visibility = Visibility.Hidden;
			LoginBackground.Source = new BitmapImage(new Uri("pack://application:,,,/Login/Images/LoginBackground.jpg"));
		}

		/// <summary>
		/// are loc cand profesorul apasa pe butonul de completare a logarii
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EnterINFO_Login_Button_Click(object sender, RoutedEventArgs e)
		{
			// Verificam ca sintaxa datelor trimise sa fie corecta
			if (!SyntaxCheck.CheckPassword(TB_Login_Password.Password) || !SyntaxCheck.CheckEmail(TB_Login_Email.Text))
			{
				TB_Login_Error.Visibility = Visibility.Visible;
				TB_Login_Error.Text = "Email sau parola scrise incorect.";
				TB_Login_Email.isSyntaxCorrect = false;
				TB_Login_Password.isSyntaxCorrect = false;
				return;
			}

			TB_Login_Email.isSyntaxCorrect = true;
			TB_Login_Password.isSyntaxCorrect = true;

			LoginUserData loginUserData = new LoginUserData() { email = TB_Login_Email.Text, password = TB_Login_Password.Password };

			#region Server Response

			// primeste raspunsul de la server
			string[] response = (await SendLoginData(loginUserData)).Split();

			// daca sunt trei segmente trimise de server inseamna ca a trimis si tokenul si numele 
			// ce inseamna automat ca logarea a fost acceptata
			if (response.Length > 1)
			{
				if ((ServerResponse)int.Parse(response[0]) != ServerResponse.Succes)
					throw new Exception("Mesajul raspuns al serverului trebuie sa fie succes daca este trimis in trei segmente");
				
				// afisam ecranul de login succes
				LoginMenu.Visibility = Visibility.Hidden;
				Login_Succes_Screen.Visibility = Visibility.Visible;
				await Task.Delay(1000);

				int profToken = int.Parse(response[1]);

				// stocam tokenul 
				//Storage.Settings.instance.profToken = profToken;

				UserData userData = new UserData
				{
					// numele va fi schimbat cand este primit roomID-ul de la server 
					name= response[2],
					clientID = PaintingClass.Storage.Settings.instance.clientID,
					profToken = profToken,
					roomId = 0
				};

				Storage.Settings.instance.profToken = profToken;

				CurrentFrame.Content = new ProfesorGenRoom(CurrentFrame,userData);
				return;
			}
			else
			{
				// pt celelalte raspunsuri de la server care nu sunt "succes"
				switch ((ServerResponse)int.Parse(response[0]))
				{
					case ServerResponse.AlreadyRegistered:
						throw new Exception("Raspuns invalid al serverului");
					case ServerResponse.Fail:
						throw new Exception("S-a produs o eraore la server cand a primit mesajul");
					case ServerResponse.NotRegistered:
						TB_Login_Error.Visibility = Visibility.Visible;
						TB_Login_Error.Text = "Email sau parola introduse gresit.";
						break;
				}
			}

			#endregion
		}

		/// <summary>
		/// Trimite informatiile de login la server ca acesta sa verifice parola si emailul 
		/// in baza de date
		/// </summary>
		/// <param name="loginUserData"></param>
		/// <returns></returns>
		private async Task<string> SendLoginData(LoginUserData loginUserData)
		{
			string result = null;
			SemaphoreSlim semaphore = new SemaphoreSlim(0);
			WebSocket ws = new WebSocket(Networking.Constants.urlWebSocket + "/login");
			ws.OnMessage += (sender, e) =>
			{
				//raspunsul serverului la datele trimise 
				if (string.IsNullOrEmpty(e.Data))
					throw new Exception("Raspunsul primit de la server este gol");
				result = e.Data;
				semaphore.Release();
				ws.Close();
			};
			ws.Connect();
			ws.Send(JsonSerializer.Serialize(loginUserData));
			await semaphore.WaitAsync();
			Debug.Assert(result != null);
			return result;
		}
		#endregion

		#region Register)G

		/// <summary>
		/// Afiseaza meniul de inregistrare
		/// </summary>
		private void ShowRegisterMenu()
		{
			InnerBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#251351"));
			StartWindow.FadeAnimateElement(LoginBackground, new Duration(new TimeSpan(0, 0, 0, 0, 300)),true);;
			StartWindow.FadeAnimateElement(RegisterBackground, new Duration(new TimeSpan(0, 0, 0, 0, 300)),false);;
			LoginMenuTitle.Visibility = Visibility.Hidden;
			LoginMenu.Visibility = Visibility.Hidden;
			RegisterMenu.Visibility = Visibility.Visible;
			RegisterBackground.Source = new BitmapImage(new Uri("pack://application:,,,/Login/Images/RegisterBackground.jpg"));
		}

		/// <summary>
		/// are loc cand profesorul apasa pe butonul de completare a inregistrarii
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void EnterINFO_Register_Button_Click(object sender, RoutedEventArgs e)
		{
			#region Check for errors
			if (!SyntaxCheck.CheckEmail(TB_Register_Email.Text))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa introduceti o adresa de email valida.";
				return;
			}

			if (TB_Register_Name.Text.Length < 5)
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Numele este prea scurt.";
				return;
			}

			if (!SyntaxCheck.CheckEmail(TB_Register_Email.Text))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa NU introduceti caractere speciale in nume.";
				return;
			}

			if (!SyntaxCheck.CheckPassword(TB_Register_Password.Password))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa folositi o parola mai lunga.";
				return;
			}

			if (TB_Register_Password.Password != TB_Register_Password_Confirm.Password)
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Parolele nu coincid.";
				return;
			}
			#endregion

			RegisterUserData registerUserData = new RegisterUserData()
			{
				email = TB_Register_Email.Text,
				name = TB_Register_Name.Text,
				password = TB_Register_Password.Password
			};

			ServerResponse result = (ServerResponse)(int.Parse(await SendRegisterationData(registerUserData)));

			switch (result)
			{
				case ServerResponse.Succes:
					Registration_Waiting_For_Confirmation_Screen.Visibility = Visibility.Hidden;
					RegisterMenu.Visibility = Visibility.Hidden;
					Registration_Succes_Screen.Visibility = Visibility.Visible;
					break;
				case ServerResponse.Fail:
					throw new Exception("Registration failed");
				case ServerResponse.AlreadyRegistered:
					TB_Register_Error.Visibility = Visibility.Visible;
					TB_Register_Error.Text = "Sunteti deja inregistrat. Va rugam frumos sa va logati.";
					break;
			}
		}


		/// <summary>
		/// contacteaza serverul pentru a trimite datele primite de la utilizator
		/// </summary>
		/// <param name="registerUserData"></param>
		/// <returns></returns>
		private async Task<string> SendRegisterationData(RegisterUserData registerUserData)
		{
			string result = null;
			SemaphoreSlim semaphore = new SemaphoreSlim(0);

			WebSocket ws = new WebSocket(Networking.Constants.urlWebSocket + "/register");
			ws.SslConfiguration.ServerCertificateValidationCallback = Networking.RoomManager.CertificateValidation;

			ws.OnMessage += (sender, e) =>
			{
				//raspunsul serverului la datele trimise 
				if (string.IsNullOrEmpty(e.Data))
					throw new Exception("Raspunsul primit de la server este gol");
				result = e.Data;
				// daca mesjul este de tip asteptare pentru confirmare informam userul si asteptam un al
				// doilea mesaj ce va determina rezultatul inregistrarii
				if ((ServerResponse)(int.Parse(e.Data)) == ServerResponse.WaitingForConfirmation)
				{
					Dispatcher.Invoke(() =>
					{
						RegisterMenu.Visibility = Visibility.Hidden;
						Registration_Waiting_For_Confirmation_Screen.Visibility = Visibility.Visible;
					});
				}
				else
				{
					ws.Close();
					semaphore.Release();
				}
			};
			ws.Connect();
			ws.Send(JsonSerializer.Serialize(registerUserData));

			await semaphore.WaitAsync();

			Debug.Assert(result != null);
			return result;
		}

		#region Colorare activa a erorilor Register

		private void TB_Register_Name_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TB_Register_Name.Text == "")
				return;

			if (!SyntaxCheck.CheckName(TB_Register_Name.Text))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa introduceti un nume valid.";
				TB_Register_Name.isSyntaxCorrect = false;
				return;
			}

			TB_Register_Name.isSyntaxCorrect = true;
			TB_Register_Error.Visibility = Visibility.Hidden;
		}

		private void TB_Register_Email_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TB_Register_Email.Text == "")
				return;
			if (!SyntaxCheck.CheckEmail(TB_Register_Email.Text))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa introduceti o adresa de email valida.";
				TB_Register_Email.isSyntaxCorrect = false;
				return;
			}
			TB_Register_Email.isSyntaxCorrect = true;
			TB_Register_Error.Visibility = Visibility.Hidden;
		}

		private void TB_Register_Password_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (TB_Register_Password.Password == "")
				return;
			if (!SyntaxCheck.CheckPassword(TB_Register_Password.Password))
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Va rugam sa folositi o parola mai puternica.";
				TB_Register_Password.isSyntaxCorrect = false;
				return;
			}
			TB_Register_Password.isSyntaxCorrect = true;
			TB_Register_Error.Visibility = Visibility.Hidden;

		}

		private void TB_Register_Password_Confirm_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (TB_Register_Password_Confirm.Password == "")
				return;
			if (TB_Register_Password.Password != TB_Register_Password_Confirm.Password)
			{
				TB_Register_Error.Visibility = Visibility.Visible;
				TB_Register_Error.Text = "Parolele nu coincid.";
				TB_Register_Password_Confirm.isSyntaxCorrect = false;
				return;
			}
			TB_Register_Password_Confirm.isSyntaxCorrect = true;
			TB_Register_Error.Visibility = Visibility.Hidden;
		}

		#endregion

		#endregion

	}
}

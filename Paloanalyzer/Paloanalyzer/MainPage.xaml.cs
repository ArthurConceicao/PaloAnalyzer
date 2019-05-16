using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http;

namespace Paloanalyzer
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(true)]
	public partial class MainPage : ContentPage
	{

		private MediaFile _mediaFile;
		public MainPage()
		{
			InitializeComponent();
		}

		private async void PickPhoto_Clicked(object sender, EventArgs args)
		{
			await CrossMedia.Current.Initialize();

			if(!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Falha", "Carregar foto indisponível.", "OK");
			}

			_mediaFile = await CrossMedia.Current.PickPhotoAsync();

			if (_mediaFile == null)
				return;

			FileImage.Source = ImageSource.FromStream(() =>
			{
				return _mediaFile.GetStream();
			});
		}

		private async void TakePhoto_Clicked(object sender, EventArgs args)
		{
			await CrossMedia.Current.Initialize();

			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("Falha", "nenhuma câmera disponível.", "OK");
			}

			_mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "myImage.png"
			});

			if (_mediaFile == null)
				return;


			FileImage.Source = ImageSource.FromStream(() =>
			{
				return _mediaFile.GetStream();
			});
		}

		private async void RequestAnalyze_Clicked(object sender, EventArgs args)
		{
			var content = new MultipartFormDataContent();

			content.Add(new StreamContent(_mediaFile.GetStream()),
				"\"image\"",
				$"\"{_mediaFile.Path}\"");

			var httpClient = new HttpClient();

			var uploadServiceBaseAddress = "http://10.0.2.2:8000/api/analyze";

			var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

			var responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

			await DisplayAlert("Response", responseMessage, "OK");
		}
	}
}

using Android.Media;
using Android.Webkit;
using CVTSistemaRondas.Models;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAudio = CVTSistemaRondas.Models.IAudio;

namespace CVTSistemaRondas.Platforms.Android
{
    public class AudioService : IAudio
    {
        private readonly IAudioManager audioManager;
        public AudioService()
        {
            audioManager = new Plugin.Maui.Audio.AudioManager();
        }
        public async void PlayAudioFile(string fileName)
        {
            var audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(fileName));
            audioPlayer.Play();
        }
    }
}

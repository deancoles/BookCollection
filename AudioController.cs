using Plugin.Maui.Audio; // Ensure you have this NuGet package installed
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoirotCollectionApp
{
    public class AudioController
    {
        private readonly IAudioManager _audioManager;
        private readonly Dictionary<string, IAudioPlayer> _audioPlayers;

        public AudioController(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _audioPlayers = new Dictionary<string, IAudioPlayer>();
        }

        public async Task LoadTrackAsync(string trackName, string filePath)
        {
            if (!_audioPlayers.ContainsKey(trackName))
            {
                var stream = await FileSystem.OpenAppPackageFileAsync(filePath);
                var player = _audioManager.CreatePlayer(stream);
                _audioPlayers[trackName] = player;
            }
        }

        public void PlayTrack(string trackName, bool loop = true)
        {
            if (_audioPlayers.TryGetValue(trackName, out var player))
            {
                player.Loop = loop;
                player.Play();
            }
        }

        public void StopTrack(string trackName)
        {
            if (_audioPlayers.TryGetValue(trackName, out var player))
            {
                player.Stop();
            }
        }
    }

}

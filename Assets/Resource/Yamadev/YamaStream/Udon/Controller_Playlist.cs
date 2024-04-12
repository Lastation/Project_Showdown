
using System;
using UdonSharp;
using UnityEngine;

namespace Yamadev.YamaStream
{
    public partial class Controller : Listener
    {
        [SerializeField] Playlist _queue;
        [SerializeField] Playlist _history;
        [SerializeField] Playlist[] _playlists;
        [SerializeField, UdonSynced, FieldChangeCallback(nameof(ShufflePlay))] bool _shuffle = false;
        [UdonSynced] int _activePlaylistIndex = -1;
        [UdonSynced] int _playingTrackIndex = -1;

        public Playlist[] Playlists => _playlists;
        public Playlist ActivePlaylist => _activePlaylistIndex >= 0 ? _playlists[_activePlaylistIndex] : null;
        public int PlayingTrackIndex => _playingTrackIndex;
        public Playlist Queue => _queue;
        public Playlist History => _history;

        public bool ShufflePlay
        {
            get => _shuffle;
            set
            {
                _shuffle = value;
                if (!_isLocal) this.SyncVariables();
                SendCustomVideoEvent(nameof(Listener.OnShufflePlayChanged));
            }
        }

        public void PlayTrack(Playlist playlist, int index)
        {
            Track track = playlist.GetTrack(index);
            if (playlist == _queue || playlist == _history)
            {
                _activePlaylistIndex = -1;
                _playingTrackIndex = -1;
            }
            else
            {
                _activePlaylistIndex = Array.IndexOf(_playlists, playlist);
                _playingTrackIndex = index;
            }
            PlayTrack(track);
        }

        public void Backward()
        {
            if (ActivePlaylist == null || _playingTrackIndex < 0 || !ActivePlaylist.IsStatic) return;
            int nextIndex = _playingTrackIndex - 1 < 0 ? ActivePlaylist.Length - 1 : _playingTrackIndex - 1;
            PlayTrack(ActivePlaylist, nextIndex);
        }

        public void Forward()
        {
            if (_queue.Length > 0)
            {
                PlayTrack(_queue, 0);
                _queue.Remove(0);
                return;
            }
            if (ActivePlaylist == null || _playingTrackIndex < 0 || !ActivePlaylist.IsStatic) return;
            int nextIndex = _shuffle ? UnityEngine.Random.Range(0, ActivePlaylist.Length) : _playingTrackIndex + 1 < ActivePlaylist.Length ? _playingTrackIndex + 1 : 0;
            PlayTrack(ActivePlaylist, nextIndex);
        }
    }
}
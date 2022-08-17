using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace amethyst_installer_gui {
    public static class SoundPlayer {

        /// <summary>
        /// Maximum amount of supported voices
        /// </summary>
        private const int MAX_VOICES = 8;

        private const int AUDIO_TIMESTAMP_MILLIS = 100;
        private static WaveOutEvent[] m_audioDevices;
        private static bool[] m_voiceAvailability;

        static SoundPlayer() {
            m_audioDevices = new WaveOutEvent[MAX_VOICES];
            m_voiceAvailability = new bool[MAX_VOICES];
            for (int i = 0; i < MAX_VOICES; i++ ) {
                m_audioDevices[i] = new WaveOutEvent();
            }
        }

        /// <summary>
        /// Returns the index of the next valid voice. Returns -1 if no voices are available.
        /// </summary>
        private static int GetVoice() {
            for (int i = 0; i < m_voiceAvailability.Length; i++ ) {
                if ( m_voiceAvailability[i] == false )
                    return i;
            }
            return -1;
        }

        public static void PlaySound(string name) {
            Task.Run(() => {
                using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.Sounds.{name}.ogg") ) {
                    using ( var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(resource) ) {

                        // The voice dedicated to this sound event
                        int voice = 0;

                        // Wait until we have a valid voice
                        while ( (voice = GetVoice()) == -1 ) {
                            Thread.Sleep(AUDIO_TIMESTAMP_MILLIS);
                        }

                        // Lock the voice
                        m_voiceAvailability[voice] = true;
                        // Play the audio
                        m_audioDevices[voice].Init(vorbisStream);
                        m_audioDevices[voice].Play();

                        // Stall the thread so that we can hear the audio
                        while ( m_audioDevices[voice].PlaybackState == PlaybackState.Playing ) {
                            Thread.Sleep(AUDIO_TIMESTAMP_MILLIS);
                        }

                        m_voiceAvailability[voice] = false;
                    }
                }
            });
        }

        public static void PlaySound(SoundEffect effect) {
            string name = "Invoke";
            switch ( effect ) {
                case SoundEffect.Focus:
                    name = "Focus";
                    break;
                case SoundEffect.Invoke:
                    name = "Invoke";
                    break;
                case SoundEffect.Show:
                    name = "Show";
                    break;
                case SoundEffect.Hide:
                    name = "Hide";
                    break;
                case SoundEffect.MoveNext:
                    name = "MoveNext";
                    break;
                case SoundEffect.MovePrevious:
                    name = "MovePrevious";
                    break;
                case SoundEffect.GoBack:
                    name = "GoBack";
                    break;
                case SoundEffect.Error:
                    name = "Error";
                    break;
            }
            PlaySound(name);
        }
    }

    public enum SoundEffect {
        Focus,
        Invoke,
        Show,
        Hide,
        MoveNext,
        MovePrevious,
        GoBack,
        Error
    }
}

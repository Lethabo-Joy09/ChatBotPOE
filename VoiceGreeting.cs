using System.Speech.Synthesis;

namespace ChatBotPOE
{
    /// <summary>
    /// VoiceGreeting provides text-to-speech functionality for the chatbot.
    /// Uses the built-in Windows Speech Synthesis engine.
    /// Speaks messages asynchronously so the UI remains responsive.
    /// </summary>
    public static class VoiceGreeting
    {
        // Speech synthesizer instance
        private static SpeechSynthesizer synth = new SpeechSynthesizer();

        /// <summary>
        public static void Speak(string text)
        {
            synth.SpeakAsync(text);
        }
    }
}
        /// Speaks the provided text using the system's default voice.
        /// Operates asynchronously (non-blocking).
        /// </summary>

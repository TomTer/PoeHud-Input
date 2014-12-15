using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace PoeHUD.Hud
{
	public class Sounds
	{
		private static readonly Dictionary<string, SoundPlayer> allSounds = new Dictionary<string, SoundPlayer>(StringComparer.OrdinalIgnoreCase);


		public const string Alert = "sounds/alert.wav";
		public const string Danger = "sounds/danger.wav";

		public static SoundPlayer AlertSound { get {
			SoundPlayer sp;
			return allSounds.TryGetValue(Alert, out sp) ? sp : null; 
		} }
		public static SoundPlayer DangerSound { get {
			SoundPlayer sp;
			return allSounds.TryGetValue(Danger, out sp) ? sp : null; 
		} }

		public static SoundPlayer GetPlayer(string fileName)
		{
			SoundPlayer sp;
			if (allSounds.TryGetValue(fileName, out sp))
				return sp;
			throw new InvalidOperationException("Sound '" + fileName + "'has to be preloaded!");
		}


		public static void PreLoadSound(string fileName)
		{
			SoundPlayer sp;
			if (allSounds.TryGetValue(fileName, out sp)) return;

			try {
				allSounds[fileName] = null;
				sp = new SoundPlayer(fileName);
				sp.LoadCompleted += (s, e) => { if( !e.Cancelled && e.Error == null ) allSounds[fileName] = sp; };
				sp.LoadAsync();
			} catch (Exception ex) {
				MessageBox.Show("Error when loading " + fileName + ": " + ex.Message);
				Environment.Exit(0);
			}
		}

		internal static void PreLoadCommonSounds()
		{
			PreLoadSound(Alert);
			PreLoadSound(Danger);
		}
	}
}
 
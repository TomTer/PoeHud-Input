using System;
using System.Media;
using System.Windows.Forms;
namespace ExileHUD
{
	public class Sounds
	{
		public static SoundPlayer AlertSound;
		public static SoundPlayer DangerSound;
		public static void LoadSounds()
		{
			try
			{
				Sounds.AlertSound = new SoundPlayer("sounds/alert.wav");
				Sounds.AlertSound.Load();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error when loading alert.wav: " + ex.Message);
				Environment.Exit(0);
			}
			try
			{
				Sounds.DangerSound = new SoundPlayer("sounds/danger.wav");
				Sounds.DangerSound.Load();
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Error when loading danger.wav: " + ex2.Message);
				Environment.Exit(0);
			}
		}
	}
}

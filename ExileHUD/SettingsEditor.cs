using BotFramework;
using System;
using System.Windows.Forms;
namespace ExileHUD
{
	public class SettingsEditor
	{
		private SettingsForm form;
		public SettingsEditorStatus Status
		{
			get
			{
				if (this.form == null || this.form.IsDisposed || !this.form.Visible)
				{
					return SettingsEditorStatus.Closed;
				}
				if (this.form.Handle == Imports.GetForegroundWindow())
				{
					return SettingsEditorStatus.Foreground;
				}
				return SettingsEditorStatus.Minimized;
			}
		}
		public void Show()
		{
			switch (this.Status)
			{
			case SettingsEditorStatus.Closed:
				this.form = new SettingsForm();
				this.form.Show();
				break;
			case SettingsEditorStatus.Minimized:
				this.form.WindowState = FormWindowState.Normal;
				this.form.TopMost = true;
				this.form.TopMost = false;
				return;
			case SettingsEditorStatus.Foreground:
				break;
			default:
				return;
			}
		}
		public void Minimize()
		{
			switch (this.Status)
			{
			case SettingsEditorStatus.Closed:
				this.form = new SettingsForm();
				this.form.Show();
				this.form.WindowState = FormWindowState.Minimized;
				return;
			case SettingsEditorStatus.Minimized:
				break;
			case SettingsEditorStatus.Foreground:
				this.form.WindowState = FormWindowState.Minimized;
				break;
			default:
				return;
			}
		}
	}
}

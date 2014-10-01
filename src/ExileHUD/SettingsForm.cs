using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PoeHUD.ExileHUD
{
	public class SettingsForm : Form
	{
		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(494, 262);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SettingsForm";
			this.Text = "SettingsEditor";
			base.ResumeLayout(false);
		}
		public SettingsForm()
		{
			this.InitializeComponent();
			foreach (CheckBox current in this.GetAll<CheckBox>())
			{
				current.Checked = Settings.GetBool((string)current.Tag);
				current.CheckedChanged += new EventHandler(this.CheckBoxChanged);
			}
		}
		private void CheckBoxChanged(object sender, EventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			Settings.SetBool((string)checkBox.Tag, checkBox.Checked);
		}
		private IEnumerable<T> GetAll<T>() where T : Control
		{
			List<T> list = new List<T>();
			Queue<Control> queue = new Queue<Control>();
			queue.Enqueue(this);
			while (queue.Count > 0)
			{
				Control control = queue.Dequeue();
				foreach (object current in control.Controls)
				{
					queue.Enqueue((Control)current);
				}
				if (control is T)
				{
					list.Add((T)((object)control));
				}
			}
			return list;
		}
	}
}

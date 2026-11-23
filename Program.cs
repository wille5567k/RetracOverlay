using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RetracOverlay;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new OverlayMenu());
    }
}

public sealed class OverlayMenu : Form
{
    const int HotkeyId = 8128;
    const int WmHotkey = 0x0312;
    bool registered;

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool RegisterHotKey(IntPtr handle, int id, uint modifiers, uint key);
    [DllImport("user32.dll")]
    static extern bool UnregisterHotKey(IntPtr handle, int id);

    public OverlayMenu()
    {
        Text = "RETRAC // COMMAND MENU";
        Size = new Size(380, 420);
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        BackColor = Color.FromArgb(12, 8, 11);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        var heading = new Label {
            Text = "RETRAC // COMMAND MENU",
            ForeColor = Color.FromArgb(255, 65, 92),
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            Location = new Point(18, 16),
            AutoSize = true
        };
        Controls.Add(heading);

        var note = new Label {
            Text = "Overlay prototype — no game data connection",
            ForeColor = Color.LightGray,
            Location = new Point(18, 57),
            AutoSize = true
        };
        Controls.Add(note);

        var tabs = new TabControl {
            Location = new Point(15, 95),
            Size = new Size(335, 260)
        };
        foreach (string title in new[] { "Aim", "Visuals", "Misc" })
        {
            var page = new TabPage(title) { BackColor = Color.FromArgb(28, 13, 19), ForeColor = Color.White };
            tabs.TabPages.Add(page);
        }
        AddCheck(tabs.TabPages[0], "Aimbot (preview)", 15, 20);
        AddCheck(tabs.TabPages[0], "Legit aim (preview)", 15, 55);
        var target = new ComboBox {
            Location = new Point(15, 95), Width = 180,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        target.Items.AddRange(new object[] { "Head", "Torso" });
        target.SelectedIndex = 0;
        tabs.TabPages[0].Controls.Add(target);
        AddCheck(tabs.TabPages[1], "Player ESP (preview)", 15, 20);
        AddCheck(tabs.TabPages[1], "Head markers (preview)", 15, 55);
        AddCheck(tabs.TabPages[2], "Macro editor (preview)", 15, 20);
        AddCheck(tabs.TabPages[2], "Spinbot (preview)", 15, 55);
        Controls.Add(tabs);

        Controls.Add(new Label {
            Text = "F8 = Show / Hide   |   No online tracking",
            Location = new Point(18, 370),
            AutoSize = true,
            ForeColor = Color.FromArgb(255, 120, 140)
        });
    }

    static void AddCheck(Control parent, string label, int x, int y)
    {
        parent.Controls.Add(new CheckBox {
            Text = label,
            Location = new Point(x, y),
            Width = 260,
            ForeColor = Color.White
        });
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        registered = RegisterHotKey(Handle, HotkeyId, 0, (uint)Keys.F8);
        if (!registered)
            MessageBox.Show("F8 is already in use. Hotkey could not be registered.");
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WmHotkey && m.WParam.ToInt32() == HotkeyId)
        {
            if (Visible) Hide();
            else { Show(); Activate(); }
            return;
        }
        base.WndProc(ref m);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (registered) UnregisterHotKey(Handle, HotkeyId);
        base.OnFormClosed(e);
    }
}

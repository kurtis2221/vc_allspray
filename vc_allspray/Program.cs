using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace vc_allspray
{
    internal static class Program
    {
        //Source
        //https://gamefaqs.gamespot.com/boards/561545-grand-theft-auto-vice-city-the-definitive-edition/53681868

        const string title = "GTA Vice City - All vehicles sprayable";
        const string game_exe = "gta-vc.exe";
        const uint hex_address = 0x308F3;

        [STAThread]
        static void Main()
        {
            byte[] hex_original = { 0x30, 0xC0 };
            byte[] hex_patched = { 0xB0, 0x01 };
            byte[] buffer = new byte[2];

            try
            {
                if (!File.Exists(game_exe))
                {
                    ShowError(game_exe + " not found!");
                    return;
                }
                using (FileStream fs = new FileStream(game_exe, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Position = hex_address;
                    fs.Read(buffer, 0, 2);
                    if (buffer.SequenceEqual(hex_original))
                    {
                        if (ShowQuestion("Patch " + game_exe + "?") != DialogResult.Yes) return;
                        fs.Position = hex_address;
                        fs.Write(hex_patched, 0, 2);
                        ShowInfo(game_exe + " patched successfully!");
                        return;
                    }
                    if (buffer.SequenceEqual(hex_patched))
                    {
                        if (ShowQuestion("Restore " + game_exe + "?") != DialogResult.Yes) return;
                        fs.Position = hex_address;
                        fs.Write(hex_original, 0, 2);
                        ShowInfo(game_exe + " restored successfully!");
                        return;
                    }
                    ShowError(game_exe + " has different bytes!\nExpected: " + BitConverter.ToString(hex_original) + "\nGot: " + BitConverter.ToString(buffer));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        static void ShowInfo(string msg)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void ShowError(string msg)
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static DialogResult ShowQuestion(string msg)
        {
            return MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
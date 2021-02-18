using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    class Main
    {
        internal const string PluginName = "Base64 Encode/Decode";
        static string iniFilePath = null;
        static bool someSetting = false;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());

        public static void OnNotification(ScNotification notification)
        {  
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Encode", base64Encode);
            PluginBase.SetCommand(1, "Decode", base64Decode);
            idMyDlg = 1;
        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }

        internal static void base64Encode() {
            try
            {
                var text = editor.GetSelText();
                if (text.Length == 0)
                {
                    MessageBox.Show("Please select the text to be encoded.");
                }
                string encodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                editor.ReplaceSel(encodedString);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
            
        }
        
        internal static void base64Decode()
        {
            try
            {
                var encodedString = editor.GetSelText();
                if (encodedString.Length == 0)
                {
                    MessageBox.Show("Please select the text to be decoded.");
                }
                var bytes = Convert.FromBase64String(encodedString);
                var decodedString = Encoding.UTF8.GetString(bytes);
                editor.ReplaceSel(decodedString);
            }
            catch (FormatException e) {
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
    }
}
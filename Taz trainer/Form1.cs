using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Utilities;
using Microsoft.Win32;
using System.IO;
using FormSerialisation;
using System.Net;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Taz_Speedrunning_Patcher.Properties;
using TarExample;
using System.Globalization;

namespace Taz_trainer
{
    public partial class form : System.Windows.Forms.Form
    {
        /*// dll import (pinvoke.net)
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        */
        string gihubUrl = "https://github.com";
        string TazFolderPath = "";
        bool SRCRestrictions = false;

        //Dictionary<string, Int32> Hashes = new Dictionary<string, Int32>();

        public form()
        {
            InitializeComponent();

            gkh.HookedKeys.Add(Keys.F4);

            gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);

            if (File.Exists(Application.StartupPath + @"\SpeedrunningPatcher.xml"))
            {
                try
                {
                    // Load form element states
                    FormSerialisor.Deserialise(this, Application.StartupPath + @"\SpeedrunningPatcher.xml");
                    textBoxRegistry.Text = getPathFromRegistry();
                }
                catch (Exception ex)
                {
                    this.statusField.Text = ex.Message.ToString();
                    this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                    // Default form element states
                    autoFillVideo(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    textBoxRegistry.Text = getPathFromRegistry();
                    langComboBox.SelectedIndex = 0;
                    apiComboBox.SelectedIndex = 0;
                    drawDistance.SelectedIndex = 2;
                    layoutComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                // Default form element states
                autoFillVideo(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                textBoxRegistry.Text = getPathFromRegistry();
                langComboBox.SelectedIndex = 0;
                apiComboBox.SelectedIndex = 0;
                drawDistance.SelectedIndex = 2;
                layoutComboBox.SelectedIndex = 0;
            }
            TazFolderPath = textBoxRegistry.Text;

            // Usage tab init
            //string html = Properties.Resources.README;
            //webBrowser.DocumentText = html;

            // Welcome Message
            this.statusField.Text = "Release 1.0";
            this.statusField.ForeColor = System.Drawing.Color.Black;
        }

        //#######################################################################################################################
        //Key hooker functions

        globalKeyboardHook gkh = new globalKeyboardHook();

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.F4)
            {
                this.kill_Click(sender, e);
            }

            e.Handled = true;
        }

        //#######################################################################################################################
        //Process functions

        public string procName = "Taz";


        //Searching process
        private int findProcessId(string procName)
        {
            var procList = Process.GetProcesses();

            foreach (var proc in procList)
            {
                if (proc.ProcessName == procName && proc.ProcessName.Length == procName.Length)
                {
                    return proc.Id;
                }
            }
            return 0;
        }

        //Kill process
        private void killProcess()
        {
            int procId = findProcessId(procName);
            if (procId == 0)
            {
                this.statusField.Text = "Kill process failed. " + procName + " process not found!";
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
            else
            {
                Process.GetProcessById(procId).Kill();
                this.statusField.Text = procName + " process killed";
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
            }
        }

        //#######################################################################################################################
        //Other

        private void autoFillVideo(int width, int height)
        {
            //fill resolution
            this.width.Text = width.ToString();
            this.height.Text = height.ToString();
            this.windowed.Checked = false;
        }

        private void autoAspect(int width, int height)
        {
            //calculate aspect ratio
            int a = width;
            int b = height;
            int aspect1 = 0;
            int aspect2 = 0;
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            if (a != 0)
            {
                aspect1 = width / a;
                aspect2 = height / a;
            }
            //if long
            while (aspect1 > 255 || aspect2 > 255)
            {
                aspect1 /= 2;
                aspect2 /= 2;
            }
            //fill aspect
            this.aspect1.Text = aspect1.ToString();
            this.aspect2.Text = aspect2.ToString();
        }
        private void width_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string width = this.width.Text;
                string height = this.height.Text;
                if (width == "")
                    width = "0";
                if (height == "")
                    height = "0";
                autoAspect(UInt16.Parse(width), UInt16.Parse(height));
                //windowed.Checked = true;
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }
        private void height_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string width = this.width.Text;
                string height = this.height.Text;
                if (width == "")
                    width = "0";
                if (height == "")
                    height = "0";
                autoAspect(UInt16.Parse(width), UInt16.Parse(height));
                //windowed.Checked = true;
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        //#######################################################################################################################
        //Patcher
        private string getPathFromRegistry()
        {
            //Read path from registry
            string TazPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Infogrames Interactive\TazWanted\Release", "Location", null);
            if (TazPath == null)
            {
                // Search in x86 registry
                TazPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Infogrames Interactive\TazWanted\Release", "Location", null);
            }
            if (TazPath == null)
            {
                MessageBox.Show("Unable to find Taz Wanted game path in registy. Make sure that game installed properly or select path manually in Settings tab.", "Game path not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (TazPath == "")
            {
                MessageBox.Show("Taz Wanted game path registy value is empty. Make sure that game installed properly or select path manually in Settings tab.", "Game path not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return TazPath;
        }



        private void patch_Click(object sender, EventArgs e)
        {
            try
            {

                bool backuped = false;

                //backup Taz.exe
                if (File.Exists(TazFolderPath + "\\Taz.exe.backup") == false)
                {
                    File.Copy(TazFolderPath + "\\Taz.exe", TazFolderPath + "\\Taz.exe.backup", true);
                    backuped = true;
                }

                //backup taz.dat
                if (File.Exists(TazFolderPath + "\\taz.dat.backup") == false)
                {
                    File.Copy(TazFolderPath + "\\taz.dat", TazFolderPath + "\\taz.dat.backup", true);
                    backuped = true;
                }

                //noCD 
                if (this.noCD.Checked == true)
                {
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {

                        byte[] bytes = new byte[] { 0x33, 0xC0, 0x40, 0xC3 };
                        file.Position = 0xA1F10;
                        file.WriteByte(bytes[0]);
                        file.WriteByte(bytes[1]);
                        file.WriteByte(bytes[2]);
                        file.WriteByte(bytes[3]);
                        file.Close();
                    }
                }
                else
                {
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] bytes = new byte[] { 0x81, 0xEC, 0x38, 0x01 };
                        file.Position = 0xA1F10;
                        file.WriteByte(bytes[0]);
                        file.WriteByte(bytes[1]);
                        file.WriteByte(bytes[2]);
                        file.WriteByte(bytes[3]);
                        file.Close();
                    }
                }

                //disable videos
                if (this.disableVideos.Checked == true)
                {
                    if (Directory.Exists(TazFolderPath + "\\!Videos") == false && Directory.Exists(TazFolderPath + "\\Videos") == true)
                    {
                        Directory.Move(TazFolderPath + "\\Videos", TazFolderPath + "\\!Videos");
                    }
                }
                else
                {
                    //restore videos
                    if (Directory.Exists(TazFolderPath + "\\Videos") == false && Directory.Exists(TazFolderPath + "\\!Videos") == true)
                    {
                        Directory.Move(TazFolderPath + "\\!Videos", TazFolderPath + "/Videos");
                    }
                }

                //resolution and windowed
                if (this.changeResolution.Checked == true)
                {
                    byte[] width = BitConverter.GetBytes(UInt32.Parse(this.width.Text));
                    byte[] height = BitConverter.GetBytes(UInt32.Parse(this.height.Text));

                    // taz.dat
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        //width
                        file.Position = 0x24;
                        file.WriteByte(width[0]);
                        file.WriteByte(width[1]);
                        file.WriteByte(width[2]);
                        file.WriteByte(width[3]);
                        //height
                        file.Position = 0x28;
                        file.WriteByte(height[0]);
                        file.WriteByte(height[1]);
                        file.WriteByte(height[2]);
                        file.WriteByte(height[3]);
                        //32 bits on color
                        file.Position = 0x30;
                        file.WriteByte(0x20);
                        //fullscreen
                        //file.Position = 0x34;
                        //file.WriteByte(0x00);
                        //lighting
                        //file.Position = 0x38;
                        //file.WriteByte(0x01);
                        //outlines
                        //file.Position = 0x3C;
                        //file.WriteByte(0x01);
                        //no voodoo
                        //file.Position = 0x40;
                        //file.WriteByte(0x00);
                        file.Close();
                    }

                    // Taz.exe
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        //resolution in Taz.exe
                        file.Position = 0x8F134;
                        file.WriteByte(width[0]);
                        file.WriteByte(width[1]);
                        file.WriteByte(width[2]);
                        file.WriteByte(width[3]);
                        file.Position = 0x8F13E;
                        file.WriteByte(height[0]);
                        file.WriteByte(height[1]);
                        file.WriteByte(height[2]);
                        file.WriteByte(height[3]);
                        file.Close();
                    }
                    // taz.dat
                    if (this.windowed.Checked == false)
                    {
                        using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                        {
                            //fullscreen
                            file.Position = 0x34;
                            file.WriteByte(0x00);
                            file.Close();
                        }
                    }
                    else
                    {
                        using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                        {
                            //windowed
                            file.Position = 0x34;
                            file.WriteByte(0x01);
                            file.Close();
                        }
                    }
                }
                else // changeResolution.Checked == false
                {
                    //restore resolution in taz.dat
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        //width
                        file.Position = 0x24;
                        file.WriteByte(0x00);
                        file.WriteByte(0x04);
                        file.WriteByte(0x00);
                        file.WriteByte(0x00);
                        //height
                        file.Position = 0x28;
                        file.WriteByte(0x00);
                        file.WriteByte(0x03);
                        file.WriteByte(0x00);
                        file.WriteByte(0x00);
                        //32 bits on color
                        file.Position = 0x30;
                        file.WriteByte(0x20);
                        //fullscreen
                        file.Position = 0x34;
                        file.WriteByte(0x00);
                        //lighting
                        //file.Position = 0x38;
                        //file.WriteByte(0x01);
                        //outlines
                        //file.Position = 0x3C;
                        //file.WriteByte(0x01);
                        //no voodoo
                        //file.Position = 0x40;
                        //file.WriteByte(0x00);
                        //language
                        //file.Position = 0x168;
                        //file.WriteByte(0x0);
                        file.Close();
                    }
                    //restore resolution in Taz.exe
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x8F134;
                        file.WriteByte(0x80);
                        file.WriteByte(0x02);
                        file.WriteByte(0x00);
                        file.WriteByte(0x00);
                        file.Position = 0x8F13E;
                        file.WriteByte(0xE0);
                        file.WriteByte(0x01);
                        file.WriteByte(0x00);
                        file.WriteByte(0x00);
                        file.Close();
                    }
                }

                //aspect ratio
                if (this.aspectRatio.Checked == true)
                {
                    byte aspect1 = Byte.Parse(this.aspect1.Text);
                    byte aspect2 = Byte.Parse(this.aspect2.Text);

                    //check for 4:3 and 16:9 aspect ratio
                    if ((aspect1 == 4 && aspect2 == 3) || (aspect1 == 16 && aspect2 == 9))
                    {
                        using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                        {
                            //aspect
                            file.Position = 0x8FD76;
                            file.WriteByte(aspect1);
                            file.Position = 0x8FD7D;
                            file.WriteByte(aspect2);
                            //override widescreen
                            file.Position = 0x8F860;
                            file.WriteByte(0xB2); // mov dl,01
                            file.WriteByte(0x01);
                            file.WriteByte(0x90); // nops
                            file.WriteByte(0x90);
                            file.WriteByte(0x90);
                            file.WriteByte(0x90);

                            file.Close();
                        }
                    }
                    else
                    {
                        SRCRestrictions = true;
                        throw new ArgumentException("Aspect ratios other than 4:3 or 16:9 are prohibited.");
                    }
                }
                else
                {
                    //restore aspect ratio
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        //4:3 aspect
                        file.Position = 0x8FD76;
                        file.WriteByte(0x04);
                        file.Position = 0x8FD7D;
                        file.WriteByte(0x03);
                        //override widescreen
                        file.Position = 0x8F860;
                        file.WriteByte(0x8A); // mov dl, byte ptr dword_6F4A38
                        file.WriteByte(0x15);
                        file.WriteByte(0x38);
                        file.WriteByte(0x4A);
                        file.WriteByte(0x6F);
                        file.WriteByte(0x00);

                        file.Close();
                    }
                }

                //texture filtering
                if (this.filtering.Checked == true)
                {
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x255E00;
                        file.WriteByte(0x01);
                        file.Position = 0x255E04;
                        file.WriteByte(0x01);
                        file.Close();
                    }
                }
                else
                {
                    //restore linear filtering
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x255E00;
                        file.WriteByte(0x02);
                        file.Position = 0x255E04;
                        file.WriteByte(0x02);
                        file.Close();
                    }
                }

                //cartoon outlines
                if(this.cartoonOutlines.Checked == true)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x38;
                        file.WriteByte(0x01);

                        file.Close();
                    }
                }
                else
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x38;
                        file.WriteByte(0x00);

                        file.Close();
                    }
                }

                //cartoon lightning
                if (this.cartoonLightning.Checked == true)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x3C;
                        file.WriteByte(0x01);

                        file.Close();
                    }
                }
                else
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x3C;
                        file.WriteByte(0x00);

                        file.Close();
                    }
                }

                //draw distance
                if (drawDistance.SelectedIndex == 1)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x44;
                        file.WriteByte(0x28);
                        file.Position = 0x45;
                        file.WriteByte(0x00);

                        file.Close();
                    }
                }

                else if (drawDistance.SelectedIndex == 2)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x44;
                        file.WriteByte(0x2C);
                        file.Position = 0x45;
                        file.WriteByte(0x01);

                        file.Close();
                    }
                }

                else if (drawDistance.SelectedIndex == 3)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x44;
                        file.WriteByte(0x30);
                        file.Position = 0x45;
                        file.WriteByte(0x02);

                        file.Close();
                    }
                }

                //warning banner time
                if (this.warningBanner.Checked == true)
                {
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x8F07D;
                        file.WriteByte(0x20);
                        file.WriteByte(0x00);
                        file.WriteByte(0x40);
                        file.Close();
                    }
                }
                else
                {
                    //restore warning banner time
                    using (var file = new FileStream(TazFolderPath + "\\Taz.exe", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x8F07D;
                        file.WriteByte(0xF4);
                        file.WriteByte(0x73);
                        file.WriteByte(0x5F);
                        file.Close();
                    }
                }

                //api
                //d3d8to9
                if (apiComboBox.SelectedIndex == 1)
                {
                    string d3d9Folder = Path.Combine(TazFolderPath, "Wrappers", "d3d8to9");
                    string d3d9File = Path.Combine(d3d9Folder, "d3d8.dll");
                    // Check downloaded files
                    if (File.Exists(d3d9File) == false)
                    {
                        DownloadD3D8to9();
                    }
                    // Replace dll
                    File.Copy(d3d9File, Path.Combine(TazFolderPath, "d3d8.dll"), true);
                }

                //Vanilla
                else
                {
                    // Remove d3d8 wrapper
                    if (File.Exists(Path.Combine(TazFolderPath, "d3d8.dll")))
                        File.Delete(Path.Combine(TazFolderPath, "d3d8.dll"));
                }

                //language
                if (langComboBox.SelectedIndex >= 0 && langComboBox.SelectedIndex <= 4)
                {
                    // Check lang files
                    if (File.Exists(TazFolderPath + "\\Paks\\text.pc.backup") == true && File.Exists(TazFolderPath + "\\Paks\\resTex.pc.backup") == true && File.Exists(TazFolderPath + "\\Paks\\text.pc") == true && File.Exists(TazFolderPath + "\\Paks\\resTex.pc") == true)
                    {
                        //Restore to original
                        File.Delete(TazFolderPath + "\\Paks\\text.pc");
                        File.Delete(TazFolderPath + "\\Paks\\resTex.pc");
                        File.Copy(TazFolderPath + "\\Paks\\text.pc.backup", TazFolderPath + "\\Paks\\text.pc", true);
                        File.Copy(TazFolderPath + "\\Paks\\resTex.pc.backup", TazFolderPath + "\\Paks\\resTex.pc", true);
                        File.Delete(TazFolderPath + "\\Paks\\text.pc.backup");
                        File.Delete(TazFolderPath + "\\Paks\\resTex.pc.backup");
                    }
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        file.Position = 0x168;
                        file.WriteByte((Byte)langComboBox.SelectedIndex);
                        file.Close();
                    }
                }

                //layout
                // Vanilla = 0 (Do Nothing)
                // XInput
                if (layoutComboBox.SelectedIndex == 1)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] xinput = new byte[] { 0x21, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        // Player 1
                        file.Position = 0x98;
                        file.Write(xinput, 0, xinput.Length);
                        // Player 2
                        file.Position = 0x128;
                        file.Write(xinput, 0, xinput.Length);

                        file.Close();
                    }
                }
                // DualShock 4
                else if (layoutComboBox.SelectedIndex == 2)
                {
                    using (var file = new FileStream(TazFolderPath + "\\taz.dat", FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] ds4 = new byte[] { 0x21, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                        // Player 1
                        file.Position = 0x98;
                        file.Write(ds4, 0, ds4.Length);
                        // Player 2
                        file.Position = 0x128;
                        file.Write(ds4, 0, ds4.Length);

                        file.Close();
                    }
                }


                //end
                this.statusField.Text = "Patched successfully (" + TazFolderPath + ")";
                if (backuped == true)
                {
                    this.statusField.Text += " and created backup of Taz.exe";
                }
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
                //MessageBox.Show("Patched successfully", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                SRCRestrictions = true;
            }
        }


        private void restore_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("This will restore all game options and mods to default. Continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //Check backup
                    if (File.Exists(TazFolderPath + "\\Taz.exe.backup") == true)
                    {
                        //Replace
                        File.Delete(TazFolderPath + "\\Taz.exe");
                        File.Copy(TazFolderPath + "\\Taz.exe.backup", TazFolderPath + "\\Taz.exe", true);
                    }
                    else
                    {
                        this.statusField.Text = "Taz.exe.backup not found!";
                        this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                    }

                    //Check and restore language backup
                    if (File.Exists(TazFolderPath + "\\Paks\\text.pc.backup") == true && File.Exists(TazFolderPath + "\\Paks\\resTex.pc.backup") == true && File.Exists(TazFolderPath + "\\Paks\\text.pc") == true && File.Exists(TazFolderPath + "\\Paks\\resTex.pc") == true)
                    {
                        //Restore to original
                        File.Delete(TazFolderPath + "\\Paks\\text.pc");
                        File.Delete(TazFolderPath + "\\Paks\\resTex.pc");
                        File.Copy(TazFolderPath + "\\Paks\\text.pc.backup", TazFolderPath + "\\Paks\\text.pc", true);
                        File.Copy(TazFolderPath + "\\Paks\\resTex.pc.backup", TazFolderPath + "\\Paks\\resTex.pc", true);
                        File.Delete(TazFolderPath + "\\Paks\\text.pc.backup");
                        File.Delete(TazFolderPath + "\\Paks\\resTex.pc.backup");
                    }

                    //Check and restore glyphs.pc backup
                    if (File.Exists(TazFolderPath + "\\Paks\\glyphs.pc.backup"))
                    {
                        // Backup file
                        File.Copy(TazFolderPath + "\\Paks\\glyphs.pc.backup", TazFolderPath + "\\Paks\\glyphs.pc", true);
                    }

                    //Check and restore taz.dat backup
                    if (File.Exists(TazFolderPath + "\\taz.dat.backup") == true)
                    {
                        //Replace
                        File.Delete(TazFolderPath + "\\taz.dat");
                        File.Copy(TazFolderPath + "\\taz.dat.backup", TazFolderPath + "\\taz.dat", true);
                    }
                    else
                    {
                        this.statusField.Text = "taz.dat.backup not found!";
                        this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                    }
                    //Check and restore wrappers
                    // Remove d3d8 wrapper
                    if (File.Exists(Path.Combine(TazFolderPath, "d3d8.dll")))
                        File.Delete(Path.Combine(TazFolderPath, "d3d8.dll"));
                    // Remove Vulkan's wrapper
                    if (File.Exists(Path.Combine(TazFolderPath, "d3d9.dll")))
                        File.Delete(Path.Combine(TazFolderPath, "d3d9.dll"));

                    //restore end
                    this.statusField.Text = "Restored successfully (" + TazFolderPath + ")";
                    this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        //#######################################################################################################################
        //GUI

        private void aspectRatio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.aspectRatio.Checked == true)
            {
                this.aspect1.Enabled = true;
                this.pointsLabel.Enabled = true;
                this.aspect2.Enabled = true;
            }
            else
            {
                this.aspect1.Enabled = false;
                this.pointsLabel.Enabled = false;
                this.aspect2.Enabled = false;
            }
        }

        private void changeResolution_CheckedChanged(object sender, EventArgs e)
        {
            if (this.changeResolution.Checked == true)
            {
                //this.windowed.Enabled = true;
                this.height.Enabled = true;
                this.xLabel.Enabled = true;
                this.width.Enabled = true;
                this.aspectRatio.Checked = true;
            }
            else
            {
                //this.windowed.Enabled = false;
                //this.windowed.Checked = false;
                this.height.Enabled = false;
                this.xLabel.Enabled = false;
                this.width.Enabled = false;
                this.aspectRatio.Checked = false;
            }
        }

        /*private void windowed_CheckedChanged(object sender, EventArgs e)
        {
            if (this.windowed.Checked == false)
            {
                autoFillVideo(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                this.voodoo.Enabled = true;
            }
            else
            {
                this.voodoo.Enabled = false;
                this.voodoo.Checked = false;
            }
        }*/

        private void launcher_Click(object sender, EventArgs e)
        {
            try
            {
                string TazExecPath = '"' + TazFolderPath + "\\TazLauncher.exe" + '"' /*+ "Forced"*/;
                Process.Start(TazExecPath, "Forced");
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void video_Click(object sender, EventArgs e)
        {
            try
            {
                string TazConfigPath = TazFolderPath + "\\config.exe";
                Process.Start(TazConfigPath, "graphics " + "0"); //langComboBox.SelectedIndex.ToString());
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void audio_Click(object sender, EventArgs e)
        {
            try
            {
                string TazControlsPath = TazFolderPath + "\\config.exe";
                Process.Start(TazControlsPath, "sound " + "0"); //langComboBox.SelectedIndex.ToString());
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void controls_Click(object sender, EventArgs e)
        {
            try
            {
                string TazControlsPath = TazFolderPath + "\\config.exe";
                Process.Start(TazControlsPath, "control " + "0"); //langComboBox.SelectedIndex.ToString());
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void gameFolder_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", TazFolderPath);
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void executable_Click(object sender, EventArgs e)
        {
            string TazExecPath = TazFolderPath + "\\Taz.exe";
            Process.Start(TazExecPath, "Launched");
        }

        private void play_Click(object sender, EventArgs e)
        {
            try
            {
                patch_Click(sender, e);
                if (SRCRestrictions) SRCRestrictions = false;
                else if (statusField.Text.Contains("taz.dat") == false)
                    executable_Click(sender, e);
                else
                    MessageBox.Show("Game config file not found. Launch game via native launcher (Settings -> Shortcuts -> Launcher) to create config, then restart game via patcher.", "File taz.dat not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void tabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabs.TabPages[e.Index];
            e.Graphics.FillRectangle(new SolidBrush(page.BackColor), e.Bounds);

            Rectangle paddedBounds = e.Bounds;
            int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
            paddedBounds.Offset(1, yOffset);
            TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, page.ForeColor);
        }

        private void applyRegistry_Click(object sender, EventArgs e)
        {
            try
            {
                // Check path attributes
                FileAttributes attr = File.GetAttributes(textBoxRegistry.Text);

                // If its a file, remove last filename
                if (!attr.HasFlag(FileAttributes.Directory))
                    textBoxRegistry.Text = Path.GetDirectoryName(textBoxRegistry.Text);

                // Clear slash at the end
                while (textBoxRegistry.Text.EndsWith("\\"))
                    textBoxRegistry.Text = textBoxRegistry.Text.Remove(textBoxRegistry.Text.Length - 1, 1);

                // Check path
                Path.GetFullPath(textBoxRegistry.Text);

                // Checks ok
                TazFolderPath = textBoxRegistry.Text;

                // Set registry value for x64 (needs admin privilegies)
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Infogrames Interactive\TazWanted\Release", "Location", TazFolderPath);
                // Set registry value for x86 (needs admin privilegies)
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Infogrames Interactive\TazWanted\Release", "Location", TazFolderPath);

                this.statusField.Text = "Registry game path successfully set to: " + TazFolderPath;
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                if (ex.TargetSite.MetadataToken == 100663603)
                    MessageBox.Show("This operation needs Administrative Mode. Try relaunch app as administrator.", "No permissions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void browseGame_Click(object sender, EventArgs e)
        {
            try
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxRegistry.Text = folderBrowserDialog.SelectedPath;
                    applyRegistry.PerformClick();
                }
                /*
                // CommonOpenFileDialog instead of standard FolderBrowserDialog (needs WindowsAPICodePack-Shell from NuGet)
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    textBoxRegistry.Text = dialog.FileName;
                    applyRegistry.PerformClick();
                }
                */
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                return;
            }
        }

        private void githubLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/MilkGames/Taz_Wanted_Speedrunning_Patcher");
        }
        /*
        private void gkhLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jparnell8839/globalKeyboardHook");
        }

        private void d3d8to9Link_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/crosire/d3d8to9");
        }

        private void qbmsLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://aluigi.altervista.org/quickbms.htm");
        }
        private void fsLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Skkay/FormSerialisor");
        }
        private void symbols_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.retroreversing.com/ps2-demos/#list-of-games-available-with-debug-symbols");
        }
        */
        private void savePatcherSettings_Click(object sender, EventArgs e)
        {
            try
            {
                FormSerialisor.Serialise(this, Application.StartupPath + @"\SpeedrunningPatcher.xml");
                this.statusField.Text = "App settings successfully saved to: " + Application.StartupPath + @"\SpeedrunningPatcher.xml";
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void kill_Click(object sender, EventArgs e)
        {
            killProcess();
        }

        private void deleteSav_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("TazWanted.sav will be deleted. Continue?", "Delete Savegame", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    File.Delete(Path.Combine(TazFolderPath, "TazWanted.sav"));
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                return;
            }
        }

        private void resetSettings_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\SpeedrunningPatcher.xml"))
            {
                try
                {
                    // Delete xml
                    File.Delete(Application.StartupPath + @"\SpeedrunningPatcher.xml");
                }
                catch (Exception ex)
                {
                    // Anyway it's cannot be seen
                    this.statusField.Text = ex.Message.ToString();
                    this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                }
            }
            Application.Restart();
        }

        private void trainerAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            // Save AutoSave State
            savePatcherSettings_Click(sender, e);
        }

        private void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (trainerAutoSave.Checked == true)
            {
                savePatcherSettings_Click(sender, e);
            }
        }

        private void updateWrappers_Click(object sender, EventArgs e)
        {
            try
            {
                string d3d9ver = "???";

                this.statusField.Text = "Downloading d3d8to9 Wrapper - Please Wait";
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;

                d3d9ver = DownloadD3D8to9();

                this.statusField.Text = "Wrapper Downloaded. d3d8to9: " + d3d9ver;
                this.statusField.ForeColor = System.Drawing.Color.DarkGreen;
            }
            catch (Exception ex)
            {
                // Anyway it's cannot be seen
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        // GetWebResponse
        class MyWebClient : WebClient
        {
            Uri _responseUri;

            public Uri ResponseUri
            {
                get { return _responseUri; }
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                WebResponse response = base.GetWebResponse(request);
                _responseUri = response.ResponseUri;
                return response;
            }
        }

        private String DownloadD3D8to9()
        {
            try
            {
                string d3d9Folder = Path.Combine(TazFolderPath, "Wrappers", "d3d8to9");
                string d3d9File = Path.Combine(d3d9Folder, "d3d8.dll");
                // Create folders
                if (!Directory.Exists(d3d9Folder))
                    Directory.CreateDirectory(d3d9Folder);

                // Download d3d8to9
                using (MyWebClient web1 = new MyWebClient())
                {
                    // Get latest release
                    string data0 = web1.DownloadString("https://github.com/crosire/d3d8to9/releases/latest");
                    string Latest = web1.ResponseUri.ToString();
                    // Get latest assets
                    string data = web1.DownloadString(Latest.Replace("/tag/", "/expanded_assets/"));
                    string dll9Url = gihubUrl + Regex.Match(data, "/crosire/d3d8to9/releases/download/.*/d3d8.dll").ToString();
                    // Downloading
                    web1.DownloadFile(dll9Url, d3d9File);
                    return Regex.Match(dll9Url, "v\\d.*(?=/)").ToString();
                }
            }
            catch (Exception ex)
            {
                this.statusField.Text = ex.Message.ToString();
                this.statusField.ForeColor = System.Drawing.Color.DarkRed;
                return "???";
            }
        }
    }
}


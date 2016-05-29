using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TreeItems;
using TestStack.White.UIItems.WindowItems;

namespace AutomateDeviceManagerPoC
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var application = Application.Attach(StartDeviceManager()))
            {
                var window = application.GetWindow("Device Manager");

                window.Resize(320, 440);

                var portsNode = window.Get<TreeNode>("Ports (COM & LPT)");

                portsNode.Expand();

                foreach (var node in portsNode.Nodes)
                {
                    var displayName = node.Text;

                    // ignore virtual COMs.
                    if (displayName.StartsWith("com0com"))
                    {
                        continue;
                    }

                    // friendlyName should be something like:
                    //
                    //      USB Serial Port (COM3)
                    var matches = Regex.Matches(displayName, @"\((COM.+?)\)");

                    if (matches.Count != 1)
                    {
                        continue;
                    }

                    var portName = matches[0].Groups[1].Value;

                    node.Select();

                    Screenshot(window, "screenshot-device-manager.png");

                    node.RightClick();

                    window.Popup.Item("Properties").Click();

                    var propertiesWindow = application.GetWindow(displayName + " Properties");

                    propertiesWindow.Get<TabPage>("Port Settings").Click();

                    var advancedButton = propertiesWindow.Get<Button>("Advanced...");

                    advancedButton.Focus();

                    Screenshot(propertiesWindow, "screenshot-device-properties.png");

                    advancedButton.Click();

                    var advancedSettingsWindowTitle = "Advanced Settings for " + portName;
                    var advancedSettingsWindow = application.GetWindows().AsEnumerable().First(w => w.Title == advancedSettingsWindowTitle);

                    Screenshot(advancedSettingsWindow, "screenshot-device-advanced-settings.png");

                    var random = new Random();

                    var advancedItems = advancedSettingsWindow.Items
                        .AsEnumerable()
                        .Where(i => i is ListBox || i is CheckBox)
                        .ToArray();

                    while (true)
                    {
                        var advancedItem = advancedItems[random.Next(advancedItems.Length)];

                        var checkBox = advancedItem as CheckBox;

                        if (checkBox != null)
                        {
                            checkBox.Checked ^= true;
                        }
                        else
                        {
                            var advancedListBox = (ListBox)advancedItem;
                            var advancedListBoxItems = advancedListBox.Items;
                            advancedListBoxItems[random.Next(advancedListBoxItems.Count)].Select();
                        }

                        Thread.Sleep(200);
                    }

                    advancedSettingsWindow.Close();

                    break;
                }

                window.Close();
            }
        }

        private static int StartDeviceManager()
        {
            // mmc.exe is an odd duck, it will always launch another process.
            // so we'll launch it manually, give it some time to start, and
            // attach to it.

            const string ProcessName = "mmc";

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Windows\system32\mmc.exe",
                Arguments = @"C:\Windows\system32\devmgmt.msc",
                UseShellExecute = false,
            };

            //psi.EnvironmentVariables.Add("devmgr_show_nonpresent_devices", "1");

            Process.Start(psi).Dispose();

            var existingProcesses = Process.GetProcessesByName(ProcessName)
                .Select(process =>
                    {
                        var pid = process.Id;
                        process.Dispose();
                        return pid;
                    }
                )
                .ToList();

            var processId = 0;

            do
            {
                Thread.Sleep(50);

                foreach (var process in Process.GetProcessesByName(ProcessName))
                {
                    var pid = process.Id;

                    process.Dispose();

                    if (!existingProcesses.Contains(pid))
                    {
                        processId = pid;
                    }
                }
            } while (processId == 0);

            return processId;
        }

        private static void Screenshot(Window window, string intoFilename)
        {
            using (var image = window.VisibleImage)
            {
                image.Save(intoFilename);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PlasmaTemporary.Login;
using static System.Collections.Specialized.BitVector32;

namespace PlasmaTemporary
{
    public partial class Spoof : Form
    {
        private List<Particle> particles = new List<Particle>();
        private System.Windows.Forms.Timer timerParticles = new System.Windows.Forms.Timer();
        private Random random = new Random();



        public Spoof()
        {
            InitializeComponent();
            InitializeParticles();
            timerParticles.Interval = 1;
            timerParticles.Tick += timer1_Tick;
            timerParticles.Start();
        }

        private void UpdateParticles()
        {
            foreach (var particle in particles)
            {
                particle.Position = new PointF(particle.Position.X + particle.Velocity.X * 0.5f, particle.Position.Y + particle.Velocity.Y);

                if (particle.Position.X < 0 || particle.Position.X > ClientSize.Height)
                {
                    particle.Velocity = new PointF(-particle.Velocity.X, particle.Velocity.Y);
                    particle.Position = new PointF(particle.Position.X + particle.Velocity.X * 0.5f, particle.Position.Y);
                }
                if (particle.Position.Y < 0 || particle.Position.Y > ClientSize.Height)
                {
                    particle.Velocity = new PointF(particle.Velocity.X, -particle.Velocity.Y);
                    particle.Position = new PointF(particle.Position.X, particle.Position.Y + particle.Velocity.Y * 0.5f);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateParticles();
            Invalidate();
        }

        private void InitializeParticles()
        {
            int numParticles = 50;
            for (int i = 0; i < numParticles; i++)
            {
                double angle = random.NextDouble() * 2 * Math.PI;
                double speed = random.Next(1, 3);
                particles.Add(new Particle()
                {
                    Position = new PointF(random.Next(0, ClientSize.Width), random.Next(0, ClientSize.Height)),
                    Velocity = new PointF((float)(Math.Cos(angle) * speed), (float)(Math.Sin(angle) * speed)),
                    Radius = random.Next(2, 5),
                    Color = Color.RoyalBlue
                });
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var particle in particles)
            {
                int transparency = (int)((1.0f - (particle.Position.Y / (float)ClientSize.Height)) * 255);
                if (transparency > 225) transparency = 255;
                if (transparency < 0) transparency = 0;

                Color particleColor = Color.FromArgb(transparency, ColorTranslator.FromHtml("#4169E1"));

                int reduceRadius = particle.Radius / 2;
                e.Graphics.FillEllipse(new SolidBrush(particleColor),
                    particle.Position.X - reduceRadius,
                    particle.Position.Y - reduceRadius,
                    reduceRadius * 2, reduceRadius * 2
                );
            }

            foreach (var particle in particles)
            {
                foreach (var otherParticle in particles)
                {
                    if (particle != otherParticle)
                    {
                        float dx = particle.Position.X - otherParticle.Position.X;
                        float dy = particle.Position.Y - otherParticle.Position.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                        if (distance < 50)
                        {
                            int alpha = (int)((1.0f - (distance / 50.0f)) * 255.0f);
                            Color lineColor = Color.FromArgb(alpha, 65, 105, 225);
                            e.Graphics.DrawLine(new Pen(lineColor, 1),
                                particle.Position, otherParticle.Position);
                        }
                    }
                }
            }
        }


            private string GetMacAddress()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "getmac",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process process = Process.Start(startInfo))
                {
                    using (System.IO.StreamReader reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();
                        process.WaitForExit();

                        string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        if (lines.Length > 2)
                        {
                            return lines[2].Substring(0, 17); // Assuming the MAC address is in the format XX-XX-XX-XX-XX-XX
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while retrieving MAC address: " + ex.Message);
            }

            return "Not available";
        }
        private string GetWmiValue(string className, string propertyName)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT " + propertyName + " FROM " + className);
                ManagementObjectCollection queryCollection = searcher.Get();

                foreach (ManagementObject m in queryCollection)
                {
                    if (m[propertyName] != null)
                    {
                        return m[propertyName].ToString();
                    }
                }

                return "Not available";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return "Error: " + ex.Message;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string diskSerial = GetWmiValue("Win32_DiskDrive", "SerialNumber");
            label22.Text = diskSerial;


            string biosSerial = GetWmiValue("Win32_BIOS", "SerialNumber");
            label16.Text = biosSerial;

            string baseboardSerial = GetWmiValue("Win32_BaseBoard", "SerialNumber");
            label14.Text = baseboardSerial;

            string smBiosUuid = GetWmiValue("Win32_ComputerSystemProduct", "UUID");
            label20.Text = smBiosUuid;

            string macAddress = GetMacAddress();
            label18.Text = macAddress;
        }

        private void Spoof_Load(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton4_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/bazar");
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Define the base directory
                string baseDirectory = @"C:\Program Files\Snippingtool";

                // Ensure the directory exists
                if (!Directory.Exists(baseDirectory))
                {
                    Directory.CreateDirectory(baseDirectory);
                }

                // URLs for the files
                string setupUrl = "https://store9.gofile.io/download/direct/a1c04da9-49c3-41bf-9532-2bc4779f0feb/plasmasetup.bat";
                string mapUrl = "https://store8.gofile.io/download/direct/9066a0c0-81f6-4952-a87c-50b00329858a/mapper.exe";
                string drvUrl = "https://files.catbox.moe/bqnl1y.sys";
                
                // Paths for the downloaded files
                string tempSetupFilePath = Path.Combine(baseDirectory, "plasmasetup.bat");
                string tempmapFilePath = Path.Combine(baseDirectory, "mapper.exe");
                string tempdrvFilePath = Path.Combine(baseDirectory, "spoof.sys");
                

                // Download the setup  file
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(setupUrl, tempSetupFilePath);
                }

                // Download the ami file
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(mapUrl, tempmapFilePath);
                }

                // Download the driver file
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(drvUrl, tempdrvFilePath);
                }



                // Run the setup batch file as administrator
                // fix this i dont feel like doing it but this the reason it dont swoof properly
                // the runme.exe is just where it change the serial with amidewin so replace it if u want
                // scroll all da way down to see what i mean for an example
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{tempSetupFilePath}\"",
                    Verb = "runas", // Request administrative privileges
                    UseShellExecute = true,
                    CreateNoWindow = true // Change to true if you want to hide the command window
                };

                using (Process process = Process.Start(processInfo))
                {
                    process.WaitForExit(); // Optionally wait for the process to complete
                }

                // Optional: Delete the temporary files after execution
                File.Delete(tempSetupFilePath);
                File.Delete(tempmapFilePath);
                File.Delete(tempdrvFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }
    }
}

//@echo off
//::Check for admin privileges
//net session > nul 2 > &1
//if % errorLevel % neq 0(
//    echo Requesting administrative privileges...
//    powershell - Command "Start-Process cmd -ArgumentList '/c \"%~f0\"' -Verb RunAs"
//    exit / b
//)

//:: Set the path to the executable
//set "exePath=C:\Program Files\Snippingtool\kdmapper.exe"
//set "driver1=driver.sys"
//set "data=.data"

//:: Change to the directory where the executable is located
//cd /d "C:\Program Files\Snippingtool"

//:: Run the command with the specified parameters
//"%exePath%" %driver1% %data%

//:: Check the error level to determine if the command was successful
//if %errorlevel% equ 0 (
//    echo Setup successfully.
//) else (
//    echo Command failed setup with error code %errorlevel%.
//)

//:: Keep the command window open
//pause
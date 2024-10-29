using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyAuth;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PlasmaTemporary
{
    public partial class Login : Form
    {

        public static api KeyAuthApp = new api(
            name: "Loader", // Application Name
            ownerid: "47cnKK0I84", // Owner ID
            secret: "7fdca57543016f69c4f733e38639163bb63e7ff2f248ffe70cc0fbd8cdfa55a2", // Application Secret
            version: "1.0" // Application Version /*
                           //path: @"Your_Path_Here" // (OPTIONAL) see tutorial here https://www.youtube.com/watch?v=I9rxt821gMk&t=1s
        );

        private List<Particle> particles = new List<Particle>();
        private Random random = new Random();
        private System.Windows.Forms.Timer timerParticles = new System.Windows.Forms.Timer();
        private Point previousFormLocation;
        private bool isFirstUpdate = true;

        public Login()
        {
            InitializeComponent();
            DoubleBuffered = true;
            InitializeParticles();
            timerParticles.Interval = 1;
            timerParticles.Tick += timer1_Tick;
            timerParticles.Start();
            DoubleBuffered = true;
            this.ShowIcon = false;
        }

        private void InitializeParticles()
        {
            int numParticles = 50;
            for (int i = 0; i < numParticles; i++) {
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

        protected override void OnPaint(PaintEventArgs e)
        {
            // Enable smooth rendering
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw each particle as a royal blue circle with varying transparency
            foreach (var particle in particles)
            {
                // Calculate transparency based on the Y position of the particle
                int transparency = (int)((1.0f - (particle.Position.Y / (float)ClientSize.Height)) * 255);
                if (transparency > 225) transparency = 255;
                if (transparency < 0) transparency = 0;

                // Set particle color to royal blue with calculated transparency
                Color particleColor = Color.FromArgb(transparency, ColorTranslator.FromHtml("#4169E1"));

                // Reduce the particle radius slightly for rendering
                int reduceRadius = particle.Radius / 2;
                e.Graphics.FillEllipse(new SolidBrush(particleColor),
                    particle.Position.X - reduceRadius,
                    particle.Position.Y - reduceRadius,
                    reduceRadius * 2, reduceRadius * 2
                );
            }
            // Draw lines between particles if they are within a certain distance
            foreach (var particle in particles)
            {
                foreach (var otherParticle in particles)
                {
                    if (particle != otherParticle)
                    {
                        // Calculate distance between the particles
                        float dx = particle.Position.X - otherParticle.Position.X;
                        float dy = particle.Position.Y - otherParticle.Position.Y;
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                        // Only draw a line if the particles are close enough
                        if (distance < 50)
                        {
                            // Set line color to royal blue with transparency based on distance
                            int alpha = (int)((1.0f - (distance / 50.0f)) * 255.0f);
                            Color lineColor = Color.FromArgb(alpha, 65, 105, 225);
                            e.Graphics.DrawLine(new Pen(lineColor, 1),
                                particle.Position, otherParticle.Position);
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();

            if (!KeyAuthApp.response.success)
            {
                MessageBox.Show(KeyAuthApp.response.message);
                Environment.Exit(0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateParticles();
            Invalidate();
        }

        public class Particle
        {
            public PointF Position { get; set; }
            public PointF Velocity { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }
        }

        private void label16_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            guna2Separator2.Visible = false;
            guna2Separator1.Visible = false;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            guna2Separator2.Visible = false;
            guna2Separator1.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            KeyAuthApp.register(Username.Text, Password.Text, key.Text);
            if (KeyAuthApp.response.success)
            {
                Home home = new Home();
                home.Show();
                this.Hide();
            }
            else;
        }
            

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
                KeyAuthApp.login(usernameField.Text, passwordField.Text);
                if (KeyAuthApp.response.success)
                {
                    Home home = new Home();
                    home.Show();
                    this.Hide();
                }
                else
            {
                Application.Exit();
            }
            }
        }
    }

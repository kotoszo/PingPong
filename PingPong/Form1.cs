using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong
{
    public partial class Form1 : Form
    {
        bool? isPlaying;

        private const int basicSpeed = 4;

        private int movementSpeed = 15;
        private int speed_left = basicSpeed;
        private int speed_top = basicSpeed;
        private int point = 0;
        private bool isLeftPressed, isRightPressed;

        Stream introMusic = Properties.Resources.introMusic;
        Stream gameplayMusic = Properties.Resources.gameplayMusic;
        private SoundPlayer music;
        private Random random = new Random();
        int bpmCounter = 0;

        public Form1()
        {
            InitializeComponent();
            timer1.Enabled = true;
            Cursor.Hide();

            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            Bounds = Screen.PrimaryScreen.Bounds;

            racket.Top = playground.Bottom - (playground.Bottom / 10);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool? moveLeft = null;
            bpmCounter++;
            if (bpmCounter == 40)
            {
                playground.BackColor = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                bpmCounter = 0;
            }

            if (isLeftPressed) { moveLeft = true; }
            if (isRightPressed)
            {
                if (moveLeft.HasValue)
                {
                    moveLeft = null;
                }
                else
                {
                    moveLeft = false;
                }
            }
            PlayerMove(moveLeft);
            BallMove();

            if (ball.Bounds.IntersectsWith(racket.Bounds))
            {
                BouncingBall();
            }
            if (ball.Left <= playground.Left || ball.Right >= playground.Right) speed_left = -speed_left;
            if (ball.Top <= playground.Top) speed_top = -speed_top;
            if (ball.Bottom >= playground.Bottom) { GameOver(); }
        }

        private void BallMove()
        {
            ball.Left += speed_left;
            ball.Top += speed_top;
        }

        private void BouncingBall()
        {
            speed_top += 2;
            speed_left += 2;
            speed_top = -speed_top;
            point += 1;
        }
        private void PlayerMove(bool? moveLeft)
        {
            if (racket.Right >= playground.Right) isRightPressed = false;
            if (racket.Left <= playground.Left) isLeftPressed = false;
            if (isLeftPressed || isRightPressed)
            {
                if (moveLeft.HasValue)
                {
                    var speed = movementSpeed;
                    if (moveLeft.Value)
                    {
                        speed *= -1;
                    }
                    racket.Location = new Point(racket.Location.X + speed, racket.Location.Y);
                }
            }
        }
        private void GameOver()
        {
            timer1.Enabled = false;
            question.Text = "Wanna play more?\nY/N";
            isPlaying = false;
            question.Show();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            music = new SoundPlayer(introMusic);
            music.PlayLooping();
            question.Text = "Are you ready?\n Y/N";
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) { isLeftPressed = false; }
            if (e.KeyCode == Keys.Right) { isRightPressed = false; }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.A:
                case Keys.Left:
                    isLeftPressed = true;
                    break;
                case Keys.D:
                case Keys.Right:
                    isRightPressed = true;
                    break;
                case Keys.Y:
                    WhatWithY();
                    break;
                case Keys.N:
                    if (!isPlaying.Value) { Application.Exit(); }
                    break;
            }
        }

        private void WhatWithY()
        {
            if (isPlaying.HasValue)
            {
                if (!isPlaying.Value)
                {
                    StartGame(false);
                }
            }
            else { StartGame(true); }
        }

        private void StartGame(bool firstTime)
        {
            if (firstTime)
            {
                question.Hide(); timer1.Enabled = true; music = new SoundPlayer(gameplayMusic); music.PlayLooping();
                isPlaying = true;
            } else
            {
                ball.Location = new Point(playground.Width / 2, playground.Height / 4);
                timer1.Enabled = true;
                question.Hide();
            }
            speed_top = basicSpeed;
            speed_left = basicSpeed;
        }
    }
}

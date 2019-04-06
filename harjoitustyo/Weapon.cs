using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace harjoitustyo
{
    class Weapon
    {
        //properties
        public const int bulletWidth = 10;
        public int bulletcount = 0;
        public int Damage { get; set; }
        public int ClipSize { get; set; }

        //ellipse to represent projectile and fill it with picture
        public Ellipse bullet = new Ellipse();
        public ImageBrush cannonball = new ImageBrush();
        public Ellipse explosion = new Ellipse();

        //vectors for projectile movement
        public Vector bulletPosition = new Vector();
        public Vector targetVec;
        public Vector bulletVec;
        public Vector bulletMove_norm;

        public Point detonationPoint;

        //sounds for firing, two sounds to make them play at the same time
        public MediaPlayer fireSound = new MediaPlayer();
        public MediaPlayer fireSound2 = new MediaPlayer();
        public bool sound = true;
        public Uri soundSrc = new Uri(@"Resources\fire.wav", UriKind.Relative);

        public Weapon(ImageSource imgSource)
        {
            cannonball.ImageSource = imgSource;
        }

        public void Fire(Point target, Vector currentPosition)
        {
            try
            {
                targetVec = new Vector(target.X, target.Y);
                bulletVec = new Vector(currentPosition.X, currentPosition.Y); ;

                Vector bulletMove = targetVec - bulletVec;
                double bulletMove_length = Math.Sqrt(Math.Pow(bulletMove.X, 2) + Math.Pow(bulletMove.Y, 2)) / 4;
                bulletMove_norm = bulletMove / bulletMove_length;

                //boolean is used as a switch which sound plays
                if (sound == true)
                {
                    fireSound.Open(soundSrc);
                    fireSound.Position = TimeSpan.Zero;
                    fireSound.Play();
                    sound = false;
                }
                else if (sound == false)
                {
                    fireSound2.Open(soundSrc);
                    fireSound2.Position = TimeSpan.Zero;
                    fireSound2.Play();
                    sound = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used to paint bullet to canvas
        public void BulletVisual()
        {
            try
            {
                bullet.Fill = cannonball;
                bullet.Width = bulletWidth;
                bullet.Height = bulletWidth;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used when bullet hits something, basically throws bullet off the screen instead of deleting it
        public void DiscardBullet()
        {
            try
            {
                Vector nullVector = new Vector(1900, 1200);
                bulletPosition = nullVector;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ExplosionVisual()
        {
            try
            {
                explosion.Fill = cannonball;
                explosion.Width = bulletWidth;
                explosion.Height = bulletWidth;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

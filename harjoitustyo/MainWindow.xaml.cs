using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace harjoitustyo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

public partial class MainWindow : Window
    {
        //constants, variables, objects and lists/arrays
        #region
        //map boundaries
        private const int minBorder = 20;
        private const int maxHeight = 880;
        private const int maxWidth = 1580;

        //enemies
        private int minDamage = 5; //minimum attack damage from an enemy
        private int maxDamage = 25; //maximum attack damage from an enemy
        private int enemyCounter = 0;
        private int enemyCount = 2; //amount of enemies
        private int enemyMem = 0;
        
        //timers
        private DispatcherTimer timer;
        private DispatcherTimer bulletTimer;
        private DispatcherTimer reloadTimer;
        private DispatcherTimer explosionTimer;
        private int difficulty = 5; //used as milliseconds-part in timer

        //projectiles
        private List<Weapon> bullets = new List<Weapon>();
        private List<Point> targets = new List<Point>();
        private int magazineSlot = 0;
        private int bulletMem = 0;

        //rocks
        private List<Point> rocks = new List<Point>(); //collection of rocks
        private const int obstacleCount = 25;

        //objects derived from classes
        List<Enemy> monsters = new List<Enemy>(); //list for enemymonsters
        Enemy monster;
        Player playerone = new Player(new BitmapImage(new Uri(@"Resources\player.png", UriKind.Relative)));
        Obstacle stone = new Obstacle(new BitmapImage(new Uri(@"Resources\stone.png", UriKind.Relative)));
        Weapon bullet;
        Weapon explosion = new Weapon(new BitmapImage(new Uri(@"Resources\explosion.png", UriKind.Relative)));

        //randomizer
        private Random rnd = new Random(); //for randomizing sizes and positions of rocks and enemies
        #endregion

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitializeStuff();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeStuff()
        {
            //set element visibility
            btnOK.Visibility = Visibility.Hidden;
            txtName.Visibility = Visibility.Hidden;
            txbMag.Visibility = Visibility.Visible;
            txbScore.Visibility = Visibility.Visible;

            //initialize timers
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, difficulty);
            timer.Tick += new EventHandler(timer_Tick);

            bulletTimer = new DispatcherTimer();
            bulletTimer.Interval = new TimeSpan(0, 0, 0, 0, difficulty);
            bulletTimer.Tick += new EventHandler(bulletTimer_Tick);

            reloadTimer = new DispatcherTimer();
            reloadTimer.Interval = new TimeSpan(0, 0, 0, 1, difficulty);
            reloadTimer.Tick += new EventHandler(reloadTimer_Tick);

            explosionTimer = new DispatcherTimer();
            explosionTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            explosionTimer.Tick += new EventHandler(explosionTimer_Tick);

            //initialize event handlers for using keyboard, mouse movement and mouse button
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            this.MouseMove += new MouseEventHandler(charMove);
            this.MouseDown += new MouseButtonEventHandler(Shoot);

            //draw rocks, initial enemies and player
            IniRocks();
            PaintPlayerOne(playerone.startingPoint);
            playerone.currentPosition = playerone.startingPoint;
            paintCanvas.Children.Add(playerone.tank);
            IniEnemies();
            IniBullets();

            //initialize health and ammo for player
            playerone.Hitpoints = 200;
            playerone.Ammo = 10;

            //project health into a progress bar stationed on the canvas
            pgbHealth.DataContext = playerone;

            //start game
            timer.Start();
        }

        //explosion timer is used to grow size of explosion and to remove explosion from canvas
        private void explosionTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (explosion.explosion.Width < 10)
                {
                    explosion.explosion.Width = 10;
                    explosion.explosion.Height = 10;
                }
                else if (explosion.explosion.Width < 50)
                {
                    explosion.explosion.Width += 2;
                    explosion.explosion.Height += 2;
                }
                else
                {
                    explosionTimer.Stop();
                    paintCanvas.Children.Remove(explosion.explosion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used to make reloading take its time
        private void reloadTimer_Tick(object sender, EventArgs e)
        {
            txbMag.Text = "Ammo left: " + Convert.ToString(playerone.Ammo);
            reloadTimer.Stop();
        }

        //creates rock to map based on value determined earlier
        private void IniRocks()
        {
            for (int n = 0; n < obstacleCount; n++)
            {
                try
                {
                    PaintRocks(n);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //creates a predetermined amount of enemies
        public void IniEnemies()
        {
            try
            {
                for (int i = 0; i < enemyCount; i++)
                {
                    monster = new Enemy(new BitmapImage(new Uri(@"Resources\pommimies.png", UriKind.Relative))); //tellls program to use this picture as enemies
                    monsters.Add(monster); //add monsters to list
                }

                for (int i = 0; i < enemyCount; i++)
                {
                    monsters[i].PaintMonster();
                    monsters[i].Damage = rnd.Next(minDamage, maxDamage); //randomizes enemy attack damage to between min and max values
                    monsters[i].EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), rnd.Next(minBorder, maxHeight)); //randomizes enemy astarting location
                    paintCanvas.Children.Add(monsters[i].character);
                }
                PaintMovingMonsters(new Vector(rnd.Next(minBorder, maxWidth), rnd.Next(minBorder, maxHeight)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 

        //initialize bullets for later use
        public void IniBullets()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    bullet = new Weapon(new BitmapImage(new Uri(@"Resources\cannonball.png", UriKind.Relative)));
                    bullets.Add(bullet);
                    bullet.BulletVisual();
                    Point target = new Point(2000, 2000);
                    targets.Add(target);
                }
                bulletTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void charMove(object sender, MouseEventArgs e)
        {
            try
            {
                //gets the mousepointer coordinates from the window and relays them into a Point variable
                Point targ = e.GetPosition(paintCanvas);
                playerone.Move(targ);
                playerone.Rotation(targ, playerone.tank);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used for shooting, basically takes mouse pointer location and sends projectile to that direction
        private void Shoot(object sender, MouseButtonEventArgs e)
        {
            if (timer.IsEnabled)
            {
                try
                {
                    switch (e.LeftButton)
                    {
                        case MouseButtonState.Pressed:

                            if (playerone.Ammo > 0 && !reloadTimer.IsEnabled) //can't shoot if reloading, also have to have ammo left to shoot
                            {
                                reloadTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                                paintCanvas.Children.Add(bullets[magazineSlot].bullet);
                                targets[magazineSlot] = e.GetPosition(paintCanvas);
                                bullets[magazineSlot].bulletPosition = playerone.currentPosition;
                                bullets[magazineSlot].Fire(targets[magazineSlot], bullets[magazineSlot].bulletPosition);
                                
                                playerone.Ammo--; //decreases amount of ammo
                                magazineSlot++; 
                                reloadTimer.Start();
                            }

                            else if (!reloadTimer.IsEnabled) //if out of ammo, starts reloading magazine
                            {
                                reloadTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);
                                reloadTimer.Start();

                                playerone.Ammo = 10;
                                txbMag.Text = "Ammo left: Reloading...";
                                magazineSlot = 0;
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void PaintRocks(int index)
        {
            try
            {
                //randomizes coordinates for a rock
                Point point = new Point(rnd.Next(minBorder, maxWidth),
                                        rnd.Next(minBorder, maxHeight));
                stone.PaintRock(point);
                Canvas.SetTop(stone.rock, point.Y);
                Canvas.SetLeft(stone.rock, point.X);
                paintCanvas.Children.Insert(index, stone.rock);
                rocks.Insert(index, point);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used when projectile hits something and explodes
        private void PaintExplosion(Point detonationPoint)
        {
            try
            {
                detonationPoint = explosion.detonationPoint;
                explosionTimer.Start();
                explosion.ExplosionVisual();
                Canvas.SetTop(explosion.explosion, detonationPoint.Y);
                Canvas.SetLeft(explosion.explosion, detonationPoint.X);
                paintCanvas.Children.Add(explosion.explosion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PaintPlayerOne(Vector currentpoint)
        {
            try
            {
                //paints the player onto the canvas at the vector currentpoint
                playerone.PaintPlayer();
                Canvas.SetTop(playerone.tank, currentpoint.Y);
                Canvas.SetLeft(playerone.tank, currentpoint.X);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //used to paint projectile to canvas
        private void PaintBullet(Vector bulletPoint)
        {
            try
            {
                foreach (Weapon projectile in bullets)
                {
                    projectile.BulletVisual();
                    Canvas.SetTop(projectile.bullet, bulletPoint.Y);
                    Canvas.SetLeft(projectile.bullet, bulletPoint.X);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MonsterFollow()
        {
            try
            {
                //sets enemies to home on to player's current position
                monsters[enemyCounter].MonsterPositionLogic(playerone.currentPosition);
                Point targetPoint = new Point(playerone.currentPosition.X, playerone.currentPosition.Y);
                monsters[enemyCounter].Rotation(targetPoint, monsters[enemyCounter].character);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MonsterCollisionDetection()
        {
            try
            {
                enemyMem = 0; //this variable is used to "remember" which monster hits player or gets shot
                //loop to go through list of enemies and compare their position to player position
                foreach (Enemy enemy in monsters)
                {
                    if ((Math.Abs(enemy.EnemyPosition.X - playerone.currentPosition.X) < 5) && 
                        (Math.Abs(enemy.EnemyPosition.Y - playerone.currentPosition.Y) < 5))
                    {
                        if (playerone.Hitpoints - enemy.Damage > 0)
                        {
                            //if enemy does damage to player with more than zero hitpoints left, decrease hitpoints accordingly to dealt damage
                            playerone.Hitpoints -= enemy.Damage;
                            Recoil();
                        }
                        else
                        {
                            GameOver();
                        }
                        enemyMem++;
                    }
                }
                    bulletMem = 0; //this variable is used to "remember" which bullet hits something
                //two loops which compare bullet locations to enemy locations
                foreach (Weapon projectile in bullets)
                    {
                        enemyMem = 0;
                    foreach (Enemy enemy in monsters)
                    {
                        if ((Math.Abs(enemy.EnemyPosition.X - projectile.bulletPosition.X) < enemy.characterWidth) &&
                            (Math.Abs(enemy.EnemyPosition.Y - projectile.bulletPosition.Y) < enemy.characterWidth))
                        {
                            explosion.detonationPoint = new Point(projectile.bulletPosition.X, projectile.bulletPosition.Y);

                            PaintExplosion(explosion.detonationPoint);
                            projectile.DiscardBullet();
                            paintCanvas.Children.Remove(projectile.bullet);
                        
                            //if bullet proximity to enemy less than characterWidth, kill enemy
                            KillMonster();
                            break;
                        }
                            else
                                enemyMem++;
                        }
                        bulletMem++;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Recoil()
        {
            try
            {
                //player is staggered away from the direction of the attack
                if (playerone.currentPosition.X - monsters[enemyMem].EnemyPosition.X > 5)
                {
                    playerone.currentPosition.X += 10;
                }

                if (playerone.currentPosition.X - monsters[enemyMem].EnemyPosition.X < 5)
                {
                    playerone.currentPosition.X -= 10;
                }

                if (playerone.currentPosition.Y - monsters[enemyMem].EnemyPosition.X > 5)
                {
                    playerone.currentPosition.Y += 10;
                }

                if (playerone.currentPosition.Y - monsters[enemyMem].EnemyPosition.X < 5)
                {
                    playerone.currentPosition.Y -= 10;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RestrictMovement()
        {
            try
            {
                //character is pushed away from boundaries
                if (playerone.currentPosition.X <= minBorder)
                {
                    playerone.currentPosition.X += 5;
                }

                if (maxWidth - playerone.currentPosition.X <= 20)
                {
                    playerone.currentPosition.X -= 5;
                }

                if (playerone.currentPosition.Y < minBorder + 50)
                {
                    playerone.currentPosition.Y += 5;
                }

                if (playerone.currentPosition.Y >= maxHeight)
                {
                    playerone.currentPosition.Y -= 5;
                }

                foreach (Point point in rocks)
                {
                    if ((Math.Abs(point.X - playerone.currentPosition.X) < stone.rock.ActualWidth - stone.rock.ActualWidth / 2) &&
                        (Math.Abs(point.Y - playerone.currentPosition.Y) < stone.rock.ActualHeight - stone.rock.ActualHeight / 2))
                    {
                        playerone.currentPosition += new Vector(rnd.Next(1, 10), rnd.Next(1, 10));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PaintMovingMonsters(Vector enemyPoint1)
        {
            try
            {
                monsters[enemyCounter].PaintMonster();
                Canvas.SetTop(monsters[enemyCounter].character, monsters[enemyCounter].EnemyPosition.Y);
                Canvas.SetLeft(monsters[enemyCounter].character, monsters[enemyCounter].EnemyPosition.X);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } 

        //used to check if monster hits rock and push it around the rock
        private void MonsterRestrictMovement ()
        {
            try
            {
                foreach (Point point in rocks)
                {
                    if ((Math.Abs(point.X - monsters[enemyCounter].EnemyPosition.X) < stone.rock.ActualWidth - stone.rock.ActualWidth / 2) &&
                        (Math.Abs(point.Y - monsters[enemyCounter].EnemyPosition.Y) < stone.rock.ActualHeight - stone.rock.ActualHeight / 2))
                    {
                        monsters[enemyCounter].EnemyPosition += new Vector(rnd.Next(1, 10), rnd.Next(1, 10));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            //keyboard controls
            switch (e.Key)
            {
                case Key.P:
                    if (timer.IsEnabled)
                        timer.Stop(); //pause game
                    else
                        timer.Start(); //resume game if previously paused
                    break;

                case Key.Escape:
                    if (timer.IsEnabled)
                        GameOver(); //forfeit game
                    else
                        this.Close();
                    break;

               case Key.Up:
                    RestrictMovement();
                    playerone.currentPosition = playerone.currentPosition + playerone.charMove_norm * difficulty;
                    break;

                case Key.W:
                    RestrictMovement();
                    playerone.currentPosition = playerone.currentPosition + playerone.charMove_norm * difficulty;
                    break;

                case Key.Down:
                    RestrictMovement();
                        playerone.currentPosition = playerone.currentPosition - playerone.charMove_norm * difficulty;
                    break;

                case Key.S:
                    RestrictMovement();
                    playerone.currentPosition = playerone.currentPosition - playerone.charMove_norm * difficulty;
                    break;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                PaintPlayerOne(playerone.currentPosition);
                for (enemyCounter = 0; enemyCounter < enemyCount; enemyCounter++)
                {
                    MonsterFollow();
                    PaintMovingMonsters(monsters[enemyCounter].EnemyPosition);
                    MonsterRestrictMovement();
                }
                    MonsterCollisionDetection();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void bulletTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (Weapon projectile in bullets)
                {
                    if (projectile.bulletPosition.Y > minBorder && projectile.bulletPosition.Y < maxHeight
                        && projectile.bulletPosition.X > minBorder && projectile.bulletPosition.X < maxWidth)
                    {
                        projectile.bulletPosition = projectile.bulletPosition + projectile.bulletMove_norm * difficulty;
                        PaintBullet(projectile.bulletPosition);
                    }
                    else
                    {
                        paintCanvas.Children.Remove(projectile.bullet);
                        projectile.DiscardBullet();
                    }
                    foreach (Point point in rocks)
                    {
                        if ((Math.Abs(point.X - projectile.bulletPosition.X) < stone.rock.ActualWidth - stone.rock.ActualWidth / 2) &&
                            (Math.Abs(point.Y - projectile.bulletPosition.Y) < stone.rock.ActualHeight - stone.rock.ActualHeight / 2))
                        {
                            explosion.detonationPoint = new Point(projectile.bulletPosition.X, projectile.bulletPosition.Y);

                            PaintExplosion(explosion.detonationPoint);
                            projectile.DiscardBullet();
                            paintCanvas.Children.Remove(projectile.bullet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void KillMonster()
        {
            try
            {
                //add scorevalue to score when an enemy is dispatched from the monsters array
                playerone.Score += monsters[enemyMem].ScoreValue;

                paintCanvas.Children.Remove(bullets[bulletMem].bullet);
                txbScore.Text = "Score: " + Convert.ToString(playerone.Score);
                SpawnMonster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SpawnMonster()
        {
            try
            {
                enemyCount++;

                monster = new Enemy(new BitmapImage(new Uri(@"Resources\pommimies.png", UriKind.Relative)));
                monsters.Add(monster);

                monster.PaintMonster();
                monster.Damage = rnd.Next(minDamage, maxDamage); //randomizes enemy attack damage to between min and max values

                //used to make enemies spawn randomly somewhere outside the screen
                switch (rnd.Next(1,5))
                {
                    case 1:
                        monster.EnemyPosition = new Vector(minBorder - 20, rnd.Next(minBorder, maxHeight));
                        paintCanvas.Children.Add(monster.character);
                        monsters[enemyMem].EnemyPosition = new Vector(minBorder - 20, rnd.Next(minBorder, maxHeight));
                        break;
                    case 2:
                        monster.EnemyPosition = new Vector(maxWidth + 20, rnd.Next(minBorder, maxHeight));
                        paintCanvas.Children.Add(monster.character);
                        monsters[enemyMem].EnemyPosition = new Vector(maxWidth + 20, rnd.Next(minBorder, maxHeight));
                        break;
                    case 3:
                        monster.EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), minBorder - 20);
                        paintCanvas.Children.Add(monster.character);
                        monsters[enemyMem].EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), minBorder - 20);
                        break;
                    case 4:
                        monster.EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), maxHeight + 20);
                        paintCanvas.Children.Add(monster.character);
                        monsters[enemyMem].EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), maxHeight + 20);
                        break;
                    case 5:
                        monster.EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), maxHeight + 20);
                        paintCanvas.Children.Add(monster.character);
                        monsters[enemyMem].EnemyPosition = new Vector(rnd.Next(minBorder, maxWidth), maxHeight + 20);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void GameOver()
        {
            try
            {
                timer.Stop();
                bulletTimer.Stop();
                explosionTimer.Stop();
                btnOK.Visibility = Visibility.Visible;
                txtName.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GameOverShow()
        {
            try
            {
                var trs = new TranslateTransform();
                var anim = new DoubleAnimation(0, 1000, TimeSpan.FromSeconds(1));
                trs.BeginAnimation(TranslateTransform.XProperty, anim);
                trs.BeginAnimation(TranslateTransform.YProperty, anim);
                paintCanvas.RenderTransform = trs;

                StartWindow start = new StartWindow();
                if (!start.IsVisible)
                {
                    start.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "Insert Player Name" || txtName.Text == "") //player has to give some name other than empty
            { MessageBox.Show("Not valid name!"); }
            else
            {
                try
                {
                    playerone.Name = txtName.Text;
                    SaveScore();
                    GameOverShow();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //used to save player scores to external file
        private void SaveScore()
        {
            string name = playerone.Name;
            int finalscore = playerone.Score;
            try
            {
                string path = "Scoreboard.txt";
                //probes if the above mentioned file exists
                if (!File.Exists(path))
                {
                    //if not, create a new file to write to
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("Name  Score  Date");
                        sw.WriteLine(name + " " + finalscore + " " + DateTime.Now.ToString("dd-MM-yyyy"));
                    }
                }
                else
                {
                    //if yes, then write into it
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(name + "  " + finalscore + "  " + DateTime.Now.ToString("dd-MM-yyyy"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}